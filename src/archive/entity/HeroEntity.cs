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
	public AnimatedSprite2D Sprite { get; private set; }
	public CollisionShape2D HurtBox { get; private set; }
	public CollisionShape2D HitBox { get; private set; }
	public HeroData Data { get; private set; }
	public uint CurrentHealth { get; set; }
	public PlayerDirection CurrentDirection { get; set; }
	public override void _Ready()
	{
		Sprite = new AnimatedSprite2D();
		HurtBox = new CollisionShape2D();
		HitBox = new CollisionShape2D();
		AddChild(Sprite);
		AddChild(HurtBox);
		AddChild(HitBox);
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
		HurtBox.Shape = Data.Assets.HurtBoxShape;
		HitBox.Shape = Data.Assets.HitBoxShape;
		NullCheck();
	}
	public void NullCheck()
	{
		byte failure = 0;
		if (HitBox == null) { GD.PrintErr($"ERROR: {this.Name} does not have HitBox set!"); failure++; }
		if (HurtBox == null) { GD.PrintErr($"ERROR: {this.Name} does not have HurtBox set!"); failure++; }
		if (Sprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have Sprite set!"); failure++; }
		if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
	}
}
