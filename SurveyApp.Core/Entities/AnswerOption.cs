namespace SurveyApp.Core.Entities;

public class AnswerOption
{
    public long Id { get; set; }

    public long AnswerTemplateId { get; set; }
    public AnswerTemplate AnswerTemplate { get; set; } = null!;

    public string Text { get; set; } = null!;
    public short SortOrder { get; set; } // 1..4
}
