using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TokenMetrics.Infrastructure.Data;

/// <summary>
/// Design-time factory for EF Core migrations.
/// </summary>
public class TokenMetricsDbContextFactory : IDesignTimeDbContextFactory<TokenMetricsDbContext>
{
    public TokenMetricsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TokenMetricsDbContext>();

        // Use connection string from appsettings or environment
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=cienceterminal;Username=postgres;Password=postgres");

        return new TokenMetricsDbContext(optionsBuilder.Options);
    }
}
