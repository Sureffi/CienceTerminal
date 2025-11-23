using AlertService.Application.Alerts;
using AlertService.Domain.Interfaces;
using CienceTerminal.Contracts.Enums;
using MediatR;

namespace AlertService.Application.Messaging.Queries;

public class GetTwitterAlertsQueryHandler : IRequestHandler<GetTwitterAlertsQuery, List<TwitterAlert>>
{
    private readonly IAlertManager _alertManager;

    public GetTwitterAlertsQueryHandler(IAlertManager alertManager)
    {
        _alertManager = alertManager;
    }

    public async Task<List<TwitterAlert>> Handle(GetTwitterAlertsQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _alertManager.GetActiveAlertsAsync();

        // Filter for Twitter alert types and cast to TwitterAlert
        var twitterAlerts = alerts
            .Where(a => a.Type == AlertType.TwitterLegit ||
                       a.Type == AlertType.TwitterSpam ||
                       a.Type == AlertType.TwitterPreLaunch)
            .Cast<TwitterAlert>()
            .ToList();

        return twitterAlerts;
    }
}
