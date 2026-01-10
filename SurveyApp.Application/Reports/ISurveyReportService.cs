namespace SurveyApp.Application.Reports;

public interface ISurveyReportService
{
    Task<List<SurveyReportListDto>> ListAsync(string? search, CancellationToken ct);
    Task<SurveyUsersReportDto?> SurveyUsersAsync(long surveyId, string? search, CancellationToken ct);
    Task<UserSurveyAnswersDto?> UserAnswersAsync(long surveyId, long userId, CancellationToken ct);
}
