using SurveyApp.Core.Entities;

namespace SurveyApp.Core.Abstractions;

public interface IQuestionRepository
{
    Task<List<Question>> ListAsync(CancellationToken ct);
    Task<Question?> GetByIdAsync(long id, bool tracking, CancellationToken ct);

    Task AddAsync(Question entity, CancellationToken ct);
    Task DeleteAsync(Question entity, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
