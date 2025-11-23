using CienceTerminal.Contracts.Models;
using Microsoft.EntityFrameworkCore;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Infrastructure.Data;

public class TokenMetricsDbContext : DbContext
{
    public TokenMetricsDbContext(DbContextOptions<TokenMetricsDbContext> options)
        : base(options)
    {
    }

    public DbSet<CaMentionRecord> CaMentionRecords => Set<CaMentionRecord>();
    public DbSet<MentionAggregate> MentionAggregates => Set<MentionAggregate>();
    public DbSet<Coin> Coins => Set<Coin>();
    public DbSet<StoredAlert> Alerts => Set<StoredAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TokenMetricsDbContext).Assembly);

        // Configure read-only access to alerts table (owned by Alert Service)
        modelBuilder.Entity<StoredAlert>(entity =>
        {
            entity.ToTable("alerts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AlertType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CoinMintAddress).HasMaxLength(44);
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
