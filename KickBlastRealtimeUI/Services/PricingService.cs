using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;

namespace KickBlastRealtimeUI.Services;

public class PricingService
{
    private readonly string _configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    public PricingConfig LoadPricing()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        return new PricingConfig
        {
            BeginnerWeeklyFee = configuration.GetValue<decimal>("Pricing:BeginnerWeeklyFee"),
            IntermediateWeeklyFee = configuration.GetValue<decimal>("Pricing:IntermediateWeeklyFee"),
            EliteWeeklyFee = configuration.GetValue<decimal>("Pricing:EliteWeeklyFee"),
            CompetitionFee = configuration.GetValue<decimal>("Pricing:CompetitionFee"),
            CoachingHourlyRate = configuration.GetValue<decimal>("Pricing:CoachingHourlyRate")
        };
    }

    public async Task SavePricingAsync(PricingConfig config)
    {
        var root = JsonNode.Parse(await File.ReadAllTextAsync(_configPath)) ?? new JsonObject();
        root["Pricing"] = new JsonObject
        {
            ["BeginnerWeeklyFee"] = config.BeginnerWeeklyFee,
            ["IntermediateWeeklyFee"] = config.IntermediateWeeklyFee,
            ["EliteWeeklyFee"] = config.EliteWeeklyFee,
            ["CompetitionFee"] = config.CompetitionFee,
            ["CoachingHourlyRate"] = config.CoachingHourlyRate
        };
        await File.WriteAllTextAsync(_configPath, root.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
    }
}

public class PricingConfig
{
    public decimal BeginnerWeeklyFee { get; set; }
    public decimal IntermediateWeeklyFee { get; set; }
    public decimal EliteWeeklyFee { get; set; }
    public decimal CompetitionFee { get; set; }
    public decimal CoachingHourlyRate { get; set; }
}
