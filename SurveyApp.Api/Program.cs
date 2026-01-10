using Serilog;
using SurveyApp.Api;
using SurveyApp.Application;
using SurveyApp.Infrastructure;
using SurveyApp.Infrastructure.Identity;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext();
});

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("default", p =>
        p.WithOrigins(allowedOrigins)
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// RateLimiter
builder.Services.AddRateLimiter(opt =>
{
    opt.AddPolicy("ip-fixed", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });

    opt.AddPolicy("auth", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: $"auth:{ip}",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 2,
                QueueLimit = 0
            });
    });

    opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Layers
builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseApiPipeline();

app.UseCors("default");

app.UseRateLimiter();

// Controllers tek yerde map’leniyor
app.MapControllers().RequireRateLimiting("ip-fixed");

// Minimal endpoints
app.MapApiEndpoints();

// Seed
if (app.Environment.IsDevelopment())
{
    await IdentitySeeder.SeedAsync(app.Services, app.Configuration);
}

app.Run();
