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
    [Export] public Info Info { get; private set; } = new Info();
    [Export] public Metadata MetaData { get; private set; } = new Metadata();
    [ExportGroup("Attributes")]
    [Export] public uint MaxLevel { get; set; } = 1;
    //[Export] public Projectile[] Projectiles { get; private set; }
    [Export] public Effect[] Effects { get; private set; }
    [Export] public float AttackSpeed { get; set; }
    [Export] public float Range { get; set; }
    [ExportGroup("Assets")]
    [Export] public Texture2D Icon { get; private set; }
    [Export] public Texture2D AttackSprite { get; private set; }
    [Export] public AudioStream SwingSound { get; private set; }
    [Export] public AudioStream HitSound { get; private set; }
}