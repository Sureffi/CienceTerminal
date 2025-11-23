using AlertService.Application.Interfaces;
using AlertService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlertService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for querying CA mention records.
/// Provides read-only access to mention data stored by Token Metrics Service.
/// </summary>
public class CaMentionRecordRepository : ICaMentionRecordRepository
{
    private readonly TokenMetricsReadOnlyDbContext _context;

    public CaMentionRecordRepository(TokenMetricsReadOnlyDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<string>> GetTopMentionerProfilePicturesAsync(
        string coinMintAddress,
        int count,
        CancellationToken cancellationToken)
    {
        // Get mentions from the last 24 hours
        var since = DateTime.UtcNow.AddHours(-24);

        // Query top mentioners by follower count, get distinct profile pictures
        var profilePictures = await _context.CaMentionRecords
            .Where(r => r.CoinMintAddress == coinMintAddress && r.Timestamp >= since)
            .OrderByDescending(r => r.Followers)
            .ThenByDescending(r => r.IsVerified)
            .Select(r => r.ProfilePicture)
            .Where(pic => !string.IsNullOrEmpty(pic))
            .Distinct()
            .Take(count)
            .ToListAsync(cancellationToken);

        return profilePictures;
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, List<string>>> GetTopMentionerProfilePicturesBatchAsync(
        IEnumerable<string> coinMintAddresses,
        int count,
        CancellationToken cancellationToken)
    {
        var addressList = coinMintAddresses.ToList();
        var since = DateTime.UtcNow.AddHours(-24);

        // Query all mentions for the given coins in one go
        var mentionGroups = await _context.CaMentionRecords
            .Where(r => addressList.Contains(r.CoinMintAddress) && r.Timestamp >= since)
            .GroupBy(r => r.CoinMintAddress)
            .Select(g => new
            {
                CoinMintAddress = g.Key,
                ProfilePictures = g
                    .OrderByDescending(r => r.Followers)
                    .ThenByDescending(r => r.IsVerified)
                    .Select(r => r.ProfilePicture)
                    .Where(pic => !string.IsNullOrEmpty(pic))
                    .Distinct()
                    .Take(count)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        // Convert to dictionary
        return mentionGroups.ToDictionary(
            g => g.CoinMintAddress,
            g => g.ProfilePictures);
    }

    /// <inheritdoc />
    public async Task<List<AlertService.Domain.Entities.CaMentionRecord>> GetMentionsByCoinAsync(
        string coinMintAddress,
        int hours = 24,
        int limit = 9,
        CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow.AddHours(-hours);

        // Query mentions for the coin, filtered to original posts only
        // Ordered by verified status first, then by follower count
        var mentions = await _context.CaMentionRecords
            .Where(r => r.CoinMintAddress == coinMintAddress
                     && r.Timestamp >= since
                     && r.IsOriginalPost) // Only show original posts
            .OrderByDescending(r => r.IsVerified)
            .ThenByDescending(r => r.Followers)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return mentions;
    }
}
