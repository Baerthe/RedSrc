namespace Core;

using Entities;
using Core.Interface;
/// <summary>
/// Event class containing all game indices for easy access to pass to game systems.
/// </summary>
public sealed class IndexEvent : IEvent
{
    public HeroIndex Heroes { get; private set; }
	public EntityIndex Templates { get; private set; }
	public ItemIndex Items { get; private set; }
	public LevelIndex Levels { get; private set; }
	public WeaponIndex Weapons { get; private set; }
    public IndexEvent(HeroIndex heroes, EntityIndex templates, ItemIndex items, LevelIndex levels, WeaponIndex weapons)
    {
        Heroes = heroes;
        Templates = templates;
        Items = items;
        Levels = levels;
        Weapons = weapons;
    }
}