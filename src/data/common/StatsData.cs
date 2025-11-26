namespace Data;

using Godot;
[GlobalClass]
public sealed partial class StatsData : Resource
{
    [Export] public uint MaxHealth { get; private set; } = 100;
    [Export] public uint DamageBonus { get; private set; } = 0;
    [Export] public float Speed { get; private set; } = 100f;
}