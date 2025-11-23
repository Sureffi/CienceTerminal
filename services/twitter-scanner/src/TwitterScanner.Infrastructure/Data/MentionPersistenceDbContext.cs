using CienceTerminal.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace TwitterScanner.Infrastructure.Data;

/// <summary>
/// DbContext for Twitter Scanner to persist CA mentions to shared database.
/// Only writes to ca_mention_records table.
/// </summary>
public class MentionPersistenceDbContext : DbContext
{
    public MentionPersistenceDbContext(DbContextOptions<MentionPersistenceDbContext> options)
        : base(options)
    {
    }

    public DbSet<CaMentionRecord> CaMentionRecords => Set<CaMentionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Same schema as Token Metrics Service
        var builder = modelBuilder.Entity<CaMentionRecord>();

        builder.ToTable("ca_mention_records");
        builder.HasKey(static x => x.Id);

        builder.Property(static x => x.CoinMintAddress)
            .IsRequired()
            .HasMaxLength(44);

        builder.Property(static x => x.TweetId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(static x => x.AuthorId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(static x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(static x => x.ProfilePicture)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(static x => x.TweetUrl)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(static x => x.TweetContent)
            .HasMaxLength(30000); // Twitter Blue allows up to 25k chars, quoted tweets can be even longer

        builder.Property(static x => x.IsOriginalPost)
            .IsRequired();

        builder.Property(static x => x.Timestamp)
            .IsRequired();

        builder.HasIndex(static x => new { x.CoinMintAddress, x.Timestamp })
            .HasDatabaseName("ix_ca_mention_records_ca_timestamp");

        builder.HasIndex(static x => new { x.TweetId, x.CoinMintAddress, x.AuthorId })
            .IsUnique()
            .HasDatabaseName("ix_ca_mention_records_tweet_ca_author_unique");
    }
}
