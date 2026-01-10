using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SurveyApp.Application.AnswerTemplates;
using SurveyApp.Application.Questions;
using SurveyApp.Application.Reports;
using SurveyApp.Application.SurveyFill;
using SurveyApp.Application.Surveys;

namespace SurveyApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Use-case / Service registrations
        services.AddScoped<ISurveyService, SurveyService>();

        services.AddScoped<IAnswerTemplateService, AnswerTemplateService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ISurveyFillService, SurveyFillService>();
        services.AddScoped<ISurveyReportService, SurveyReportService>();
        services.AddScoped<ISurveyService, SurveyService>();

        return services;
    }
}
