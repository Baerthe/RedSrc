namespace Entities;

using Godot;
/// <summary>
/// Holds references to weapon data for easy access.
/// All weapons in the game should be listed here.
/// </summary>
[GlobalClass]
public sealed partial class WeaponIndex : Resource
{
    [ExportCategory("Weapon Data")]
    [Export] public WeaponData[] AllWeapons { get; private set; }
}