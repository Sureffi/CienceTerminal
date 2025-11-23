using AlertService.Application.Interfaces;
using AlertService.Domain.Entities;
using AlertService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlertService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for querying mention aggregates.
/// Provides read-only access to trending data computed by Token Metrics Service.
/// </summary>
public class MentionAggregateRepository : IMentionAggregateRepository
{
    private readonly TokenMetricsReadOnlyDbContext _context;

    public MentionAggregateRepository(TokenMetricsReadOnlyDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<MentionAggregate>> GetTopTrendingAsync(int topN = 25, CancellationToken cancellationToken = default)
    {
        return await _context.MentionAggregates
            .OrderByDescending(m => m.TrendingScore)
            .Take(topN)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MentionAggregate?> GetByCoinMintAddressAsync(string coinMintAddress, CancellationToken cancellationToken = default)
    {
        return await _context.MentionAggregates
            .FirstOrDefaultAsync(m => m.CoinMintAddress == coinMintAddress, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<MentionAggregate>> GetRecentlyMentionedAsync(int minutesAgo = 5, CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddMinutes(-minutesAgo);

        return await _context.MentionAggregates
            .Where(m => m.LastMentioned >= threshold)
            .OrderByDescending(m => m.LastMentioned)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<MentionAggregate>> GetByTrendingScoreThresholdAsync(double minimumTrendingScore, CancellationToken cancellationToken = default)
    {
        return await _context.MentionAggregates
            .Where(m => m.TrendingScore >= minimumTrendingScore)
            .OrderByDescending(m => m.TrendingScore)
            .ToListAsync(cancellationToken);
    }
}
