using System.Collections.ObjectModel;
using System.Windows.Input;
using KickBlastRealtimeUI.Data;
using KickBlastRealtimeUI.Helpers;
using KickBlastRealtimeUI.Models;
using KickBlastRealtimeUI.Services;
using Microsoft.EntityFrameworkCore;

namespace KickBlastRealtimeUI.ViewModels;

public class CalculatorViewModel : ObservableObject
{
    private readonly AppDbContext _dbContext;
    private readonly AppEvents _appEvents;
    private readonly FeeCalculatorService _feeCalculatorService;
    private readonly Action<string> _status;

    public ObservableCollection<Athlete> Athletes { get; } = [];

    private Athlete? _selectedAthlete;
    private decimal _coachingHours;
    private int _competitions;
    private string _summary = "No calculation yet.";
    private string _weightStatus = "";
    private string _secondSaturday = "";

    public Athlete? SelectedAthlete { get => _selectedAthlete; set => SetProperty(ref _selectedAthlete, value); }
    public decimal CoachingHours { get => _coachingHours; set => SetProperty(ref _coachingHours, value); }
    public int Competitions { get => _competitions; set => SetProperty(ref _competitions, value); }
    public string Summary { get => _summary; set => SetProperty(ref _summary, value); }
    public string WeightStatus { get => _weightStatus; set => SetProperty(ref _weightStatus, value); }
    public string SecondSaturday { get => _secondSaturday; set => SetProperty(ref _secondSaturday, value); }

    public ICommand CalculateCommand { get; }
    public ICommand SaveCommand { get; }

    private CalculationResult? _result;

    public CalculatorViewModel(AppDbContext dbContext, AppEvents appEvents, FeeCalculatorService feeCalculatorService, Action<string> status)
    {
        _dbContext = dbContext;
        _appEvents = appEvents;
        _feeCalculatorService = feeCalculatorService;
        _status = status;

        CalculateCommand = new RelayCommand(Calculate);
        SaveCommand = new RelayCommand(async () => await SaveAsync());

        appEvents.AthleteChanged += async () => await LoadAthletesAsync();
        appEvents.PricingUpdated += () => _status("Pricing updated. New rates are now active.");

        _ = LoadAthletesAsync();
    }

    private async Task LoadAthletesAsync()
    {
        Athletes.Clear();
        var athletes = await _dbContext.Athletes.Include(a => a.TrainingPlan).AsNoTracking().OrderBy(a => a.FullName).ToListAsync();
        foreach (var athlete in athletes) Athletes.Add(athlete);
    }

    private void Calculate()
    {
        try
        {
            if (SelectedAthlete is null)
            {
                _status("Select an athlete first.");
                return;
            }

            if (!Validators.IsCoachingHoursValid(CoachingHours) || !Validators.IsCompetitionCountValid(Competitions))
            {
                _status("Invalid coaching hours or competitions.");
                return;
            }

            _result = _feeCalculatorService.Calculate(SelectedAthlete, CoachingHours, Competitions);

            Summary = $"Training: {CurrencyHelper.ToLkr(_result.TrainingCost)} | Coaching: {CurrencyHelper.ToLkr(_result.CoachingCost)} | Competition: {CurrencyHelper.ToLkr(_result.CompetitionCost)} | Total: {CurrencyHelper.ToLkr(_result.Total)}";
            WeightStatus = SelectedAthlete.WeightKg > SelectedAthlete.TargetWeightKg
                ? "Over target"
                : SelectedAthlete.WeightKg < SelectedAthlete.TargetWeightKg ? "Under target" : "On target";
            SecondSaturday = DateHelper.GetSecondSaturday(DateTime.Now.Year, DateTime.Now.Month).ToString("dd MMM yyyy");
        }
        catch (Exception ex)
        {
            _status(ex.Message);
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (SelectedAthlete is null || _result is null)
            {
                _status("Run calculation before saving.");
                return;
            }

            var now = DateTime.Now;
            _dbContext.MonthlyCalculations.Add(new MonthlyCalculation
            {
                AthleteId = SelectedAthlete.Id,
                Month = now.Month,
                Year = now.Year,
                WeeklyFee = _result.WeeklyFee,
                TrainingCost = _result.TrainingCost,
                CoachingHours = _result.CoachingHours,
                CoachingCost = _result.CoachingCost,
                Competitions = _result.Competitions,
                CompetitionFee = _result.CompetitionFee,
                CompetitionCost = _result.CompetitionCost,
                Total = _result.Total,
                CreatedAt = now
            });

            await _dbContext.SaveChangesAsync();
            _appEvents.PublishCalculationSaved();
            _status("✅ Calculation saved successfully.");
        }
        catch (Exception ex)
        {
            _status($"Failed to save calculation: {ex.Message}");
        }
    }
}
