using Microsoft.EntityFrameworkCore;
using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;
using SurveyApp.Infrastructure.Persistence;

namespace SurveyApp.Infrastructure.Repositories;

public sealed class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _db;
    public QuestionRepository(AppDbContext db) => _db = db;

    public async Task<List<Question>> ListAsync(CancellationToken ct)
        => await _db.Questions
            .AsNoTracking()
            .Include(x => x.AnswerTemplate)
            .OrderByDescending(x => x.Id)
            .ToListAsync(ct);

    public async Task<Question?> GetByIdAsync(long id, bool tracking, CancellationToken ct)
    {
        var q = _db.Questions
            .Include(x => x.AnswerTemplate)
            .Where(x => x.Id == id);

        if (!tracking) q = q.AsNoTracking();

        return await q.FirstOrDefaultAsync(ct);
    }

    public Task AddAsync(Question entity, CancellationToken ct)
        => _db.Questions.AddAsync(entity, ct).AsTask();

    public Task DeleteAsync(Question entity, CancellationToken ct)
    {
        _db.Questions.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
