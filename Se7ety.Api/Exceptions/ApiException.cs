namespace Se7ety.Api.Exceptions;

public sealed class ApiException : Exception
{
    public ApiException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public ApiException(int statusCode, string message, IReadOnlyDictionary<string, string[]> errors)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }

    public int StatusCode { get; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; }
}
