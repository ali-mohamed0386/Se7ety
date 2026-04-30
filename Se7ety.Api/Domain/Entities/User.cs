using Se7ety.Api.Domain.Enums;

namespace Se7ety.Api.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public PatientProfile? PatientProfile { get; set; }
    public DoctorProfile? DoctorProfile { get; set; }
}
