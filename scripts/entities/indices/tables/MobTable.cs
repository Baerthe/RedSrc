namespace Entities;

using Godot;
/// <summary>
/// Holds references to mob data for easy access.
/// Levels can use different tables.
/// </summary>
[GlobalClass]
public sealed partial class MobTable : Resource
{
    [ExportCategory("Mob Data")]
    [Export] public MobData[] Mobs { get; private set; }
}