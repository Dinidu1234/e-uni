using System.IO;
using System.Windows;
using KickBlastRealtimeUI.Data;
using KickBlastRealtimeUI.Services;

namespace KickBlastRealtimeUI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Data"));

        var pricingService = new PricingService();
        var dbContext = new AppDbContext();
        DbInitializer.Initialize(dbContext);

        var appEvents = new AppEvents();
        var navigationService = new NavigationService();
        var feeCalculatorService = new FeeCalculatorService(pricingService);

        var mainWindow = new MainWindow
        {
            DataContext = new ViewModels.MainViewModel(
                dbContext,
                appEvents,
                navigationService,
                pricingService,
                feeCalculatorService)
        };

        MainWindow = mainWindow;
        mainWindow.Show();
    }
}
