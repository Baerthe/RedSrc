namespace Data;

using Interface;
using Godot;
/// <summary>
/// Data container for projectile properties
/// </summary>
[GlobalClass]
public partial class ProjectileData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public InfoData Info { get; private set; } = new InfoData();
    [Export] public Metadata MetaData { get; private set; } = new Metadata();
    [ExportGroup("Attributes")]
    [Export] public float Lifetime { get; private set; } = 2f;
    [Export] public Effect[] Effects { get; private set; }
    [ExportGroup("Assets")]
    [Export] public AssetData Assets { get; private set; } = new AssetData();
}