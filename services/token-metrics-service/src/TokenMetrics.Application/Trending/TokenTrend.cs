using System.Text.Json;

namespace TokenMetrics.Application.Trending;

/// <summary>
/// Manages EMA trackers for multiple timeframes and computes trending score.
/// </summary>
public class TokenTrend
{
    public Dictionary<TimeSpan, EmaTracker> Emas { get; }

    public TokenTrend(string? emaJson = null)
    {
        // Initialize with default alphas
        Emas = new Dictionary<TimeSpan, EmaTracker>
        {
            [TimeSpan.FromMinutes(5)] = new EmaTracker(0.33),
            [TimeSpan.FromHours(1)] = new EmaTracker(0.032),
            [TimeSpan.FromHours(6)] = new EmaTracker(0.0055),
            [TimeSpan.FromHours(24)] = new EmaTracker(0.0014)
        };

        // Load previous EMA values if provided
        if (!string.IsNullOrEmpty(emaJson) && emaJson != "{}")
        {
            try
            {
                var values = JsonSerializer.Deserialize<Dictionary<string, double>>(emaJson);
                if (values != null)
                {
                    if (values.TryGetValue("5m", out var val5m))
                        Emas[TimeSpan.FromMinutes(5)] = new EmaTracker(0.33, val5m);
                    if (values.TryGetValue("1h", out var val1h))
                        Emas[TimeSpan.FromHours(1)] = new EmaTracker(0.032, val1h);
                    if (values.TryGetValue("6h", out var val6h))
                        Emas[TimeSpan.FromHours(6)] = new EmaTracker(0.0055, val6h);
                    if (values.TryGetValue("24h", out var val24h))
                        Emas[TimeSpan.FromHours(24)] = new EmaTracker(0.0014, val24h);
                }
            }
            catch
            {
                // If deserialization fails, use default initialized EMAs
            }
        }
    }

    public double ComputeTrendScore(double totalMentions)
    {
        double eps = 1e-6; // Prevent division by zero
        double ema5m = Emas[TimeSpan.FromMinutes(5)].Value;
        double ema1h = Emas[TimeSpan.FromHours(1)].Value;
        double ema6h = Emas[TimeSpan.FromHours(6)].Value;
        double ema24h = Emas[TimeSpan.FromHours(24)].Value;

        double shortRatio = ema5m / (ema1h + eps);
        double midRatio = ema1h / (ema6h + eps);
        double longRatio = ema6h / (ema24h + eps);

        var score = shortRatio + (0.5 * midRatio) + (0.25 * longRatio);

        // Volume weighting - suppress low-mention tokens
        double volumeFactor = Math.Tanh(totalMentions / 10.0);

        return score * volumeFactor;
    }

    public void UpdateMentions(TimeSpan timeframe, double newCount)
    {
        Emas[timeframe].Update(newCount);
    }

    public string SerializeEmas()
    {
        var values = new Dictionary<string, double>
        {
            ["5m"] = Emas[TimeSpan.FromMinutes(5)].Value,
            ["1h"] = Emas[TimeSpan.FromHours(1)].Value,
            ["6h"] = Emas[TimeSpan.FromHours(6)].Value,
            ["24h"] = Emas[TimeSpan.FromHours(24)].Value
        };
        return JsonSerializer.Serialize(values);
    }
}
