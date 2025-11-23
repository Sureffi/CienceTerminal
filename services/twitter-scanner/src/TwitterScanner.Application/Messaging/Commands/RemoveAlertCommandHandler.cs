using CienceTerminal.AWS.Abstractions;
using CienceTerminal.AWS.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TwitterScanner.Application.Messaging.Commands;

public class RemoveAlertCommandHandler : IRequestHandler<RemoveAlertCommand>
{
    private readonly IEventProducer _eventProducer;
    private readonly ILogger<RemoveAlertCommandHandler> _logger;
    private readonly AwsOptions _awsOptions;

    public RemoveAlertCommandHandler(
        IEventProducer eventProducer,
        ILogger<RemoveAlertCommandHandler> logger,
        IOptions<AwsOptions> awsOptions)
    {
        _eventProducer = eventProducer;
        _logger = logger;
        _awsOptions = awsOptions.Value;
    }

    public async Task Handle(RemoveAlertCommand request, CancellationToken cancellationToken)
    {
        await _eventProducer.PublishAsync(_awsOptions.SNS.AlertRemovalTopicArn, request.AlertRemovalEvent, cancellationToken);

        _logger.LogInformation("Published alert removal for alert {AlertId} with reason: {Reason}",
            request.AlertRemovalEvent.AlertId, request.AlertRemovalEvent.Reason);
    }
}
