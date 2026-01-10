using SurveyApp.Core.Abstractions;

namespace SurveyApp.Application.Reports;

public sealed class SurveyReportService : ISurveyReportService
{
    private readonly ISurveyReportRepository _repo;
    public SurveyReportService(ISurveyReportRepository repo) => _repo = repo;

    public async Task<List<SurveyReportListDto>> ListAsync(string? search, CancellationToken ct)
    {
        var rows = await _repo.ListSurveysAsync(search, ct);

        return rows.Select(r => new SurveyReportListDto(
            r.SurveyId,
            r.Title,
            r.StartsAtUtc,
            r.EndsAtUtc,
            r.AssignedCount,
            r.CompletedCount,
            PendingCount: Math.Max(0, r.AssignedCount - r.CompletedCount)
        )).ToList();
    }

    public async Task<SurveyUsersReportDto?> SurveyUsersAsync(long surveyId, string? search, CancellationToken ct)
    {
        var data = await _repo.GetSurveyUsersAsync(surveyId, search, ct);
        if (data is null) return null;

        return new SurveyUsersReportDto(
            data.SurveyId,
            data.Title,
            data.Users.Select(u => new SurveyUserStatusDto(u.UserId, u.Email, u.Completed, u.SubmittedAtUtc)).ToList()
        );
    }

    public async Task<UserSurveyAnswersDto?> UserAnswersAsync(long surveyId, long userId, CancellationToken ct)
    {
        var data = await _repo.GetUserAnswersAsync(surveyId, userId, ct);
        if (data is null) return null;

        return new UserSurveyAnswersDto(
            data.SurveyId,
            data.Title,
            data.UserId,
            data.Email,
            data.SubmittedAtUtc,
            data.Answers.Select(a => new UserAnswerDetailDto(a.QuestionId, a.QuestionText, a.SelectedOptionIndex, a.SelectedOptionText)).ToList()
        );
    }
}
