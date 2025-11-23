using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Domain.Interfaces;

/// <summary>
/// Repository for token metadata storage and retrieval
/// </summary>
public interface ITokenMetricsRepository
{
    /// <summary>
    /// Gets token metadata from cache/database
    /// </summary>
    Task<TokenMetadata?> GetTokenMetadataAsync(string contractAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves or updates token metadata
    /// </summary>
    Task SaveTokenMetadataAsync(TokenMetadata metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tokens in a specific cache tier
    /// </summary>
    Task<List<TokenMetadata>> GetTokensByCacheTierAsync(CacheTier tier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the cache tier for a token (e.g., promote to Hot when added to top 25)
    /// </summary>
    Task UpdateCacheTierAsync(string contractAddress, CacheTier tier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search tokens by symbol or contract address
    /// </summary>
    Task<List<TokenMetadata>> SearchTokensAsync(string query, int limit = 10, CancellationToken cancellationToken = default);
}
