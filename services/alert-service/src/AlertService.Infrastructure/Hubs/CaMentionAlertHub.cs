using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using CienceTerminal.Contracts.Enums;
using Microsoft.AspNetCore.SignalR;

namespace AlertService.Infrastructure.Hubs;

public class CaMentionAlertHub : Hub
{
    private readonly IAlertManager _alertManager;

    public CaMentionAlertHub(IAlertManager alertManager)
    {
        _alertManager = alertManager;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;

        // Store user metadata in connection
        Context.Items["UserId"] = userId;

        await base.OnConnectedAsync();
    }

    public async Task<List<Alert>> GetActiveCaMentionAlerts()
    {
        var allAlerts = await _alertManager.GetActiveAlertsAsync();
        return allAlerts.Where(a => a.Type == AlertType.CaMention).ToList();
    }

    public async Task RemoveAlert(string alertId)
    {
        await _alertManager.RemoveAlertAsync(alertId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
