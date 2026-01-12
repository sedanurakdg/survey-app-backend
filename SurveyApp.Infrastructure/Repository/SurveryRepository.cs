using Microsoft.EntityFrameworkCore;
using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;
using SurveyApp.Infrastructure.Persistence;

namespace SurveyApp.Infrastructure.Repositories;

public sealed class SurveyRepository : ISurveyRepository
{
    private readonly AppDbContext _db;
    public SurveyRepository(AppDbContext db) => _db = db;

    public async Task<List<Survey>> ListAsync(CancellationToken ct)
        => await _db.Surveys
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.Id)
            .ToListAsync(ct);

    public async Task<Survey?> GetByIdAsync(long id, bool tracking, CancellationToken ct)
    {
        IQueryable<Survey> q = _db.Surveys
            .Include(x => x.Questions)
            .Include(x => x.Assignments)
            .Where(x => x.Id == id);

        if (!tracking) q = q.AsNoTracking();

        return await q.FirstOrDefaultAsync(ct);
    }

    public Task AddAsync(Survey entity, CancellationToken ct)
        => _db.Surveys.AddAsync(entity, ct).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);

    public async Task<HashSet<long>> GetExistingQuestionIdsAsync(IReadOnlyCollection<long> questionIds, CancellationToken ct)
    {
        if (questionIds.Count == 0) return new HashSet<long>();

        var ids = await _db.Questions
            .AsNoTracking()
            .Where(x => questionIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(ct);

        return ids.ToHashSet();
    }

    public async Task<HashSet<long>> GetExistingUserIdsAsync(IReadOnlyCollection<long> userIds, CancellationToken ct)
    {
        if (userIds.Count == 0) return new HashSet<long>();

        // AppDbContext Identity tablosuna sahip olduğu için Users erişilebilir.
        var ids = await _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync(ct);

        return ids.ToHashSet();
    }
}
