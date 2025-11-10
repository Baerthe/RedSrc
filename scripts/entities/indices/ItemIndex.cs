namespace Entities;

using Godot;
/// <summary>
/// Holds references to item data for easy access.
/// Every item in the game should be listed here.
/// </summary>
[GlobalClass]
public sealed partial class ItemIndex : Resource
{
    [ExportCategory("Item Data")]
    [Export] public ItemData[] AllItems { get; private set; }
}