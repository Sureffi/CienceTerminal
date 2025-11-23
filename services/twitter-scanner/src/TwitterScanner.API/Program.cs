using DotNetEnv;
using TwitterScanner.Application;
using TwitterScanner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Load .env file from service directory
// TODO: This is loading wrong .env
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($".env file loaded from service directory");
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
