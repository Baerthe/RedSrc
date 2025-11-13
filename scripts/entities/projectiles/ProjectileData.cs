namespace Data;

using Interfaces;
using Godot;
/// <summary>
/// Data container for projectile properties
/// </summary>
[GlobalClass]
public partial class ProjectileData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public Info Info { get; private set; } = new Info();
    [Export] public Metadata MetaData { get; private set; } = new Metadata();
    [ExportGroup("Attributes")]
    [Export] public float Speed { get; private set; } = 400f;
    [Export] public float Lifetime { get; private set; } = 2f;
    [Export] public Effect[] Effects { get; private set; }
    [ExportGroup("Assets")]
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public AudioStream LaunchSound { get; private set; }
    [Export] public AudioStream HitSound { get; private set; }
}