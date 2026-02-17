using KickBlastRealtimeUI.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastRealtimeUI.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.TrainingPlans.Any())
        {
            context.TrainingPlans.AddRange(
                new TrainingPlan { Name = "Beginner", WeeklyFee = 2500 },
                new TrainingPlan { Name = "Intermediate", WeeklyFee = 4000 },
                new TrainingPlan { Name = "Elite", WeeklyFee = 6500 }
            );
            context.SaveChanges();
        }

        if (!context.Athletes.Any())
        {
            var plans = context.TrainingPlans.AsNoTracking().ToList();
            context.Athletes.AddRange(
                new Athlete { FullName = "Kasun Perera", WeightKg = 74, TargetWeightKg = 72, TrainingPlanId = plans.First(p => p.Name == "Beginner").Id },
                new Athlete { FullName = "Nimali Fernando", WeightKg = 58, TargetWeightKg = 56, TrainingPlanId = plans.First(p => p.Name == "Intermediate").Id },
                new Athlete { FullName = "Ravindu Silva", WeightKg = 82, TargetWeightKg = 79, TrainingPlanId = plans.First(p => p.Name == "Elite").Id },
                new Athlete { FullName = "Shanika Jay", WeightKg = 64, TargetWeightKg = 61, TrainingPlanId = plans.First(p => p.Name == "Beginner").Id },
                new Athlete { FullName = "Thisara Deen", WeightKg = 71, TargetWeightKg = 70, TrainingPlanId = plans.First(p => p.Name == "Intermediate").Id },
                new Athlete { FullName = "Ayesha Mendis", WeightKg = 67, TargetWeightKg = 63, TrainingPlanId = plans.First(p => p.Name == "Elite").Id }
            );
            context.SaveChanges();
        }
    }
}
