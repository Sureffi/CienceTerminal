namespace ApiGateway.API.Configuration;

/// <summary>
/// Configuration options for the API Gateway service.
/// </summary>
public class ApiGatewayOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "ApiGateway";

    /// <summary>
    /// Determines whether authentication is required for API endpoints.
    /// When false, all endpoints are accessible without authentication.
    /// When true, routes with AuthorizationPolicy will enforce authentication.
    /// </summary>
    public bool RequireAuthentication { get; set; } = true;

    /// <summary>
    /// Allowed CORS origins for frontend applications.
    /// Configure via appsettings.json or environment variables.
    /// </summary>
    public string[] CorsOrigins { get; set; } = Array.Empty<string>();
}
