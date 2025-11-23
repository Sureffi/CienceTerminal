using System;

namespace AlertService.Domain.Entities;

/// <summary>
/// Read-only view of time-based rolling aggregates for CA mention trending analysis.
/// This entity mirrors the mention_aggregates table in the shared cienceterminal database,
/// which is owned and managed by the Token Metrics Service.
/// Alert Service has read-only access for querying trending data.
/// </summary>
public class MentionAggregate
{
    /// <summary>
    /// Unique identifier for the aggregate record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Solana contract address (mint address). Must be 44 characters.
    /// </summary>
    public string CoinMintAddress { get; set; } = string.Empty;

    /// <summary>
    /// Number of mentions in the last 5 minutes.
    /// </summary>
    public double MentionCount5m { get; set; }

    /// <summary>
    /// Number of mentions in the last 1 hour.
    /// </summary>
    public double MentionCount1h { get; set; }

    /// <summary>
    /// Number of mentions in the last 6 hours.
    /// </summary>
    public double MentionCount6h { get; set; }

    /// <summary>
    /// Number of mentions in the last 24 hours.
    /// </summary>
    public double MentionCount24h { get; set; }

    /// <summary>
    /// Calculated trending score based on EMA (Exponential Moving Average) of mention velocity.
    /// Higher scores indicate stronger trending momentum.
    /// </summary>
    public double TrendingScore { get; set; }

    /// <summary>
    /// Position in the top 25 trending list (1-25). Null if not in top 25.
    /// </summary>
    public int? Rank { get; set; }

    /// <summary>
    /// Timestamp of the most recent mention for this CA.
    /// </summary>
    public DateTime LastMentioned { get; set; }

    /// <summary>
    /// Timestamp when these aggregates were last calculated by the Token Metrics Service background job.
    /// </summary>
    public DateTime LastCalculated { get; set; }
}
