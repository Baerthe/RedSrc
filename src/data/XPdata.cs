namespace Data;

using Interface;
using Godot;
public sealed partial class XPdata : Resource, IData
{
    [ExportCategory("XP Data")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    [Export] public RarityType XPAmount { get; private set; }
    [Export] public AssetData Assets { get; private set; } 
}