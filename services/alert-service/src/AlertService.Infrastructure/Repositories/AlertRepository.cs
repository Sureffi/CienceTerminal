using System.Text.Json;
using AlertService.Application.Alerts;
using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using AlertService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlertService.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AlertServiceDbContext _context;
    private readonly ILogger<AlertRepository> _logger;

    // JSON serializer options for polymorphic Alert types
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public AlertRepository(AlertServiceDbContext context, ILogger<AlertRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        // Check if alert already exists (upsert behavior)
        var existing = await _context.StoredAlerts.FindAsync(new object[] { alert.Id }, cancellationToken);

        if (existing != null)
        {
            // Update existing alert
            existing.AlertType = alert.Type.ToString();
            existing.AlertData = JsonSerializer.Serialize(alert, alert.GetType(), _jsonOptions);
            existing.CreatedAt = alert.Timestamp;
            existing.CoinMintAddress = ExtractCoinMintAddress(alert);

            _context.StoredAlerts.Update(existing);
            _logger.LogDebug("Updated alert {AlertId} of type {AlertType} in database", alert.Id, alert.Type);
        }
        else
        {
            // Insert new alert
            var storedAlert = new StoredAlert
            {
                Id = alert.Id,
                AlertType = alert.Type.ToString(),
                AlertData = JsonSerializer.Serialize(alert, alert.GetType(), _jsonOptions),
                CreatedAt = alert.Timestamp,
                CoinMintAddress = ExtractCoinMintAddress(alert)
            };

            _context.StoredAlerts.Add(storedAlert);
            _logger.LogDebug("Inserted new alert {AlertId} of type {AlertType} into database", alert.Id, alert.Type);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        var storedAlert = await _context.StoredAlerts.FindAsync(new object[] { alertId }, cancellationToken);
        if (storedAlert != null)
        {
            _context.StoredAlerts.Remove(storedAlert);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Removed alert {AlertId} from database", alertId);
        }
    }

    public async Task<List<Alert>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var storedAlerts = await _context.StoredAlerts
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        var alerts = new List<Alert>();
        foreach (var stored in storedAlerts)
        {
            try
            {
                var alert = DeserializeAlert(stored);
                if (alert != null)
                {
                    alerts.Add(alert);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize alert {AlertId} of type {AlertType}",
                    stored.Id, stored.AlertType);
            }
        }

        _logger.LogInformation("Loaded {Count} alerts from database", alerts.Count);
        return alerts;
    }

    public async Task RemoveByCoinAsync(string coinMintAddress, CancellationToken cancellationToken = default)
    {
        var alertsToRemove = await _context.StoredAlerts
            .Where(a => a.CoinMintAddress == coinMintAddress)
            .ToListAsync(cancellationToken);

        if (alertsToRemove.Any())
        {
            _context.StoredAlerts.RemoveRange(alertsToRemove);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Removed {Count} alerts for coin {CoinMint} from database",
                alertsToRemove.Count, coinMintAddress);
        }
    }

    /// <summary>
    /// Deserializes a StoredAlert back into a concrete Alert subclass
    /// </summary>
    private Alert? DeserializeAlert(StoredAlert stored)
    {
        // Determine concrete type based on AlertType discriminator
        var alertType = stored.AlertType switch
        {
            "CaMention" => typeof(CaMentionAlert),
            "TwitterLegit" => typeof(TwitterAlert),
            "TwitterSpam" => typeof(TwitterAlert),
            "TwitterPreLaunch" => typeof(TwitterAlert),
            _ => null
        };

        if (alertType == null)
        {
            _logger.LogWarning("Unknown alert type {AlertType} for alert {AlertId}",
                stored.AlertType, stored.Id);
            return null;
        }

        return JsonSerializer.Deserialize(stored.AlertData, alertType, _jsonOptions) as Alert;
    }

    /// <summary>
    /// Extracts coin mint address from alert for denormalized storage
    /// </summary>
    private string? ExtractCoinMintAddress(Alert alert)
    {
        return alert switch
        {
            CaMentionAlert caMention => caMention.CoinAddress,
            TwitterAlert twitter when !string.IsNullOrEmpty(twitter.CoinMint) => twitter.CoinMint,
            _ => null
        };
    }
}
