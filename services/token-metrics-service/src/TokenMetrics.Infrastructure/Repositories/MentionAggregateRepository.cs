using Microsoft.EntityFrameworkCore;
using TokenMetrics.Application.Interfaces;
using TokenMetrics.Domain.Entities;
using TokenMetrics.Infrastructure.Data;

namespace TokenMetrics.Infrastructure.Repositories;

public class MentionAggregateRepository : IMentionAggregateRepository
{
    private readonly TokenMetricsDbContext _context;

    public MentionAggregateRepository(TokenMetricsDbContext context)
    {
        _context = context;
    }

    public async Task<MentionAggregate?> GetByMintAddressAsync(string coinMintAddress, CancellationToken cancellationToken = default)
    {
        return await _context.MentionAggregates
            .FirstOrDefaultAsync(ma => ma.CoinMintAddress == coinMintAddress, cancellationToken);
    }

    public async Task<List<MentionAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MentionAggregates.ToListAsync(cancellationToken);
    }

    public async Task<List<MentionAggregate>> GetTopByTrendingScoreAsync(int top, CancellationToken cancellationToken = default)
    {
        return await _context.MentionAggregates
            .OrderByDescending(ma => ma.TrendingScore)
            .Take(top)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MentionAggregate>> GetTop25Async(CancellationToken cancellationToken = default)
    {
        return await _context.MentionAggregates
            .Where(ma => ma.Rank != null)
            .OrderBy(ma => ma.Rank)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MentionAggregate aggregate, CancellationToken cancellationToken = default)
    {
        await _context.MentionAggregates.AddAsync(aggregate, cancellationToken);
    }

    public Task UpdateAsync(MentionAggregate aggregate, CancellationToken cancellationToken = default)
    {
        _context.MentionAggregates.Update(aggregate);
        return Task.CompletedTask;
    }

    public async Task UpsertBatchAsync(List<MentionAggregate> aggregates, CancellationToken cancellationToken = default)
    {
        foreach (var aggregate in aggregates)
        {
            var existing = await GetByMintAddressAsync(aggregate.CoinMintAddress, cancellationToken);
            if (existing != null)
            {
                // Update existing record (preserve Id)
                aggregate.Id = existing.Id;
                _context.Entry(existing).CurrentValues.SetValues(aggregate);
            }
            else
            {
                // Insert new record
                await _context.MentionAggregates.AddAsync(aggregate, cancellationToken);
            }
        }
    }

    public async Task DeleteStaleAggregatesAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var staleAggregates = await _context.MentionAggregates
            .Where(ma => ma.LastMentioned < olderThan)
            .ToListAsync(cancellationToken);

        _context.MentionAggregates.RemoveRange(staleAggregates);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
