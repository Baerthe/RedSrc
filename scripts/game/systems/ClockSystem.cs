namespace Game;

using Godot;
using Core;
using System;
using System.Collections.Generic;
using Core.Interface;
using Game.Interface;
/// <summary>
/// ClockSystem manages the game's timing mechanisms, including pulse, slow pulse, mob spawn, chest spawn, game, and starting timers. It integrates with the EventService to provide timed events that other game systems can subscribe to.
/// </summary>
public partial class ClockSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private Timer _pulseTimer;
    private Timer _slowPulseTimer;
    private Timer _gameTimer;
    private Timer _startingTimer;
    private Timer _mobSpawnTimer;
    private Timer _ChestSpawnTimer;
    private Dictionary<byte, Timer> _timers = new();
    // Timer Default Intervals
    private const float PulseInterval = 0.05f;        // 20 hrz (~1200 per minute)
    private const float SlowPulseInterval = 0.2f;     // 5 hrz (~300 per minute)
    private const float MobSpawnInterval = 5f;       // 0.2 hrz (~12 per minute)
    private const float ChestSpawnInterval = 10f;     // 0.1 hrz (~6 per minute)
    private const float GameInterval = 60f;          // 0.016 hrz (~1 per minute)
    private const float StartingInterval = 3f;       // OneShot (~3 seconds)
    // Dependency Services
    private readonly IEventService _eventService;
    public ClockSystem(IEventService eventService)
    {
        _eventService = eventService;
    }
    public override void _Ready()
    {
        GD.Print("ClockSystem Present.");
        _eventService.Subscribe<Init>(OnInit);
        _eventService.Subscribe<PulseTimeout>(OnPulseTimeout);
        _eventService.Subscribe<SlowPulseTimeout>(OnSlowPulseTimeout);
        _eventService.Subscribe<MobSpawnTimeout>(OnMobSpawnTimeout);
        _eventService.Subscribe<ChestSpawnTimeout>(OnChestSpawnTimeout);
        _eventService.Subscribe<GameTimeout>(OnGameTimeout);
        _eventService.Subscribe<StartingTimeout>(OnStartingTimeout);
        CreatePulseTimer();
        CreateSlowPulseTimer();
        CreateMobSpawnTimer();
        CreateChestSpawnTimer();
        CreateGameTimer();
        CreateStartingTimer();
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<Init>(OnInit);
        _eventService.Unsubscribe<PulseTimeout>(OnPulseTimeout);
        _eventService.Unsubscribe<SlowPulseTimeout>(OnSlowPulseTimeout);
        _eventService.Unsubscribe<MobSpawnTimeout>(OnMobSpawnTimeout);
        _eventService.Unsubscribe<ChestSpawnTimeout>(OnChestSpawnTimeout);
        _eventService.Unsubscribe<GameTimeout>(OnGameTimeout);
        _eventService.Unsubscribe<StartingTimeout>(OnStartingTimeout);
    }
    public void OnInit()
    {
        if (IsInitialized)
        {
            GD.PrintErr("ClockService is already initialized. OnInit should only be called once per game session.");
            return;
        }
        StartTimers();
        GD.PrintRich("[color=#00ff88]ClockService initialized, late stage, and timers loaded.[/color]");
        IsInitialized = true;
    }
    public void ResetGame()
    {
        StopTimers();
    }
    public void PauseTimers()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Paused = true;
        }
    }
    public void ResumeTimers()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Paused = false;
        }
    }
    private void OnChestSpawnTimeout()
    {
        GD.PrintRich("[color=#afdd00]Chest Spawn Timeout triggered.");
    }
    private void OnSlowPulseTimeout()
    {
        GD.PrintRich("[color=#afdd00]Slow Pulse Tick processing...");
    }
    private void OnMobSpawnTimeout()
    {
        GD.PrintRich("[color=#afdd00]Mob Spawn Timeout triggered.");
    }
    private void OnGameTimeout()
    {
        GD.PrintRich("[color=#afdd00]Game Timeout triggered.");
        _mobSpawnTimer.WaitTime -= 0.25f;
        _ChestSpawnTimer.WaitTime -= 0.5f;
        if (_mobSpawnTimer.WaitTime < 0.5f)
            _mobSpawnTimer.WaitTime = 0.5f;
        if (_ChestSpawnTimer.WaitTime < 1f)
            _ChestSpawnTimer.WaitTime = 1f;
    }
    private void OnPulseTimeout()
    {
        GD.PrintRich("[color=#afdd00]Pulse Tick processing...");
    }
    private void OnStartingTimeout()
    {
        GD.PrintRich("[color=#afdd00]Starting Timeout triggered.");
    }
    /// <summary>
    /// Starts all timers. Used when initializing the game.
    /// </summary>
    private void StartTimers()
    {
        foreach (var timer in _timers.Values)
        {
            if (timer.GetParent() == null)
            {
                AddChild(timer);
            }
            timer.Start();
        }
        if (_pulseTimer.IsStopped() || _slowPulseTimer.IsStopped() || _mobSpawnTimer.IsStopped() || _ChestSpawnTimer.IsStopped() || _gameTimer.IsStopped() || _startingTimer.IsStopped())
        {
            GD.PrintErr($"One or more timers failed to start! Pulse: {_pulseTimer.IsStopped()}, SlowPulse: {_slowPulseTimer.IsStopped()}, MobSpawn: {_mobSpawnTimer.IsStopped()}, ChestSpawn: {_ChestSpawnTimer.IsStopped()}, Game: {_gameTimer.IsStopped()}, Starting: {_startingTimer.IsStopped()}");
        }
    }
    /// <summary>
    /// Stops all timers. Used when resetting the game.
    /// </summary>
    private void StopTimers()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Stop();
        }
        if (_pulseTimer.IsStopped() == false || _slowPulseTimer.IsStopped() == false || _mobSpawnTimer.IsStopped() == false || _ChestSpawnTimer.IsStopped() == false || _gameTimer.IsStopped() == false || _startingTimer.IsStopped() == false)
        {
            GD.PrintErr($"One or more timers failed to stop! Pulse: {_pulseTimer.IsStopped()}, SlowPulse: {_slowPulseTimer.IsStopped()}, MobSpawn: {_mobSpawnTimer.IsStopped()}, ChestSpawn: {_ChestSpawnTimer.IsStopped()}, Game: {_gameTimer.IsStopped()}, Starting: {_startingTimer.IsStopped()}");
        }
    }
    /// <summary>
    /// Builds a Timer with the specified parameters and attaches the onTimeout action to its Timeout signal.
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="oneShot"></param>
    /// <param name="autostart"></param>
    /// <param name="onTimeout"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the Timer fails to initialize. Lists which createTimer method was called.</exception>
    private Timer BuildTimer(float waitTime, bool oneShot, bool autostart)
    {
        Timer timer = new Timer
        {
            WaitTime = waitTime,
            OneShot = oneShot,
            Autostart = autostart
        };
        if (timer == null)
        {
            GD.PrintErr("Timer is null after creation!");
            throw new InvalidOperationException($"ERROR 001: Timer failed to initialize in ClockService.");
        }
        _timers.Add((byte)timer.GetHashCode(), timer);
        return timer;
    }
    private void CreatePulseTimer()
    {
        if (_pulseTimer != null) return;
        _pulseTimer = BuildTimer(PulseInterval, false, false);
        _pulseTimer.Timeout += _eventService.Publish<PulseTimeout>;
        GD.Print("Pulse Timer created with WaitTime 0.05f (20hrz), ~1200 per minute");
    }
    private void CreateSlowPulseTimer()
    {
        if (_slowPulseTimer != null) return;
        _slowPulseTimer = BuildTimer(SlowPulseInterval, false, false);
        _slowPulseTimer.Timeout += _eventService.Publish<SlowPulseTimeout>;
        GD.Print("Slow Pulse Timer created with WaitTime 0.2f (5hrz), ~300 per minute");
    }
    private void CreateMobSpawnTimer()
    {
        if (_mobSpawnTimer != null) return;
        _mobSpawnTimer = BuildTimer(MobSpawnInterval, false, false);
        _mobSpawnTimer.Timeout += _eventService.Publish<MobSpawnTimeout>;
        GD.Print("Mob Spawn Timer created with WaitTime 5f (0.2hrz), ~12 per minute");
    }
    private void CreateChestSpawnTimer()
    {
        if (_ChestSpawnTimer != null) return;
        _ChestSpawnTimer = BuildTimer(ChestSpawnInterval, false, false);
        _ChestSpawnTimer.Timeout += _eventService.Publish<ChestSpawnTimeout>;
        GD.Print("Pickup Spawn Timer created with WaitTime 10f (0.1hrz), ~6 per minute");
    }
    private void CreateGameTimer()
    {
        if (_gameTimer != null) return;
        _gameTimer = BuildTimer(GameInterval, false, false);
        _gameTimer.Timeout += _eventService.Publish<GameTimeout>;
        GD.Print("Game Timer created with WaitTime 60f (0.016hrz), ~1 per minute");
    }
    private void CreateStartingTimer()
    {
        if (_startingTimer != null) return;
        _startingTimer = BuildTimer(StartingInterval, true, false);
        _startingTimer.Timeout += _eventService.Publish<StartingTimeout>;
        GD.Print("Starting Timer created with WaitTime 3f (OneShot), ~3 seconds");
    }
}
