namespace SurveyApp.Core.Entities;

public class Survey
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<SurveyQuestion> Questions { get; set; } = new List<SurveyQuestion>();
    public ICollection<SurveyAssignment> Assignments { get; set; } = new List<SurveyAssignment>();
}
