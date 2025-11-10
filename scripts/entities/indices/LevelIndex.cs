namespace Entities;

using Godot;
/// <summary>
/// Holds references to all level data for easy access.
/// </summary>
[GlobalClass]
public sealed partial class LevelIndex : Resource
{
    [ExportCategory("Level Data")]
    [Export] public LevelData[] AllLevels { get; private set; }
}