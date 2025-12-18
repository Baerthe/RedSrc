namespace Core;

using Godot;
using Event;
using System;
using Interface;
using System.Collections.Generic;
/// <summary>
/// Handles the event system, including subscription and publication of events.
/// </summary>
public sealed partial class EventCore : Node2D
{
    public Heart heart { get; private set; }
    private Dictionary<Type, List<Action>> _subs = new();
    private Dictionary<Type, List<Action<IEvent>>> _subsData = new();
    // Timer Default Intervals
	private const float PulseInterval = 0.05f;        // 20 hrz (~1200 per minute)
	private const float SlowPulseInterval = 0.2f;     // 5 hrz (~300 per minute)
	private const float MobSpawnInterval = 5f;       // 0.2 hrz (~12 per minute)
	private const float ChestSpawnInterval = 10f;     // 0.1 hrz (~6 per minute)
	private const float GameInterval = 60f;          // 0.016 hrz (~1 per minute)
	private const float StartingInterval = 3f;       // OneShot (~3 seconds)
    public override void _Ready()
    {
        GD.PrintRich("[color=#000][bgcolor=#00ff00]EventCore ready.[/bgcolor][/color]");
        AddChild(heart = new Heart());
        BuildTimers();
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
    private void BuildTimers()
    {
        heart.BuildTimer<PulseTimeout>("PulseTimer", PulseInterval, false, false);
        heart.BuildTimer<SlowPulseTimeout>("SlowPulseTimer", SlowPulseInterval, false, false);
        heart.BuildTimer<MobSpawnTimeout>("MobSpawnTimer", MobSpawnInterval, false, false);
        heart.BuildTimer<ChestSpawnTimeout>("ChestSpawnTimer", ChestSpawnInterval, false, false);
        heart.BuildTimer<GameTimeout>("GameTimer", GameInterval, false, false);
        heart.BuildTimer<StartingTimeout>("StartingTimer", StartingInterval, true, false);
    }
}