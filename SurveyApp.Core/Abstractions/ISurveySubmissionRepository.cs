using SurveyApp.Core.Entities;

namespace SurveyApp.Core.Abstractions;

public interface ISurveySubmissionRepository
{
    Task<bool> HasSubmittedAsync(long surveyId, long userId, CancellationToken ct);
    Task AddAsync(SurveySubmission entity, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);

    Task<SurveySubmission?> GetBySurveyAndUserAsync(long surveyId, long userId, CancellationToken ct);
    Task<HashSet<long>> GetSubmittedSurveyIdsAsync(long userId, IReadOnlyCollection<long> surveyIds, CancellationToken ct);

}
