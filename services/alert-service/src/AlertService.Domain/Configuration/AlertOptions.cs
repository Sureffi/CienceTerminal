namespace AlertService.Domain.Configuration;

public class AlertOptions
{
    public int MaxActiveAlerts { get; set; } = 50;
    public Dictionary<string, int> MaxActiveAlertsByType { get; set; } = new();
    public int TopTrendingLimit { get; set; } = 20;
}
