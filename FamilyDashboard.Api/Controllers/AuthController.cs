using FamilyDashboard.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FamilyDashboard.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("status")]
    public ActionResult<AuthStatusResponse> GetStatus()
    {
        if (User.Identity?.IsAuthenticated != true)
            return Ok(new AuthStatusResponse(false, null));

        return Ok(new AuthStatusResponse(true, User.Identity.Name));
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthActionResponse>> Register([FromBody] AuthRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new AuthActionResponse(false, "Username and password are required."));

        var normalizedUsername = request.UserName.Trim().ToLowerInvariant();
        var existingUser = await _userManager.FindByNameAsync(normalizedUsername);
        if (existingUser is not null)
            return Conflict(new AuthActionResponse(false, "Username already exists."));

        var user = new IdentityUser
        {
            UserName = normalizedUsername,
            Email = $"{normalizedUsername}@example.com"
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return BadRequest(new AuthActionResponse(false, string.Join(" ", createResult.Errors.Select(x => x.Description))));

        await _signInManager.SignInAsync(user, isPersistent: true);

        return Ok(new AuthActionResponse(true));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthActionResponse>> Login([FromBody] AuthRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new AuthActionResponse(false, "Username and password are required."));

        var normalizedUsername = request.UserName.Trim().ToLowerInvariant();
        var result = await _signInManager.PasswordSignInAsync(normalizedUsername, request.Password, isPersistent: true, lockoutOnFailure: false);

        if (!result.Succeeded)
            return Unauthorized(new AuthActionResponse(false, "Invalid username or password."));

        return Ok(new AuthActionResponse(true));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }
}
