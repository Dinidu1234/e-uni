using System.Windows.Threading;
using KickBlastRealtimeUI.Data;
using KickBlastRealtimeUI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace KickBlastRealtimeUI.ViewModels;

public class DashboardViewModel : ObservableObject
{
    private readonly AppDbContext _dbContext;

    private int _totalAthletes;
    private int _calculationsThisMonth;
    private string _totalRevenueThisMonth = "LKR 0.00";
    private string _nextCompetitionDate = string.Empty;

    public int TotalAthletes { get => _totalAthletes; set => SetProperty(ref _totalAthletes, value); }
    public int CalculationsThisMonth { get => _calculationsThisMonth; set => SetProperty(ref _calculationsThisMonth, value); }
    public string TotalRevenueThisMonth { get => _totalRevenueThisMonth; set => SetProperty(ref _totalRevenueThisMonth, value); }
    public string NextCompetitionDate { get => _nextCompetitionDate; set => SetProperty(ref _nextCompetitionDate, value); }

    public DashboardViewModel(AppDbContext dbContext, Services.AppEvents appEvents)
    {
        _dbContext = dbContext;
        appEvents.AthleteChanged += async () => await LoadDataAsync();
        appEvents.CalculationSaved += async () => await LoadDataAsync();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        timer.Tick += async (_, _) => await LoadDataAsync();
        timer.Start();

        _ = LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        try
        {
            var now = DateTime.Now;
            TotalAthletes = await _dbContext.Athletes.CountAsync();
            CalculationsThisMonth = await _dbContext.MonthlyCalculations.CountAsync(c => c.Month == now.Month && c.Year == now.Year);
            var revenue = await _dbContext.MonthlyCalculations.Where(c => c.Month == now.Month && c.Year == now.Year).SumAsync(c => (decimal?)c.Total) ?? 0;
            TotalRevenueThisMonth = CurrencyHelper.ToLkr(revenue);
            NextCompetitionDate = DateHelper.GetSecondSaturday(now.Year, now.Month).ToString("dd MMM yyyy");
        }
        catch
        {
            // suppress; status handled elsewhere
        }
    }
}
