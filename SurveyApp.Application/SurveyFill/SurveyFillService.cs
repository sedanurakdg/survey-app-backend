using Microsoft.EntityFrameworkCore;
using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;

namespace SurveyApp.Application.SurveyFill;

public sealed class SurveyFillService : ISurveyFillService
{
    private readonly ISurveyFillReadRepository _readRepo;
    private readonly ISurveySubmissionRepository _submissionRepo;

    public SurveyFillService(ISurveyFillReadRepository readRepo, ISurveySubmissionRepository submissionRepo)
    {
        _readRepo = readRepo;
        _submissionRepo = submissionRepo;
    }

    public async Task<List<UserSurveyListDto>> ListMyActiveAsync(long userId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var surveys = await _readRepo.ListAssignedActiveAsync(userId, now, ct);
        if (surveys.Count == 0) return new List<UserSurveyListDto>();

        var surveyIds = surveys.Select(s => s.Id).ToList();
        var submittedIds = await _submissionRepo.GetSubmittedSurveyIdsAsync(userId, surveyIds, ct);

        return surveys.Select(s => new UserSurveyListDto(
            s.Id,
            s.Title,
            s.StartsAtUtc,
            s.EndsAtUtc,
            IsSubmitted: submittedIds.Contains(s.Id)
        )).ToList();
    }

    public async Task<UserSurveyDetailDto?> GetForFillAsync(long surveyId, long userId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var survey = await _readRepo.GetAssignedActiveDetailAsync(surveyId, userId, now, ct);
        if (survey is null) return null;

        var questions = survey.Questions
            .OrderBy(x => x.SortOrder)
            .Select(x =>
            {
                var q = x.Question;
                var t = q.AnswerTemplate;

                var choices = t.Options
                    .OrderBy(o => o.SortOrder)
                    .Select(o => new ChoiceDto(o.SortOrder, o.Text))
                    .ToList();

                return new FillQuestionDto(q.Id, q.Text, choices);
            })
            .ToList();

        return new UserSurveyDetailDto(
            survey.Id,
            survey.Title,
            survey.Description,
            survey.StartsAtUtc,
            survey.EndsAtUtc,
            questions
        );
    }

    public async Task<SubmitResult> SubmitAsync(long surveyId, long userId, SubmitSurveyRequest req, CancellationToken ct)
    {
        // 1) tekrar doldurmayı engelle
        if (await _submissionRepo.HasSubmittedAsync(surveyId, userId, ct))
            return new SubmitResult(false, "Bu anket daha önce doldurulmuş.");

        // 2) erişim + aktif + tarih aralığı
        var detail = await GetForFillAsync(surveyId, userId, ct);
        if (detail is null)
            return new SubmitResult(false, "Anket bulunamadı veya erişim yok ya da tarih aralığı dışında.");

        var answers = req.Answers ?? new List<SubmitAnswerDto>();
        if (answers.Count == 0)
            return new SubmitResult(false, "Cevaplar boş olamaz.");

        // 3) her soruya 1 cevap
        var questionIds = detail.Questions.Select(q => q.QuestionId).ToHashSet();
        var providedIds = answers.Select(a => a.QuestionId).ToList();

        if (providedIds.Count != providedIds.Distinct().Count())
            return new SubmitResult(false, "Aynı soruya birden fazla cevap gönderilemez.");

        if (providedIds.Count != questionIds.Count || providedIds.Any(id => !questionIds.Contains(id)))
            return new SubmitResult(false, "Gönderilen cevaplar anket sorularıyla uyuşmuyor.");

        // 4) seçilen option SortOrder doğrulaması
        var allowedByQuestion = detail.Questions.ToDictionary(
            q => q.QuestionId,
            q => q.Choices.Select(c => c.Index).ToHashSet()
        );

        foreach (var a in answers)
        {
            if (!allowedByQuestion.TryGetValue(a.QuestionId, out var allowed) || !allowed.Contains(a.SelectedOptionIndex))
                return new SubmitResult(false, $"Geçersiz seçenek. QuestionId={a.QuestionId}, SelectedOptionIndex={a.SelectedOptionIndex}.");
        }

        // 5) persist
        var submission = new SurveySubmission
        {
            SurveyId = surveyId,
            UserId = userId,
            SubmittedAtUtc = DateTime.UtcNow,
            Answers = answers.Select(a => new SurveySubmissionAnswer
            {
                QuestionId = a.QuestionId,
                SelectedOptionIndex = a.SelectedOptionIndex
            }).ToList()
        };

        try
        {
            await _submissionRepo.AddAsync(submission, ct);
            await _submissionRepo.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // DB unique index (SurveyId, UserId) çakışması -> aynı anda iki submit gibi durumlar
            return new SubmitResult(false, "Bu anket daha önce doldurulmuş.");
        }

        return new SubmitResult(true, null);
    }
}
