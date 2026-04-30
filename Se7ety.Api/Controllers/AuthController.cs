using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Se7ety.Api.DTOs.Auth;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("patient/register")]
    public async Task<ActionResult<AuthResponse>> RegisterPatient(AuthRequest request, CancellationToken cancellationToken)
    {
        return Ok(await authService.RegisterPatientAsync(request, cancellationToken));
    }

    [HttpPost("doctor/register")]
    public async Task<ActionResult<AuthResponse>> RegisterDoctor(AuthRequest request, CancellationToken cancellationToken)
    {
        return Ok(await authService.RegisterDoctorAsync(request, cancellationToken));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(AuthRequest request, CancellationToken cancellationToken)
    {
        return Ok(await authService.LoginAsync(request, cancellationToken));
    }
}
