using Microsoft.Extensions.DependencyInjection;
using SaintMichaelKit.LiteMediator.Interfaces;

namespace SaintMichaelKit.LiteMediator.Implementation;

/// <summary>
/// Represents a void return type for requests without a response.
/// </summary>
public sealed class Unit
{
    public static readonly Unit Value = new();
    private Unit() { }
}

/// <summary>
/// Default mediator implementation that handles requests, responses, and notifications.
/// </summary>
public class Mediator(IServiceProvider provider) : IMediator
{
    /// <summary>
    /// Sends a command or request that does not return a value (Unit).
    /// </summary>
    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
        var handler = provider.GetService(handlerType)
                ?? throw new InvalidOperationException($"Handler not found for {request.GetType().Name}");

        // Get all behaviors for this request (no response)
        var behaviorType = typeof(IHandlerBehavior<,>).MakeGenericType(request.GetType(), typeof(Unit));
        var behaviors = provider.GetServices(behaviorType).Cast<object>().Reverse().ToList();

        // Core handler logic wrapped as a delegate
        Func<Task<Unit>> HandlerFunc = async () =>
        {
            await (Task)handlerType.GetMethod("Handle")!.Invoke(handler, [request, cancellationToken])!;
            return Unit.Value;
        };

        if (behaviors.Count == 0)
        {
            await HandlerFunc();
            return;
        }

        // Build behavior pipeline for the request
        Func<Task<Unit>> pipeline = HandlerFunc;
        foreach (var behavior in behaviors)
        {
            var currentNext = pipeline;
            var currentBehavior = behavior;

            pipeline = () =>
            {
                var behaviorMethod = currentBehavior.GetType().GetMethod("Handle")!;
                return (Task<Unit>)behaviorMethod.Invoke(currentBehavior, [request, currentNext, cancellationToken])!;
            };
        }

        await pipeline();
    }

    /// <summary>
    /// Sends a request and returns a response of type TResponse.
    /// </summary>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = provider.GetService(handlerType)
                ?? throw new InvalidOperationException($"Handler not found for {request.GetType().Name}");

        // Get all behaviors for this request (with response)
        var behaviorType = typeof(IHandlerBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var behaviors = provider.GetServices(behaviorType).Cast<object>().Reverse().ToList();

        // Core handler logic
        Func<Task<TResponse>> HandlerFunc = () =>
            (Task<TResponse>)handlerType.GetMethod("Handle")!.Invoke(handler, [request, cancellationToken])!;

        if (behaviors.Count == 0)
        {
            return await HandlerFunc();
        }

        // Build behavior pipeline for request with response
        Func<Task<TResponse>> pipeline = HandlerFunc;

        foreach (var behavior in behaviors)
        {
            var currentNext = pipeline;
            var currentBehavior = behavior;

            pipeline = () =>
            {
                var behaviorMethod = currentBehavior.GetType().GetMethod("Handle")!;
                return (Task<TResponse>)behaviorMethod.Invoke(currentBehavior, [request, currentNext, cancellationToken])!;
            };
        }

        return await pipeline();
    }

    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = provider.GetServices(handlerType);

        // Call all notification handlers
        foreach (var handler in handlers)
            await (Task)handlerType
                .GetMethod("Handle")!
                .Invoke(handler, [notification, cancellationToken])!;
    }
}
