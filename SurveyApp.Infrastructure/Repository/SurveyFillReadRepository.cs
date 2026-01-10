using Microsoft.EntityFrameworkCore;
using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;
using SurveyApp.Infrastructure.Persistence;

namespace SurveyApp.Infrastructure.Repositories;

public sealed class SurveyFillReadRepository : ISurveyFillReadRepository
{
    private readonly AppDbContext _db;
    public SurveyFillReadRepository(AppDbContext db) => _db = db;

    public async Task<List<Survey>> ListAssignedActiveAsync(long userId, DateTime nowUtc, CancellationToken ct)
        => await _db.Surveys
            .AsNoTracking()
            .Where(s => s.IsActive
                     && s.StartsAtUtc <= nowUtc
                     && nowUtc <= s.EndsAtUtc
                     && s.Assignments.Any(a => a.UserId == userId))
            .OrderByDescending(s => s.Id)
            .ToListAsync(ct);

    public async Task<Survey?> GetAssignedActiveDetailAsync(long surveyId, long userId, DateTime nowUtc, CancellationToken ct)
        => await _db.Surveys
            .AsNoTracking()
            .Where(s => s.Id == surveyId
                     && s.IsActive
                     && s.StartsAtUtc <= nowUtc
                     && nowUtc <= s.EndsAtUtc
                     && s.Assignments.Any(a => a.UserId == userId))
            .Include(s => s.Questions.OrderBy(x => x.SortOrder))
                .ThenInclude(sq => sq.Question)
                    .ThenInclude(q => q.AnswerTemplate)
                        .ThenInclude(t => t.Options)
            .FirstOrDefaultAsync(ct);
}
