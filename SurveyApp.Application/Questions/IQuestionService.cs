namespace SurveyApp.Application.Questions;

public interface IQuestionService
{
    Task<List<QuestionListDto>> ListAsync(CancellationToken ct);
    Task<QuestionDetailDto?> GetAsync(long id, CancellationToken ct);
    Task<long> CreateAsync(CreateQuestionRequest req, CancellationToken ct);
    Task<bool> UpdateAsync(long id, UpdateQuestionRequest req, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
}
