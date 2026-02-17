using System.Windows.Input;
using KickBlastRealtimeUI.Helpers;
using KickBlastRealtimeUI.Services;

namespace KickBlastRealtimeUI.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private readonly PricingService _pricingService;
    private readonly AppEvents _appEvents;
    private readonly Action<string> _status;

    public decimal BeginnerWeeklyFee { get; set; }
    public decimal IntermediateWeeklyFee { get; set; }
    public decimal EliteWeeklyFee { get; set; }
    public decimal CompetitionFee { get; set; }
    public decimal CoachingHourlyRate { get; set; }

    public ICommand SaveCommand { get; }

    public SettingsViewModel(PricingService pricingService, AppEvents appEvents, Action<string> status)
    {
        _pricingService = pricingService;
        _appEvents = appEvents;
        _status = status;
        SaveCommand = new RelayCommand(async () => await SaveAsync());
        Load();
    }

    private void Load()
    {
        var config = _pricingService.LoadPricing();
        BeginnerWeeklyFee = config.BeginnerWeeklyFee;
        IntermediateWeeklyFee = config.IntermediateWeeklyFee;
        EliteWeeklyFee = config.EliteWeeklyFee;
        CompetitionFee = config.CompetitionFee;
        CoachingHourlyRate = config.CoachingHourlyRate;
        OnPropertyChanged(nameof(BeginnerWeeklyFee));
        OnPropertyChanged(nameof(IntermediateWeeklyFee));
        OnPropertyChanged(nameof(EliteWeeklyFee));
        OnPropertyChanged(nameof(CompetitionFee));
        OnPropertyChanged(nameof(CoachingHourlyRate));
    }

    private async Task SaveAsync()
    {
        try
        {
            await _pricingService.SavePricingAsync(new PricingConfig
            {
                BeginnerWeeklyFee = BeginnerWeeklyFee,
                IntermediateWeeklyFee = IntermediateWeeklyFee,
                EliteWeeklyFee = EliteWeeklyFee,
                CompetitionFee = CompetitionFee,
                CoachingHourlyRate = CoachingHourlyRate
            });

            _appEvents.PublishPricingUpdated();
            _status("Pricing settings saved.");
        }
        catch (Exception ex)
        {
            _status($"Unable to save settings: {ex.Message}");
        }
    }
}
