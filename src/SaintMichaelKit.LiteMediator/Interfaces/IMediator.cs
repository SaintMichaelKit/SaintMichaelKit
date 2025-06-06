namespace SaintMichaelKit.LiteMediator.Interfaces;

/// <summary>
/// Defines the core mediator interface for sending requests and publishing notifications.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request without expecting a response (fire-and-forget style).
    /// </summary>
    Task Send(IRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request and returns a response of type TResponse.
    /// </summary>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}