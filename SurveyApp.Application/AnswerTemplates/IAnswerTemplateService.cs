namespace SurveyApp.Application.AnswerTemplates;

public interface IAnswerTemplateService
{
    Task<List<AnswerTemplateListDto>> ListAsync(CancellationToken ct);
    Task<AnswerTemplateDetailDto?> GetAsync(long id, CancellationToken ct);
    Task<long> CreateAsync(CreateAnswerTemplateRequest req, CancellationToken ct);
    Task<bool> UpdateAsync(long id, UpdateAnswerTemplateRequest req, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
}
