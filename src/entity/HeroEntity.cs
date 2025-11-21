namespace Entity;

using Data;
using Godot;
using System;
using Interface;
/// <summary>
/// The Entity class for Heroes, stores components and runtime data
/// </summary>
[GlobalClass]
public partial class HeroEntity : CharacterBody2D, IEntity
{
	[ExportCategory("Stats")]
	[ExportGroup("Components")]
	[Export] public CollisionObject2D Hitbox { get; private set; }
	[Export] public CollisionObject2D PickupHitbox { get; private set; }
	[Export] public AnimatedSprite2D Sprite { get; private set; }
	public HeroData Data { get; private set; }
	public uint CurrentHealth { get; set; }
	public PlayerDirection CurrentDirection { get; set; }
	public override void _Ready()
	{
		NullCheck();
		AddToGroup("players");
	}
	public void Inject(IData data)
	{
		if (Data != null)
		{
			GD.PrintErr($"HeroEntity {Name} already initialized with data!");
			return;
		}
		Data = (HeroData)data ?? throw new ArgumentNullException(nameof(data));
		CurrentHealth = Data.Stats.MaxHealth;
		Sprite.SpriteFrames = Data.Assets.Sprite;
		Sprite.Modulate = Data.Assets.TintColor;
		CollisionShape2D shape = new CollisionShape2D();
		shape.Shape = Data.Assets.CollisionShape;
		Hitbox.AddChild(shape);
	}
	public void NullCheck()
	{
		byte failure = 0;
		if (Hitbox == null) { GD.PrintErr($"ERROR: {this.Name} does not have Hitbox set!"); failure++; }
		if (Sprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have Sprite set!"); failure++; }
		if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
	}
}
