namespace TokenMetrics.Domain.Common;

/// <summary>
/// Generic result type for operations that can succeed or fail
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public ResultErrorType ErrorType { get; }

    private Result(bool isSuccess, T? data, string? errorMessage, ResultErrorType errorType)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static Result<T> Success(T data)
        => new(true, data, null, ResultErrorType.None);

    public static Result<T> NotFound(string message = "Resource not found")
        => new(false, default, message, ResultErrorType.NotFound);

    public static Result<T> RateLimited(string message = "Rate limit exceeded")
        => new(false, default, message, ResultErrorType.RateLimited);

    public static Result<T> NetworkError(string message = "Network connection failed")
        => new(false, default, message, ResultErrorType.NetworkError);

    public static Result<T> ServerError(string message = "Server error occurred")
        => new(false, default, message, ResultErrorType.ServerError);

    public static Result<T> ValidationError(string message)
        => new(false, default, message, ResultErrorType.ValidationError);

    public static Result<T> Failure(string message, ResultErrorType errorType = ResultErrorType.Unknown)
        => new(false, default, message, errorType);
}

/// <summary>
/// Type of error for Result failures
/// </summary>
public enum ResultErrorType
{
    None,
    NotFound,
    RateLimited,
    NetworkError,
    ServerError,
    ValidationError,
    Unknown
}
