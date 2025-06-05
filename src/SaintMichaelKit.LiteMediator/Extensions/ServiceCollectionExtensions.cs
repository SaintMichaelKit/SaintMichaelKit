using Microsoft.Extensions.DependencyInjection;
using SaintMichaelKit.LiteMediator.Implementation;
using SaintMichaelKit.LiteMediator.Interfaces;
using System.Reflection;

namespace SaintMichaelKit.LiteMediator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleMediator(
    this IServiceCollection services,
    params object[] args)
    {
        var assemblies = ResolveAssemblies(args);

        services.AddSingleton<IMediator, Mediator>();

        RegisterHandlers(services, assemblies, typeof(INotificationHandler<>));
        RegisterHandlers(services, assemblies, typeof(IRequestHandler<>));
        RegisterHandlers(services, assemblies, typeof(IRequestHandler<,>));

        return services;
    }

    private static Assembly[] ResolveAssemblies(object[] args)
    {
        // Return ALL
        if (args == null || args.Length == 0)
            return [.. AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))];

        // Return all informed (same behavior as above)
        if (args.All(a => a is Assembly))
            return [.. args.Cast<Assembly>()];

        // Return filtered by namespace (most performatic)
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