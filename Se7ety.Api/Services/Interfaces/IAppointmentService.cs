using Se7ety.Api.DTOs.Appointments;

namespace Se7ety.Api.Services.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponse> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentResponse>> GetMyPatientBookingsAsync(CancellationToken cancellationToken = default);
    Task<AppointmentResponse> CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentResponse>> GetMyDoctorBookingsAsync(CancellationToken cancellationToken = default);
    Task<AppointmentResponse> AcceptAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<AppointmentResponse> RejectAsync(Guid appointmentId, CancellationToken cancellationToken = default);
}
