using DotNetEnv;
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
    Env.Load(envPath);
    Console.WriteLine($"[Token Metrics] .env file loaded from: {Path.GetFullPath(envPath)}");
}
else
{
    Console.WriteLine($"[Token Metrics] Warning: .env file not found. Tried: {Path.GetFullPath(envPath)}");
}

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

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
