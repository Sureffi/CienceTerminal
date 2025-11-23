using CienceTerminal.Contracts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TokenMetrics.Infrastructure.Data.Configurations;

public class CaMentionRecordConfiguration : IEntityTypeConfiguration<CaMentionRecord>
{
    public void Configure(EntityTypeBuilder<CaMentionRecord> builder)
    {
        builder.ToTable("ca_mention_records");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CoinMintAddress)
            .IsRequired()
            .HasMaxLength(44); // Solana address length

        builder.Property(x => x.TweetId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.AuthorId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProfilePicture)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.TweetUrl)
            .IsRequired()
            .HasMaxLength(200); // Twitter URL format: https://twitter.com/{username}/status/{tweetId}

        builder.Property(x => x.TweetContent)
            .HasMaxLength(30000); // Twitter Blue allows up to 25k chars, quoted tweets can be even longer

        builder.Property(x => x.IsOriginalPost)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();

        // Index for efficient queries by CA and time range
        builder.HasIndex(x => new { x.CoinMintAddress, x.Timestamp })
            .HasDatabaseName("ix_ca_mention_records_ca_timestamp");

        // Index for deduplication checks
        builder.HasIndex(x => new { x.TweetId, x.CoinMintAddress, x.AuthorId })
            .IsUnique()
            .HasDatabaseName("ix_ca_mention_records_tweet_ca_author_unique");
    }
}
