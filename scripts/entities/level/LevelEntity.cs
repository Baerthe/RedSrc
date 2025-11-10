namespace Entities;

using Godot;
using System;
using Entities.Interfaces;
/// <summary>
/// LevelEntity is a Node2D that represents a level in the game. It contains various properties that define the level's characteristics, including its name, description, data, tilemap layers, player spawn point, and various systems for managing chests, mobs, and players.
/// </summary>
[GlobalClass]
public partial class LevelEntity : Node2D, IEntity
{
    public IMap Map { get; private set; }
    public IData Data { get; set; }
    public override void _Ready()
    {
        NullCheck();
        AddToGroup("levels");
    }
    public void Inject(IData data)
    {
        if (Data != null)
        {
            GD.PrintErr($"LevelEntity {Name} already initialized with data!");
            return;
        }
        Data = data ?? throw new ArgumentNullException(nameof(data));
        var mapScene = ResourceLoader.Load<PackedScene>((Data as LevelData)?.Map.ResourcePath) ?? throw new InvalidOperationException("LevelData does not contain a valid Map scene!");
        Map = mapScene.Instantiate<IMap>();
        AddChild(Map as Node2D);
    }
    public void NullCheck() { } // No components to check currently
}