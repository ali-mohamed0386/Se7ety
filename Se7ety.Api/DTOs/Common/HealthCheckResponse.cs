namespace Se7ety.Api.DTOs.Common;

public sealed record HealthCheckResponse(
    string Status,
    string Environment,
    DateTime CheckedAtUtc);
