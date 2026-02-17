using KickBlastRealtimeUI.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastRealtimeUI.Data;

public class AppDbContext : DbContext
{
    public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();
    public DbSet<Athlete> Athletes => Set<Athlete>();
    public DbSet<MonthlyCalculation> MonthlyCalculations => Set<MonthlyCalculation>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "kickblast_realtime.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainingPlan>().HasData(
            new TrainingPlan { Id = 1, Name = "Beginner", WeeklyFee = 2500 },
            new TrainingPlan { Id = 2, Name = "Intermediate", WeeklyFee = 4000 },
            new TrainingPlan { Id = 3, Name = "Elite", WeeklyFee = 6500 }
        );
    }
}
