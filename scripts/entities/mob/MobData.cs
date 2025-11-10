namespace Entities;

using Godot;
using Entities.Interfaces;
/// <summary>
/// Data container for mobs
/// </summary>
[GlobalClass]
public partial class MobData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public Info Info { get; private set; } = new Info();
    [Export] public Metadata MetaData { get; private set; } = new Metadata();
    [ExportGroup("Attributes")]
    [Export] public MobTribe Tribe { get; private set; }
    [Export] public MobLevel Level { get; private set; } = MobLevel.Basic;
    [Export] public MobStats Stats { get; private set; }
    [ExportGroup("Behavior")]
    [Export] public MobAbility Ability { get; private set; } = MobAbility.None;
    [Export] public uint AbilityStrength { get; private set; }
    [Export] public MobMovement MovementType { get; private set; } = MobMovement.PlayerAttracted;
    [ExportGroup("Assets")]
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public AudioStream HitSound { get; private set; }
    [Export] public AudioStream DeathSound { get; private set; }
    [Export] public Color TintColor { get; private set; } = Colors.White;
    [Export] public Shape2D CollisionShape { get; private set; }
    [Export] public Shader AnimationShader { get; private set; } = ResourceLoader.Load<Shader>("res://data/shaders/mobs/BasicMobMovement.gdshader");
}