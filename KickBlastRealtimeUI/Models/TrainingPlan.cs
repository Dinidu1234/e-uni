namespace KickBlastRealtimeUI.Models;

public class TrainingPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal WeeklyFee { get; set; }

    public ICollection<Athlete> Athletes { get; set; } = new List<Athlete>();
}
