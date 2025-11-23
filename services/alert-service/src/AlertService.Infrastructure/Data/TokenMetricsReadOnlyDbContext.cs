using AlertService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlertService.Infrastructure.Data;

/// <summary>
/// DbContext for querying data from the shared cienceterminal database.
/// Alert Service has read-only access to all data, with write access ONLY to Coin.IsActive field.
/// IsActive is owned by Alert Service and reflects whether a coin has active alerts.
/// All other fields are owned by Token Metrics Service.
/// </summary>
public class TokenMetricsReadOnlyDbContext : DbContext
{
    public TokenMetricsReadOnlyDbContext(DbContextOptions<TokenMetricsReadOnlyDbContext> options)
        : base(options)
    {
    }

    public DbSet<MentionAggregate> MentionAggregates => Set<MentionAggregate>();
    public DbSet<Coin> Coins => Set<Coin>();
    public DbSet<CaMentionRecord> CaMentionRecords => Set<CaMentionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure MentionAggregate entity to map to existing table
        modelBuilder.Entity<MentionAggregate>(entity =>
        {
            entity.ToTable("mention_aggregates");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CoinMintAddress)
                .IsRequired()
                .HasMaxLength(44);

            entity.Property(e => e.MentionCount5m)
                .HasDefaultValue(0.0);

            entity.Property(e => e.MentionCount1h)
                .HasDefaultValue(0.0);

            entity.Property(e => e.MentionCount6h)
                .HasDefaultValue(0.0);

            entity.Property(e => e.MentionCount24h)
                .HasDefaultValue(0.0);

            entity.Property(e => e.TrendingScore)
                .HasDefaultValue(0.0);

            entity.Property(e => e.Rank)
                .IsRequired(false);

            entity.Property(e => e.LastMentioned)
                .IsRequired();

            entity.Property(e => e.LastCalculated)
                .IsRequired();

            // Indexes (read-only, just for query optimization hints)
            entity.HasIndex(e => e.CoinMintAddress)
                .IsUnique()
                .HasDatabaseName("ix_mention_aggregates_coin_address");

            entity.HasIndex(e => e.Rank)
                .HasDatabaseName("ix_mention_aggregates_rank")
                .HasFilter("[Rank] IS NOT NULL");

            entity.HasIndex(e => e.TrendingScore)
                .IsDescending()
                .HasDatabaseName("ix_mention_aggregates_trending_score");
        });

        // Configure Coin entity to map to existing table
        modelBuilder.Entity<Coin>(entity =>
        {
            entity.ToTable("coins");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CoinMintAddress)
                .IsRequired()
                .HasMaxLength(44);

            entity.Property(e => e.CoinSymbol)
                .HasMaxLength(20);

            entity.Property(e => e.CoinImage)
                .HasMaxLength(500);

            entity.Property(e => e.MentionCount24h)
                .HasDefaultValue(0);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.IsBlacklisted)
                .HasDefaultValue(false);

            entity.Property(e => e.LastUpdated)
                .IsRequired();

            // Indexes (read-only, just for query optimization hints)
            entity.HasIndex(e => e.CoinMintAddress)
                .IsUnique()
                .HasDatabaseName("ix_coins_coin_mint_address");

            entity.HasIndex(e => new { e.MentionCount24h, e.IsActive })
                .HasDatabaseName("ix_coins_mention_count_active")
                .HasFilter("[IsActive] = true");

            entity.HasIndex(e => e.LastUpdated)
                .HasDatabaseName("ix_coins_last_updated");
        });

        // Configure CaMentionRecord entity to map to existing table
        modelBuilder.Entity<CaMentionRecord>(entity =>
        {
            entity.ToTable("ca_mention_records");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CoinMintAddress)
                .IsRequired()
                .HasMaxLength(44);

            entity.Property(e => e.TweetId)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.AuthorId)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ProfilePicture)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.TweetUrl)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.TweetContent)
                .HasMaxLength(30000);

            entity.Property(e => e.Timestamp)
                .IsRequired();

            // Indexes (read-only, just for query optimization hints)
            entity.HasIndex(e => new { e.CoinMintAddress, e.Timestamp })
                .HasDatabaseName("ix_ca_mention_records_coin_timestamp");

            entity.HasIndex(e => new { e.CoinMintAddress, e.TweetId, e.AuthorId })
                .IsUnique()
                .HasDatabaseName("ix_ca_mention_records_unique_mention");
        });
    }

    /// <summary>
    /// Override SaveChanges to validate only allowed modifications.
    /// Alert Service may only modify Coin.IsActive and Coin.LastUpdated fields.
    /// </summary>
    public override int SaveChanges()
    {
        ValidateChanges();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to validate only allowed modifications.
    /// Alert Service may only modify Coin.IsActive and Coin.LastUpdated fields.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ValidateChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Validates that only allowed fields are being modified.
    /// Alert Service can only modify Coin.IsActive and Coin.LastUpdated.
    /// </summary>
    private void ValidateChanges()
    {
        var entries = ChangeTracker.Entries<Coin>()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            var modifiedProperties = entry.Properties
                .Where(p => p.IsModified)
                .Select(p => p.Metadata.Name)
                .ToList();

            // Only IsActive and LastUpdated can be modified
            var invalidProperties = modifiedProperties
                .Where(p => p != nameof(Coin.IsActive) && p != nameof(Coin.LastUpdated))
                .ToList();

            if (invalidProperties.Any())
            {
                throw new InvalidOperationException(
                    $"Alert Service can only modify Coin.IsActive and Coin.LastUpdated fields. " +
                    $"Attempted to modify: {string.Join(", ", invalidProperties)}");
            }
        }

        // Validate no other entities are being modified (MentionAggregates and CaMentionRecords are read-only)
        var nonCoinModifications = ChangeTracker.Entries()
            .Where(e => e.Entity is not Coin &&
                       (e.State == EntityState.Modified || e.State == EntityState.Deleted || e.State == EntityState.Added))
            .ToList();

        if (nonCoinModifications.Any())
        {
            var entityTypes = string.Join(", ", nonCoinModifications.Select(e => e.Entity.GetType().Name));
            throw new InvalidOperationException(
                $"Alert Service can only modify Coin entities. MentionAggregates and CaMentionRecords are read-only. Attempted to modify: {entityTypes}");
        }
    }
}
