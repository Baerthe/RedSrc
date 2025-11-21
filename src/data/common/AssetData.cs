namespace Data;

using Godot;
public sealed partial class AssetData : Resource
{
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public AudioStream InteractSound { get; set; }
    [Export] public AudioStream DeathSound { get; set; }
    [Export] public byte CollisionRadius { get; private set; } = 16;
    [Export] public Shape2D CollisionShape { get; set; }
    [Export] public Color TintColor { get; set; } = Colors.White;
    [Export] public Shader AnimationShader { get; private set; } = ResourceLoader.Load<Shader>("res://data/shaders/mobs/BasicMobMovement.gdshader");
}