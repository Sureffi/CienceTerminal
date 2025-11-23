using Amazon.SQS;
using CienceTerminal.Contracts.Events;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.AWS.Services;
using AlertService.Application.Messaging.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertService.Infrastructure.Consumers;

public class CoinBlacklistedConsumer : SqsEventConsumer<CoinBlacklistedEvent>
{
    private readonly IMediator _mediator;

    public CoinBlacklistedConsumer(
        IAmazonSQS sqs,
        ILogger<CoinBlacklistedConsumer> logger,
        IMediator mediator,
        IOptions<AwsOptions> awsOptions)
        : base(sqs, logger, awsOptions.Value.SQS.CoinBlacklistedQueueUrl)
    {
        _mediator = mediator;
    }

    public override async Task HandleEventAsync(CoinBlacklistedEvent @event, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ProcessCoinBlacklistedCommand(@event), cancellationToken);
    }
}
