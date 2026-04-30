using System.ComponentModel.DataAnnotations;

namespace Se7ety.Api.DTOs.Profiles;

public sealed class UpdateDoctorProfileRequest
{
    [MaxLength(150)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Specialty { get; set; }

    [MaxLength(250)]
    public string? Location { get; set; }

    [Phone]
    [MaxLength(32)]
    public string? Phone { get; set; }

    [Range(0, 100000)]
    public decimal? Price { get; set; }

    [MaxLength(1500)]
    public string? Bio { get; set; }

    public List<DateTime> AvailableSlots { get; set; } = [];

    public IFormFile? ProfileImage { get; set; }
}
