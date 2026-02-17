namespace KickBlastRealtimeUI.Models;

public class MonthlyCalculation
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public Athlete? Athlete { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public decimal WeeklyFee { get; set; }
    public decimal TrainingCost { get; set; }
    public decimal CoachingHours { get; set; }
    public decimal CoachingCost { get; set; }
    public int Competitions { get; set; }
    public decimal CompetitionFee { get; set; }
    public decimal CompetitionCost { get; set; }
    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
