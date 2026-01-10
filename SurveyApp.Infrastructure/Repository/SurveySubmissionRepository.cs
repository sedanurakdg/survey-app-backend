using Microsoft.EntityFrameworkCore;
using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;
using SurveyApp.Infrastructure.Persistence;

namespace SurveyApp.Infrastructure.Repositories;

public sealed class SurveySubmissionRepository : ISurveySubmissionRepository
{
    private readonly AppDbContext _db;
    public SurveySubmissionRepository(AppDbContext db) => _db = db;

    public Task<bool> HasSubmittedAsync(long surveyId, long userId, CancellationToken ct)
        => _db.SurveySubmissions.AsNoTracking().AnyAsync(x => x.SurveyId == surveyId && x.UserId == userId, ct);

    public Task AddAsync(SurveySubmission entity, CancellationToken ct)
        => _db.SurveySubmissions.AddAsync(entity, ct).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
    public async Task<HashSet<long>> GetSubmittedSurveyIdsAsync(long userId, IReadOnlyCollection<long> surveyIds, CancellationToken ct)
    {
        if (surveyIds.Count == 0) return new HashSet<long>();

        var ids = await _db.SurveySubmissions.AsNoTracking()
            .Where(x => x.UserId == userId && surveyIds.Contains(x.SurveyId))
            .Select(x => x.SurveyId)
            .Distinct()
            .ToListAsync(ct);

        return ids.ToHashSet();
    }

    public async Task<SurveySubmission?> GetBySurveyAndUserAsync(long surveyId, long userId, CancellationToken ct)
        => await _db.SurveySubmissions
            .AsNoTracking()
            .Where(x => x.SurveyId == surveyId && x.UserId == userId)
            .Include(x => x.Answers)
            .FirstOrDefaultAsync(ct);
}
