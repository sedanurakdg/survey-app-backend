using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Core.Identity;

namespace SurveyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public sealed class AdminUsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<long>> _roleManager;

    public AdminUsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole<long>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public sealed record CreateUserRequest(string Email, string Password, string? Role);
    public sealed record UserDto(long Id, string Email, IList<string> Roles);

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        var role = string.IsNullOrWhiteSpace(req.Role) ? "User" : req.Role.Trim();

        if (!await _roleManager.RoleExistsAsync(role))
            return BadRequest($"Role '{role}' does not exist.");

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            return Conflict("User already exists.");

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var create = await _userManager.CreateAsync(user, req.Password);
        if (!create.Succeeded)
            return BadRequest(create.Errors.Select(e => e.Description));

        var addRole = await _userManager.AddToRoleAsync(user, role);
        if (!addRole.Succeeded)
            return BadRequest(addRole.Errors.Select(e => e.Description));

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UserDto(user.Id, user.Email ?? user.UserName ?? "", roles));
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> List([FromQuery] string? search)
    {
        var q = _userManager.Users;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            q = q.Where(u => (u.Email ?? "").ToLower().Contains(s) || (u.UserName ?? "").ToLower().Contains(s));
        }

        var users = q.OrderBy(u => u.Email).Take(100).ToList();

        var result = new List<UserDto>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            result.Add(new UserDto(u.Id, u.Email ?? u.UserName ?? "", roles));
        }

        return Ok(result);
    }
}
