using Se7ety.Api.DTOs.Profiles;

namespace Se7ety.Api.Services.Interfaces;

public interface IProfileService
{
    Task<PatientProfileResponse> GetMyPatientProfileAsync(CancellationToken cancellationToken = default);
    Task<PatientProfileResponse> UpdatePatientProfileAsync(UpdatePatientProfileRequest request, CancellationToken cancellationToken = default);
    Task<DoctorProfileResponse> GetMyDoctorProfileAsync(CancellationToken cancellationToken = default);
    Task<DoctorProfileResponse> UpdateDoctorProfileAsync(UpdateDoctorProfileRequest request, CancellationToken cancellationToken = default);
    Task<DoctorProfileResponse> UpdateDoctorSlotsAsync(UpdateDoctorSlotsRequest request, CancellationToken cancellationToken = default);
}
