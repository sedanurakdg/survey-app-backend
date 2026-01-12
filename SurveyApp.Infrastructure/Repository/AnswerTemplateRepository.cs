using Microsoft.EntityFrameworkCore;
using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;
using SurveyApp.Infrastructure.Persistence;

namespace SurveyApp.Infrastructure.Repositories;

public sealed class AnswerTemplateRepository : IAnswerTemplateRepository
{
    private readonly AppDbContext _db;

    public AnswerTemplateRepository(AppDbContext db) => _db = db;

    public async Task<List<AnswerTemplate>> ListAsync(CancellationToken ct)
        => await _db.AnswerTemplates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Include(x => x.Options)
            .ToListAsync(ct);
    public Task<bool> ExistsAsync(long id, CancellationToken ct)
    => _db.AnswerTemplates.AsNoTracking().AnyAsync(x => x.Id == id, ct);

    public async Task<AnswerTemplate?> GetByIdAsync(long id, bool tracking, CancellationToken ct)
    {
        var q = _db.AnswerTemplates
            .Include(x => x.Options)
            .Where(x => x.Id == id);

        if (!tracking) q = q.AsNoTracking();

        return await q.FirstOrDefaultAsync(ct);
    }

    public Task AddAsync(AnswerTemplate entity, CancellationToken ct)
        => _db.AnswerTemplates.AddAsync(entity, ct).AsTask();

    public Task DeleteAsync(AnswerTemplate entity, CancellationToken ct)
    {
        _db.AnswerTemplates.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
