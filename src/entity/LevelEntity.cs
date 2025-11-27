namespace Entity;

using Data;
using Godot;
using System;
using Interface;
/// <summary>
/// LevelEntity is a Node2D that represents a level in the game. It contains various properties that define the level's characteristics, including its name, description, data, tilemap layers, player spawn point, and various systems for managing chests, mobs, and players.
/// </summary>
[GlobalClass]
public partial class LevelEntity : Node2D, IEntity
{
	public LevelMap Map { get; private set; }
	public LevelData Data { get; set; }
	public override void _Ready()
	{
		NullCheck();
	}
	public void Inject(IData data)
	{
		if (Data != null)
		{
			GD.PrintErr($"LevelEntity {Name} already initialized with data!");
			return;
		}
		Data = (data as LevelData) ?? throw new ArgumentNullException(nameof(data));
		Map = ResourceLoader.Load<PackedScene>(Data?.Map.ResourcePath).Instantiate<LevelMap>() ?? throw new InvalidOperationException("LevelData does not contain a valid Map scene!");
		AddChild(Map);
	}
	public void NullCheck() { } // No components to check currently
}
