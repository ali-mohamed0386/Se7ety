using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.DTOs.Profiles;
using Se7ety.Api.Exceptions;
using Se7ety.Api.Helpers;
using Se7ety.Api.Repositories.Interfaces;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class ProfileService(
    IRepository<PatientProfile> patientProfiles,
    IRepository<DoctorProfile> doctorProfiles,
    ICurrentUserService currentUser,
    IFileStorageService fileStorage,
    IMapper mapper) : IProfileService
{
    public async Task<PatientProfileResponse> GetMyPatientProfileAsync(CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentPatientProfileAsync(cancellationToken);
        return mapper.Map<PatientProfileResponse>(profile);
    }

    public async Task<PatientProfileResponse> UpdatePatientProfileAsync(UpdatePatientProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentPatientProfileAsync(cancellationToken);

        profile.PhoneNumber = TrimToNull(request.PhoneNumber);
        profile.Bio = TrimToNull(request.Bio);
        profile.UpdatedAtUtc = DateTime.UtcNow;

        if (request.ProfileImage is not null)
        {
            profile.ProfileImageUrl = await fileStorage.SaveProfileImageAsync(request.ProfileImage, cancellationToken);
        }

        patientProfiles.Update(profile);
        await patientProfiles.SaveChangesAsync(cancellationToken);

        return mapper.Map<PatientProfileResponse>(profile);
    }

    public async Task<DoctorProfileResponse> GetMyDoctorProfileAsync(CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentDoctorProfileAsync(cancellationToken);
        return MapDoctorProfile(profile);
    }

    public async Task<DoctorProfileResponse> UpdateDoctorProfileAsync(UpdateDoctorProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentDoctorProfileAsync(cancellationToken);

        profile.Name = TrimToNull(request.Name);
        profile.Specialty = TrimToNull(request.Specialty);
        profile.Location = TrimToNull(request.Location);
        profile.Phone = TrimToNull(request.Phone);
        profile.Price = request.Price ?? profile.Price;
        profile.Bio = TrimToNull(request.Bio);
        profile.UpdatedAtUtc = DateTime.UtcNow;

        if (request.AvailableSlots.Count > 0)
        {
            profile.WorkingTimes = SlotSerializer.Serialize(request.AvailableSlots);
        }

        if (request.ProfileImage is not null)
        {
            profile.ProfileImageUrl = await fileStorage.SaveProfileImageAsync(request.ProfileImage, cancellationToken);
        }

        doctorProfiles.Update(profile);
        await doctorProfiles.SaveChangesAsync(cancellationToken);

        return MapDoctorProfile(profile);
    }

    public async Task<DoctorProfileResponse> UpdateDoctorSlotsAsync(UpdateDoctorSlotsRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentDoctorProfileAsync(cancellationToken);
        profile.WorkingTimes = SlotSerializer.Serialize(request.AvailableSlots);
        profile.UpdatedAtUtc = DateTime.UtcNow;

        doctorProfiles.Update(profile);
        await doctorProfiles.SaveChangesAsync(cancellationToken);

        return MapDoctorProfile(profile);
    }

    private async Task<PatientProfile> GetCurrentPatientProfileAsync(CancellationToken cancellationToken)
    {
        return await patientProfiles.Query()
            .Include(profile => profile.User)
            .FirstOrDefaultAsync(profile => profile.UserId == currentUser.UserId, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Patient profile was not found.");
    }

    private async Task<DoctorProfile> GetCurrentDoctorProfileAsync(CancellationToken cancellationToken)
    {
        return await doctorProfiles.Query()
            .Include(profile => profile.User)
            .Include(profile => profile.Ratings)
            .FirstOrDefaultAsync(profile => profile.UserId == currentUser.UserId, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Doctor profile was not found.");
    }

    private DoctorProfileResponse MapDoctorProfile(DoctorProfile profile)
    {
        var response = mapper.Map<DoctorProfileResponse>(profile);
        response.AvailableSlots = SlotSerializer.GetSlots(profile);
        return response;
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
