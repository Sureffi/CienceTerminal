namespace TokenMetrics.Application.Interfaces;

/// <summary>
/// Read-only repository for querying Alert Service's alerts table.
/// Used to determine which coins have active alerts and need metrics updates.
/// </summary>
public interface IAlertRepository
{
    /// <summary>
    /// Gets distinct coin mint addresses from all active alerts.
    /// These are the coins that need metrics updates.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of unique coin mint addresses with active alerts</returns>
    Task<List<string>> GetActiveCoinsAsync(CancellationToken cancellationToken = default);
}
