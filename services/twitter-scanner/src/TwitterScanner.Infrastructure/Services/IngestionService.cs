using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitterScanner.Application.Interfaces;
using TwitterScanner.Domain.Events;

namespace TwitterScanner.Infrastructure.Services;

/// <summary>
/// Service for orchestrating data ingestion
/// </summary>
public class IngestionService : BackgroundService
{
    private readonly ILogger<IngestionService> _logger;
    private readonly ITwitterStreamingClient _twitterStreamingClient;
    private readonly IServiceScopeFactory _scopeFactory;

    public IngestionService(
        ILogger<IngestionService> logger,
        ITwitterStreamingClient twitterStreamingClient,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _twitterStreamingClient = twitterStreamingClient;
        _scopeFactory = scopeFactory;
    }

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting data ingestion");

        // Connect to twitter and publish mediator notification on tweet received
        await _twitterStreamingClient.ConnectAsync(async (tweet) =>
        {
            // Create a scope for each tweet to resolve scoped services
            // TODO: Is creating a scope here really a good pattern?
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(new TweetReceivedNotification(tweet), stoppingToken);
        }, stoppingToken);

        _logger.LogInformation("Data ingestion started");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunAsync(stoppingToken);
    }
}
