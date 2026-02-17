namespace KickBlastRealtimeUI.Helpers;

public static class Validators
{
    public static bool IsAthleteNameValid(string? name) => !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 3;

    public static bool IsCoachingHoursValid(decimal hours) => hours >= 0 && hours <= 5;

    public static bool IsCompetitionCountValid(int competitions) => competitions >= 0;
}
