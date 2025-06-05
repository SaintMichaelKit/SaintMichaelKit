using Microsoft.Extensions.DependencyInjection;
using SaintMichaelKit.LiteMediator.Interfaces;

namespace SaintMichaelKit.LiteMediator.Implementation;


public class Mediator : IMediator
{
    private readonly IServiceProvider _provider;

    public Mediator(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
        var handler = _provider.GetService(handlerType)
                ?? throw new InvalidOperationException($"Handler not found for {request.GetType().Name}");
        var method = handlerType.GetMethod("Handle");
        await (Task)method!.Invoke(handler, [request, cancellationToken])!;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _provider.GetService(handlerType)
                ?? throw new InvalidOperationException($"Handler not found for {request.GetType().Name}");
        return await (Task<TResponse>)handlerType
            .GetMethod("Handle")!
            .Invoke(handler, [request, cancellationToken])!;
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = _provider.GetServices(handlerType);

        foreach (var handler in handlers)
            await (Task)handlerType
                .GetMethod("Handle")!
                .Invoke(handler, [notification, cancellationToken])!;
    }
}