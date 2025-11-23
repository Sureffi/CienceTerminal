using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Application.Interfaces;

/// <summary>
/// Repository interface for managing Coin entities (token metadata).
/// </summary>
public interface ICoinRepository
{
    /// <summary>
    /// Gets a coin by its mint address.
    /// </summary>
    Task<Coin?> GetByMintAddressAsync(string coinMintAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active coins (IsActive = true).
    /// </summary>
    Task<List<Coin>> GetActiveCoinsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets coins that need metadata refresh (LastUpdated older than threshold).
    /// </summary>
    Task<List<Coin>> GetCoinsNeedingRefreshAsync(DateTime olderThan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new coin to the database.
    /// </summary>
    Task AddAsync(Coin coin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing coin.
    /// </summary>
    Task UpdateAsync(Coin coin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch upserts coins (insert if not exists, update if exists).
    /// Efficient for bulk metadata enrichment.
    /// </summary>
    Task UpsertBatchAsync(List<Coin> coins, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new coin with minimal data (just mint address and IsActive = true).
    /// Used when a coin is first mentioned/alerted.
    /// </summary>
    Task CreateCoinAsync(string coinMintAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
