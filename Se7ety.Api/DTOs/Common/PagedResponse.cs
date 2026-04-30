namespace Se7ety.Api.DTOs.Common;

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);
