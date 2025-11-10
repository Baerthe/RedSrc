namespace Game;

using Godot;
using Core;
using Entities;
using Game.Interface;
using Core.Interface;
/// <summary>
/// ChestSystem is responsible for managing chest spawning and interactions within the game. It implements the IGameSystem interface and utilizes a Path2D and PathFollow2D to determine chest spawn locations relative to the player.
/// </summary>
public sealed partial class ChestSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private LevelEntity _levelRef;
    private HeroEntity _playerRef;
    private Path2D _chestPath;
    private PathFollow2D _chestSpawner;
    private Vector2 _offsetBetweenChestAndPlayer;
    private PackedScene _chestTemplate;
    // Dependency Services
    private readonly IAudioService _audioService;
    private readonly IEventService _eventService;
    public ChestSystem(PackedScene chestTemplate, IAudioService audioService, IEventService eventService)
    {
        GD.Print("ChestSystem: Initializing...");
        _chestTemplate = chestTemplate;
        _audioService = audioService;
        _eventService = eventService;
    }
    public override void _Ready()
    {
        _eventService.Subscribe<Init>(OnInit);
        _eventService.Subscribe<ChestSpawnTimeout>(OnChestSpawnTimeout);
        GD.Print("ChestSystem Ready.");
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<Init>(OnInit);
        _eventService.Unsubscribe<ChestSpawnTimeout>(OnChestSpawnTimeout);
    }
    public void OnInit()
    {
        if (IsInitialized) return;
        _levelRef = GetTree().GetFirstNodeInGroup("level") as LevelEntity;
        _playerRef = GetTree().GetFirstNodeInGroup("player") as HeroEntity;
        _chestPath = CreatePath();
        _chestSpawner = new PathFollow2D();
        _chestPath.AddChild(_chestSpawner);
        _playerRef.AddChild(_chestPath);
        IsInitialized = true;
    }
    /// <summary>
    /// Updates the offset between the chest and the player instance.
    /// </summary>
    public void OnPulseTimeOut()
    {
        _chestPath.GlobalPosition = _playerRef.GlobalPosition - _offsetBetweenChestAndPlayer;
    }
    /// <summary>
    /// Creates a circular Path2D for chest spawning, its always slightly outside the player's viewport.
    /// </summary>
    private Path2D CreatePath()
    {
        var path = new Path2D();
        path.Curve = new Curve2D();
        path.Curve.AddPoint(new Vector2(0, 0));
        for (int i = 1; i <= 10; i++)
        {
            var angle = i * (Mathf.Pi * 2 / 10);
            var radius = 400;
            var x = radius * Mathf.Cos(angle);
            var y = radius * Mathf.Sin(angle);
            path.Curve.AddPoint(new Vector2(x, y));
        }
        path.GlobalPosition = _playerRef.GlobalPosition;
        path.GlobalPosition -= new Vector2(200, 200);
        _offsetBetweenChestAndPlayer = _playerRef.GlobalPosition - path.GlobalPosition;
        _levelRef.AddChild(path);
        return path;
    }
    /// <summary>
    /// Handles the ChestSpawnTimeout event to spawn a new chest.
    /// </summary>
    private void OnChestSpawnTimeout()
    {
        if (!IsInitialized) return;
        _chestPath.GlobalPosition = _offsetBetweenChestAndPlayer;
        _chestSpawner.ProgressRatio = GD.Randf();
        var chestInstance = _chestTemplate.Instantiate<ChestEntity>();
        chestInstance.GlobalPosition = _chestSpawner.GlobalPosition;
        chestInstance.AddToGroup("chests");
        _levelRef.AddChild(chestInstance);
    }
}