namespace CienceTerminal.Contracts.Events;

/// <summary>
/// Published by Token Metrics Service when a coin fails blacklist validation
/// Consumed by Alert Service to remove all active alerts for the coin
/// </summary>
public class CoinBlacklistedEvent
{
    public string CoinMintAddress { get; set; } = string.Empty;
    public DateTime BlacklistedAt { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Specific validation failures (e.g., "LowLiquidity", "HighTopHolderConcentration")
    /// </summary>
    public List<string> ValidationFailures { get; set; } = new();
}
