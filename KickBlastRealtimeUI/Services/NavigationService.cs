using KickBlastRealtimeUI.Helpers;

namespace KickBlastRealtimeUI.Services;

public class NavigationService : ObservableObject
{
    private object? _currentView;

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }
}
