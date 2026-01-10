namespace SurveyApp.Core.Entities;

public class SurveySubmissionAnswer
{
    public long SubmissionId { get; set; }
    public SurveySubmission Submission { get; set; } = null!;

    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    // 1..N (N = AnswerTemplate.ChoiceCount)
    public short SelectedOptionIndex { get; set; }
}
