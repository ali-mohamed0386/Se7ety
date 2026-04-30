using System.ComponentModel.DataAnnotations;

namespace Se7ety.Api.DTOs.Appointments;

public sealed class BookAppointmentRequest
{
    [Required]
    public Guid DoctorProfileId { get; set; }

    [Required]
    public DateTime ScheduledAtUtc { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
