namespace KickBlastRealtimeUI.Helpers;

public static class DateHelper
{
    public static DateTime GetSecondSaturday(int year, int month)
    {
        var firstDay = new DateTime(year, month, 1);
        var daysToSaturday = ((int)DayOfWeek.Saturday - (int)firstDay.DayOfWeek + 7) % 7;
        var firstSaturday = firstDay.AddDays(daysToSaturday);
        return firstSaturday.AddDays(7);
    }
}
