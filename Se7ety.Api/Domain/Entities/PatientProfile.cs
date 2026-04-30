namespace Se7ety.Api.Domain.Entities;

public sealed class PatientProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Rating> Ratings { get; set; } = [];
}
