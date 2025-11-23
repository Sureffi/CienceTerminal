using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using CienceTerminal.AWS.Abstractions;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.AWS.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CienceTerminal.AWS;

public static class DependencyInjection
{
    public static IServiceCollection AddAwsEventProducer(this IServiceCollection services, IConfiguration configuration)
    {
        var awsOptions = configuration.GetSection(AwsOptions.SectionName).Get<AwsOptions>() ?? new AwsOptions();
        services.Configure<AwsOptions>(configuration.GetSection(AwsOptions.SectionName));

        // Configure AWS clients
        if (awsOptions.UseLocalStack)
        {
            var credentials = new BasicAWSCredentials("test", "test");
            var config = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = awsOptions.LocalStackUrl,
                UseHttp = true,
                AuthenticationRegion = awsOptions.Region,
                DisableLogging = false
            };
            services.AddSingleton<IAmazonSimpleNotificationService>(_ => new AmazonSimpleNotificationServiceClient(credentials, config));
        }
        else
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSimpleNotificationService>();
        }

        services.AddSingleton<IEventProducer, SnsEventProducer>();

        return services;
    }

    public static IServiceCollection AddAwsEventConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        var awsOptions = configuration.GetSection(AwsOptions.SectionName).Get<AwsOptions>() ?? new AwsOptions();
        services.Configure<AwsOptions>(configuration.GetSection(AwsOptions.SectionName));

        // Configure AWS clients
        if (awsOptions.UseLocalStack)
        {
            var credentials = new BasicAWSCredentials("test", "test");
            var config = new AmazonSQSConfig
            {
                UseHttp = true,
                AuthenticationRegion = awsOptions.Region,
                ServiceURL = awsOptions.LocalStackUrl
            };
            services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(credentials, config));
        }
        else
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSQS>();
        }

        return services;
    }
}
