using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Infrastructure.Data.Configurations;

public class CoinConfiguration : IEntityTypeConfiguration<Coin>
{
    public void Configure(EntityTypeBuilder<Coin> builder)
    {
        builder.ToTable("coins");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.CoinMintAddress)
            .HasColumnName("CoinMintAddress")
            .HasMaxLength(44)
            .IsRequired();

        builder.Property(c => c.CoinSymbol)
            .HasColumnName("CoinSymbol")
            .HasMaxLength(50);

        builder.Property(c => c.CoinImage)
            .HasColumnName("CoinImage")
            .HasMaxLength(500);

        builder.Property(c => c.MentionCount24h)
            .HasColumnName("MentionCount24h")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(c => c.HolderCount)
            .HasColumnName("HolderCount");

        builder.Property(c => c.Liquidity)
            .HasColumnName("Liquidity")
            .HasPrecision(28, 8); // Support very large numbers with 8 decimal places

        builder.Property(c => c.Volume24h)
            .HasColumnName("Volume24h")
            .HasPrecision(28, 8);

        builder.Property(c => c.MarketCap)
            .HasColumnName("MarketCap")
            .HasPrecision(28, 8);

        builder.Property(c => c.PriceChange24H)
            .HasColumnName("PriceChange24H")
            .HasPrecision(10, 4); // -999999.9999 to 999999.9999 (percentage change)

        builder.Property(c => c.TopHoldersPercentage)
            .HasColumnName("TopHoldersPercentage")
            .HasPrecision(5, 2); // 0.00 to 100.00

        builder.Property(c => c.FirstPoolCreatedAt)
            .HasColumnName("FirstPoolCreatedAt")
            .HasColumnType("timestamptz");

        builder.Property(c => c.LastUpdated)
            .HasColumnName("LastUpdated")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(c => c.IsBlacklisted)
            .HasColumnName("IsBlacklisted")
            .HasDefaultValue(false)
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.CoinMintAddress)
            .HasDatabaseName("ix_coins_mint_address")
            .IsUnique();

        builder.HasIndex(c => c.MentionCount24h)
            .HasDatabaseName("ix_coins_mention_count")
            .IsDescending()
            .HasFilter("\"IsActive\" = true"); // Partial index for active coins only

        builder.HasIndex(c => c.LastUpdated)
            .HasDatabaseName("ix_coins_last_updated");
    }
}
