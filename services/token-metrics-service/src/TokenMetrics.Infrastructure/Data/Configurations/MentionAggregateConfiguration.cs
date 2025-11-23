using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Infrastructure.Data.Configurations;

public class MentionAggregateConfiguration : IEntityTypeConfiguration<MentionAggregate>
{
    public void Configure(EntityTypeBuilder<MentionAggregate> builder)
    {
        builder.ToTable("mention_aggregates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CoinMintAddress)
            .HasColumnName("CoinMintAddress")
            .HasMaxLength(44) // Solana address length
            .IsRequired();

        builder.Property(x => x.MentionCount5m)
            .HasColumnName("MentionCount5m")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.MentionCount1h)
            .HasColumnName("MentionCount1h")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.MentionCount6h)
            .HasColumnName("MentionCount6h")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.MentionCount24h)
            .HasColumnName("MentionCount24h")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.TrendingScore)
            .HasColumnName("TrendingScore")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.Rank)
            .HasColumnName("Rank");

        builder.Property(x => x.LastMentioned)
            .HasColumnName("LastMentioned")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.LastCalculated)
            .HasColumnName("LastCalculated")
            .HasColumnType("timestamptz")
            .IsRequired();

        // Unique index on CoinMintAddress (business key)
        builder.HasIndex(x => x.CoinMintAddress)
            .HasDatabaseName("ix_mention_aggregates_coin_address")
            .IsUnique();

        // Index for trending score queries (top 25 retrieval)
        builder.HasIndex(x => x.TrendingScore)
            .HasDatabaseName("ix_mention_aggregates_trending_score")
            .IsDescending();

        // Partial index for rank queries (only top 25 have ranks)
        builder.HasIndex(x => x.Rank)
            .HasDatabaseName("ix_mention_aggregates_rank")
            .HasFilter("\"Rank\" IS NOT NULL");
    }
}
