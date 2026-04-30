using Se7ety.Api.DTOs.Common;
using Se7ety.Api.DTOs.Doctors;

namespace Se7ety.Api.Services.Interfaces;

public interface IDoctorService
{
    Task<PagedResponse<DoctorCardResponse>> GetDoctorsAsync(DoctorListQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SpecialtyResponse>> GetSpecialtiesAsync(CancellationToken cancellationToken = default);
    Task<PagedResponse<DoctorCardResponse>> SearchDoctorsAsync(DoctorSearchQuery query, CancellationToken cancellationToken = default);
    Task<DoctorDetailsResponse> GetDoctorDetailsAsync(Guid doctorProfileId, CancellationToken cancellationToken = default);
}
