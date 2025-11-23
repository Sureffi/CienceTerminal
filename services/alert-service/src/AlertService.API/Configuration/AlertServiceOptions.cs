namespace AlertService.API.Configuration;

public class AlertServiceOptions
{
    public const string SectionName = "AlertService";

    public string Port { get; set; } = "5148";
    public string[] CorsOrigins { get; set; } = Array.Empty<string>();
}
