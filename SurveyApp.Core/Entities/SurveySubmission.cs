using SurveyApp.Core.Identity;

namespace SurveyApp.Core.Entities;

public class SurveySubmission
{
    public long Id { get; set; }

    public long SurveyId { get; set; }
    public Survey Survey { get; set; } = null!;

    public long UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<SurveySubmissionAnswer> Answers { get; set; } = new List<SurveySubmissionAnswer>();
}
