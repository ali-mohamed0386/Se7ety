using Se7ety.Api.Domain.Enums;

namespace Se7ety.Api.Domain.Entities;

public sealed class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientProfileId { get; set; }
    public Guid DoctorProfileId { get; set; }
    public DateTime ScheduledAtUtc { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public PatientProfile PatientProfile { get; set; } = null!;
    public DoctorProfile DoctorProfile { get; set; } = null!;
}
