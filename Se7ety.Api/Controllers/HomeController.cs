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
public sealed class HomeController(IDoctorService doctorService) : ControllerBase
{
    [HttpGet("doctors")]
    public async Task<ActionResult<PagedResponse<DoctorCardResponse>>> GetDoctors(
        [FromQuery] DoctorListQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await doctorService.GetDoctorsAsync(query, cancellationToken));
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<SpecialtyResponse>>> GetCategories(CancellationToken cancellationToken)
    {
        return Ok(await doctorService.GetSpecialtiesAsync(cancellationToken));
    }
}
