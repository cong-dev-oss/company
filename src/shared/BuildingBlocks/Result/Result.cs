namespace BuildingBlocks.Result;

/// <summary>
/// Result pattern implementation for type-safe error handling
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error");
        
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);
    public static Result<T> Failure<T>(Error error) => new(default!, false, error);
}

public class Result<T> : Result
{
    private readonly T _value;

    protected internal Result(T value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access value of a failed result");

    public static implicit operator Result<T>(T value) => Success(value);
}

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    
    public static Error NotFound(string entity) => new(
        "NOT_FOUND",
        $"{entity} was not found");
    
    public static Error Validation(string message) => new(
        "VALIDATION_ERROR",
        message);
    
    public static Error Conflict(string message) => new(
        "CONFLICT",
        message);
    
    public static Error Unauthorized() => new(
        "UNAUTHORIZED",
        "Unauthorized access");
    
    public static Error Forbidden() => new(
        "FORBIDDEN",
        "Access forbidden");
}

