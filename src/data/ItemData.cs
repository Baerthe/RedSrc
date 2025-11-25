namespace Data;

using Godot;
using Interface;
/// <summary>
/// ItemData is a Resource that defines the properties and attributes of an item entity in the game.
/// </summary>
[GlobalClass]
public partial class ItemData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    [Export] public int MaxStackSize { get; set; } = 64;
    [ExportGroup("Assets")]
    [Export] public AssetData Assets { get; private set; } 
}