using System.ComponentModel.DataAnnotations;

namespace Se7ety.Api.DTOs.Ratings;

public sealed class RateDoctorRequest
{
    [Required]
    public Guid DoctorProfileId { get; set; }

    [Range(1, 5)]
    public int Value { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}
