using SurveyApp.Core.Entities;

namespace SurveyApp.Core.Abstractions;

public interface ISurveyFillReadRepository
{
    Task<List<Survey>> ListAssignedActiveAsync(long userId, DateTime nowUtc, CancellationToken ct);

    // Survey + Questions + Question + AnswerTemplate (fill ekranı için)
    Task<Survey?> GetAssignedActiveDetailAsync(long surveyId, long userId, DateTime nowUtc, CancellationToken ct);
}
