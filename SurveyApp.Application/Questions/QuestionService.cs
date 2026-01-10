using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;
using SurveyApp.Core.Exceptions;

namespace SurveyApp.Application.Questions;

public sealed class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _repo;
    private readonly IAnswerTemplateRepository _answerTemplates;

    public QuestionService(IQuestionRepository repo, IAnswerTemplateRepository answerTemplates) { _repo = repo; _answerTemplates = answerTemplates; }

    public async Task<List<QuestionListDto>> ListAsync(CancellationToken ct)
    {
        var items = await _repo.ListAsync(ct);

        return items.Select(x =>
            new QuestionListDto(
                x.Id,
                x.Text,
                x.AnswerTemplateId,
                x.AnswerTemplate.Name,
                x.IsActive))
            .ToList();
    }

    public async Task<QuestionDetailDto?> GetAsync(long id, CancellationToken ct)
    {
        var q = await _repo.GetByIdAsync(id, tracking: false, ct);
        if (q is null) return null;

        return new QuestionDetailDto(q.Id, q.Text, q.AnswerTemplateId, q.AnswerTemplate.Name, q.IsActive);
    }

    public async Task<long> CreateAsync(CreateQuestionRequest req, CancellationToken ct)
    {
        if (!await _answerTemplates.ExistsAsync(req.AnswerTemplateId, ct))
            throw new DomainValidationException("answerTemplateId", "Seçilen cevap şablonu bulunamadı.");

        var entity = new Question
        {
            Text = req.Text.Trim(),
            AnswerTemplateId = req.AnswerTemplateId,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(long id, UpdateQuestionRequest req, CancellationToken ct)
    {
        if (!await _answerTemplates.ExistsAsync(req.AnswerTemplateId, ct))
            throw new DomainValidationException("answerTemplateId", "Seçilen cevap şablonu bulunamadı.");

        var entity = await _repo.GetByIdAsync(id, tracking: true, ct);
        if (entity is null) return false;

        entity.Text = req.Text.Trim();
        entity.AnswerTemplateId = req.AnswerTemplateId;
        entity.IsActive = req.IsActive;

        await _repo.SaveChangesAsync(ct);
        return true;
    }


    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, tracking: true, ct);
        if (entity is null) return false;

        entity.IsActive = false;
        await _repo.SaveChangesAsync(ct);
        return true;
    }

}
