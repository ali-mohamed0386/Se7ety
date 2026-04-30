namespace Se7ety.Api.Domain.Entities;

public sealed class DoctorProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public string? Specialty { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public decimal Price { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? WorkingTimes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Rating> Ratings { get; set; } = [];
}
