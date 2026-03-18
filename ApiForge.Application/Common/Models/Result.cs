namespace ApiForge.Application.Common.Models;

/// <summary>
/// Discriminated union that represents either a success value or a typed error.
/// Avoids exception-driven control flow for expected business errors.
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ErrorType ErrorKind { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        ErrorKind = ErrorType.None;
    }

    private Result(string error, ErrorType kind)
    {
        IsSuccess = false;
        Error = error;
        ErrorKind = kind;
    }

    // ── Factories ─────────────────────────────────────────────────────────

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(string error, ErrorType kind = ErrorType.General)
        => new(error, kind);

    public static Result<T> NotFound(string error) => new(error, ErrorType.NotFound);

    public static Result<T> Conflict(string error) => new(error, ErrorType.Conflict);

    public static Result<T> Validation(string error) => new(error, ErrorType.Validation);

    public static Result<T> Unauthorized(string error) => new(error, ErrorType.Unauthorized);
}

public enum ErrorType
{
    None,
    General,
    NotFound,
    Validation,
    Conflict,
    Unauthorized,
}
