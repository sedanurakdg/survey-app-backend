namespace SurveyApp.Application.AnswerTemplates;

public sealed record AnswerOptionDto(long? Id, string Text, short SortOrder);

public sealed record AnswerTemplateListDto(long Id, string Name, bool IsActive, int OptionCount);

public sealed record AnswerTemplateDetailDto(long Id, string Name, bool IsActive, IReadOnlyList<AnswerOptionDto> Options);

public sealed record CreateAnswerTemplateRequest(string Name, List<AnswerOptionDto> Options);

public sealed record UpdateAnswerTemplateRequest(string Name, bool IsActive, List<AnswerOptionDto> Options);
