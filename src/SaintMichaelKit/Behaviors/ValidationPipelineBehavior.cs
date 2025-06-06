using FluentValidation;
using FluentValidation.Results;
using SaintMichaelKit.Commons;
using SaintMichaelKit.LiteMediator.Interfaces;

namespace SaintMichaelKit.Behaviors;
/// <summary>
/// Pipeline behavior that validates a request before passing it to the next handler.
/// If validation fails, it short-circuits the pipeline and returns a failure result.
/// </summary>
/// <typeparam name="TCommand">The request type.</typeparam>
/// <typeparam name="TResponse">The expected response type (must inherit from Result).</typeparam>
public sealed class ValidationPipelineBehavior<TCommand, TResponse>(
    IEnumerable<IValidator<TCommand>> validators)
    : IHandlerBehavior<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
    where TResponse : Result
{
    /// <summary>
    /// Executes validation logic before the request reaches the handler.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">The delegate representing the next behavior or final handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A successful result if validation passes; otherwise, a failure result containing validation errors.</returns>
    public async Task<TResponse> Handle(TCommand request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        ValidationFailure[] validationFailures = await ValidateAsync(request, validators);

        if (validationFailures.Length == 0)
        {
            return await next();
        }

        // Return a failure result with validation error details
        return (TResponse)Result.Failure([.. validationFailures.Select(f => Error.Validation(f.ErrorCode, $"{f.PropertyName}: {f.ErrorMessage}"))]);
    }

    /// <summary>
    /// Executes all validators for the current request and aggregates validation errors.
    /// </summary>
    /// <param name="command">The request to validate.</param>
    /// <param name="validators">A collection of validators registered for the request type.</param>
    /// <returns>An array of validation failures, or an empty array if the request is valid.</returns>
    private static async Task<ValidationFailure[]> ValidateAsync(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = [.. validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)];

        return validationFailures;
    }
}