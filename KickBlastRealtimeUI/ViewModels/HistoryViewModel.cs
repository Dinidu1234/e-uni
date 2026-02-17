using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using KickBlastRealtimeUI.Data;
using KickBlastRealtimeUI.Helpers;
using KickBlastRealtimeUI.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastRealtimeUI.ViewModels;

public class HistoryViewModel : ObservableObject
{
    private readonly AppDbContext _dbContext;
    public ObservableCollection<MonthlyCalculation> Calculations { get; } = [];
    public ObservableCollection<Athlete> Athletes { get; } = [];
    public ICollectionView CalculationsView { get; }

    private Athlete? _selectedAthlete;
    private int _selectedMonth;
    private int _selectedYear;

    public Athlete? SelectedAthlete { get => _selectedAthlete; set { if (SetProperty(ref _selectedAthlete, value)) CalculationsView.Refresh(); } }
    public int SelectedMonth { get => _selectedMonth; set { if (SetProperty(ref _selectedMonth, value)) CalculationsView.Refresh(); } }
    public int SelectedYear { get => _selectedYear; set { if (SetProperty(ref _selectedYear, value)) CalculationsView.Refresh(); } }

    public HistoryViewModel(AppDbContext dbContext, Services.AppEvents appEvents)
    {
        _dbContext = dbContext;
        SelectedMonth = DateTime.Now.Month;
        SelectedYear = DateTime.Now.Year;

        CalculationsView = CollectionViewSource.GetDefaultView(Calculations);
        CalculationsView.Filter = Filter;

        appEvents.CalculationSaved += async () => await LoadDataAsync();
        appEvents.AthleteChanged += async () => await LoadDataAsync();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        timer.Tick += async (_, _) => await LoadDataAsync();
        timer.Start();

        _ = LoadDataAsync();
    }

    private bool Filter(object obj)
    {
        if (obj is not MonthlyCalculation calc) return false;
        var athleteMatch = SelectedAthlete is null || calc.AthleteId == SelectedAthlete.Id;
        var monthMatch = SelectedMonth <= 0 || calc.Month == SelectedMonth;
        var yearMatch = SelectedYear <= 0 || calc.Year == SelectedYear;
        return athleteMatch && monthMatch && yearMatch;
    }

    public async Task LoadDataAsync()
    {
        var athletes = await _dbContext.Athletes.AsNoTracking().OrderBy(a => a.FullName).ToListAsync();
        var calculations = await _dbContext.MonthlyCalculations.Include(c => c.Athlete).AsNoTracking().OrderByDescending(c => c.CreatedAt).ToListAsync();

        Athletes.Clear();
        foreach (var athlete in athletes) Athletes.Add(athlete);

        Calculations.Clear();
        foreach (var calc in calculations) Calculations.Add(calc);

        CalculationsView.Refresh();
    }
}
