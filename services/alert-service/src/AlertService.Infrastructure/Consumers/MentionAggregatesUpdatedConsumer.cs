using Amazon.SQS;
using AlertService.Application.Messaging.Commands;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.AWS.Services;
using CienceTerminal.Contracts.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertService.Infrastructure.Consumers;

/// <summary>
/// Consumes MentionAggregatesUpdatedEvent from SQS and processes mention aggregates updates.
/// This event is published by Token Metrics Service after completing aggregation calculations.
/// </summary>
public class MentionAggregatesUpdatedConsumer : SqsEventConsumer<MentionAggregatesUpdatedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MentionAggregatesUpdatedConsumer(
        IAmazonSQS sqs,
        ILogger<MentionAggregatesUpdatedConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<AwsOptions> awsOptions)
        : base(sqs, logger, awsOptions.Value.SQS.MentionAggregatesUpdatedQueueUrl)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task HandleEventAsync(MentionAggregatesUpdatedEvent @event, CancellationToken cancellationToken)
    {
        // Create a scope to resolve scoped dependencies (like DbContext and repositories)
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new ProcessMentionAggregatesUpdateCommand(@event), cancellationToken);
    }
}
