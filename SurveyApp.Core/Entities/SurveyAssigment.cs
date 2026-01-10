using SurveyApp.Core.Identity;

namespace SurveyApp.Core.Entities;

public class SurveyAssignment
{
    public long SurveyId { get; set; }
    public Survey Survey { get; set; } = null!;

    public long UserId { get; set; }
    public AppUser User { get; set; } = null!;
}


