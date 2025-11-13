namespace Data;

using Godot;
/// <summary>
/// Holds references to entity templates for easy access.
/// </summary>
[GlobalClass]
public sealed partial class EntityIndex : Resource
{
    [ExportCategory("Entity Templates")]
    [Export] public PackedScene ChestTemplate { get; private set; }
    [Export] public PackedScene HeroTemplate { get; private set; }
    [Export] public PackedScene ItemTemplate { get; private set; }
    [Export] public PackedScene LevelTemplate { get; private set; }
    [Export] public PackedScene MobTemplate { get; private set; }
    [Export] public PackedScene ProjectileTemplate { get; private set; }
    [Export] public PackedScene WeaponTemplate { get; private set; }
    [Export] public PackedScene XPTemplate { get; private set; }
}