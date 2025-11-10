namespace Game;

using Godot;
using Core;
using Entities;
using Game.Interface;
using Core.Interface;
using System.Collections.Generic;
/// <summary>
/// The player is the main character that the user controls. This class handles movement, health, and collisions with mobs.
/// </summary>
public sealed partial class PlayerSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private HeroEntity _playerRef;
    private LevelEntity _levelRef;
    private List<ItemEntity> _items = new();
    private List<WeaponEntity> _weapons = new();
    private PackedScene _heroTemplate;
    // Dependency Services
    private readonly IAudioService _audioService;
    private readonly IEventService _eventService;
    private readonly IHeroService _heroService;
    public PlayerSystem(PackedScene heroTemplate, IAudioService audioService, IEventService eventService, IHeroService heroService)
    {
        GD.Print("PlayerSystem: Initializing...");
        _audioService = audioService;
        _eventService = eventService;
        _heroService = heroService;
        _heroTemplate = heroTemplate;
    }
    public override void _Ready()
    {
        _eventService.Subscribe<Init>(OnInit);
        GD.Print("PlayerSystem Ready.");
    }
    public override void _Process(double delta)
    {
        if (!IsInitialized) return;
        if (_playerRef == null) return;
        if (_playerRef.CurrentHealth <= 0)
        {
            GD.Print("Player has died.");
            Defeat();
            return;
        }
        switch (_playerRef.CurrentDirection)
        {
            case PlayerDirection.Up:
                _playerRef.Sprite.Animation = "Up";
                break;
            case PlayerDirection.Down:
                _playerRef.Sprite.Animation = "Down";
                break;
            case PlayerDirection.Diagonal:
                _playerRef.Sprite.Animation = "Right";
                break;
            case PlayerDirection.Left:
                _playerRef.Sprite.FlipH = true;
                _playerRef.Sprite.Animation = "Right";
                break;
            case PlayerDirection.Right:
                _playerRef.Sprite.FlipH = false;
                _playerRef.Sprite.Animation = "Right";
                break;
            default:
                _playerRef.Sprite.Animation = "Idle";
                break;
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!IsInitialized) return;
        if (_playerRef == null) return;
        var velocity = Vector2.Zero;
        //What direction is the player going?
        if (Input.IsActionPressed("move_up"))
        {
            velocity.Y -= 1;
            _playerRef.CurrentDirection = PlayerDirection.Up;
        }
        if (Input.IsActionPressed("move_down"))
        {
            velocity.Y += 1;
            _playerRef.CurrentDirection = PlayerDirection.Down;
        }
        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
            _playerRef.CurrentDirection = PlayerDirection.Left;
        }
        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
            _playerRef.CurrentDirection = PlayerDirection.Right;
        }
        if (velocity.Y != 0 && velocity.X != 0)
            _playerRef.CurrentDirection = PlayerDirection.Diagonal;
        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * (_playerRef.Data as HeroData).Stats.Speed;
            _playerRef.Sprite.Play();
        }
        else
            _playerRef.Sprite.Stop();
        // Move the player.
        Position += velocity * (float)delta;
        _playerRef.Sprite.FlipV = false; // Make sure we never flip vertically
        _playerRef.Sprite.FlipH = velocity.X < 0;
        _playerRef.MoveAndSlide();
        //ExecuteWeaponAttacks();
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<Init>(OnInit);
        _playerRef.QueueFree();
        _items.Clear();
        _weapons.Clear();
        IsInitialized = false;
    }
    public void OnInit()
    {
        if (IsInitialized)
        {
            GD.PrintErr("PlayerSystem is already initialized. Init should only be called once per level load.");
            return;
        }
        GD.Print("PlayerSystem initialized.");
        LoadPlayer(CoreProvider.HeroService().CurrentHero);
        _levelRef = GetTree().GetFirstNodeInGroup("level") as LevelEntity;
        _playerRef.Show();

        IsInitialized = true;
    }
    /// <summary>
    /// Adds an item to the player's inventory. If the item already exists and is stackable, it increases the stack size.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(ItemEntity item)
    {
        if (item == _items.Find(i => i.Data.Info.Name == item.Data.Info.Name))
        {
            item.QueueFree();
            item = _items[_items.IndexOf(item)];
            var itemData = item.Data as ItemData;
            item.CurrentStackSize += 1;
            if (item.CurrentStackSize > itemData.MaxStackSize)
                item.CurrentStackSize = itemData.MaxStackSize;
            return;
        }
        _items.Add(item);
    }
    public void AddWeapon(WeaponEntity weapon)
    {
        if (weapon == _weapons.Find(w => w.Data.Info.Name == weapon.Data.Info.Name))
            return;
        _weapons.Add(weapon);
    }
    private void Defeat()
    {
        GD.Print("PlayerSystem: Defeat sequence triggered.");
        _eventService.Publish<PlayerDefeat>();
        IsInitialized = false;
        _playerRef.Hide();
        _items.Clear();
        _weapons.Clear();
    }
    private void LoadPlayer(HeroData hero)
    {
        if (hero == null)
        {
            GD.PrintErr("PlayerSystem: LoadPlayer called with null hero data.");
            return;
        }
        if (_playerRef != null)
        {
            _playerRef.QueueFree();
        }
        _playerRef = _heroTemplate.Instantiate<HeroEntity>();
        _playerRef.Inject(hero);
        _playerRef.Position = _levelRef.Map.PlayerSpawn.Position;
        _playerRef.CurrentHealth = hero.Stats.MaxHealth;
        _playerRef.Hide();
        AddChild(_playerRef);
        GD.Print($"PlayerSystem: Loaded player '{hero.Info.Name}'.");
    }
}
