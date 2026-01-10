using SurveyApp.Core.Entities;

namespace SurveyApp.Core.Abstractions;

public interface IAnswerTemplateRepository
{
    Task<List<AnswerTemplate>> ListAsync(CancellationToken ct);
    Task<AnswerTemplate?> GetByIdAsync(long id, bool tracking, CancellationToken ct);

    Task AddAsync(AnswerTemplate entity, CancellationToken ct);
    Task DeleteAsync(AnswerTemplate entity, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
    Task<bool> ExistsAsync(long id, CancellationToken ct);
}
