namespace Core;

using Godot;
using System;
using Interface;
using System.Collections.Generic;

public sealed partial class EventCore : Node2D
{
    public static EventCore Instance { get; private set; }
    [Export] public Heart heart { get; private set; }
    private Dictionary<Type, List<Action>> _subs = new();
    private Dictionary<Type, List<Action<IEvent>>> _subsData = new();
    public override void _Ready()
    {
        Instance = this;
        GD.PrintRich("[color=#000][bgcolor=#00ff00]EventCore ready.[/bgcolor][/color]");
    }
    public void UnsubscribeAll()
    {
        _subs.Clear();
        _subsData.Clear();
        GD.PrintRich("[color=#ff8800]EventCore: All subscriptions cleared. They will be recreated on next call.[/color]");
    }
    public void Subscribe<T>(Action<IEvent> handler)
    {
        var type = typeof(T);
        if (!_subsData.ContainsKey(type))
        {
            GD.PrintRich($"[color=#0088ff]EventCore: Creating subscription list for event type {type.Name}.[/color]");
            _subsData[type] = new List<Action<IEvent>>();
        }
        GD.PrintRich($"[color=#0044FF]EventCore: Subscribing handler to event type {type.Name}.[/color]");
        _subsData[type].Add(handler);
    }
    public void Subscribe<T>(Action handler)
    {
        var type = typeof(T);
        if (!_subs.ContainsKey(type))
        {
            GD.PrintRich($"[color=#0088ff]EventCore: Creating subscription list for event type {type.Name}.[/color]");
            _subs[type] = new List<Action>();
        }
        GD.PrintRich($"[color=#0044FF]EventCore: Subscribing handler to event type {type.Name}.[/color]");
        _subs[type].Add(handler);
    }
    public void Unsubscribe<T>(Action<IEvent> eventHandler)
    {
        var type = typeof(T);
        if (_subsData.ContainsKey(type))
        {
            bool removed = _subsData[type].Remove(eventHandler);
            if (removed)
            {
                GD.PrintRich($"[color=#FF4444]EventCore: Unsubscribed handler from event type {type.Name}.[/color]");
            }
            else
            {
                GD.PrintErr($"EventCore: Attempted to unsubscribe handler from event type {type.Name} but handler was not found. Never subbed or already unsubscribed?");
            }
            if (_subsData[type].Count == 0)
            {
                _subsData.Remove(type);
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
                GD.PrintRich($"[color=#FF4444]EventCore: Unsubscribed handler from event type {type.Name}.[/color]");
            }
            else
            {
                GD.PrintErr($"EventCore: Attempted to unsubscribe handler from event type {type.Name} but handler was not found. Never subbed or already unsubscribed?");
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
        if (!_subsData.ContainsKey(type))
        {
            GD.PrintErr($"EventCore: Publish with data called for event type {type.Name} but no subscriptions exist. Did you forget to subscribe?");
            return;
        }
        else
        {
            foreach (var handler in _subsData[type])
            {
                GD.PrintRich($"[color=#00ff88]EventCore: Publishing event of type {type.Name}.[/color]");
                ((Action<IEvent>)handler)(eventData);
            }
        }
    }
    public void Publish<T>()
    {
        var type = typeof(T);
        if (!_subs.ContainsKey(type))
        {
            GD.PrintErr($"EventCore: Publish called for event type {type.Name} but no subscriptions exist. Did you forget to subscribe?");
            return;
        }
        else
        {
            foreach (var handler in _subs[type])
            {
                GD.PrintRich($"[color=#00ff88]EventCore: Publishing event of type {type.Name}.[/color]");
                ((Action)handler)();
            }
        }
    }
}