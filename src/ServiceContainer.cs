using Godot;
using Interface;
using System;
using System.Collections.Generic;
/// <summary>
/// ServiceContainer is a simple dependency injection container for managing core services in the game. It allows registering and resolving services by their interface types.
/// </summary>
public class ServiceContainer
{
    private Dictionary<Type, object> _services = new();
    /// <summary>
    /// Registers a core service with its interface and implementation.
    /// </summary>
    /// <typeparam name="Tinterface">The interface type of the service.</typeparam>
    /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
    public void Register<Tinterface, TImplementation>() where TImplementation : Tinterface, new()
    {
        _services[typeof(Tinterface)] = new TImplementation();
        GD.PrintRich($"[color=#00ff00]Registered service: {typeof(Tinterface).Name} as {typeof(TImplementation).Name}[/color]");
    }
    /// <summary>
    /// Resolves a core service by its interface type.
    /// </summary>
    /// <typeparam name="T">The interface type of the service to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    public T Resolve<T>() where T : IService
    {
        _services.TryGetValue(typeof(T), out var core);
        if (core == null)
        {
            GD.PrintErr($"Service of type {typeof(T).Name} is not registered.");
            throw new InvalidOperationException($"ERROR 098: Service of type {typeof(T).Name} is not registered in Services. Game cannot load.");
        }
        GD.PrintRich($"[color=#00ff8A]Delivery Time! Resolving service: {typeof(T).Name} as {core?.GetType().Name ?? "null"}[/color]");
        return core is T service ? service : throw new InvalidCastException($"Registered service cannot be cast to type {typeof(T).Name}.");
    }
}