using SurveyApp.Core.Abstractions;
using SurveyApp.Core.Entities;

namespace SurveyApp.Application.AnswerTemplates;

public sealed class AnswerTemplateService : IAnswerTemplateService
{
    private readonly IAnswerTemplateRepository _repo;

    public AnswerTemplateService(IAnswerTemplateRepository repo) => _repo = repo;

    public async Task<List<AnswerTemplateListDto>> ListAsync(CancellationToken ct)
    {
        var items = await _repo.ListAsync(ct);

        return items
            .OrderByDescending(x => x.Id)
            .Where(s => s.IsActive)
            .Select(x => new AnswerTemplateListDto(x.Id, x.Name, x.IsActive, x.Options.Count))
            .ToList();
    }

    public async Task<AnswerTemplateDetailDto?> GetAsync(long id, CancellationToken ct)
    {
        var t = await _repo.GetByIdAsync(id, tracking: false, ct);
        if (t is null) return null;

        var options = t.Options
            .OrderBy(o => o.SortOrder)
            .Select(o => new AnswerOptionDto(o.Id, o.Text, o.SortOrder))
            .ToList();

        return new AnswerTemplateDetailDto(t.Id, t.Name, t.IsActive, options);
    }

    public async Task<long> CreateAsync(CreateAnswerTemplateRequest req, CancellationToken ct)
    {
        var entity = new AnswerTemplate
        {
            Name = req.Name.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            Options = req.Options
                .OrderBy(x => x.SortOrder)
                .Select(x => new AnswerOption
                {
                    Text = x.Text.Trim(),
                    SortOrder = x.SortOrder
                })
                .ToList()
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(long id, UpdateAnswerTemplateRequest req, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, tracking: true, ct);
        if (entity is null) return false;

        entity.Name = req.Name.Trim();
        entity.IsActive = req.IsActive;

        // Sağlam yaklaşım: seçenekleri resetle
        entity.Options.Clear();
        foreach (var opt in req.Options.OrderBy(x => x.SortOrder))
        {
            entity.Options.Add(new AnswerOption
            {
                Text = opt.Text.Trim(),
                SortOrder = opt.SortOrder
            });
        }

        await _repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, tracking: true, ct);
        if (entity is null) return false;

        entity.IsActive = false;
        await _repo.SaveChangesAsync(ct);
        return true;
    }

}
