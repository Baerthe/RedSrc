namespace Data;

using Godot;
using Interface;
/// <summary>
/// Data container for mobs
/// </summary>
[GlobalClass]
public partial class MobData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    [ExportGroup("Attributes")]
    [Export] public MobTribe Tribe { get; private set; }
    [Export] public MobLevel Level { get; private set; } = MobLevel.Basic;
    [Export] public StatsData Stats { get; private set; }
    [Export] public Effect[] Effects { get; private set; }
    [ExportGroup("Behavior")]
    [Export] public MobAbility Ability { get; private set; } = MobAbility.None;
    [Export] public uint AbilityStrength { get; private set; }
    [Export] public MobMovement MovementType { get; private set; } = MobMovement.PlayerAttracted;
    [ExportGroup("Assets")]
    [Export] public AssetData Assets { get; private set; } 
}