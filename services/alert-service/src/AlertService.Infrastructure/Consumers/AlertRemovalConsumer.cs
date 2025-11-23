using Amazon.SQS;
using CienceTerminal.Contracts.Events;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.AWS.Services;
using AlertService.Application.Messaging.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertService.Infrastructure.Consumers;

public class AlertRemovalConsumer : SqsEventConsumer<AlertRemovalEvent>
{
    private readonly IMediator _mediator;

    public AlertRemovalConsumer(
        IAmazonSQS sqs,
        ILogger<AlertRemovalConsumer> logger,
        IMediator mediator,
        IOptions<AwsOptions> awsOptions)
        : base(sqs, logger, awsOptions.Value.SQS.AlertRemovalQueueUrl)
    {
        _mediator = mediator;
    }

    public override async Task HandleEventAsync(AlertRemovalEvent @event, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ProcessAlertRemovalCommand(@event), cancellationToken);
    }
}