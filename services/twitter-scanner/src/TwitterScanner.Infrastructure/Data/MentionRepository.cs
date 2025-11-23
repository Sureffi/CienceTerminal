using CienceTerminal.Contracts.Models;
using Microsoft.Extensions.Logging;
using TwitterScanner.Application.Interfaces;

namespace TwitterScanner.Infrastructure.Data;

public class MentionRepository : IMentionRepository
{
    private readonly MentionPersistenceDbContext _dbContext;
    private readonly ILogger<MentionRepository> _logger;

    public MentionRepository(
        MentionPersistenceDbContext dbContext,
        ILogger<MentionRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> AddMentionAsync(CaMentionRecord record, CancellationToken cancellationToken = default)
    {
        try
        {
            _dbContext.CaMentionRecords.Add(record);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Persisted CA mention: {CaAddress} from @{Username}",
                record.CoinMintAddress, record.Username);

            return true;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (
            ex.InnerException?.Message?.Contains("ix_ca_mention_records_tweet_ca_author_unique") == true ||
            ex.InnerException?.Message?.Contains("ix_ca_mention_records_tweet_ca_unique") == true ||
            ex.InnerException?.Message?.Contains("23505") == true) // PostgreSQL unique violation code
        {
            // Duplicate mentions (same tweet + CA) are silently ignored
            // Clear the failed entity from change tracker
            _dbContext.Entry(record).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            _logger.LogDebug(
                "Duplicate CA mention ignored: {CaAddress} from tweet {TweetId}",
                record.CoinMintAddress, record.TweetId);

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist CA mention for {CaAddress}", record.CoinMintAddress);
            throw;
        }
    }
}
