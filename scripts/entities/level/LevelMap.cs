namespace Entities;

using Godot;
/// <summary>
/// LevelMap is a Node2D that represents the map of a level in the game. It contains references to the foreground and background tile map layers, as well as the player spawn point.
/// </summary>
public partial class LevelMap : Node2D
{
    [ExportCategory("Components")]
    [ExportGroup("TileMaps")]
    [Export] public TileMapLayer ForegroundLayer { get; private set; }
    [Export] public TileMapLayer BackgroundLayer { get; private set; }
    [ExportGroup("Markers")]
    [Export] public Node2D PlayerSpawn { get; private set; }
}