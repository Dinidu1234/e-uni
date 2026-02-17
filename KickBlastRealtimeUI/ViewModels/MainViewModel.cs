using KickBlastRealtimeUI.Data;
using KickBlastRealtimeUI.Helpers;
using KickBlastRealtimeUI.Services;
using System.Windows.Input;

namespace KickBlastRealtimeUI.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    public DashboardViewModel DashboardViewModel { get; }
    public AthletesViewModel AthletesViewModel { get; }
    public CalculatorViewModel CalculatorViewModel { get; }
    public HistoryViewModel HistoryViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    private string _statusMessage = "Ready.";

    public object? CurrentViewModel => _navigationService.CurrentView;
    public string CurrentDateText => DateTime.Now.ToString("dddd, dd MMMM yyyy");

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowAthletesCommand { get; }
    public ICommand ShowCalculatorCommand { get; }
    public ICommand ShowHistoryCommand { get; }
    public ICommand ShowSettingsCommand { get; }

    public MainViewModel(AppDbContext dbContext, AppEvents appEvents, NavigationService navigationService, PricingService pricingService, FeeCalculatorService feeCalculatorService)
    {
        _navigationService = navigationService;
        _navigationService.PropertyChanged += (_, _) => OnPropertyChanged(nameof(CurrentViewModel));

        DashboardViewModel = new DashboardViewModel(dbContext, appEvents);
        AthletesViewModel = new AthletesViewModel(dbContext, appEvents, UpdateStatus);
        CalculatorViewModel = new CalculatorViewModel(dbContext, appEvents, feeCalculatorService, UpdateStatus);
        HistoryViewModel = new HistoryViewModel(dbContext, appEvents);
        SettingsViewModel = new SettingsViewModel(pricingService, appEvents, UpdateStatus);

        ShowDashboardCommand = new RelayCommand(() => _navigationService.CurrentView = DashboardViewModel);
        ShowAthletesCommand = new RelayCommand(() => _navigationService.CurrentView = AthletesViewModel);
        ShowCalculatorCommand = new RelayCommand(() => _navigationService.CurrentView = CalculatorViewModel);
        ShowHistoryCommand = new RelayCommand(() => _navigationService.CurrentView = HistoryViewModel);
        ShowSettingsCommand = new RelayCommand(() => _navigationService.CurrentView = SettingsViewModel);

        _navigationService.CurrentView = DashboardViewModel;
    }

    private void UpdateStatus(string message)
    {
        StatusMessage = message;
    }
}
