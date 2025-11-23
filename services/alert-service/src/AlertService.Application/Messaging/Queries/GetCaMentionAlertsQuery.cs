using AlertService.Application.Alerts;
using MediatR;

namespace AlertService.Application.Messaging.Queries;

public class GetCaMentionAlertsQuery : IRequest<List<CaMentionAlert>>
{
}
