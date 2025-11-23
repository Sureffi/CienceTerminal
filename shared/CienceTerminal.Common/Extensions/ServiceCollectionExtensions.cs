using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace CienceTerminal.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonSerialization(this IServiceCollection services)
    {
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = false;
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        return services;
    }
}
