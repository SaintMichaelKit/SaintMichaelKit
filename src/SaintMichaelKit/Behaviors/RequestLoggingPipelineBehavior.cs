using Microsoft.Extensions.Logging;
using SaintMichaelKit.Commons;
using SaintMichaelKit.LiteMediator.Interfaces;

namespace SaintMichaelKit.Behaviors;

/// <summary>
/// Pipeline behavior that logs the lifecycle of a command request, including start, completion, and any failure details.
/// Useful for observability and debugging command execution flows.
/// </summary>
/// <typeparam name="TCommand">The request type.</typeparam>
/// <typeparam name="TResponse">The response type (must inherit from <see cref="Result"/>).</typeparam>
public class RequestLoggingPipelineBehavior<TCommand, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TCommand, TResponse>> logger)
    : IHandlerBehavior<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
    where TResponse : Result
{
    /// <summary>
    /// Logs the execution start and end of a command, along with success or failure details.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">The delegate representing the next behavior or final handler.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the command execution.</returns>
    public async Task<TResponse> Handle(
        TCommand request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        logger.LogInformation("Starting command {CommandName} at {DateTime}", commandName, DateTime.UtcNow);

        var result = await next();

        if (result.IsSuccess)
        {
            logger.LogInformation("Completed command {CommandName} successfully at {DateTime}", commandName, DateTime.UtcNow);
            logger.LogInformation("Result: {result}", result);
        }
        else
        {
            logger.LogWarning("Command {CommandName} failed at {DateTime} with errors: {@Errors}", commandName, DateTime.UtcNow, result.Errors);
        }

        return result;
    }
}
