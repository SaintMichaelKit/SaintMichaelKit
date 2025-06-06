namespace SaintMichaelKit.LiteMediator.Interfaces;

/// <summary>
/// Handles a request that does not return a response.
/// </summary>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles the request.
    /// </summary>
    Task Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Handles a request and returns a response of type TResponse.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request and returns the response.
    /// </summary>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}