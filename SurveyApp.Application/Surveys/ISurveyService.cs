namespace SurveyApp.Application.Surveys;

public interface ISurveyService
{
    Task<List<SurveyListDto>> ListAsync(CancellationToken ct);
    Task<SurveyDetailDto?> GetAsync(long id, CancellationToken ct);

    Task<long> CreateAsync(CreateSurveyRequest req, CancellationToken ct);
    Task<bool> UpdateAsync(long id, UpdateSurveyRequest req, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
}
