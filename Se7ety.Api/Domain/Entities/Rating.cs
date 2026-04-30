namespace Se7ety.Api.Domain.Entities;

public sealed class Rating
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientProfileId { get; set; }
    public Guid DoctorProfileId { get; set; }
    public int Value { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public PatientProfile PatientProfile { get; set; } = null!;
    public DoctorProfile DoctorProfile { get; set; } = null!;
}
