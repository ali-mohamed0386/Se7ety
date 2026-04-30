using Se7ety.Api.DTOs.Ratings;

namespace Se7ety.Api.Services.Interfaces;

public interface IRatingService
{
    Task<RatingResponse> RateDoctorAsync(RateDoctorRequest request, CancellationToken cancellationToken = default);
}
