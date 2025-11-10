namespace Entities;

using Godot;
[GlobalClass]
public partial class HeroStats : Resource
{
    [Export] public HeroLevel Level { get; private set; } = HeroLevel.Basic;
    [Export] public uint MaxHealth { get; private set; } = 100;
    [Export] public uint DamageBonus { get; private set; } = 0;
    [Export] public uint ElementDamageBonus { get; private set; } = 0;
    [Export] public ElementType ElementBonus { get; private set; } = ElementType.None;
    [Export] public float Speed { get; private set; } = 100f;
    [Export] public byte CollisionRadius { get; private set; } = 16;
}