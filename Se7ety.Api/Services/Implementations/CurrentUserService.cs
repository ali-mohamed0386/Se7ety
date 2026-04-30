using System.Security.Claims;
using Se7ety.Api.Exceptions;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(value, out var userId))
            {
                throw new ApiException(StatusCodes.Status401Unauthorized, "Authentication is required.");
            }

            return userId;
        }
    }

    public string? Email => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public string? Role => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}
