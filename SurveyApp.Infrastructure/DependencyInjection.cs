using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Identity;
using SurveyApp.Infrastructure.Persistence;
using SurveyApp.Infrastructure.Repositories;

namespace SurveyApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        // Repositories
        services.AddScoped<IAnswerTemplateRepository, AnswerTemplateRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<ISurveyFillReadRepository, SurveyFillReadRepository>();
        services.AddScoped<ISurveySubmissionRepository, SurveySubmissionRepository>();
        services.AddScoped<ISurveyReportRepository, SurveyReportRepository>();
        services.AddScoped<ISurveyRepository, SurveyRepository>();
        // Identity (EF Stores)
        services
            .AddIdentity<AppUser, IdentityRole<long>>(opt => opt.User.RequireUniqueEmail = true)
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
