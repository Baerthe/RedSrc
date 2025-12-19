namespace Interface;

using Data;
using Godot;

public interface IEffect
{
    public InfoData Info { get; }
    public Metadata MetaData { get; }
    public ElementType Element { get; }
    public MaterialType Material { get; }
    public uint Power { get; }
    void Apply(IEntity entity);
}