using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SaintMichaelKit.Commons;
public class Result
{
    private readonly List<Error> _errors = [];
    public bool IsFailure => Errors.Count > 0;
    public bool IsSuccess => !IsFailure;

    public IReadOnlyCollection<string> Errors => [.. _errors.Select(e => e.Message)];
    public IReadOnlyList<Error> ErrorDetails => _errors;

    protected Result() { }

    protected Result(IEnumerable<Error> errors)
    {
        if (errors == null || !errors.Any())
            throw new ArgumentException("Error list cannot be null or empty for a failed result.", nameof(errors));

        _errors.AddRange(errors);
    }

    [JsonConstructor]
    public Result(bool isSuccess, List<Error>? errors = null)
    {
        if (!isSuccess && (errors == null || errors.Count == 0))
            throw new ArgumentException("Error list cannot be null or empty for a failed result.", nameof(errors));

        if (!isSuccess)
            _errors.AddRange(errors!);
    }

    public static Result Ok() => new();

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result([error]);
    }

    public static Result Failure(IEnumerable<Error> errors)
    {
        if (errors == null || !errors.Any())
            throw new ArgumentException("Error list cannot be null or empty.", nameof(errors));

        return new Result(errors);
    }

    public static Result<TValue> Ok<TValue>(TValue value) => new(value);

    public static Result<TValue> Failure<TValue>(Error error) => new([error]);

    public static Result<TValue> Failure<TValue>(IEnumerable<Error> errors) => new(errors);


    public static implicit operator Result(Error error) => Failure(error);

    public static implicit operator Result(List<Error> errors) => Failure(errors);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue value)
    {
        _value = value;
    }

    protected internal Result(IEnumerable<Error> errors)
        : base(errors) { }

    [JsonConstructor]
    public Result(bool isSuccess, List<Error>? errors = null, TValue? value = default) : base(isSuccess, errors)
    {
        if (isSuccess)
            _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static implicit operator Result<TValue>(TValue value) => Ok(value);

    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

    public static implicit operator Result<TValue>(List<Error> errors) => Failure<TValue>(errors);
}