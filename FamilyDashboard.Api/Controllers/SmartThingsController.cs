using FamilyDashboard.Api.Contracts;
using FamilyDashboard.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyDashboard.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me/smarthings")]
public class SmartThingsController : ControllerBase
{
    private readonly ISmartThingsCredentialStore _credentialStore;
    private readonly ISmartThingsProxyService _proxyService;

    public SmartThingsController(ISmartThingsCredentialStore credentialStore, ISmartThingsProxyService proxyService)
    {
        _credentialStore = credentialStore;
        _proxyService = proxyService;
    }

    [HttpGet("status")]
    public async Task<ActionResult<SmartThingsStatusResponse>> GetStatus()
    {
        var userId = GetUserId();
        var token = await _credentialStore.GetTokenAsync(userId);
        if (token is null)
            return Ok(new SmartThingsStatusResponse(false, false));

        var isValid = await _proxyService.ValidateTokenAsync(token);
        return Ok(new SmartThingsStatusResponse(true, isValid));
    }

    [HttpPost("token")]
    public async Task<ActionResult<SmartThingsStatusResponse>> SaveToken([FromBody] SmartThingsTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            return BadRequest("Token is required.");

        var trimmedToken = request.Token.Trim();
        var isValid = await _proxyService.ValidateTokenAsync(trimmedToken);
        if (!isValid)
            return BadRequest(new SmartThingsStatusResponse(false, false));

        var userId = GetUserId();
        await _credentialStore.SaveTokenAsync(userId, trimmedToken);

        return Ok(new SmartThingsStatusResponse(true, true));
    }

    [HttpDelete("token")]
    public async Task<IActionResult> DeleteToken()
    {
        var userId = GetUserId();
        await _credentialStore.RemoveTokenAsync(userId);
        return NoContent();
    }

    [HttpGet("devices")]
    public async Task<IActionResult> GetDevices()
    {
        var token = await GetRequiredTokenAsync();
        if (token is null)
            return BadRequest("No SmartThings token configured for this user.");

        var devices = await _proxyService.GetDevicesAsync(token);
        return Ok(devices);
    }

    [HttpGet("devices/{id}")]
    public async Task<IActionResult> GetDevice(string id)
    {
        var token = await GetRequiredTokenAsync();
        if (token is null)
            return BadRequest("No SmartThings token configured for this user.");

        var device = await _proxyService.GetDeviceAsync(token, id);
        return Ok(device);
    }

    [HttpPost("devices/{id}/switch")]
    public async Task<IActionResult> SetSwitch(string id, [FromBody] SetSwitchRequest request)
    {
        var token = await GetRequiredTokenAsync();
        if (token is null)
            return BadRequest("No SmartThings token configured for this user.");

        await _proxyService.SetSwitchAsync(token, id, request.SwitchValue);
        return NoContent();
    }

    private string GetUserId()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("Missing authenticated user id.");

        return userId;
    }

    private async Task<string?> GetRequiredTokenAsync()
    {
        var userId = GetUserId();
        return await _credentialStore.GetTokenAsync(userId);
    }
}
