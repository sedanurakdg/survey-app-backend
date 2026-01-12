namespace SurveyApp.Application.SurveyFill;

public sealed record UserSurveyListDto(long Id, string Title, DateTime StartsAtUtc, DateTime EndsAtUtc, bool IsSubmitted);

public sealed record ChoiceDto(short Index, string Text);

public sealed record FillQuestionDto(
    long QuestionId,
    string Text,
    List<ChoiceDto> Choices,
    int? SelectedOptionIndex
);


public sealed record UserSurveyDetailDto(long Id, string Title, string? Description, DateTime StartsAtUtc, DateTime EndsAtUtc, bool IsSubmitted, List<FillQuestionDto> Questions);

public sealed record SubmitAnswerDto(long QuestionId, short SelectedOptionIndex);

public sealed record SubmitSurveyRequest(List<SubmitAnswerDto> Answers);
