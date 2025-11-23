using AlertService.Application.Interfaces;
using AlertService.Domain.Entities;
using AlertService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlertService.Infrastructure.Repositories;

/// <summary>
/// Repository for querying coin data and managing IsActive flag.
/// Alert Service has read access to all coin metadata (owned by Token Metrics Service)
/// and write access to IsActive flag (owned by Alert Service).
/// </summary>
public class CoinRepository : ICoinRepository
{
    private readonly TokenMetricsReadOnlyDbContext _context;
    private readonly ILogger<CoinRepository> _logger;

    public CoinRepository(TokenMetricsReadOnlyDbContext context, ILogger<CoinRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Coin>> GetActiveCoinsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Coins
            .Where(static c => c.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Coin?> GetByMintAddressAsync(string coinMintAddress, CancellationToken cancellationToken = default)
    {
        return await _context.Coins
            .FirstOrDefaultAsync(c => c.CoinMintAddress == coinMintAddress, cancellationToken);
    }

    public async Task<Dictionary<string, Coin>> GetByMintAddressesAsync(
        IEnumerable<string> coinMintAddresses,
        CancellationToken cancellationToken = default)
    {
        var addressList = coinMintAddresses.ToList();

        var coins = await _context.Coins
            .Where(c => addressList.Contains(c.CoinMintAddress))
            .ToListAsync(cancellationToken);

        return coins.ToDictionary(c => c.CoinMintAddress, c => c);
    }

    public async Task<bool> IsBlacklistedAsync(string coinMintAddress, CancellationToken cancellationToken = default)
    {
        return await _context.Coins
            .Where(c => c.CoinMintAddress == coinMintAddress)
            .Select(c => c.IsBlacklisted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SetActiveStatusAsync(string coinMintAddress, bool isActive, CancellationToken cancellationToken = default)
    {
        // Use tracking query to enable updates
        var coin = await _context.Coins
            .AsTracking()
            .FirstOrDefaultAsync(c => c.CoinMintAddress == coinMintAddress, cancellationToken);

        if (coin == null)
        {
            // Coin doesn't exist yet - Token Metrics Service should create it first
            _logger.LogWarning(
                "Cannot set IsActive for {CoinMint} - coin not found in database. " +
                "Token Metrics Service should create coins before Alert Service activates them.",
                coinMintAddress);
            return;
        }

        if (coin.IsActive == isActive)
        {
            _logger.LogDebug(
                "Coin {CoinMint} IsActive already {IsActive}, no update needed",
                coinMintAddress, isActive);
            return;
        }

        // Update ONLY IsActive and LastUpdated - mark only these properties as modified
        _logger.LogInformation(
            "Updating coin {CoinMint} IsActive: {OldValue} → {NewValue}",
            coinMintAddress, coin.IsActive, isActive);

        coin.IsActive = isActive;
        coin.LastUpdated = DateTime.UtcNow;

        // Explicitly mark only these two properties as modified
        var entry = _context.Entry(coin);
        entry.Property(c => c.IsActive).IsModified = true;
        entry.Property(c => c.LastUpdated).IsModified = true;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
