using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Se7ety.Api.Domain.Enums;
using Se7ety.Api.DTOs.Ratings;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(UserRole.Patient))]
[Route("api/[controller]")]
public sealed class RatingsController(IRatingService ratingService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RatingResponse>> RateDoctor(
        RateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await ratingService.RateDoctorAsync(request, cancellationToken));
    }
}
