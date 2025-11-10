namespace Entities.Interfaces;

using Godot;
/// <summary>
/// IMap interface defines the essential components of a level map, including foreground and background tilemap layers and the player spawn point.
/// </summary>
public interface IMap
{
    public TileMapLayer ForegroundLayer { get; }
    public TileMapLayer BackgroundLayer { get; }
    public Node2D PlayerSpawn { get; }
}