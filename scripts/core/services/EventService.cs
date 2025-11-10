namespace Core;

using Godot;
using System;
using Core.Interface;
using System.Collections.Generic;
/// <summary>
/// Service that manages event publishing and subscribing within the game; Publish events to the service so that other parts of the game, like systems, can subscribe to them.
/// It supports both custom typed events (say you need to send some data) and named events (just a simple signal with no data).
/// These are not Godot Signals; these are delegates being managed.
/// </summary>
/// <remarks>
/// The EventService allows different parts of the game to communicate without direct references, promoting loose coupling through the singleton core service provider.
/// Sadly, no likes, only subs UwU (and pubs lol).
/// </remarks>
public sealed class EventService : IEventService
{
    private Dictionary<Type, List<Delegate>> _subs = new();
    public EventService()
    {
        GD.PrintRich("[color=#00ff88]EventService initialized.[/color]");
    }
    public void UnsubscribeAll()
    {
        _subs.Clear();
        GD.PrintRich("[color=#ff8800]EventService: All subscriptions cleared. They will be recreated on next call.[/color]");
    }
    public void Subscribe<T>(Action<IEvent> handler)
    {
        var type = typeof(T);
        if (!_subs.ContainsKey(type))
        {
            GD.PrintRich($"[color=#0088ff]EventService: Creating subscription list for event type {type.Name}.[/color]");
            _subs[type] = new List<Delegate>();
        }
        GD.PrintRich($"[color=#0044FF]EventService: Subscribing handler to event type {type.Name}.[/color]");
        _subs[type].Add(handler);
    }
    public void Subscribe<T>(Action handler)
    {
        var type = typeof(T);
        if (!_subs.ContainsKey(type))
        {
            GD.PrintRich($"[color=#0088ff]EventService: Creating subscription list for event type {type.Name}.[/color]");
            _subs[type] = new List<Delegate>();
        }
        GD.PrintRich($"[color=#0044FF]EventService: Subscribing handler to event type {type.Name}.[/color]");
        _subs[type].Add(handler);
    }
    public void Unsubscribe<T>(Action<IEvent> eventHandler)
    {
        var type = typeof(T);
        if (_subs.ContainsKey(type))
        {
            bool removed = _subs[type].Remove(eventHandler);
            if (removed)
            {
                GD.PrintRich($"[color=#FF4444]EventService: Unsubscribed handler from event type {type.Name}.[/color]");
            }
            else
            {
                GD.PrintErr($"EventService: Attempted to unsubscribe handler from event type {type.Name} but handler was not found. Never subbed or already unsubscribed?");
            }
            if (_subs[type].Count == 0)
            {
                _subs.Remove(type);
            }
        }
    }
    public void Unsubscribe<T>(Action eventHandler)
    {
        var type = typeof(T);
        if (_subs.ContainsKey(type))
        {
            bool removed = _subs[type].Remove(eventHandler);
            if (removed)
            {
                GD.PrintRich($"[color=#FF4444]EventService: Unsubscribed handler from event type {type.Name}.[/color]");
            }
            else
            {
                GD.PrintErr($"EventService: Attempted to unsubscribe handler from event type {type.Name} but handler was not found. Never subbed or already unsubscribed?");
            }
            if (_subs[type].Count == 0)
            {
                _subs.Remove(type);
            }
        }
    }
    public void Publish<T>(IEvent eventData)
    {
        var type = typeof(T);
        if (!_subs.ContainsKey(type))
        {
            GD.PrintErr($"EventService: Publish called for event type {type.Name} but no subscriptions exist. Did you forget to subscribe?");
            return;
        }
        else
        {
            foreach (var handler in _subs[type])
            {
                ((Action<IEvent>)handler)(eventData);
            }
        }
    }
    public void Publish<T>()
    {
        var type = typeof(T);
        if (!_subs.ContainsKey(type))
        {
            GD.PrintErr($"EventService: Publish called for event type {type.Name} but no subscriptions exist. Did you forget to subscribe?");
            return;
        }
        else
        {
            foreach (var handler in _subs[type])
            {
                ((Action)handler)();
            }
        }
    }
}