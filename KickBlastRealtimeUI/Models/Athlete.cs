using System.ComponentModel.DataAnnotations;

namespace KickBlastRealtimeUI.Models;

public class Athlete
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    public decimal WeightKg { get; set; }
    public decimal TargetWeightKg { get; set; }

    public int TrainingPlanId { get; set; }
    public TrainingPlan? TrainingPlan { get; set; }

    public ICollection<MonthlyCalculation> MonthlyCalculations { get; set; } = new List<MonthlyCalculation>();
}
