using TokenMetrics.Domain.Common;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Domain.Interfaces;

/// <summary>
/// Client for Jupiter Aggregator API to fetch Solana token metadata
/// </summary>
public interface IJupiterClient
{
    /// <summary>
    /// Fetches complete token metadata from Jupiter API
    /// </summary>
    /// <param name="mintAddress">Solana token mint address (contract address)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing Jupiter token metadata or error details</returns>
    Task<Result<JupiterTokenData>> GetTokenMetadataAsync(string mintAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches token metadata for multiple tokens in a single batch request
    /// </summary>
    /// <param name="mintAddresses">List of Solana token mint addresses (max 100 per request)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary mapping mint addresses to their token data</returns>
    Task<Dictionary<string, JupiterTokenData>> GetBatchTokenMetadataAsync(IEnumerable<string> mintAddresses, CancellationToken cancellationToken = default);
}
