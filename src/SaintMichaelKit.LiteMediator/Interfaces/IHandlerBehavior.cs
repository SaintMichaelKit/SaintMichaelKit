namespace SaintMichaelKit.LiteMediator.Interfaces;

/// <summary>
/// Represents a middleware behavior that can be executed around a request handler.
/// Useful for cross-cutting concerns such as logging, validation, and transactions.
/// </summary>
public interface IHandlerBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request and optionally calls the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">The delegate to invoke the next behavior or final handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response of type TResponse.</returns>
    Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default);
}