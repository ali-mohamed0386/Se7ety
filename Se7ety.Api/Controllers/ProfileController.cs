using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Se7ety.Api.Domain.Enums;
using Se7ety.Api.DTOs.Profiles;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ProfileController(IProfileService profileService) : ControllerBase
{
    [HttpGet("patient/me")]
    [Authorize(Roles = nameof(UserRole.Patient))]
    public async Task<ActionResult<PatientProfileResponse>> GetPatientProfile(CancellationToken cancellationToken)
    {
        return Ok(await profileService.GetMyPatientProfileAsync(cancellationToken));
    }

    [HttpPut("patient")]
    [Authorize(Roles = nameof(UserRole.Patient))]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PatientProfileResponse>> UpdatePatientProfile(
        [FromForm] UpdatePatientProfileRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await profileService.UpdatePatientProfileAsync(request, cancellationToken));
    }

    [HttpGet("doctor/me")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<ActionResult<DoctorProfileResponse>> GetDoctorProfile(CancellationToken cancellationToken)
    {
        return Ok(await profileService.GetMyDoctorProfileAsync(cancellationToken));
    }

    [HttpPut("doctor")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<DoctorProfileResponse>> UpdateDoctorProfile(
        [FromForm] UpdateDoctorProfileRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await profileService.UpdateDoctorProfileAsync(request, cancellationToken));
    }

    [HttpPut("doctor/available-slots")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<ActionResult<DoctorProfileResponse>> UpdateDoctorSlots(
        UpdateDoctorSlotsRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await profileService.UpdateDoctorSlotsAsync(request, cancellationToken));
    }
}
