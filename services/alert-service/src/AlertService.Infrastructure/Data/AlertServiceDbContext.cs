using AlertService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlertService.Infrastructure.Data;

/// <summary>
/// DbContext for Alert Service's own data (alerts persistence).
/// This context manages data owned by the Alert Service.
/// </summary>
public class AlertServiceDbContext : DbContext
{
    public AlertServiceDbContext(DbContextOptions<AlertServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<StoredAlert> StoredAlerts => Set<StoredAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoredAlert>(entity =>
        {
            entity.ToTable("alerts");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.AlertType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.AlertData)
                .IsRequired()
                .HasColumnType("jsonb");

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.CoinMintAddress)
                .HasMaxLength(44);

            // Index on CreatedAt for cleanup queries
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("ix_alerts_created_at");

            // Index on AlertType for type-specific queries
            entity.HasIndex(e => e.AlertType)
                .HasDatabaseName("ix_alerts_type");

            // Index on CoinMintAddress for coin-specific removal
            entity.HasIndex(e => e.CoinMintAddress)
                .HasDatabaseName("ix_alerts_coin_mint_address")
                .HasFilter("\"CoinMintAddress\" IS NOT NULL");
        });
    }
}
