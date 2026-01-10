using Microsoft.EntityFrameworkCore;
using SurveyApp.Core.Abstractions;
using SurveyApp.Infrastructure.Persistence;

namespace SurveyApp.Infrastructure.Repositories;

public sealed class SurveyReportRepository : ISurveyReportRepository
{
    private readonly AppDbContext _db;
    public SurveyReportRepository(AppDbContext db) => _db = db;

    public async Task<List<SurveyReportRow>> ListSurveysAsync(string? search, CancellationToken ct)
    {
        var surveys = _db.Surveys.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            surveys = surveys.Where(s => EF.Functions.Like(s.Title, pattern));
        }

        var query =
            from s in surveys
            join a in _db.SurveyAssignments.AsNoTracking()
                .GroupBy(x => x.SurveyId)
                .Select(g => new { SurveyId = g.Key, Cnt = g.Count() })
                on s.Id equals a.SurveyId into aj
            from a in aj.DefaultIfEmpty()

            join c in _db.SurveySubmissions.AsNoTracking()
                .GroupBy(x => x.SurveyId)
                .Select(g => new { SurveyId = g.Key, Cnt = g.Count() })
                on s.Id equals c.SurveyId into cj
            from c in cj.DefaultIfEmpty()

            orderby s.Id descending
            select new SurveyReportRow(
                s.Id,
                s.Title,
                s.StartsAtUtc,
                s.EndsAtUtc,
                a != null ? a.Cnt : 0,
                c != null ? c.Cnt : 0
            );

        return await query.ToListAsync(ct);
    }


    public async Task<SurveyUsersReport?> GetSurveyUsersAsync(long surveyId, string? search, CancellationToken ct)
    {
        var survey = await _db.Surveys.AsNoTracking()
            .Where(s => s.Id == surveyId)
            .Select(s => new { s.Id, s.Title })
            .FirstOrDefaultAsync(ct);

        if (survey is null) return null;

        var assignments =
            from a in _db.SurveyAssignments.AsNoTracking()
            join u in _db.Users.AsNoTracking() on a.UserId equals u.Id
            where a.SurveyId == surveyId
            select new { u.Id, Email = u.Email ?? u.UserName ?? "" };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            assignments = assignments.Where(x => EF.Functions.Like(x.Email, pattern));
        }

        var submissions = _db.SurveySubmissions.AsNoTracking()
            .Where(x => x.SurveyId == surveyId)
            .Select(x => new { x.UserId, x.SubmittedAtUtc });

        var query =
            from a in assignments
            join s in submissions on a.Id equals s.UserId into sj
            from s in sj.DefaultIfEmpty()
            orderby a.Email
            select new SurveyUserRow(
                a.Id,
                a.Email,
                s != null,
                s == null ? null : s.SubmittedAtUtc
            );

        var users = await query.ToListAsync(ct);
        return new SurveyUsersReport(survey.Id, survey.Title, users);
    }


    public async Task<UserSurveyAnswersReport?> GetUserAnswersAsync(long surveyId, long userId, CancellationToken ct)
    {
        var submission = await _db.SurveySubmissions.AsNoTracking()
            .Where(x => x.SurveyId == surveyId && x.UserId == userId)
            .Select(x => new { x.Id, x.SubmittedAtUtc })
            .FirstOrDefaultAsync(ct);

        if (submission is null) return null;

        var survey = await _db.Surveys.AsNoTracking()
            .Where(s => s.Id == surveyId)
            .Select(s => new { s.Id, s.Title })
            .FirstAsync(ct);

        var user = await _db.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, Email = u.Email ?? u.UserName ?? "" })
            .FirstAsync(ct);

        var rows = await (
            from a in _db.SurveySubmissionAnswers.AsNoTracking()
            join q in _db.Questions.AsNoTracking() on a.QuestionId equals q.Id

            join o in _db.AnswerOptions.AsNoTracking()
                on new { TemplateId = q.AnswerTemplateId, SortOrder = a.SelectedOptionIndex }
                equals new { TemplateId = o.AnswerTemplateId, SortOrder = o.SortOrder }
                into oj
            from o in oj.DefaultIfEmpty()

            where a.SubmissionId == submission.Id
            select new
            {
                QuestionId = q.Id,
                QuestionText = q.Text,
                a.SelectedOptionIndex,
                SelectedOptionText = o == null ? "" : o.Text
            }
        ).ToListAsync(ct);

        var answerRows = rows.Select(r => new UserAnswerRow(
            r.QuestionId,
            r.QuestionText,
            r.SelectedOptionIndex,
            r.SelectedOptionText
        )).ToList();

        return new UserSurveyAnswersReport(
            survey.Id,
            survey.Title,
            user.Id,
            user.Email,
            submission.SubmittedAtUtc,
            answerRows
        );
    }


}
