using AlertService.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlertService.Infrastructure.Services;

/// <summary>
/// Hosted service that initializes AlertManager on startup by loading persisted alerts from database.
/// Ensures alert state is restored after service restarts.
/// </summary>
public class AlertManagerInitializationService : IHostedService
{
    private readonly IAlertManager _alertManager;
    private readonly ILogger<AlertManagerInitializationService> _logger;

    public AlertManagerInitializationService(
        IAlertManager alertManager,
        ILogger<AlertManagerInitializationService> logger)
    {
        _alertManager = alertManager;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Initializing Alert Manager from database...");
            await _alertManager.InitializeAsync(cancellationToken);
            _logger.LogInformation("Alert Manager initialization complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Alert Manager");
            // Don't throw - service should still start even if alert loading fails
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // No cleanup needed
        return Task.CompletedTask;
    }
}
