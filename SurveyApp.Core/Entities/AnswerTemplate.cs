
namespace SurveyApp.Core.Entities;

public class AnswerTemplate
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<AnswerOption> Options { get; set; } = new List<AnswerOption>();

    public string GetOptionText(short selectedIndex)
    {
        if (selectedIndex < 1) return "";

        return Options
            .OrderBy(o => o.SortOrder)
            .FirstOrDefault(o => o.SortOrder == selectedIndex)
            ?.Text
            ?? "";
    }

}
