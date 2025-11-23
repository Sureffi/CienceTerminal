using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Application.Interfaces;

/// <summary>
/// Repository for managing CA aggregate data (trending metrics and counters).
/// </summary>
public interface ICaAggregateRepository
{
    /// <summary>
    /// Gets an aggregate by CA address, or null if not found.
    /// </summary>
    Task<CaAggregate?> GetByCaAddressAsync(string caAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new aggregate.
    /// </summary>
    Task AddAsync(CaAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing aggregate.
    /// </summary>
    Task UpdateAsync(CaAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
