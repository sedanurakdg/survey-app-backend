namespace SurveyApp.Application.SurveyFill;

public interface ISurveyFillService
{
    Task<List<UserSurveyListDto>> ListMyActiveAsync(long userId, CancellationToken ct);
    Task<UserSurveyDetailDto?> GetForFillAsync(long surveyId, long userId, CancellationToken ct);
    Task<SubmitResult> SubmitAsync(long surveyId, long userId, SubmitSurveyRequest req, CancellationToken ct);
}

public sealed record SubmitResult(bool Succeeded, string? Error);
