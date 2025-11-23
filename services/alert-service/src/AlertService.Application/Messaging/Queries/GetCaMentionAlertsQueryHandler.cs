using AlertService.Application.Alerts;
using AlertService.Domain.Interfaces;
using CienceTerminal.Contracts.Enums;
using MediatR;

namespace AlertService.Application.Messaging.Queries;

public class GetCaMentionAlertsQueryHandler : IRequestHandler<GetCaMentionAlertsQuery, List<CaMentionAlert>>
{
    private readonly IAlertManager _alertManager;

    public GetCaMentionAlertsQueryHandler(IAlertManager alertManager)
    {
        _alertManager = alertManager;
    }

    public async Task<List<CaMentionAlert>> Handle(GetCaMentionAlertsQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _alertManager.GetActiveAlertsAsync();

        // Filter for CA mention alerts and cast to CaMentionAlert
        var caMentionAlerts = alerts
            .Where(a => a.Type == AlertType.CaMention)
            .Cast<CaMentionAlert>()
            .ToList();

        return caMentionAlerts;
    }
}
