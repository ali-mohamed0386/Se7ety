using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Se7ety.Api.DTOs.Common;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public sealed class HealthController(IHealthService healthService) : ControllerBase
{
    [HttpGet]
    public ActionResult<HealthCheckResponse> Get()
    {
        return Ok(healthService.GetStatus());
    }
}
