using Amazon.SQS;
using CienceTerminal.Contracts.Events;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.AWS.Services;
using AlertService.Application.Messaging.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertService.Infrastructure.Consumers;

public class TwitterAlertConsumer : SqsEventConsumer<TwitterAlertEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TwitterAlertConsumer(
        IAmazonSQS sqs,
        ILogger<TwitterAlertConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<AwsOptions> awsOptions)
        : base(sqs, logger, awsOptions.Value.SQS.TwitterAlertsQueueUrl)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task HandleEventAsync(TwitterAlertEvent @event, CancellationToken cancellationToken)
    {
        // Create a scope to resolve scoped dependencies (like DbContext and repositories)
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new ProcessTwitterAlertCommand(@event), cancellationToken);
    }
}