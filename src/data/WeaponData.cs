namespace Data;

using Godot;
using Interface;
/// <summary>
/// WeaponData is a Resource that defines the properties and attributes of a weapon entity in the game.
/// </summary>
[GlobalClass]
public partial class WeaponData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public InfoData Info { get; private set; } = new InfoData();
    [Export] public Metadata MetaData { get; private set; } = new Metadata();
    [ExportGroup("Attributes")]
    [Export] public uint MaxLevel { get; set; } = 1;
    //[Export] public Projectile[] Projectiles { get; private set; }
    [Export] public Effect[] Effects { get; private set; }
    [Export] public float AttackSpeed { get; set; }
    [Export] public float Range { get; set; }
    [ExportGroup("Assets")]
    [Export] public AssetData Assets { get; private set; } = new AssetData();
}