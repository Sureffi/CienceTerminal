using AlertService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlertService.Application.Messaging.Commands;

public class ProcessAlertRemovalCommandHandler : IRequestHandler<ProcessAlertRemovalCommand>
{
    private readonly IAlertManager _alertManager;
    private readonly ILogger<ProcessAlertRemovalCommandHandler> _logger;

    public ProcessAlertRemovalCommandHandler(
        IAlertManager alertManager,
        ILogger<ProcessAlertRemovalCommandHandler> logger)
    {
        _alertManager = alertManager;
        _logger = logger;
    }

    public async Task Handle(ProcessAlertRemovalCommand request, CancellationToken cancellationToken)
    {
        var removalEvent = request.AlertRemovalEvent;

        await _alertManager.RemoveAlertAsync(removalEvent.AlertId);

        _logger.LogInformation("Processed alert removal for alert {AlertId} with reason: {Reason} from source: {Source}",
            removalEvent.AlertId, removalEvent.Reason, removalEvent.Source);
    }
}
