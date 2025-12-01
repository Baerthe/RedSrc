namespace Data;

using Godot;
[GlobalClass]
public sealed partial class AssetData : Resource
{
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public AudioStream OnInteractSound { get; set; }
    [Export] public AudioStream OnInitSound { get; set; }
    [Export] public AudioStream OnFreeSound { get; set; }
    [Export] public Shape2D HurtBoxShape { get; set; }
    [Export] public Shape2D HitBoxShape { get; set; }
    [Export] public Color TintColor { get; set; } = Colors.White;
    [Export] public Shader AnimationShader { get; private set; } = ResourceLoader.Load<Shader>("res://data/shaders/mobs/BasicMobMovement.gdshader");
}