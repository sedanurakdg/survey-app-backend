using SurveyApp.Api;
using SurveyApp.Application;
using SurveyApp.Infrastructure;
using SurveyApp.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Katman kayýtlarý
builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseApiPipeline();
app.MapApiEndpoints();

// Seed (sadece Development)
if (app.Environment.IsDevelopment())
{
    await IdentitySeeder.SeedAsync(app.Services, app.Configuration);
}

app.Run();
