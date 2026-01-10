using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.AnswerTemplates;

namespace SurveyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/answer-templates")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public sealed class AnswerTemplateController : ControllerBase
{
    private readonly IAnswerTemplateService _service;

    public AnswerTemplateController(IAnswerTemplateService service) => _service = service;


    [HttpGet]
    public async Task<ActionResult<List<AnswerTemplateListDto>>> List(CancellationToken ct)
        => Ok(await _service.ListAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AnswerTemplateDetailDto>> Get(long id, CancellationToken ct)
    {
        var item = await _service.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnswerTemplateRequest req, CancellationToken ct)
    {
        var id = await _service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateAnswerTemplateRequest req, CancellationToken ct)
        => await _service.UpdateAsync(id, req, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
