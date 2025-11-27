namespace Manager;

using Data;
using Entity;
using Event;
using Godot;
using Interface;
using Service;
using System;
using Utility;
[GlobalClass]
public partial class GameManager : Node2D
{
    public static GameManager Instance { get; private set; }
    public ClockUtility CurrentClockUtility { get; private set; }
    public ChestUtility CurrentChestUtility { get; private set; }
    public MapUtility CurrentMapUtility { get; private set; }
    public MobUtility CurrentMobUtility { get; private set; }
    public PlayerUtility CurrentPlayerUtility { get; private set; }
    public XPUtility CurrentXPUtility { get; private set; }
    public LevelData CurrentLevelData { get; private set; }
    public Camera2D Camera { get; private set; }
    public EntityIndex Templates { get; private set; }
    public (LevelData, HeroData) DebugInfo { get; private set; }
    public bool IsPlaying { get; private set; } = false;
    public bool IsLevelLoaded { get; private set; } = false;
    private LevelEntity _levelInstance;
    private IAudioService _audioService;
    private IEventService _eventService;
    private IHeroService _heroService;
    private IPrefService _prefService;
    private ILevelService _levelService;
    // Debug
    private bool _isDebugMode = false;
    // Godot Methods
    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            GD.PrintRich("[color=#00ff88]GameManager: Instance Selected.[/color]");
        }
        else
        {
            GD.PrintErr("Multiple instances of GameManager detected! There should only be one instance of GameManager in the scene tree.");
            QueueFree();
        }
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<IndexEvent>(OnIndexEvent);
        _eventService.Unsubscribe<PulseTimeout>(OnPulseTimeout);
        _eventService.Unsubscribe<SlowPulseTimeout>(OnSlowPulseTimeout);
        _eventService.Unsubscribe<DebugModeEvent>(OnDebugModeEvent);
    }
    public override void _Process(double delta)
    {
        if (!IsLevelLoaded) return;
        if (!IsPlaying) return;
    }
    // GameManager Methods
    public void Initialize()
    {
        GD.PrintRich("[color=#00ff88]GameManager: Initializing services...[/color]");
        _audioService = ServiceProvider.AudioService();
        _eventService = ServiceProvider.EventService();
        _heroService = ServiceProvider.HeroService();
        _prefService = ServiceProvider.PrefService();
        _levelService = ServiceProvider.LevelService();
        _eventService.Subscribe<IndexEvent>(OnIndexEvent);
        _eventService.Subscribe<PulseTimeout>(OnPulseTimeout);
        _eventService.Subscribe<SlowPulseTimeout>(OnSlowPulseTimeout);
        _eventService.Subscribe<DebugModeEvent>(OnDebugModeEvent);
        Camera = GetParent().GetNode<Camera2D>("MainCamera");
        GD.PrintRich("[color=#00ff88]GameManager: Services initialized.[/color]");
    }
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
        //_eventService.Publish<LoadingProgress>(new LoadingProgress(0));
        var levelload = ResourceLoader.Load<PackedScene>(Templates.LevelTemplate.ResourcePath);
        var chestLoad = ResourceLoader.Load<PackedScene>(Templates.ChestTemplate.ResourcePath);
        var mobLoad = ResourceLoader.Load<PackedScene>(Templates.MobTemplate.ResourcePath);
        var heroLoad = ResourceLoader.Load<PackedScene>(Templates.HeroTemplate.ResourcePath);
        //_eventService.Publish<LoadingProgress>(new LoadingProgress(50));
        // Instantiate level entity
        _levelInstance = levelload.Instantiate<LevelEntity>();
        _levelInstance.Inject(CurrentLevelData);
        AddChild(_levelInstance);
        _levelInstance.AddToGroup("level");
        //_eventService.Publish<LoadingProgress>(new LoadingProgress(70));
        // Initialize and add core systems
        CurrentClockUtility = new(_eventService);
        CurrentChestUtility = new(chestLoad, _audioService, _eventService);
        CurrentMapUtility = new(_eventService);
        CurrentMobUtility = new(mobLoad, _audioService, _eventService);
        CurrentPlayerUtility = new(heroLoad, _audioService, _eventService, _heroService);
        CurrentXPUtility = new(_audioService, _eventService);
        //_eventService.Publish<LoadingProgress>(new LoadingProgress(90));
        // Add systems to level entity
        _levelInstance.AddChild(CurrentPlayerUtility);
        _levelInstance.AddChild(CurrentMapUtility);
        // Map and player must be added before mobs and chests, since those reference the player and map itself.
        _levelInstance.AddChild(CurrentClockUtility);
        _levelInstance.AddChild(CurrentChestUtility);
        _levelInstance.AddChild(CurrentMobUtility);
        _levelInstance.AddChild(CurrentXPUtility);
        //_eventService.Publish<LoadingProgress>(new LoadingProgress(100));
        // Initialize systems
        IsLevelLoaded = true;
        GD.PrintRich("[color=#00ff88]GameManager: Level loaded and systems initialized.[/color]");
        _eventService.Publish<InitEvent>();
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
    private void OnIndexEvent(IEvent eventData)
    {
        Templates = ((IndexEvent)eventData).Templates;
        DebugInfo = (((IndexEvent)eventData).Levels.DebugLevel, ((IndexEvent)eventData).Heroes.DebugHero);
        GD.PrintRich("[color=#00ff88]GameManager: Indices set.[/color]");
    }
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
    private void OnDebugModeEvent()
    {
        _isDebugMode = true;
        ServiceProvider.LevelService().LoadLevel(DebugInfo.Item1);
        ServiceProvider.HeroService().LoadHero(DebugInfo.Item2);
        GD.PrintRich("[color=#000][bgcolor=#ff0000]Debug Level Loading.[/bgcolor][/color]");
        LoadLevel();
    }
}