namespace Se7ety.Api.DTOs.Appointments;

public sealed class AppointmentResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledAtUtc { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public Guid DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public string? DoctorSpecialty { get; set; }
    public string? DoctorProfileImageUrl { get; set; }
    public Guid PatientId { get; set; }
    public string PatientEmail { get; set; } = string.Empty;
    public string? PatientPhoneNumber { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
