namespace Entities;

using Godot;
/// <summary>
/// Data container for mob stats
/// </summary>
[GlobalClass]
public partial class MobStats : Resource
{
    [Export] public uint MaxHealth { get; private set; } = 10;
    [Export] public uint Damage { get; private set; } = 1;
    [Export] public float Speed { get; private set; } = 100f;
    [Export] public Effect[] Effects { get; private set; }
}