using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using SurveyApp.Core.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyApp.Api.Controllers;

[ApiController]
[EnableRateLimiting("auth")]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    private readonly IWebHostEnvironment _env;
    public AuthController(UserManager<AppUser> userManager, IConfiguration config, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _config = config;
        _env = env;
    }
    public sealed record RegisterRequest(string Email, string Password);
    public sealed record LoginRequest(string Email, string Password);
    public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc);

    [HttpPost("register")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!_env.IsDevelopment()) return NotFound();
        var email = req.Email.Trim().ToLowerInvariant();

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        // Default role: User
        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "User created." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Unauthorized("Invalid credentials.");

        var passwordOk = await _userManager.CheckPasswordAsync(user, req.Password);
        if (!passwordOk)
            return Unauthorized("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);

        var token = CreateJwt(user, roles);
        return Ok(token);
    }

    private AuthResponse CreateJwt(AppUser user, IList<string> roles)
    {
        var jwt = _config.GetSection("Jwt");
        var secret = jwt["Secret"]!;
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())

        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expires = DateTime.UtcNow.AddHours(8);

        var jwtToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return new AuthResponse(accessToken, expires);
    }
}
