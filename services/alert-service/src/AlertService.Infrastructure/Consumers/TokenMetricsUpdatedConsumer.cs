using Amazon.SQS;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.AWS.Services;
using CienceTerminal.Contracts.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AlertService.Application.Messaging.Commands;

namespace AlertService.Infrastructure.Consumers;

/// <summary>
/// Consumes TokenMetricsUpdatedEvent from SQS queue.
/// Published by Token Metrics Service when coin metrics are refreshed.
/// </summary>
public class TokenMetricsUpdatedConsumer : SqsEventConsumer<TokenMetricsUpdatedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TokenMetricsUpdatedConsumer(
        IAmazonSQS sqs,
        ILogger<TokenMetricsUpdatedConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<AwsOptions> awsOptions)
        : base(sqs, logger, awsOptions.Value.SQS.TokenMetricsUpdatedQueueUrl)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task HandleEventAsync(TokenMetricsUpdatedEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new UpdateCoinMetricsCommand(@event), cancellationToken);
    }
}
