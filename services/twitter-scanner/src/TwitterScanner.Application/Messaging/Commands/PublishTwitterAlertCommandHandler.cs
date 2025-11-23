using CienceTerminal.AWS.Abstractions;
using CienceTerminal.AWS.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TwitterScanner.Application.Messaging.Commands;

public class PublishTwitterAlertCommandHandler : IRequestHandler<PublishTwitterAlertCommand>
{
    private readonly IEventProducer _eventProducer;
    private readonly ILogger<PublishTwitterAlertCommandHandler> _logger;
    private readonly AwsOptions _awsOptions;

    public PublishTwitterAlertCommandHandler(
        IEventProducer eventProducer,
        ILogger<PublishTwitterAlertCommandHandler> logger,
        IOptions<AwsOptions> awsOptions)
    {
        _eventProducer = eventProducer;
        _logger = logger;
        _awsOptions = awsOptions.Value;
    }

    public async Task Handle(PublishTwitterAlertCommand request, CancellationToken cancellationToken)
    {
        await _eventProducer.PublishAsync(_awsOptions.SNS.TwitterAlertsTopicArn, request.AlertEvent, cancellationToken);

        _logger.LogInformation("Published Twitter alert {AlertType} for tweet {TweetId}",
            request.AlertEvent.Type, request.AlertEvent.TweetId);
    }
}
