using AlertService.Application.Interfaces;
using AlertService.Domain.Interfaces;
using AlertService.Infrastructure.Consumers;
using AlertService.Infrastructure.Data;
using AlertService.Infrastructure.Repositories;
using AlertService.Infrastructure.Services;
using CienceTerminal.AWS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlertService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Alert Service's own database (for alert persistence)
        var alertDbConnectionString = configuration.GetConnectionString("AlertServiceDb")
            ?? throw new InvalidOperationException("Connection string 'AlertServiceDb' not found.");

        services.AddDbContext<AlertServiceDbContext>(options =>
            options.UseNpgsql(alertDbConnectionString));

        // Token Metrics database (read-only access to shared data)
        var tokenMetricsConnectionString = configuration.GetConnectionString("CienceTerminalDb")
            ?? throw new InvalidOperationException("Connection string 'CienceTerminalDb' not found.");

        services.AddDbContext<TokenMetricsReadOnlyDbContext>(options =>
            options.UseNpgsql(tokenMetricsConnectionString));

        // Repositories
        services.AddScoped<IMentionAggregateRepository, MentionAggregateRepository>();
        services.AddScoped<ICoinRepository, CoinRepository>();
        services.AddScoped<ICaMentionRecordRepository, CaMentionRecordRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();

        // Domain services
        services.AddSingleton<IAlertNotificationService, AlertNotificationService>();

        // Alert Manager initialization (must run before consumers start)
        services.AddHostedService<AlertManagerInitializationService>();

        // AWS SQS consumers
        services.AddAwsEventProducer(configuration);
        services.AddAwsEventConsumer(configuration);
        services.AddHostedService<TwitterAlertConsumer>();
        services.AddHostedService<AlertRemovalConsumer>();
        services.AddHostedService<MentionAggregatesUpdatedConsumer>();
        services.AddHostedService<CoinBlacklistedConsumer>();
        services.AddHostedService<TokenMetricsUpdatedConsumer>();

        return services;
    }
}
