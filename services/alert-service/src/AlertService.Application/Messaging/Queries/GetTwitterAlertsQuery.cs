using AlertService.Application.Alerts;
using MediatR;

namespace AlertService.Application.Messaging.Queries;

public class GetTwitterAlertsQuery : IRequest<List<TwitterAlert>>
{
}
