namespace SurveyApp.Application.Surveys;

public sealed record SurveyListDto(long Id, string Title, bool IsActive, DateTime StartsAtUtc, DateTime EndsAtUtc);

public sealed record SurveyDetailDto(
    long Id,
    string Title,
    string? Description,
    bool IsActive,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    List<SurveyQuestionDto> Questions,
    List<long> AssignedUserIds);

public sealed record SurveyQuestionDto(long QuestionId, int SortOrder);

public sealed record CreateSurveyRequest(
    string Title,
    string? Description,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    List<long> QuestionIds,
    List<long> UserIds);

public sealed record UpdateSurveyRequest(
    string Title,
    string? Description,
    bool IsActive,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    List<long> QuestionIds,
    List<long> UserIds);
