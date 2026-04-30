using Se7ety.Api.Domain.Entities;

namespace Se7ety.Api.Services.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateToken(User user);
}
