using System.Globalization;

namespace KickBlastRealtimeUI.Helpers;

public static class CurrencyHelper
{
    public static string ToLkr(decimal amount)
    {
        return $"LKR {amount.ToString("N2", CultureInfo.InvariantCulture)}";
    }
}
