using SurveyApp.Core.Entities;

namespace SurveyApp.Core.Abstractions;

public interface ISurveyRepository
{
    Task<List<Survey>> ListAsync(CancellationToken ct);
    Task<Survey?> GetByIdAsync(long id, bool tracking, CancellationToken ct);

    Task AddAsync(Survey entity, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);

    // Existence checks (service katmanında 400 üretmek için)
    Task<HashSet<long>> GetExistingQuestionIdsAsync(IReadOnlyCollection<long> questionIds, CancellationToken ct);
    Task<HashSet<long>> GetExistingUserIdsAsync(IReadOnlyCollection<long> userIds, CancellationToken ct);
}
