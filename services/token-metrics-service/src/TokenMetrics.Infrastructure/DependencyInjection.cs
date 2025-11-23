using CienceTerminal.AWS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TokenMetrics.Application.Interfaces;
using TokenMetrics.Domain.Interfaces;
using TokenMetrics.Infrastructure.Consumers;
using TokenMetrics.Infrastructure.Data;
using TokenMetrics.Infrastructure.ExternalServices;
using TokenMetrics.Infrastructure.Repositories;
using TokenMetrics.Infrastructure.Services;

namespace TokenMetrics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("TokenMetricsDb")
            ?? throw new InvalidOperationException("Connection string 'TokenMetricsDb' not found.");

        services.AddDbContext<TokenMetricsDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IMentionAggregateRepository, MentionAggregateRepository>();
        services.AddScoped<ICoinRepository, CoinRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();

        // MediatR handlers in Infrastructure layer (for publishing events)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Background services
        services.AddHostedService<TrendingAggregationService>(); // Runs every 1 minute, reads from ca_mention_records
        services.AddHostedService<CoinMetricsUpdateService>(); // Runs every 60 seconds, updates metrics for active coins

        // External API Clients
        services.AddHttpClient<IJupiterClient, JupiterClient>();

        // AWS SNS/SQS Messaging
        services.AddAwsEventProducer(configuration);  // For publishing events
        services.AddAwsEventConsumer(configuration);  // For consuming events
        services.AddHostedService<CaMentionDetectedConsumer>();

        return services;
    }
}
