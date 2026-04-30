using Se7ety.Api.DTOs.Common;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class HealthService(IHostEnvironment environment) : IHealthService
{
    public HealthCheckResponse GetStatus()
    {
        return new HealthCheckResponse("Healthy", environment.EnvironmentName, DateTime.UtcNow);
    }
}
