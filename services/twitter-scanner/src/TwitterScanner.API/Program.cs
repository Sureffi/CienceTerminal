using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TwitterScanner.Application;
using TwitterScanner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Load .env file from service directory (services/twitter-scanner/.env)
// Try relative path from bin directory first (for published/compiled apps)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");
if (!File.Exists(envPath))
{
    // Try relative path from source directory (for dotnet run during development)
    envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
}

if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"[Twitter Scanner] .env file loaded from: {Path.GetFullPath(envPath)}");
}
else
{
    Console.WriteLine($"[Twitter Scanner] Warning: .env file not found. Tried: {Path.GetFullPath(envPath)}");
}

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application and infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


// Configure for deployment
var port = Environment.GetEnvironmentVariable("TWITTER_SCANNER_PORT") ?? Environment.GetEnvironmentVariable("PORT") ?? "5147";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Run database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<TwitterScanner.Infrastructure.Data.MentionPersistenceDbContext>();
        Console.WriteLine("[Twitter Scanner] Running database migrations...");
        dbContext.Database.Migrate();
        Console.WriteLine("[Twitter Scanner] Database migrations completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Twitter Scanner] ERROR: Migration failed: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllers();

app.Run();

public partial class Program { }
