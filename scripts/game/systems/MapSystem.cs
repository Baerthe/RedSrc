namespace Game;

using Godot;
using Core;
using System.Collections.Generic;
using Game.Interface;
using Core.Interface;
using Entities;
/// <summary>
/// A system for handling tiling of scene map elements.
/// </summary>
public sealed partial class MapSystem : Node2D, IGameSystem
{
	public bool IsInitialized { get; private set; } = false;
	private LevelEntity _levelRef;
	private HeroEntity _playerRef;
	private TileMapLayer _foregroundLayer;
	private TileMapLayer _backgroundLayer;
	private Rect2 _usedRect;
	private Rect2 _worldRect;
	private float _width;
	private float _height;
	private readonly Dictionary<string, (TileMapLayer background, TileMapLayer foreground)> _chunks = new();
	// Dependency Services
	private readonly IEventService _eventService;
	private enum Direction : byte
	{
		NorthWest,
		North,
		NorthEast,
		West,
		East,
		SouthWest,
		South,
		SouthEast,
		OutOfBounds
	}
	public MapSystem(IEventService eventService)
	{
		GD.Print("MapSystem: Initializing...");
		_eventService = eventService;
	}
	public override void _Ready()
	{
		_eventService.Subscribe<Init>(OnInit);
		GD.Print("MapSystem Ready.");
	}
	public override void _ExitTree()
    {
        _eventService.Unsubscribe<Init>(OnInit);
    }
    public void OnInit()
	{
		if (IsInitialized) return;
        _levelRef = GetTree().GetFirstNodeInGroup("level") as LevelEntity;
        _playerRef = GetTree().GetFirstNodeInGroup("player") as HeroEntity;
		LoadTiles();
		IsInitialized = true;
	}
	public void Update()
    {
		HasPlayerCrossedBorder();
    }
	public Rect2 GetWorldRect() => _worldRect;
	public void LoadTiles()
	{
		if (_foregroundLayer == null || _backgroundLayer == null)
		{
			GD.PrintErr("ForegroundLayer or BackgroundLayer not set. Attempting to find in Tree.");
			_foregroundLayer = _levelRef.Map.ForegroundLayer;
			_backgroundLayer = _levelRef.Map.BackgroundLayer;
			if (_foregroundLayer == null || _backgroundLayer == null)
			{
				GD.PrintErr("ForegroundLayer or BackgroundLayer not found in Tree.");
				return;
			}
		}
		_usedRect = _backgroundLayer.GetUsedRect();
		float tileSize = _backgroundLayer.TileSet.TileSize.X;
		_width = _usedRect.Size.X * tileSize;
		_height = _usedRect.Size.Y * tileSize;
		_worldRect = new Rect2(
			0, 0,
			_width,
			_height
		);
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				if (x == 0 && y == 0) continue;
				var backgroundDuplicate = _backgroundLayer.Duplicate() as TileMapLayer;
				var foregroundDuplicate = _foregroundLayer.Duplicate() as TileMapLayer;
				backgroundDuplicate.Position += new Vector2(x * _width, y * _height);
				foregroundDuplicate.Position += new Vector2(x * _width, y * _height);
				if (backgroundDuplicate == null || foregroundDuplicate == null)
				{
					GD.PrintErr("Failed to duplicate TileMapLayer.");
					continue;
				}
				string chunkName = $"{x},{y}";
				_chunks.Add(chunkName, (backgroundDuplicate, foregroundDuplicate));
				GD.Print($"Added chunk {chunkName} at position {backgroundDuplicate.Position}");
				GD.Print($"Added chunk {chunkName} at position {foregroundDuplicate.Position}");
				backgroundDuplicate.Visible = false;
				foregroundDuplicate.Visible = false;
				backgroundDuplicate.Name = $"Background_{chunkName}";
				foregroundDuplicate.Name = $"Foreground_{chunkName}";
				backgroundDuplicate.ZIndex = -999;
				foregroundDuplicate.ZIndex = -998;
				AddChild(backgroundDuplicate);
				AddChild(foregroundDuplicate);
				backgroundDuplicate.Visible = true;
				foregroundDuplicate.Visible = true;
			}
		}
	}
	private void HasPlayerCrossedBorder()
	{
		if (_playerRef == null || _worldRect.HasPoint(_playerRef.Position)) return;
		Direction direction = GetBorderDirection(_playerRef.Position);
		if (direction == Direction.OutOfBounds) return;
		MoveLayers(direction);
	}
	private Direction GetBorderDirection(Vector2 playerPosition)
	{
		bool isWest = playerPosition.X < _worldRect.Position.X;
		bool isEast = playerPosition.X > _worldRect.End.X;
		bool isNorth = playerPosition.Y < _worldRect.Position.Y;
		bool isSouth = playerPosition.Y > _worldRect.End.Y;

		if (isNorth && isWest) return Direction.NorthWest;
		if (isNorth && isEast) return Direction.NorthEast;
		if (isSouth && isWest) return Direction.SouthWest;
		if (isSouth && isEast) return Direction.SouthEast;
		if (isWest) return Direction.West;
		if (isEast) return Direction.East;
		if (isNorth) return Direction.North;
		if (isSouth) return Direction.South;

		GD.PrintErr("Player is outside world rect but no direction matched. How they do that??? TilingService cannot find the direction and has lost the player.");
		return Direction.OutOfBounds;
	}
	private void MoveLayers(Direction direction)
	{
		Vector2 offset = direction switch
		{
			Direction.NorthWest => new Vector2(-_width, -_height),
			Direction.North => new Vector2(0, -_height),
			Direction.NorthEast => new Vector2(_width, -_height),
			Direction.West => new Vector2(-_width, 0),
			Direction.East => new Vector2(_width, 0),
			Direction.SouthWest => new Vector2(-_width, _height),
			Direction.South => new Vector2(0, _height),
			Direction.SouthEast => new Vector2(_width, _height),
			_ => Vector2.Zero
		};
		_worldRect.Position += offset;
		GD.Print($"World Rect moved to: {_worldRect.Position}");
		GD.Print($"Moving layers due to player crossing {direction} border.");
		_backgroundLayer.Position += offset;
		_foregroundLayer.Position += offset;
		foreach (var chunk in _chunks.Values)
		{
			chunk.background.Position += offset;
			chunk.foreground.Position += offset;
		}
	}
}
