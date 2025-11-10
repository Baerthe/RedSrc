namespace Entities;
using Godot;
using System;
using Entities.Interfaces;
/// <summary>
/// WeaponEntity is a Node2D that represents a weapon in the game. It contains various properties that define the weapon's characteristics, including its name, description, lore, data, icon, and sounds. It ensures that all necessary properties are set and adds itself to the "weapons" group for easy management within the game.
/// </summary>
[GlobalClass]
public partial class WeaponEntity : Node2D, IEntity
{
    [ExportCategory("Components")]
    [ExportGroup("Components")]
    [Export] public Sprite2D AttackSprite { get; private set; }
    public WeaponData Data { get; private set; }
    public uint CurrentLevel
    {
        get
        {
            return Math.Clamp(_currentLevel, 1, MaxLevel);
        }
        set
        {
            if (value < 1 || value > MaxLevel)
                GD.PrintErr($"WARNING: Tried to set {this}'s CurrentLevel to {value}, which is outside the valid range of 1 to {MaxLevel}. Clamping to valid range.");
            _currentLevel = Math.Clamp(value, 1, MaxLevel);
        }
    }
    private uint _currentLevel = 1;
    private uint MaxLevel => (Data as WeaponData)?.MaxLevel ?? 1;

    public override void _Ready()
    {
        NullCheck();
        AddToGroup("weapons");
    }
    public void Inject(IData data)
    {
        if (Data != null)
        {
            GD.PrintErr($"WeaponEntity {Name} already initialized with data!");
            return;
        }
        Data = (WeaponData)data ?? throw new ArgumentNullException(nameof(data));
        AttackSprite.Texture = Data.AttackSprite;
    }
    public void NullCheck()
    {
        byte failure = 0;
        if (AttackSprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have AttackSprite set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}