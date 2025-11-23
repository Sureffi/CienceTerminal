using AlertService.Domain.Entities;

namespace AlertService.Domain.Interfaces;

/// <summary>
/// Repository for persisting and loading alerts from database.
/// Enables alert state recovery after service restarts.
/// </summary>
public interface IAlertRepository
{
    /// <summary>
    /// Persists an alert to the database (upsert - inserts if new, updates if exists)
    /// </summary>
    Task AddAsync(Alert alert, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an alert from the database
    /// </summary>
    Task RemoveAsync(Guid alertId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads all active alerts from the database (for startup recovery)
    /// </summary>
    Task<List<Alert>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all alerts for a specific coin
    /// </summary>
    Task RemoveByCoinAsync(string coinMintAddress, CancellationToken cancellationToken = default);
}
