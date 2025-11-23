using Amazon.SQS;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.AWS.Services;
using CienceTerminal.Contracts.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TokenMetrics.Application.Messaging.Commands;

namespace TokenMetrics.Infrastructure.Consumers;

/// <summary>
/// Consumes CaMentionDetectedEvent from SQS and processes CA mentions.
/// This event is published by Twitter Scanner when a CA is detected in a tweet.
/// Responsible for creating Coin records and inserting CaMentionRecords.
/// </summary>
public class CaMentionDetectedConsumer : SqsEventConsumer<CaMentionDetectedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CaMentionDetectedConsumer(
        IAmazonSQS sqs,
        ILogger<CaMentionDetectedConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<AwsOptions> awsOptions)
        : base(sqs, logger, awsOptions.Value.SQS.CaMentionDetectedQueueUrl)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task HandleEventAsync(CaMentionDetectedEvent @event, CancellationToken cancellationToken)
    {
        // Create a scope to resolve scoped dependencies (like DbContext and repositories)
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new ProcessCaMentionDetectedCommand(@event), cancellationToken);
    }
}
