namespace Core;
using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// CoreContainer is a simple dependency injection container for managing core services in the game. It allows registering and resolving services by their interface types.
/// </summary>
public class CoreContainer
{
    private Dictionary<Type, object> _cores = new();
    /// <summary>
    /// Registers a core service with its interface and implementation.
    /// </summary>
    /// <typeparam name="Tinterface">The interface type of the service.</typeparam>
    /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
    public void Register<Tinterface, TImplementation>() where TImplementation : Tinterface, new()
    {
        _cores[typeof(Tinterface)] = new TImplementation();
        GD.PrintRich($"[color=#00ff00]Registered core: {typeof(Tinterface).Name} as {typeof(TImplementation).Name}[/color]");
    }
    /// <summary>
    /// Resolves a core service by its interface type.
    /// </summary>
    /// <typeparam name="T">The interface type of the service to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    public T Resolve<T>() where T : class
    {
        _cores.TryGetValue(typeof(T), out var core);
        if (core == null)
        {
            GD.PrintErr($"Service of type {typeof(T).Name} is not registered.");
            throw new InvalidOperationException($"ERROR 098: Service of type {typeof(T).Name} is not registered in Cores. Game cannot load.");
        }
        GD.PrintRich($"[color=#00ff8A]Delivery Time! Resolving core: {typeof(T).Name} as {core?.GetType().Name ?? "null"}[/color]");
        return core as T;
    }
}