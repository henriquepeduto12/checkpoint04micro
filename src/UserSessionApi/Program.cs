
using Microsoft.Extensions.Options;
using UserSessionApi.Infrastructure;
using UserSessionApi.Repositories;
using UserSessionApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add configuration (appsettings.json already loaded by default)
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Mongo + Redis singletons
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<RedisConnection>();

// Repos/Services
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<ISessionService, SessionService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
