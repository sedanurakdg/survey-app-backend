namespace SurveyApp.Application.Questions;

public sealed record QuestionListDto(long Id, string Text, long AnswerTemplateId, string AnswerTemplateName, bool IsActive);

public sealed record QuestionDetailDto(long Id, string Text, long AnswerTemplateId, string AnswerTemplateName, bool IsActive);

public sealed record CreateQuestionRequest(string Text, long AnswerTemplateId);

public sealed record UpdateQuestionRequest(string Text, long AnswerTemplateId, bool IsActive);
