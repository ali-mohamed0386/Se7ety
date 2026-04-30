using Se7ety.Api.DTOs.Settings;

namespace Se7ety.Api.Services.Interfaces;

public interface ISettingsService
{
    Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
}
