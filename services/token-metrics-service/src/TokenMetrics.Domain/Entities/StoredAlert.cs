namespace TokenMetrics.Domain.Entities;

/// <summary>
/// Read-only view of alerts from Alert Service database.
/// Used to determine which coins need metrics updates (coins with active alerts).
/// </summary>
public class StoredAlert
{
    public Guid Id { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string? CoinMintAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}
