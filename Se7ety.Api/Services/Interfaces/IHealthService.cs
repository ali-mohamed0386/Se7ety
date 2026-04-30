using Se7ety.Api.DTOs.Common;

namespace Se7ety.Api.Services.Interfaces;

public interface IHealthService
{
    HealthCheckResponse GetStatus();
}
