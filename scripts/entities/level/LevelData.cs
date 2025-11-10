namespace Entities;

using Godot;
using Entities.Interfaces;
/// <summary>
/// LevelData is a Resource that encapsulates the core attributes of a game level, including its type, tier, and the mob spawn table associated with it.
/// </summary>
[GlobalClass]
public partial class LevelData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public Info Info { get; private set; } = new Info();
    [Export] public Metadata MetaData { get; private set; } = new Metadata();
    [Export] public LevelType Type { get; set; }
    [Export] public LevelTier Tier { get; set; }
    [Export] public uint MaxLevel { get; set; } = 1;
    [Export] public float MaxTime { get; set; } = 600f;
    [ExportCategory("Scenes")]
    [Export] public MobTable MobTable { get; private set; }
    [Export] public PackedScene Map { get; private set; }
}
