namespace Se7ety.Api.DTOs.Common;

public sealed record ApiErrorResponse(
    int StatusCode,
    string Message,
    string? TraceId,
    IReadOnlyDictionary<string, string[]>? Errors = null);
