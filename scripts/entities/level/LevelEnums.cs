namespace Entities;
/// <summary>
/// LevelEnums contains enumerations related to level types and tiers in the game.
/// </summary>
public enum LevelType : byte
{
    Unset = 0,
    Forest = 4,
    Village = 8,
    City = 12,
    Swamp = 16,
    Plains = 20
}
public enum LevelTier : byte
{
    Basic = 0,
    Advanced = 2,
    Expert = 4,
    Master = 6,
    Ascendant = 8
}