namespace Entities;

using Godot;
/// <summary>
/// Holds data for experience point (XP) entities.
/// All XP types in the game should be listed here.
/// </summary>
[GlobalClass]
public sealed partial class XPIndex : Resource
{
    [ExportCategory("XP Data")]
    [Export] public XPdata[] AllXP { get; private set; }
}