namespace Entities;
/// Global enumerations used across various entities in the game.
/// <summary>
/// An enumeration of material types that can be used for various game mechanics.
/// </summary>
public enum MaterialType : byte
{
    Arcane = 0,
    Balm = 2,
    Blunt = 4,
    Cut = 6,
    Force = 8,
    Pierce = 10,
    Necro = 12,
    Wild = 14
}
/// <summary>
/// An enumeration of elemental types that can be used for various game mechanics.
/// </summary>
public enum ElementType : byte
{
    None = 0,
    Fire = 2,
    Water = 4,
    Earth = 6,
    Air = 8,
    Light = 10,
    Dark = 12,
    Electric = 14,
    Ice = 16
}
/// <summary>
/// An enumeration of rarity types for items, effects, or entities in the game.
/// </summary>
public enum RarityType : byte
{
    Basic = 0,
    Common = 2,
    Uncommon = 4,
    Rare = 6,
    Epic = 8,
    Legendary = 10,
    Mythic = 12,
    Ascendant = 14,
    Cosmic = 16,
    Eldritch = 18,
    Multiversal = 20,
    Omniversal = 22
}