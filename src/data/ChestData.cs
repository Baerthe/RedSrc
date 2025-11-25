namespace Data;

using Godot;
using Interface;
/// <summary>
/// ChestData is a Resource that defines the properties and attributes of a chest entity in the game.
/// </summary>
[GlobalClass]
public partial class ChestData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    [Export] public ChestType Type { get; private set; } = ChestType.Item;
    [ExportGroup("Assets")]
    [Export] public AssetData Assets { get; private set; }
}