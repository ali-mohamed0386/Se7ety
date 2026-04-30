namespace Se7ety.Api.DTOs.Auth;

public sealed class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public Guid UserId { get; set; }
    public Guid? ProfileId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
