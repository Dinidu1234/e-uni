using KickBlastRealtimeUI.Models;

namespace KickBlastRealtimeUI.Services;

public class FeeCalculatorService(PricingService pricingService)
{
    public CalculationResult Calculate(Athlete athlete, decimal coachingHours, int competitions)
    {
        var pricing = pricingService.LoadPricing();

        if (coachingHours is < 0 or > 5)
        {
            throw new ArgumentException("Coaching hours must be between 0 and 5.");
        }

        if (competitions < 0)
        {
            throw new ArgumentException("Competitions cannot be negative.");
        }

        var planName = athlete.TrainingPlan?.Name ?? string.Empty;
        if (planName.Equals("Beginner", StringComparison.OrdinalIgnoreCase))
        {
            competitions = 0;
        }

        var weeklyFee = planName switch
        {
            "Beginner" => pricing.BeginnerWeeklyFee,
            "Intermediate" => pricing.IntermediateWeeklyFee,
            "Elite" => pricing.EliteWeeklyFee,
            _ => 0m
        };

        var trainingCost = weeklyFee * 4;
        var coachingCost = coachingHours * 4 * pricing.CoachingHourlyRate;
        var competitionCost = competitions * pricing.CompetitionFee;

        return new CalculationResult
        {
            WeeklyFee = weeklyFee,
            TrainingCost = trainingCost,
            CoachingHours = coachingHours,
            CoachingCost = coachingCost,
            Competitions = competitions,
            CompetitionFee = pricing.CompetitionFee,
            CompetitionCost = competitionCost,
            Total = trainingCost + coachingCost + competitionCost
        };
    }
}

public class CalculationResult
{
    public decimal WeeklyFee { get; set; }
    public decimal TrainingCost { get; set; }
    public decimal CoachingHours { get; set; }
    public decimal CoachingCost { get; set; }
    public int Competitions { get; set; }
    public decimal CompetitionFee { get; set; }
    public decimal CompetitionCost { get; set; }
    public decimal Total { get; set; }
}
