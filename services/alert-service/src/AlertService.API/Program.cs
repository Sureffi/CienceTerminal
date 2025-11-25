using AlertService.API.Configuration;
using AlertService.Application.Messaging.Commands;
using AlertService.Application.Services;
using AlertService.Domain.Configuration;
using AlertService.Domain.Interfaces;
using AlertService.Infrastructure;
using AlertService.Infrastructure.Hubs;
using AlertService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Load .env file from service directory (services/alert-service/.env)
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
    Console.WriteLine($"[Alert Service] .env file loaded from: {Path.GetFullPath(envPath)}");
}
else
{
    Console.WriteLine($"[Alert Service] Warning: .env file not found. Tried: {Path.GetFullPath(envPath)}");
}

// Add environment variables to configuration (secrets can override appsettings)
builder.Configuration.AddEnvironmentVariables();

// Bind configuration
var serviceOptions = builder.Configuration.GetSection(AlertServiceOptions.SectionName).Get<AlertServiceOptions>() ?? new AlertServiceOptions();
builder.Services.Configure<AlertServiceOptions>(builder.Configuration.GetSection(AlertServiceOptions.SectionName));
builder.Services.Configure<AlertOptions>(builder.Configuration.GetSection("AlertOptions"));

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(static options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Add MediatR
builder.Services.AddMediatR(static cfg => cfg.RegisterServicesFromAssemblyContaining<ProcessTwitterAlertCommandHandler>());

// Register application services
builder.Services.AddSingleton<IAlertManager, AlertManager>();
builder.Services.AddScoped<IAlertNotificationService, AlertNotificationService>();

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS for frontend (configured via appsettings)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (serviceOptions.CorsOrigins.Length > 0)
        {
            policy.WithOrigins(serviceOptions.CorsOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // Production: no CORS or configure via gateway
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// Configure port (env vars can override appsettings)
var port = Environment.GetEnvironmentVariable("ALERT_SERVICE_PORT")
           ?? Environment.GetEnvironmentVariable("PORT")
           ?? serviceOptions.Port;
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.MapControllers();
app.MapHub<TwitterAlertHub>("/twitter-alert-hub");
app.MapHub<CaMentionAlertHub>("/ca-mention-alert-hub");

app.Run();
