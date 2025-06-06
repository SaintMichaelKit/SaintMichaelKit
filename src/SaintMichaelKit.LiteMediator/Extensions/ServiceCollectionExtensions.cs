using Microsoft.Extensions.DependencyInjection;
using SaintMichaelKit.LiteMediator.Implementation;
using SaintMichaelKit.LiteMediator.Interfaces;
using System.Reflection;

namespace SaintMichaelKit.LiteMediator.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the mediator, handlers, and behaviors into the DI container.
    /// Accepts no parameters, a list of assemblies, or namespace prefixes for filtering.
    /// </summary>
    public static IServiceCollection AddSimpleMediator(
        this IServiceCollection services,
        params object[] args)
    {
        var assemblies = ResolveAssemblies(args);

        // Register the core mediator implementation as Scoped
        services.AddScoped<IMediator, Mediator>();

        // Register all types implementing handler interfaces
        RegisterHandlers(services, assemblies, typeof(INotificationHandler<>));
        RegisterHandlers(services, assemblies, typeof(IRequestHandler<>));
        RegisterHandlers(services, assemblies, typeof(IRequestHandler<,>));

        // Register all behavior middleware
        RegisterBehaviors(services, assemblies);

        return services;
    }

    /// <summary>
    /// Registers a generic behavior type (open generic).
    /// </summary>
    public static IServiceCollection AddMediatorBehavior(
        this IServiceCollection services,
        Type behaviorType)
    {
        if (!behaviorType.IsGenericTypeDefinition)
        {
            throw new ArgumentException("Behavior type must be a generic type definition", nameof(behaviorType));
        }

        services.AddTransient(typeof(IHandlerBehavior<,>), behaviorType);

        return services;
    }

    /// <summary>
    /// Registers a concrete or open generic behavior class.
    /// </summary>
    public static IServiceCollection AddMediatorBehavior<TBehavior>(
        this IServiceCollection services)
        where TBehavior : class
    {
        var behaviorType = typeof(TBehavior);

        if (behaviorType.IsGenericTypeDefinition)
        {
            services.AddTransient(typeof(IHandlerBehavior<,>), behaviorType);
        }
        else
        {
            // Register concrete class for all implemented IHandlerBehavior<,> interfaces
            var interfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(IHandlerBehavior<,>));

            foreach (var iface in interfaces)
            {
                services.AddTransient(iface, behaviorType);
            }
        }

        return services;
    }

    /// <summary>
    /// Finds and registers all IHandlerBehavior implementations in the given assemblies.
    /// </summary>
    private static void RegisterBehaviors(IServiceCollection services, Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            if (type.IsGenericTypeDefinition)
            {
                var implementsHandlerBehavior = type.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                             i.GetGenericTypeDefinition() == typeof(IHandlerBehavior<,>));

                if (implementsHandlerBehavior)
                {
                    services.AddTransient(typeof(IHandlerBehavior<,>), type);
                }
            }
            else
            {
                var interfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(IHandlerBehavior<,>));

                foreach (var iface in interfaces)
                {
                    services.AddTransient(iface, type);
                }
            }
        }
    }

    /// <summary>
    /// Resolves which assemblies to scan based on the input parameters.
    /// </summary>
    private static Assembly[] ResolveAssemblies(object[] args)
    {
        if (args == null || args.Length == 0)
            return [.. AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))];

        if (args.All(a => a is Assembly))
            return [.. args.Cast<Assembly>()];

        if (args.All(a => a is string))
        {
            var prefixes = args.Cast<string>().ToArray();
            return [.. AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a =>
                    !a.IsDynamic &&
                    !string.IsNullOrWhiteSpace(a.FullName) &&
                    prefixes.Any(p => a.FullName!.StartsWith(p)))];
        }

        throw new ArgumentException("Invalid parameters for AddSimpleMediator(). Use: no arguments, Assembly[], or prefix strings.");
    }

    /// <summary>
    /// Registers all classes that implement the specified handler interface.
    /// </summary>
    private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies, Type handlerInterface)
    {
        var types = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == handlerInterface);

            foreach (var iface in interfaces)
                services.AddTransient(iface, type);
        }
    }
}
