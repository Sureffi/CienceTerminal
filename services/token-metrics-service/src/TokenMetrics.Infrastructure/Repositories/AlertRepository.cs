using Microsoft.EntityFrameworkCore;
using TokenMetrics.Application.Interfaces;
using TokenMetrics.Infrastructure.Data;

namespace TokenMetrics.Infrastructure.Repositories;

/// <summary>
/// Read-only repository for querying Alert Service's alerts table.
/// </summary>
public class AlertRepository : IAlertRepository
{
    private readonly TokenMetricsDbContext _context;

    public AlertRepository(TokenMetricsDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetActiveCoinsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .Where(a => a.CoinMintAddress != null)
            .Select(a => a.CoinMintAddress!)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
