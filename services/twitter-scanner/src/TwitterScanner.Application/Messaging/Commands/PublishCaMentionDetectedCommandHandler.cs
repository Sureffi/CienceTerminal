using CienceTerminal.AWS.Abstractions;
using CienceTerminal.AWS.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TwitterScanner.Application.Messaging.Commands;

// TODO: Is this event even needed?
public class PublishCaMentionDetectedCommandHandler : IRequestHandler<PublishCaMentionDetectedCommand>
{
    private readonly IEventProducer _eventProducer;
    private readonly ILogger<PublishCaMentionDetectedCommandHandler> _logger;
    private readonly AwsOptions _awsOptions;

    public PublishCaMentionDetectedCommandHandler(
        IEventProducer eventProducer,
        ILogger<PublishCaMentionDetectedCommandHandler> logger,
        IOptions<AwsOptions> awsOptions)
    {
        _eventProducer = eventProducer;
        _logger = logger;
        _awsOptions = awsOptions.Value;
    }

    public async Task Handle(PublishCaMentionDetectedCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _eventProducer.PublishAsync(
                _awsOptions.SNS.CaMentionDetectedTopicArn,
                request.Event,
                cancellationToken);

            _logger.LogInformation(
                "Published CA mention detected event for CA {CaAddress} in tweet {TweetId}",
                request.Event.CoinMintAddress,
                request.Event.TweetId);
        }
        catch (Exception ex)
        {
            // Log error but don't throw - prioritize database persistence over event delivery
            _logger.LogError(ex,
                "Failed to publish CA mention detected event for CA {CaAddress} in tweet {TweetId}. Event will not be retried.",
                request.Event.CoinMintAddress,
                request.Event.TweetId);
        }
    }
}
