using Microsoft.EntityFrameworkCore;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.DTOs.Ratings;
using Se7ety.Api.Exceptions;
using Se7ety.Api.Repositories.Interfaces;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class RatingService(
    IRepository<Rating> ratings,
    IRepository<PatientProfile> patientProfiles,
    IRepository<DoctorProfile> doctorProfiles,
    ICurrentUserService currentUser) : IRatingService
{
    public async Task<RatingResponse> RateDoctorAsync(RateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        var patient = await patientProfiles.Query()
            .FirstOrDefaultAsync(profile => profile.UserId == currentUser.UserId, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Patient profile was not found.");

        var doctorExists = await doctorProfiles.Query()
            .AnyAsync(profile => profile.Id == request.DoctorProfileId, cancellationToken);

        if (!doctorExists)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Doctor was not found.");
        }

        var rating = await ratings.Query()
            .FirstOrDefaultAsync(current =>
                current.PatientProfileId == patient.Id &&
                current.DoctorProfileId == request.DoctorProfileId,
                cancellationToken);

        if (rating is null)
        {
            rating = new Rating
            {
                PatientProfileId = patient.Id,
                DoctorProfileId = request.DoctorProfileId,
                Value = request.Value,
                Comment = TrimToNull(request.Comment)
            };

            await ratings.AddAsync(rating, cancellationToken);
        }
        else
        {
            rating.Value = request.Value;
            rating.Comment = TrimToNull(request.Comment);
            rating.UpdatedAtUtc = DateTime.UtcNow;
            ratings.Update(rating);
        }

        await ratings.SaveChangesAsync(cancellationToken);

        var averageRating = await ratings.Query()
            .Where(current => current.DoctorProfileId == request.DoctorProfileId)
            .AverageAsync(current => (double)current.Value, cancellationToken);

        return new RatingResponse
        {
            Id = rating.Id,
            DoctorProfileId = rating.DoctorProfileId,
            PatientProfileId = rating.PatientProfileId,
            Value = rating.Value,
            Comment = rating.Comment,
            DoctorAverageRating = Math.Round(averageRating, 1),
            CreatedAtUtc = rating.CreatedAtUtc
        };
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
