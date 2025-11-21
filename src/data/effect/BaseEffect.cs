namespace Data;

using Godot;
using Interface;

/// <summary>
/// BaseEffect is a Resource that serves as the foundational class for all effect types in the game.
/// </summary>
public abstract partial class Effect : Resource, IData
{
	[Export] public InfoData Info { get; private set; }
	[Export] public Metadata MetaData { get; private set; }
	[Export] public ElementType Element { get; private set; }
	[Export] public MaterialType Material { get; private set; }
	[Export] public uint Power { get; private set; }
	[Export] public AssetData Assets { get; private set; }
}
