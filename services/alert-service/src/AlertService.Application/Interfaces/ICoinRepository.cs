using AlertService.Domain.Entities;

namespace AlertService.Application.Interfaces;

/// <summary>
/// Repository for querying coin data from the shared cienceterminal database.
/// Alert Service has read access to all coin metadata (owned by Token Metrics Service)
/// and write access to the IsActive flag (owned by Alert Service).
/// IsActive is managed based on active alert count - true when alerts exist, false when no alerts.
/// </summary>
public interface ICoinRepository
{
    /// <summary>
    /// Gets a coin by its mint address.
    /// </summary>
    /// <param name="coinMintAddress">The Solana contract address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Coin if found, null otherwise</returns>
    Task<Coin?> GetByMintAddressAsync(string coinMintAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple coins by their mint addresses in a single query.
    /// </summary>
    /// <param name="coinMintAddresses">List of Solana contract addresses</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary mapping mint address to coin data</returns>
    Task<Dictionary<string, Coin>> GetByMintAddressesAsync(IEnumerable<string> coinMintAddresses, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a coin is blacklisted.
    /// </summary>
    /// <param name="coinMintAddress">The Solana contract address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if blacklisted, false otherwise</returns>
    Task<bool> IsBlacklistedAsync(string coinMintAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all coins that are marked as active (IsActive = true).
    /// Used by startup recovery to clean up stale activation state after service restart
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active coins</returns>
    Task<List<Coin>> GetActiveCoinsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the IsActive flag for a coin. Does nothing if coin doesn't exist.
    /// Alert Service owns this flag - it reflects whether the coin has active alerts.
    /// The coin must already exist in the database (created by Token Metrics Service).
    /// </summary>
    /// <param name="coinMintAddress">The Solana contract address</param>
    /// <param name="isActive">True if coin should be active, false otherwise</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetActiveStatusAsync(string coinMintAddress, bool isActive, CancellationToken cancellationToken = default);
}
