namespace Entities;

using Godot;
/// <summary>
/// Holds references to all hero data for easy access.
/// </summary>
[GlobalClass]
public sealed partial class HeroIndex : Resource
{
    [ExportCategory("Hero Data")]
    [Export] public HeroData[] AllHeroes { get; private set; }
}