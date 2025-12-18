namespace Entity;

using Data;
using Godot;
using System;
using Interface;
/// <summary>
/// A representation of Projectile entities within the game.
/// </summary>
public partial class ProjectileEntity : Node2D, IEntity
{
    [ExportGroup("Node References")]
    [Export] public Area2D Area { get; private set; }
    [Export] public AnimatedSprite2D Sprite { get; private set; }
    public CollisionShape2D HitBox { get; private set; }
    public ProjectileData Data { get; private set; }
    public override void _Ready()
    {
        NullCheck();
        AddToGroup("projectiles");
    }
    public void Inject(IData data)
    {
        if (Data != null)
        {
            GD.PrintErr($"ProjectileEntity {Name} already initialized with data!");
            return;
        }
        Data = (ProjectileData)data ?? throw new ArgumentNullException(nameof(data));
        Sprite.SpriteFrames = Data.Assets.Sprite;
        HitBox = Area.GetNode<CollisionShape2D>("CollisionShape2D");
    }
    public void NullCheck()
    {
        byte failure = 0;
        if (Area == null) { GD.PrintErr($"ERROR: {this.Name} does not have Area set!"); failure++; }
        if (Sprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have Sprite set!"); failure++; }
        if (HitBox == null) { GD.PrintErr($"ERROR: {this.Name} does not have HitBox set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}