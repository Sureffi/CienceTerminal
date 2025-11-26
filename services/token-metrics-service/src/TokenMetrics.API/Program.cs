using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TokenMetrics.Application;
using TokenMetrics.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Load .env file from service directory (services/token-metrics-service/.env)
// Try relative path from bin directory first (for published/compiled apps)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");
if (!File.Exists(envPath))
{
    // Try relative path from source directory (for dotnet run during development)
    envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
}

if (File.Exists(envPath))
{
    // Load .env but don't overwrite existing environment variables (ECS takes precedence)
    Env.Load(envPath, new LoadOptions(setEnvVars: true, clobberExistingVars: false));
    Console.WriteLine($"[Token Metrics] .env file loaded from: {Path.GetFullPath(envPath)}");
}
else
{
    Console.WriteLine($"[Token Metrics] Warning: .env file not found. Tried: {Path.GetFullPath(envPath)}");
}

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// DEBUG: Log AWS configuration values
Console.WriteLine($"[Token Metrics] DEBUG - AWS__UseLocalStack from env: {Environment.GetEnvironmentVariable("AWS__UseLocalStack")}");
Console.WriteLine($"[Token Metrics] DEBUG - AWS__LocalStackUrl from env: {Environment.GetEnvironmentVariable("AWS__LocalStackUrl")}");
Console.WriteLine($"[Token Metrics] DEBUG - AWS__Region from env: {Environment.GetEnvironmentVariable("AWS__Region")}");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS configuration for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Run database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<TokenMetrics.Infrastructure.Data.TokenMetricsDbContext>();
        Console.WriteLine("[Token Metrics] Running database migrations...");
        dbContext.Database.Migrate();
        Console.WriteLine("[Token Metrics] Database migrations completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Token Metrics] ERROR: Migration failed: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
