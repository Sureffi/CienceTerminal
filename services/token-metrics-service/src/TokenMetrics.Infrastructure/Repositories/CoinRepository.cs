using Microsoft.EntityFrameworkCore;
using TokenMetrics.Application.Interfaces;
using TokenMetrics.Domain.Entities;
using TokenMetrics.Infrastructure.Data;

namespace TokenMetrics.Infrastructure.Repositories;

public class CoinRepository : ICoinRepository
{
    private readonly TokenMetricsDbContext _context;

    public CoinRepository(TokenMetricsDbContext context)
    {
        _context = context;
    }

    public async Task<Coin?> GetByMintAddressAsync(string coinMintAddress, CancellationToken cancellationToken = default)
    {
        return await _context.Coins
            .FirstOrDefaultAsync(c => c.CoinMintAddress == coinMintAddress, cancellationToken);
    }

    public async Task<List<Coin>> GetActiveCoinsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Coins
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Coin>> GetCoinsNeedingRefreshAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        return await _context.Coins
            .Where(c => c.IsActive && c.LastUpdated < olderThan)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Coin>> GetTopByMentionCountAsync(int top, CancellationToken cancellationToken = default)
    {
        return await _context.Coins
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.MentionCount24h)
            .Take(top)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Coin coin, CancellationToken cancellationToken = default)
    {
        await _context.Coins.AddAsync(coin, cancellationToken);
    }

    public Task UpdateAsync(Coin coin, CancellationToken cancellationToken = default)
    {
        _context.Coins.Update(coin);
        return Task.CompletedTask;
    }

    public async Task UpsertBatchAsync(List<Coin> coins, CancellationToken cancellationToken = default)
    {
        foreach (var coin in coins)
        {
            var existing = await GetByMintAddressAsync(coin.CoinMintAddress, cancellationToken);
            if (existing != null)
            {
                // Update existing record
                _context.Entry(existing).CurrentValues.SetValues(coin);
            }
            else
            {
                // Insert new record
                await _context.Coins.AddAsync(coin, cancellationToken);
            }
        }
    }

    public async Task CreateCoinAsync(string coinMintAddress, CancellationToken cancellationToken = default)
    {
        var coin = new Coin
        {
            Id = Guid.NewGuid(),
            CoinMintAddress = coinMintAddress,
            IsActive = true,
            IsBlacklisted = false,
            LastUpdated = DateTime.UtcNow
        };

        await _context.Coins.AddAsync(coin, cancellationToken);
    }

    public async Task SetActiveStatusAsync(string coinMintAddress, bool isActive, CancellationToken cancellationToken = default)
    {
        var coin = await GetByMintAddressAsync(coinMintAddress, cancellationToken);
        if (coin != null)
        {
            coin.IsActive = isActive;
            coin.LastUpdated = DateTime.UtcNow;
            _context.Coins.Update(coin);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
