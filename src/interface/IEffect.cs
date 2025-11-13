namespace Interface;

using Interface;
using Godot;

public interface IEffect
{
    public Info Info { get; }
    public Texture2D Icon { get; }
    public ElementType Element { get; }
    public MaterialType Material { get; }
    public uint Power { get; }
    void Apply(IEntity entity);
}