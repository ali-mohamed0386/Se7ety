namespace Se7ety.Api.DTOs.Profiles;

public sealed class PatientProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
}
