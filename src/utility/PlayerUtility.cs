namespace Utility;

using Data;
using Entity;
using Event;
using Godot;
using Interface;
using System.Collections.Generic;
/// <summary>
/// The player is the main character that the user controls. This class handles movement, health, and collisions with mobs.
/// </summary>
public sealed partial class PlayerUtility : Node2D, IUtility
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
    public PlayerUtility(PackedScene heroTemplate, IAudioService audioService, IEventService eventService, IHeroService heroService)
    {
        GD.Print("PlayerUtility: Initializing...");
        _audioService = audioService;
        _eventService = eventService;
        _heroService = heroService;
        _heroTemplate = heroTemplate;
    }
    public override void _Ready()
    {
        _eventService.Subscribe<InitEvent>(OnInit);
        GD.Print("PlayerUtility Ready.");
    }
    public override void _Process(double delta)
    {
        if (!IsInitialized) return;
        if (_playerRef == null) return;
        if (_playerRef.CurrentHealth <= 0)
        {
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
        // Are we trying to interact with something?
        if (Input.IsActionJustPressed("interact"))
        {
            var interactable = GetInteractableInRange();
            if (interactable != null && interactable.IsInteractable)
            {
                interactable.OnInteract();
            }
        }
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
            velocity = velocity.Normalized() * _playerRef.Data.Stats.Speed;
            _playerRef.Sprite.Play();
        }
        else
            _playerRef.Sprite.Stop();
        Position += velocity * (float)delta;
        _playerRef.Sprite.FlipV = false; // Make sure we never flip vertically
        _playerRef.Sprite.FlipH = velocity.X < 0;
        _playerRef.MoveAndSlide();
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<InitEvent>(OnInit);
        _playerRef.QueueFree();
        _items.Clear();
        _weapons.Clear();
        IsInitialized = false;
    }
    public void OnInit()
    {
        if (IsInitialized)
        {
            GD.PrintErr("PlayerUtility is already initialized. InitEvent should only be called once per level load.");
            return;
        }
        GD.Print("PlayerUtility initialized.");
        LoadPlayer(ServiceProvider.HeroService().CurrentHero);
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
        if (item == _items.Find(i => i.Data.Info.Named == item.Data.Info.Named))
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
        if (weapon == _weapons.Find(w => w.Data.Info.Named == weapon.Data.Info.Named))
            return;
        _weapons.Add(weapon);
    }
    private void Defeat()
    {
        GD.Print("PlayerUtility: Defeat sequence triggered.");
        _eventService.Publish<PlayerDefeat>();
        IsInitialized = false;
        _playerRef.Hide();
        _items.Clear();
        _weapons.Clear();
    }
    /// <summary>
    /// Gets an interactable entity within range of the player based on their current direction.
    /// </summary>
    private IInteractable GetInteractableInRange()
    {
        IInteractable entity = null;
        var ray = new RayCast2D();
        ray.GlobalPosition = _playerRef.GlobalPosition;
        ray.TargetPosition = _playerRef.CurrentDirection switch
        {
            PlayerDirection.Up => new Vector2(0, -8),
            PlayerDirection.Down => new Vector2(0, 8),
            PlayerDirection.Left => new Vector2(-8, 0),
            PlayerDirection.Right => new Vector2(8, 0),
            PlayerDirection.Diagonal => new Vector2(8, -8),
            _ => Vector2.Zero,
        };
        ray.Enabled = true;
        ray.CollisionMask = 2; // Interactable layer ?? TODO: make sure.
        _playerRef.AddChild(ray);
        ray.ForceRaycastUpdate();
        if (ray.IsColliding())
        {
            var collider = ray.GetCollider();
            if (collider is IInteractable interactable)
            {
                entity = interactable;
            }
        }
        _playerRef.RemoveChild(ray);
        ray.QueueFree();
        return entity;
    }
    private void LoadPlayer(HeroData hero)
    {
        if (hero == null)
        {
            GD.PrintErr("PlayerUtility: LoadPlayer called with null hero data.");
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
        GD.Print($"PlayerUtility: Loaded player '{hero.Info.Named}'.");
    }
}
