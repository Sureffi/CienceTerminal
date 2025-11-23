using AlertService.Domain.Entities;
using AlertService.Domain.Models;
using CienceTerminal.Contracts.Events;

namespace AlertService.Domain.Interfaces;

public interface IAlertManager
{
    /// <summary>
    /// Initializes the alert manager by loading persisted alerts from database.
    /// Should be called once on service startup.
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<List<Alert>> GetActiveAlertsAsync();
    Task AddOrUpdateAlertAsync(Alert alert);
    Task AddTwitterAlertAsync(TwitterAlertData alertData);
    Task AddCaMentionAlertAsync(CaMentionAlertData alertData);

    Task RemoveAlertAsync(string alertId);
    Task RemoveAlertAsync(Guid alertId);
    Task RemoveAlertsByCoinAsync(string coinMintAddress);
}
