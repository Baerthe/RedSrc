namespace Data;

using Godot;
/// <summary>
/// BaseEffect is a Resource that serves as the foundational class for all effect types in the game. Do not instantiate directly.
/// </summary>
public abstract partial class Effect : Resource
{
    [Export] public Info Info { get; private set; }
    [Export] public Texture2D Icon { get; private set; }
    [Export] public ElementType Element { get; private set; }
    [Export] public MaterialType Material { get; private set; }
    [Export] public uint Power { get; private set; }
}