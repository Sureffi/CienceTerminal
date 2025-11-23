using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using AlertService.Infrastructure.Hubs;
using CienceTerminal.Contracts.Enums;
using Microsoft.AspNetCore.SignalR;

namespace AlertService.Infrastructure.Services;

public class AlertNotificationService : IAlertNotificationService
{
    private readonly IHubContext<TwitterAlertHub> _twitterHubContext;
    private readonly IHubContext<CaMentionAlertHub> _caMentionHubContext;

    public AlertNotificationService(
        IHubContext<TwitterAlertHub> twitterHubContext,
        IHubContext<CaMentionAlertHub> caMentionHubContext)
    {
        _twitterHubContext = twitterHubContext;
        _caMentionHubContext = caMentionHubContext;
    }

    public async Task NotifyAlertAddedAsync(Alert alert)
    {
        // Route to appropriate hub based on alert type
        if (alert.Type is AlertType.TwitterLegit or AlertType.TwitterSpam)
        {
            await NotifyTwitterAlertAsync(alert);
        }
        else if (alert.Type == AlertType.CaMention)
        {
            await NotifyCaMentionAlertAsync(alert);
        }
    }

    public async Task NotifyAlertRemovedAsync(Guid alertId)
    {
        // Send removal notification to both hubs (they'll handle it appropriately)
        await _twitterHubContext.Clients.All.SendAsync("AlertRemoved", alertId);
        await _caMentionHubContext.Clients.All.SendAsync("AlertRemoved", alertId);
    }

    private async Task NotifyTwitterAlertAsync(Alert alert)
    {
        // Send to all connected clients immediately
        await _twitterHubContext.Clients.All.SendAsync("AlertAdded", alert);
    }

    private async Task NotifyCaMentionAlertAsync(Alert alert)
    {
        // Send to all connected clients immediately
        await _caMentionHubContext.Clients.All.SendAsync("AlertAdded", alert);
    }
}
