using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Surveys;

namespace SurveyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/surveys")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public sealed class SurveysController : ControllerBase
{
    private readonly ISurveyService _service;
    public SurveysController(ISurveyService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<SurveyListDto>>> List(CancellationToken ct)
        => Ok(await _service.ListAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<SurveyDetailDto>> Get(long id, CancellationToken ct)
    {
        var item = await _service.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSurveyRequest req, CancellationToken ct)
    {
        var id = await _service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateSurveyRequest req, CancellationToken ct)
        => await _service.UpdateAsync(id, req, ct) ? NoContent() : NotFound();

    // Soft delete
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
