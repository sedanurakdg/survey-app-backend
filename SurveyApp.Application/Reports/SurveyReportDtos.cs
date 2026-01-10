namespace SurveyApp.Application.Reports;

public sealed record SurveyReportListDto(long SurveyId, string Title, DateTime StartsAtUtc, DateTime EndsAtUtc, int AssignedCount, int CompletedCount, int PendingCount);

public sealed record SurveyUserStatusDto(long UserId, string Email, bool Completed, DateTime? SubmittedAtUtc);

public sealed record SurveyUsersReportDto(long SurveyId, string Title, List<SurveyUserStatusDto> Users);

public sealed record UserAnswerDetailDto(long QuestionId, string QuestionText, short SelectedOptionIndex, string SelectedOptionText);

public sealed record UserSurveyAnswersDto(long SurveyId, string Title, long UserId, string Email, DateTime SubmittedAtUtc, List<UserAnswerDetailDto> Answers);
