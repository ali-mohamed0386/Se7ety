using System.ComponentModel.DataAnnotations;

namespace Se7ety.Api.DTOs.Auth;

public sealed class AuthRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}
