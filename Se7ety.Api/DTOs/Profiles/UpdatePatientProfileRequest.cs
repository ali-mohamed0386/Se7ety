using System.ComponentModel.DataAnnotations;

namespace Se7ety.Api.DTOs.Profiles;

public sealed class UpdatePatientProfileRequest
{
    [Phone]
    [MaxLength(32)]
    public string? PhoneNumber { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public IFormFile? ProfileImage { get; set; }
}
