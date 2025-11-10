namespace Entities;

using Godot;
using System;
using Entities.Interfaces;
/// <summary>
/// MobEntity is a RigidBody2D that represents a mobile entity (mob) in the game. It contains various properties that define the mob's characteristics, including its name, description, lore, data, hitbox, sprite, cry sound, and visibility notifier. It ensures that all necessary properties are set and adds itself to the "mobs" group for easy management within the game.
/// </summary>
[GlobalClass]
public partial class MobEntity : RigidBody2D, IEntity
{
    [ExportGroup("Node References")]
    [Export] public CollisionObject2D Hitbox { get; private set; }
    [Export] public AnimatedSprite2D Sprite { get; private set; }
    [Export] public VisibleOnScreenNotifier2D Notifier2D { get; private set; }
    public MobData Data { get; private set; }
    public uint CurrentHealth { get; set; }
    public byte FrameSkipCounter { get; set; }
    public override void _Ready()
    {
        if (Data == null)
        {
            GD.PrintErr($"MobEntity {Name} was not initialized with data before _Ready! Did you not call Inject() before adding to scene? Deleting instance.");
            QueueFree();
            return;
        }
        NullCheck();
        AddToGroup("mobs");
    }
    public void Inject(IData data)
    {
        if (Data != null)
        {
            GD.PrintErr($"MobEntity {Name} already initialized with data!");
            return;
        }
        Data = (MobData)data ?? throw new ArgumentNullException(nameof(data));
        CurrentHealth = Data.Stats.MaxHealth;
        Sprite.SpriteFrames = Data.Sprite;
        Sprite.Modulate = Data.TintColor;
        CollisionShape2D shape = new CollisionShape2D();
        shape.Shape = Data.CollisionShape;
        Hitbox.AddChild(shape);
    }
    public void NullCheck()
    {
        byte failure = 0;
        if (Hitbox == null) { GD.PrintErr($"ERROR: {this.Name} does not have Hitbox set!"); failure++; }
        if (Sprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have Sprite set!"); failure++; }
        if (Notifier2D == null) { GD.PrintErr($"ERROR: {this.Name} does not have Notifier2D set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}