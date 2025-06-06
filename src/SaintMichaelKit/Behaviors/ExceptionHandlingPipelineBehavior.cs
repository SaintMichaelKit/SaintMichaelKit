using Microsoft.Extensions.Logging;
using SaintMichaelKit.Commons;
using SaintMichaelKit.LiteMediator.Interfaces;

namespace SaintMichaelKit.Behaviors;

/// <summary>
/// Pipeline behavior that handles unhandled exceptions thrown during the processing of a request.
/// Logs the exception and rethrows it to allow higher-level handling or middleware to act accordingly.
/// </summary>
/// <typeparam name="TCommand">The type of request.</typeparam>
/// <typeparam name="TResponse">The expected response type (must inherit from Result).</typeparam>
public class ExceptionHandlingPipelineBehavior<TCommand, TResponse>(
    ILogger<ExceptionHandlingPipelineBehavior<TCommand, TResponse>> logger)
    : IHandlerBehavior<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
    where TResponse : Result
{
    /// <summary>
    /// Handles the request by wrapping the execution in a try-catch block to catch and log exceptions.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">The delegate that invokes the next behavior or the final handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the next behavior or handler, or propagates the exception if one occurs.</returns>
    public async Task<TResponse> Handle(
        TCommand request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing command {CommandName} at {DateTime}", typeof(TCommand).Name, DateTime.UtcNow);
            throw;
        }
    }
}
