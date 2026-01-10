using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.SurveyFill;
using System.Security.Claims;

namespace SurveyApp.Api.Controllers.User;

[ApiController]
[Route("api/user/surveys")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
public sealed class UserSurveysController : ControllerBase
{
    private readonly ISurveyFillService _service;
    public UserSurveysController(ISurveyFillService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<UserSurveyListDto>>> List(CancellationToken ct)
    {
        var userId = GetUserId();
        return Ok(await _service.ListMyActiveAsync(userId, ct));
    }

    [HttpGet("{surveyId:long}")]
    public async Task<ActionResult<UserSurveyDetailDto>> Get(long surveyId, CancellationToken ct)
    {
        var userId = GetUserId();
        var dto = await _service.GetForFillAsync(surveyId, userId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("{surveyId:long}/submit")]
    public async Task<IActionResult> Submit(long surveyId, [FromBody] SubmitSurveyRequest req, CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await _service.SubmitAsync(surveyId, userId, req, ct);

        if (!result.Succeeded)
            return BadRequest(new { message = result.Error });

        return Ok(new { status = "submitted" });
    }

    private long GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");

        return long.Parse(sub!);
    }
}
