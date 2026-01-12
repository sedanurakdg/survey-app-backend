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
        var surveys = _db.Surveys.AsNoTracking()
            .Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            surveys = surveys.Where(s => EF.Functions.Like(s.Title, pattern));
        }

        var assignmentsAgg =
            _db.SurveyAssignments.AsNoTracking()
                .GroupBy(x => x.SurveyId)
                .Select(g => new { SurveyId = g.Key, Cnt = g.Count() });

        var submissionsAgg =
            _db.SurveySubmissions.AsNoTracking()
                .GroupBy(x => x.SurveyId)
                .Select(g => new { SurveyId = g.Key, Cnt = g.Count() });

        var query =
            from s in surveys
            orderby s.Id descending
            select new SurveyReportRow(
                s.Id,
                s.Title,
                s.StartsAtUtc,
                s.EndsAtUtc,
                assignmentsAgg.Where(x => x.SurveyId == s.Id).Select(x => x.Cnt).FirstOrDefault(),
                submissionsAgg.Where(x => x.SurveyId == s.Id).Select(x => x.Cnt).FirstOrDefault()
            );

        return await query.ToListAsync(ct);
    }



    public async Task<SurveyUsersReport?> GetSurveyUsersAsync(long surveyId, string? search, CancellationToken ct)
    {
        var survey = await _db.Surveys.AsNoTracking()
            .Where(s => s.Id == surveyId && s.IsActive)
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
            .Where(x => x.SurveyId == surveyId);

        var users = await assignments
            .OrderBy(a => a.Email)
            .Select(a => new SurveyUserRow(
                a.Id,
                a.Email,
                submissions.Any(s => s.UserId == a.Id),
                submissions
                    .Where(s => s.UserId == a.Id)
                    .Select(s => (DateTime?)s.SubmittedAtUtc)   // nullable projection
                    .FirstOrDefault()
            ))
            .ToListAsync(ct);

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
            .FirstOrDefaultAsync(ct);

        if (survey is null) return null;

        var user = await _db.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, Email = u.Email ?? u.UserName ?? "" })
            .FirstOrDefaultAsync(ct);

        if (user is null) return null;

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
