using System.ComponentModel.DataAnnotations;

namespace Se7ety.Api.DTOs.Settings;

public sealed class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string NewPassword { get; set; } = string.Empty;
}
