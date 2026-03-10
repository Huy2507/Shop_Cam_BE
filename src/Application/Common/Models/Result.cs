using MediatR;

namespace Shop_Cam_BE.Application.Common.Models;

public class Result<T>
{
    protected Result(bool succeeded, T? value = default, string? errorCode = null, IEnumerable<string>? errors = null)
    {
        Succeeded = succeeded;
        Value = value;
        ErrorCode = errorCode;
        Errors = errors?.ToArray() ?? Array.Empty<string>();
    }

    public bool Succeeded { get; }
    public T? Value { get; }
    public string? ErrorCode { get; }
    public string[] Errors { get; }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(string errorCode, params string[] errors) =>
        new(false, default, errorCode, errors);
    public static Result<T> Failure(string errorCode, IEnumerable<string> errors) =>
        new(false, default, errorCode, errors);
}

public class Result : Result<Unit>
{
    private Result(bool succeeded, string? errorCode = null, IEnumerable<string>? errors = null)
        : base(succeeded, Unit.Value, errorCode, errors) { }

    public static Result Success() => new(true);
    public static new Result Failure(string errorCode, params string[] errors) =>
        new(false, errorCode, errors);
    public static new Result Failure(string errorCode, IEnumerable<string> errors) =>
        new(false, errorCode, errors);
}
