namespace KickBlastRealtimeUI.Services;

public class AppEvents
{
    public event Action? AthleteChanged;
    public event Action? CalculationSaved;
    public event Action? PricingUpdated;

    public void PublishAthleteChanged() => AthleteChanged?.Invoke();
    public void PublishCalculationSaved() => CalculationSaved?.Invoke();
    public void PublishPricingUpdated() => PricingUpdated?.Invoke();
}
