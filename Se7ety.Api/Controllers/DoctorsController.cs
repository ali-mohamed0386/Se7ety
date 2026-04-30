using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Se7ety.Api.Domain.Enums;
using Se7ety.Api.DTOs.Common;
using Se7ety.Api.DTOs.Doctors;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(UserRole.Patient))]
[Route("api/[controller]")]
public sealed class DoctorsController(IDoctorService doctorService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<DoctorCardResponse>>> Search(
        [FromQuery] DoctorSearchQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await doctorService.SearchDoctorsAsync(query, cancellationToken));
    }

    [HttpGet("{doctorProfileId:guid}")]
    public async Task<ActionResult<DoctorDetailsResponse>> GetDetails(
        Guid doctorProfileId,
        CancellationToken cancellationToken)
    {
        return Ok(await doctorService.GetDoctorDetailsAsync(doctorProfileId, cancellationToken));
    }
}
