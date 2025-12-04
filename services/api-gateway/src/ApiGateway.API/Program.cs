using System.Security.Claims;
using ApiGateway.API.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Load .env file from service directory (services/api-gateway/.env)
// Try relative path from bin directory first (for published/compiled apps)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");
if (!File.Exists(envPath))
{
    // Try relative path from source directory (for dotnet run during development)
    envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
}

if (File.Exists(envPath))
{
    DotNetEnv.Env.Load(envPath);
    Console.WriteLine($"[API Gateway] .env file loaded from: {Path.GetFullPath(envPath)}");
}
else
{
    Console.WriteLine($"[API Gateway] Warning: .env file not found. Tried: {Path.GetFullPath(envPath)}");
}

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Bind configuration
var gatewayOptions = builder.Configuration.GetSection(ApiGatewayOptions.SectionName).Get<ApiGatewayOptions>() ?? new ApiGatewayOptions();
builder.Services.Configure<ApiGatewayOptions>(builder.Configuration.GetSection(ApiGatewayOptions.SectionName));

// Add Auth0 authentication (only if RequireAuthentication is enabled)
var auth0Domain = builder.Configuration["AUTH0_DOMAIN"] ?? Environment.GetEnvironmentVariable("AUTH0_DOMAIN");
var auth0Audience = builder.Configuration["AUTH0_AUDIENCE"] ?? Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");

if (gatewayOptions.RequireAuthentication && !string.IsNullOrEmpty(auth0Domain) && !string.IsNullOrEmpty(auth0Audience))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{auth0Domain}/";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier,
                // Accept audience from array (Auth0 adds userinfo endpoint to audience array)
                ValidateAudience = true,
                ValidAudiences = new[]
                {
                    auth0Audience,
                    $"https://{auth0Domain}/userinfo"
                }
            };

            // Configure to accept tokens from query string for SignalR
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for SignalR hub
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/alerts/hub"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogError(context.Exception,
                        "Authentication failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Token validated successfully for user: {User}",
                        context.Principal?.Identity?.Name ?? "unknown");
                    return Task.CompletedTask;
                }
            };
        });
}
else if (gatewayOptions.RequireAuthentication)
{
    // RequireAuthentication is true but Auth0 credentials not configured
    Console.WriteLine("WARNING: RequireAuthentication is enabled but AUTH0_DOMAIN or AUTH0_AUDIENCE not configured. API will be accessible without authentication.");
}

// If authentication is disabled, configure authorization to allow anonymous access
if (!gatewayOptions.RequireAuthentication)
{
    builder.Services.AddAuthorization(options =>
    {
        // Create a policy that allows anonymous access
        // This matches the "AuthorizationPolicy": "ApiPolicy" in route configuration
        options.AddPolicy("ApiPolicy", policy => policy.RequireAssertion(_ => true));
    });
}
else
{
    // When authentication is enabled, create a policy that requires authentication
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiPolicy", policy => policy.RequireAuthenticatedUser());
    });
}

// Add controllers for Swagger documentation
builder.Services.AddControllers();

// Add HttpClient for proxying requests to downstream services
var alertServiceUrl = Environment.GetEnvironmentVariable("ALERT_SERVICE_URL")
    ?? builder.Configuration["AlertServiceUrl"]
    ?? "http://alert-service:8080";
var twitterScannerUrl = Environment.GetEnvironmentVariable("TWITTER_SCANNER_URL")
    ?? builder.Configuration["TwitterScannerUrl"]
    ?? "http://twitter-scanner:8080";

Console.WriteLine($"Alert Service URL: {alertServiceUrl}");
Console.WriteLine($"Twitter Scanner URL: {twitterScannerUrl}");

builder.Services.AddHttpClient("AlertService", client =>
{
    client.BaseAddress = new Uri(alertServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("TwitterScanner", client =>
{
    client.BaseAddress = new Uri(twitterScannerUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add YARP reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (gatewayOptions.CorsOrigins.Length > 0)
        {
            policy
                .WithOrigins(gatewayOptions.CorsOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            // Fallback to localhost for development
            policy
                .WithOrigins("http://localhost:3000", "http://localhost:5173", "https://localhost:3000", "https://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    });
});

// Add health checks
builder.Services.AddHealthChecks();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CienceTerminal API Gateway",
        Version = "v1",
        Description = "API Gateway for CienceTerminal microservices architecture. Routes requests to Twitter Scanner, Alert Service, and User Management services."
    });

    // Enable XML comments for better Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT authentication to Swagger UI
    if (gatewayOptions.RequireAuthentication)
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
});

// Add logging
builder.Logging.AddConsole();

// Configure for deployment
var port = Environment.GetEnvironmentVariable("API_GATEWAY_PORT") ?? Environment.GetEnvironmentVariable("PORT") ?? "5149";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Configure the HTTP request pipeline

// Enable Swagger in all environments for easy API exploration
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CienceTerminal API Gateway v1");
    options.RoutePrefix = "swagger"; // Access at /swagger
    options.DocumentTitle = "CienceTerminal API Documentation";
});

app.UseCors();

// Only use authentication/authorization middleware if authentication is required
if (gatewayOptions.RequireAuthentication)
{
    app.UseAuthentication();
    app.UseAuthorization();

    // Add middleware to forward user context to downstream services
    app.Use(async (context, next) =>
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // Extract user information from JWT claims
            var auth0Sub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                          context.User.FindFirst("sub")?.Value;
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value ??
                       context.User.FindFirst("email")?.Value;
            var emailVerified = context.User.FindFirst("email_verified")?.Value;
            var tier = context.User.FindFirst("https://cienceterminal.com/tier")?.Value;

            // Forward user context via headers
            if (!string.IsNullOrEmpty(auth0Sub))
                context.Request.Headers["X-User-Auth0-Sub"] = auth0Sub;
            if (!string.IsNullOrEmpty(email))
                context.Request.Headers["X-User-Email"] = email;
            if (!string.IsNullOrEmpty(emailVerified))
                context.Request.Headers["X-User-Email-Verified"] = emailVerified;
            if (!string.IsNullOrEmpty(tier))
                context.Request.Headers["X-User-Tier"] = tier;
        }

        await next();
    });
}
else
{
    // When authentication is disabled, still use authorization middleware
    // This allows the "Default" policy to work (which allows all requests)
    app.UseAuthorization();
}

// Map controller endpoints
app.MapControllers();

// Add health check endpoint
app.MapHealthChecks("/health");

// Add a simple status endpoint for the gateway itself
app.MapGet("/", () => new
{
    service = "API Gateway",
    status = "running",
    timestamp = DateTime.UtcNow
})
.WithName("GetGatewayStatus")
.WithTags("Gateway")
.WithOpenApi(operation => new(operation)
{
    Summary = "Get API Gateway status",
    Description = "Returns the current status of the API Gateway service"
});

// Document available routes for Swagger UI
app.MapGet("/api/routes", () => new
{
    routes = new[]
    {
        new { method = "GET", path = "/api/v1/alerts/twitter", service = "Alert Service", description = "Get all Twitter alerts" },
        new { method = "GET", path = "/api/v1/alerts/ca-mentions", service = "Alert Service", description = "Get all CA mention alerts" },
        new { method = "GET", path = "/api/v1/ca-mentions/{address}", service = "Twitter Scanner", description = "Get CA mention details by contract address" },
        new { method = "WebSocket", path = "/alerts/hub/twitter", service = "Alert Service", description = "Twitter alerts SignalR hub" },
        new { method = "WebSocket", path = "/alerts/hub/ca-mentions", service = "Alert Service", description = "CA mention alerts SignalR hub" },
        new { method = "GET", path = "/api/v1/users", service = "User Management", description = "User management endpoints (if available)" },
        new { method = "GET", path = "/health", service = "Gateway", description = "Health check endpoint" }
    },
    note = "All routes are documented in Swagger UI at /swagger. Controller endpoints proxy to downstream microservices."
})
.WithName("GetAvailableRoutes")
.WithTags("Gateway")
.WithOpenApi(operation => new(operation)
{
    Summary = "List all available API routes",
    Description = "Returns a list of all routes available through the API Gateway and their target services"
});

// Map the reverse proxy
app.MapReverseProxy();

app.Run();
