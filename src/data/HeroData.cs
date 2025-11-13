namespace Data;

using Godot;
using Interface;
/// <summary>
/// The Data class for Heroes, stores static data
/// </summary>
[GlobalClass]
public partial class HeroData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public Info Info { get; private set; } = new Info();
    [Export] public Metadata MetaData { get; private set; } = new Metadata();
    [ExportGroup("Attributes")]
    [Export] public HeroStats Stats { get; private set; }
    [ExportGroup("Modifiers")]
    [Export] public HeroClass Class { get; private set; } = HeroClass.Warrior;
    [Export] public HeroAbility Ability { get; private set; } = HeroAbility.None;
    [Export] public uint AbilityStrength { get; private set; }
    [Export] public HeroMovement Movement { get; private set; } = HeroMovement.Walk;
    [ExportGroup("Assets")]
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public AudioStream HitSound { get; set; }
    [Export] public AudioStream DeathSound { get; set; }
    [Export] public Shape2D CollisionShape { get; set; }
    [Export] public Color TintColor { get; set; } = Colors.White;
}