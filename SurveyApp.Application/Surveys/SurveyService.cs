using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;
using SurveyApp.Core.Exceptions;

namespace SurveyApp.Application.Surveys;

public sealed class SurveyService : ISurveyService
{
    private readonly ISurveyRepository _repo;

    public SurveyService(ISurveyRepository repo) => _repo = repo;

    public async Task<List<SurveyListDto>> ListAsync(CancellationToken ct)
    {
        var items = await _repo.ListAsync(ct);

        return items.Select(x => new SurveyListDto(
                x.Id,
                x.Title,
                x.IsActive,
                x.StartsAtUtc,
                x.EndsAtUtc))
            .ToList();
    }

    public async Task<SurveyDetailDto?> GetAsync(long id, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync(id, tracking: false, ct);
        if (s is null) return null;

        var questions = s.Questions
            .OrderBy(x => x.SortOrder)
            .Select(x => new SurveyQuestionDto(x.QuestionId, x.SortOrder))
            .ToList();

        var userIds = s.Assignments.Select(x => x.UserId).ToList();

        return new SurveyDetailDto(
            s.Id,
            s.Title,
            s.Description,
            s.IsActive,
            s.StartsAtUtc,
            s.EndsAtUtc,
            questions,
            userIds);
    }

    public async Task<long> CreateAsync(CreateSurveyRequest req, CancellationToken ct)
    {
        var title = (req.Title ?? "").Trim();
        var description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();

        var questionIds = (req.QuestionIds ?? new List<long>())
            .Where(x => x > 0)
            .Distinct()
            .ToList();

        var userIds = (req.UserIds ?? new List<long>())
            .Where(x => x > 0)
            .Distinct()
            .ToList();

        // referential doğrulama (async) – 500 yerine kontrollü fail için
        var existingQ = await _repo.GetExistingQuestionIdsAsync(questionIds, ct);
        var missingQ = questionIds.Where(id => !existingQ.Contains(id)).ToList();
        if (missingQ.Count > 0)
            throw new DomainValidationException(
                "questionIds",
                $"Bulunamayan soru id(leri): {string.Join(",", missingQ)}");


        var existingU = await _repo.GetExistingUserIdsAsync(userIds, ct);
        var missingU = userIds.Where(id => !existingU.Contains(id)).ToList();
        if (missingU.Count > 0)
            throw new DomainValidationException(
                "userIds",
                $"Bulunamayan kullanıcı id(leri): {string.Join(",", missingU)}");

        var entity = new Survey
        {
            Title = title,
            Description = description,
            StartsAtUtc = req.StartsAtUtc,
            EndsAtUtc = req.EndsAtUtc,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        // SurveyQuestions: sortOrder = index+1
        for (int i = 0; i < questionIds.Count; i++)
        {
            entity.Questions.Add(new SurveyQuestion
            {
                QuestionId = questionIds[i],
                SortOrder = i + 1
            });
        }

        // Assignments
        foreach (var uid in userIds)
        {
            entity.Assignments.Add(new SurveyAssignment { UserId = uid });
        }

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(long id, UpdateSurveyRequest req, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, tracking: true, ct);
        if (entity is null) return false;

        var title = (req.Title ?? "").Trim();
        var description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();

        var questionIds = (req.QuestionIds ?? new List<long>())
            .Where(x => x > 0)
            .Distinct()
            .ToList();

        var userIds = (req.UserIds ?? new List<long>())
            .Where(x => x > 0)
            .Distinct()
            .ToList();

        var existingQ = await _repo.GetExistingQuestionIdsAsync(questionIds, ct);
        var missingQ = questionIds.Where(qid => !existingQ.Contains(qid)).ToList();
        if (missingQ.Count > 0)
            throw new DomainValidationException(
                "questionIds",
                $"Bulunamayan soru id(leri): {string.Join(",", missingQ)}");

        var existingU = await _repo.GetExistingUserIdsAsync(userIds, ct);
        var missingU = userIds.Where(uid => !existingU.Contains(uid)).ToList();
        if (missingU.Count > 0)
            throw new DomainValidationException(
                "userIds",
                $"Bulunamayan kullanıcı id(leri): {string.Join(",", missingU)}");

        entity.Title = title;
        entity.Description = description;
        entity.StartsAtUtc = req.StartsAtUtc;
        entity.EndsAtUtc = req.EndsAtUtc;
        entity.IsActive = req.IsActive;

        entity.Questions.Clear();
        for (int i = 0; i < questionIds.Count; i++)
        {
            entity.Questions.Add(new SurveyQuestion
            {
                SurveyId = entity.Id,
                QuestionId = questionIds[i],
                SortOrder = i + 1
            });
        }

        entity.Assignments.Clear();
        foreach (var uid in userIds)
        {
            entity.Assignments.Add(new SurveyAssignment
            {
                SurveyId = entity.Id,
                UserId = uid
            });
        }

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
