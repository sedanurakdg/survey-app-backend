namespace SurveyApp.Core.Entities;

public class Question
{
    public long Id { get; set; }

    public string Text { get; set; } = null!;

    public long AnswerTemplateId { get; set; }
    public AnswerTemplate AnswerTemplate { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
