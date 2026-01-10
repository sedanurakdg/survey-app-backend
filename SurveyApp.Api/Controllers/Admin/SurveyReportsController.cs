using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Reports;

namespace SurveyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/reports/surveys")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public sealed class SurveyReportsController : ControllerBase
{
    private readonly ISurveyReportService _service;
    public SurveyReportsController(ISurveyReportService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? search, CancellationToken ct)
        => Ok(await _service.ListAsync(search, ct));

    [HttpGet("{surveyId:long}/users")]
    public async Task<IActionResult> Users(long surveyId, [FromQuery] string? search, CancellationToken ct)
    {
        var dto = await _service.SurveyUsersAsync(surveyId, search, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{surveyId:long}/users/{userId:long}")]
    public async Task<IActionResult> UserAnswers(long surveyId, long userId, CancellationToken ct)
    {
        var dto = await _service.UserAnswersAsync(surveyId, userId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}
