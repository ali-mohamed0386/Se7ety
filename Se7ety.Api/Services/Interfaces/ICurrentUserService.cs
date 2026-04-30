namespace Se7ety.Api.Services.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Email { get; }
    string? Role { get; }
}
