namespace Entities;

using Godot;
using System;
using Entities.Interfaces;
/// <summary>
/// The Entity class for Chests, stores components and runtime data
/// </summary>
[GlobalClass]
public partial class ChestEntity : Node2D, IEntity
{
    [ExportCategory("Components")]
    [ExportGroup("Components")]
    [Export] public CollisionObject2D Hitbox { get; private set; }
    [Export] public AnimatedSprite2D Sprite { get; private set; }
    public ChestData Data { get; private set; }
    public override void _Ready()
    {
        if (Data == null)
        {
            GD.PrintErr($"ChestEntity {Name} was not initialized with data before _Ready! Did you not call Inject() before adding to scene? Deleting instance.");
            QueueFree();
            return;
        }
        NullCheck();
        AddToGroup("chests");
    }
    public void Inject(IData data)
    {
        if (Data != null)
        {
            GD.PrintErr($"ChestEntity {Name} already initialized with data!");
            return;
        }
        Data = (ChestData)data ?? throw new ArgumentNullException(nameof(data));
        Sprite.SpriteFrames = Data.Sprite;
    }
    public void NullCheck()
    {
        byte failure = 0;
        if (Hitbox == null) { GD.PrintErr($"ERROR: {this.Name} does not have Hitbox set!"); failure++; }
        if (Sprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have Sprite set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}