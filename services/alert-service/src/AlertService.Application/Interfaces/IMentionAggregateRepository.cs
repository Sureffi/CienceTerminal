using AlertService.Domain.Entities;

namespace AlertService.Application.Interfaces;

/// <summary>
/// Repository for querying mention aggregates from the shared cienceterminal database.
/// Provides read-only access to trending token data computed by Token Metrics Service.
/// </summary>
public interface IMentionAggregateRepository
{
    /// <summary>
    /// Gets the top N trending tokens ranked by trending score.
    /// </summary>
    /// <param name="topN">Number of top trending tokens to retrieve (default: 25)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of mention aggregates ordered by rank</returns>
    Task<List<MentionAggregate>> GetTopTrendingAsync(int topN = 25, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets mention aggregate for a specific contract address.
    /// </summary>
    /// <param name="coinMintAddress">The Solana contract address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mention aggregate if found, null otherwise</returns>
    Task<MentionAggregate?> GetByCoinMintAddressAsync(string coinMintAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tokens with mention activity in the last N minutes.
    /// </summary>
    /// <param name="minutesAgo">Time window in minutes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of mention aggregates with recent activity</returns>
    Task<List<MentionAggregate>> GetRecentlyMentionedAsync(int minutesAgo = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tokens with significant velocity changes (potential alert candidates).
    /// </summary>
    /// <param name="minimumTrendingScore">Minimum trending score threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of tokens exceeding the trending threshold</returns>
    Task<List<MentionAggregate>> GetByTrendingScoreThresholdAsync(double minimumTrendingScore, CancellationToken cancellationToken = default);
}
