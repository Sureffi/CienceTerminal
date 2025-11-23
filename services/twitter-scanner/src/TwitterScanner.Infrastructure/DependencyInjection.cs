using CienceTerminal.AWS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solnet.Rpc;
using TwitterScanner.Application.Interfaces;
using TwitterScanner.Infrastructure.Data;
using TwitterScanner.Infrastructure.ExternalServices;
using TwitterScanner.Infrastructure.Services;

namespace TwitterScanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database for persisting CA mentions
        var connectionString = configuration.GetConnectionString("TokenMetricsDb")
            ?? throw new InvalidOperationException("Connection string 'TokenMetricsDb' not found.");

        services.AddDbContext<MentionPersistenceDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IMentionRepository, MentionRepository>();

        // Memory cache
        services.AddMemoryCache();

        // Background services
        services.AddHostedService<IngestionService>();

        // Add IRpcClient with either helius cluster or mainnet if helius not found in config
        // TODO: Way to see which cluster is being used
        // Could log?
        services.AddSingleton<IRpcClient>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var heliusEndpoint = config["Endpoints:Helius"];
            return string.IsNullOrEmpty(heliusEndpoint)
                ? ClientFactory.GetClient(Cluster.MainNet)
                : ClientFactory.GetClient(heliusEndpoint);
        });
        services.AddSingleton<ISolanaRpcService, SolanaRpcService>();
        services.AddSingleton<ITwitterStreamingClient, TwitterStreamingClient>();
        services.AddSingleton<IGroqClient, GroqClient>();

        // AWS SNS producer
        services.AddAwsEventProducer(configuration);

        return services;
    }
}
