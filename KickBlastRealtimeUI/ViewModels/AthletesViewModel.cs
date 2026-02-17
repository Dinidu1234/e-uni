using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using KickBlastRealtimeUI.Data;
using KickBlastRealtimeUI.Helpers;
using KickBlastRealtimeUI.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastRealtimeUI.ViewModels;

public class AthletesViewModel : ObservableObject
{
    private readonly AppDbContext _dbContext;
    private readonly Services.AppEvents _appEvents;
    private readonly Action<string> _status;

    public ObservableCollection<Athlete> Athletes { get; } = [];
    public ObservableCollection<TrainingPlan> Plans { get; } = [];
    public ICollectionView AthletesView { get; }

    private Athlete? _selectedAthlete;
    public Athlete? SelectedAthlete
    {
        get => _selectedAthlete;
        set
        {
            if (SetProperty(ref _selectedAthlete, value) && value is not null)
            {
                AthleteName = value.FullName;
                AthleteWeight = value.WeightKg;
                TargetWeight = value.TargetWeightKg;
                SelectedPlan = Plans.FirstOrDefault(p => p.Id == value.TrainingPlanId);
            }
        }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) AthletesView.Refresh(); }
    }

    private TrainingPlan? _selectedPlanFilter;
    public TrainingPlan? SelectedPlanFilter
    {
        get => _selectedPlanFilter;
        set { if (SetProperty(ref _selectedPlanFilter, value)) AthletesView.Refresh(); }
    }

    private string _athleteName = string.Empty;
    private decimal _athleteWeight;
    private decimal _targetWeight;
    private TrainingPlan? _selectedPlan;

    public string AthleteName { get => _athleteName; set => SetProperty(ref _athleteName, value); }
    public decimal AthleteWeight { get => _athleteWeight; set => SetProperty(ref _athleteWeight, value); }
    public decimal TargetWeight { get => _targetWeight; set => SetProperty(ref _targetWeight, value); }
    public TrainingPlan? SelectedPlan { get => _selectedPlan; set => SetProperty(ref _selectedPlan, value); }

    public ICommand AddOrUpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand { get; }

    public AthletesViewModel(AppDbContext dbContext, Services.AppEvents appEvents, Action<string> status)
    {
        _dbContext = dbContext;
        _appEvents = appEvents;
        _status = status;
        AthletesView = CollectionViewSource.GetDefaultView(Athletes);
        AthletesView.Filter = FilterAthletes;

        AddOrUpdateCommand = new RelayCommand(async () => await AddOrUpdateAsync());
        DeleteCommand = new RelayCommand(async () => await DeleteAsync());
        ClearCommand = new RelayCommand(ClearForm);

        appEvents.AthleteChanged += async () => await LoadDataAsync();
        _ = LoadDataAsync();
    }

    private bool FilterAthletes(object obj)
    {
        if (obj is not Athlete athlete) return false;
        var searchMatch = string.IsNullOrWhiteSpace(SearchText) || athlete.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var planMatch = SelectedPlanFilter is null || athlete.TrainingPlanId == SelectedPlanFilter.Id;
        return searchMatch && planMatch;
    }

    public async Task LoadDataAsync()
    {
        var plans = await _dbContext.TrainingPlans.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
        var athletes = await _dbContext.Athletes.Include(a => a.TrainingPlan).AsNoTracking().OrderBy(a => a.FullName).ToListAsync();

        Plans.Clear();
        foreach (var plan in plans) Plans.Add(plan);

        Athletes.Clear();
        foreach (var athlete in athletes) Athletes.Add(athlete);

        AthletesView.Refresh();
    }

    private async Task AddOrUpdateAsync()
    {
        try
        {
            if (!Validators.IsAthleteNameValid(AthleteName) || SelectedPlan is null)
            {
                _status("Please enter valid athlete details.");
                return;
            }

            if (SelectedAthlete is null)
            {
                _dbContext.Athletes.Add(new Athlete
                {
                    FullName = AthleteName.Trim(),
                    WeightKg = AthleteWeight,
                    TargetWeightKg = TargetWeight,
                    TrainingPlanId = SelectedPlan.Id
                });
                _status("Athlete added successfully.");
            }
            else
            {
                var athlete = await _dbContext.Athletes.FindAsync(SelectedAthlete.Id);
                if (athlete is null) return;
                athlete.FullName = AthleteName.Trim();
                athlete.WeightKg = AthleteWeight;
                athlete.TargetWeightKg = TargetWeight;
                athlete.TrainingPlanId = SelectedPlan.Id;
                _status("Athlete updated successfully.");
            }

            await _dbContext.SaveChangesAsync();
            ClearForm();
            _appEvents.PublishAthleteChanged();
        }
        catch (Exception ex)
        {
            _status($"Unable to save athlete: {ex.Message}");
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedAthlete is null) return;
        if (System.Windows.MessageBox.Show($"Delete {SelectedAthlete.FullName}?", "Confirm Delete", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            var athlete = await _dbContext.Athletes.FindAsync(SelectedAthlete.Id);
            if (athlete is null) return;
            _dbContext.Athletes.Remove(athlete);
            await _dbContext.SaveChangesAsync();
            _status("Athlete deleted.");
            ClearForm();
            _appEvents.PublishAthleteChanged();
        }
        catch (Exception ex)
        {
            _status($"Unable to delete athlete: {ex.Message}");
        }
    }

    private void ClearForm()
    {
        SelectedAthlete = null;
        AthleteName = string.Empty;
        AthleteWeight = 0;
        TargetWeight = 0;
        SelectedPlan = null;
    }
}
