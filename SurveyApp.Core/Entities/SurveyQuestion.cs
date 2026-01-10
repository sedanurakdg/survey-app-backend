namespace SurveyApp.Core.Entities;

public class SurveyQuestion
{
    public long SurveyId { get; set; }
    public Survey Survey { get; set; } = null!;

    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int SortOrder { get; set; }
}
