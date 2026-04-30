using Se7ety.Api.DTOs.Auth;

namespace Se7ety.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterPatientAsync(AuthRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterDoctorAsync(AuthRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default);
}
