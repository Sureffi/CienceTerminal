using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Application.Interfaces;

/// <summary>
/// Repository interface for managing MentionAggregate entities (trending metrics).
/// </summary>
public interface IMentionAggregateRepository
{
    /// <summary>
    /// Gets a mention aggregate by coin mint address.
    /// </summary>
    Task<MentionAggregate?> GetByMintAddressAsync(string coinMintAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all mention aggregates.
    /// </summary>
    Task<List<MentionAggregate>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top N aggregates ordered by trending score (descending).
    /// </summary>
    Task<List<MentionAggregate>> GetTopByTrendingScoreAsync(int top, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all aggregates currently in the top 25 (Rank is not null).
    /// </summary>
    Task<List<MentionAggregate>> GetTop25Async(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new mention aggregate to the database.
    /// </summary>
    Task AddAsync(MentionAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing mention aggregate.
    /// </summary>
    Task UpdateAsync(MentionAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch upserts mention aggregates (insert if not exists, update if exists).
    /// Efficient for bulk aggregation updates.
    /// </summary>
    Task UpsertBatchAsync(List<MentionAggregate> aggregates, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes aggregates that haven't been mentioned in X days (cleanup).
    /// </summary>
    Task DeleteStaleAggregatesAsync(DateTime olderThan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
