using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Se7ety.Api.DTOs.Settings;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class SettingsController(ISettingsService settingsService) : ControllerBase
{
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await settingsService.ChangePasswordAsync(request, cancellationToken);
        return NoContent();
    }
}
