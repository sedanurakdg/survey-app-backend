namespace SurveyApp.Core.Abstractions;

public interface ISurveyReportRepository
{
    Task<List<SurveyReportRow>> ListSurveysAsync(string? search, CancellationToken ct);
    Task<SurveyUsersReport?> GetSurveyUsersAsync(long surveyId, string? search, CancellationToken ct);
    Task<UserSurveyAnswersReport?> GetUserAnswersAsync(long surveyId, long userId, CancellationToken ct);
}

// Read-models (repo çıktısı)
public sealed record SurveyReportRow(long SurveyId, string Title, DateTime StartsAtUtc, DateTime EndsAtUtc, int AssignedCount, int CompletedCount);

public sealed record SurveyUserRow(long UserId, string Email, bool Completed, DateTime? SubmittedAtUtc);

public sealed record SurveyUsersReport(long SurveyId, string Title, List<SurveyUserRow> Users);

public sealed record UserAnswerRow(long QuestionId, string QuestionText, short SelectedOptionIndex, string SelectedOptionText);

public sealed record UserSurveyAnswersReport(long SurveyId, string Title, long UserId, string Email, DateTime SubmittedAtUtc, List<UserAnswerRow> Answers);
