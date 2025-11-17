namespace Manager;

using Data;
using Entity;
using Event;
using Godot;
using Interface;
using System;
using Utility;
[GlobalClass]
public partial class GameManager : Node2D
{
    public ClockUtility CurrentClockUtility { get; private set; }
    public ChestUtility CurrentChestUtility { get; private set; }
    public MapUtility CurrentMapUtility { get; private set; }
    public MobUtility CurrentMobUtility { get; private set; }
    public PlayerUtility CurrentPlayerUtility { get; private set; }
    public XPUtility CurrentXPUtility { get; private set; }
    public LevelData CurrentLevelData { get; private set; }
    public Camera2D Camera { get; private set; }
    public EntityIndex Templates { get; private set; }
    public bool IsPlaying { get; private set; } = false;
    public bool IsLevelLoaded { get; private set; } = false;
    public byte LoadingProgress { get; private set; } = 0;
    private LevelEntity _levelInstance;
    private IAudioService _audioService;
    private IEventService _eventService;
    private IHeroService _heroService;
    private IPrefService _prefService;
    private ILevelService _levelService;
    // Godot Methods
    public override void _Ready()
    {
        _audioService = CoreProvider.AudioService();
        _eventService = CoreProvider.EventService();
        _heroService = CoreProvider.HeroService();
        _prefService = CoreProvider.PrefService();
        _levelService = CoreProvider.LevelService();
        _eventService.Subscribe<IndexEvent>(OnIndexEvent);
        _eventService.Subscribe<PulseTimeout>(OnPulseTimeout);
        _eventService.Subscribe<SlowPulseTimeout>(OnSlowPulseTimeout);
        Camera = GetParent().GetNode<Camera2D>("MainCamera");
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<IndexEvent>(OnIndexEvent);
        _eventService.Unsubscribe<PulseTimeout>(OnPulseTimeout);
        _eventService.Unsubscribe<SlowPulseTimeout>(OnSlowPulseTimeout);
    }
    public override void _Process(double delta)
    {
        if (!IsLevelLoaded) return;
        if (!IsPlaying) return;
    }
    // GameManager Methods
    /// <summary>
    /// Prepares the level by initializing and adding the core systems to the provided level node.
    /// </summary>
    /// <param name="Level"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void LoadLevel()
    {
        if (IsLevelLoaded)
        {
            GD.PrintErr("Level already loaded in GameManager");
            throw new InvalidOperationException("ERROR 300: Level already loaded in GameManager. Cannot load another level.");
        }
        // Start loading systems and scenes
        CurrentLevelData = _levelService.CurrentLevel;
        LoadingProgress = 10;
        var levelload = ResourceLoader.Load<PackedScene>(Templates.LevelTemplate.ResourcePath);
        var chestLoad = ResourceLoader.Load<PackedScene>(Templates.ChestTemplate.ResourcePath);
        var mobLoad = ResourceLoader.Load<PackedScene>(Templates.MobTemplate.ResourcePath);
        var heroLoad = ResourceLoader.Load<PackedScene>(Templates.HeroTemplate.ResourcePath);
        LoadingProgress = 50;
        // Instantiate level entity
        _levelInstance = levelload.Instantiate<LevelEntity>();
        _levelInstance.Inject(CurrentLevelData);
        AddChild(_levelInstance);
        _levelInstance.AddToGroup("level");
        LoadingProgress = 70;
        // Initialize and add core systems
        CurrentClockUtility = new(_eventService);
        CurrentChestUtility = new(chestLoad, _audioService, _eventService);
        CurrentMapUtility = new(_eventService);
        CurrentMobUtility = new(mobLoad, _audioService, _eventService);
        CurrentPlayerUtility = new(heroLoad, _audioService, _eventService, _heroService);
        CurrentXPUtility = new(_audioService, _eventService);
        LoadingProgress = 90;
        // Add systems to level entity
        _levelInstance.AddChild(CurrentClockUtility);
        _levelInstance.AddChild(CurrentChestUtility);
        _levelInstance.AddChild(CurrentMapUtility);
        _levelInstance.AddChild(CurrentMobUtility);
        _levelInstance.AddChild(CurrentPlayerUtility);
        _levelInstance.AddChild(CurrentXPUtility);
        LoadingProgress = 100;
        // Initialize systems
        _eventService.Publish<Init>();
        IsLevelLoaded = true;
        GD.PrintRich("[color=#00ff88]GameManager: Level loaded and systems initialized.[/color]");
    }
    /// <summary>
    /// Toggles the paused state of the game. When paused, it stops processing for the player and mob systems.
    /// </summary>
    public void TogglePause()
    {
        IsPlaying = !IsPlaying;
        if (!IsPlaying)
        {
            GetTree().Paused = true;
            CurrentClockUtility.PauseTimers();
        }
        else
        {
            GetTree().Paused = false;
            CurrentClockUtility.ResumeTimers();
        }
    }
    /// <summary>
    /// Unloads the current level by freeing all utilities and resetting the level loaded state.
    /// </summary>
    public void UnloadLevel()
    {
        if (!IsLevelLoaded)
        {
            GD.PrintErr("No level loaded in GameManager to unload");
            return;
        }
        CurrentClockUtility.QueueFree();
        CurrentClockUtility = null;
        CurrentChestUtility.QueueFree();
        CurrentChestUtility = null;
        CurrentMapUtility.QueueFree();
        CurrentMapUtility = null;
        CurrentMobUtility.QueueFree();
        CurrentMobUtility = null;
        CurrentPlayerUtility.QueueFree();
        CurrentPlayerUtility = null;
        _levelInstance.QueueFree();
        CurrentLevelData = null;
        IsLevelLoaded = false;
    }
    // Event Handlers
    private void OnIndexEvent(IEvent eventData) => Templates = ((IndexEvent)eventData).Templates;
    private void OnPulseTimeout(IEvent eventData)
    {
        if (!IsLevelLoaded) return;
        if (!IsPlaying) return;
        CurrentMapUtility.Update();
        CurrentMobUtility.Update();
    }
    private void OnSlowPulseTimeout(IEvent eventData)
    {
        if (!IsLevelLoaded) return;
        if (!IsPlaying) return;
        CurrentXPUtility.Update();
    }
}