namespace Entities;

using Godot;
using Entities.Interfaces;
/// <summary>
/// ChestData is a Resource that defines the properties and attributes of a chest entity in the game.
/// </summary>
[GlobalClass]
public partial class ChestData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public Info Info { get; private set; } = new Info();
    [Export] public Metadata MetaData { get; private set; } = new Metadata();
    [Export] public ChestType Type { get; private set; } = ChestType.Item;
    [ExportGroup("Assets")]
    [Export] public SpriteFrames Sprite { get; set; }
}