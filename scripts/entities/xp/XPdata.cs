namespace Entities;

using Entities.Interfaces;
using Godot;
public sealed partial class XPdata : Resource, IData
{
    [ExportCategory("XP Data")]
    [Export] public Info Info { get; private set; }
    [Export] public RarityType XPAmount { get; private set; }
    [Export] public Texture2D XPTexture { get; private set; }
    [Export] public Shape2D XPCollisionShape { get; private set; }
}