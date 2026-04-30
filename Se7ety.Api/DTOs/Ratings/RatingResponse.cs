namespace Se7ety.Api.DTOs.Ratings;

public sealed class RatingResponse
{
    public Guid Id { get; set; }
    public Guid DoctorProfileId { get; set; }
    public Guid PatientProfileId { get; set; }
    public int Value { get; set; }
    public string? Comment { get; set; }
    public double DoctorAverageRating { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
