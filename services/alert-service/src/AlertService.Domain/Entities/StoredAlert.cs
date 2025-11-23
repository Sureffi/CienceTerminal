namespace AlertService.Domain.Entities;

/// <summary>
/// Database entity for persisting alerts with JSONB storage.
/// Enables alert state recovery after service restarts.
/// </summary>
public class StoredAlert
{
    /// <summary>
    /// Alert ID (matches Alert.Id)
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Alert type discriminator (CaMention, Twitter, etc.)
    /// Used for querying and deserialization.
    /// </summary>
    public string AlertType { get; set; } = string.Empty;

    /// <summary>
    /// Serialized Alert object as JSON.
    /// Stored in JSONB column for efficient storage.
    /// </summary>
    public string AlertData { get; set; } = string.Empty;

    /// <summary>
    /// When the alert was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Coin mint address for fast queries (denormalized from AlertData)
    /// Nullable because not all alerts have a coin association
    /// </summary>
    public string? CoinMintAddress { get; set; }
}
