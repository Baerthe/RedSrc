namespace Entities;
/// <summary>
/// The Enums used for Heroes
/// </summary>
public enum PlayerDirection : byte
	{
		Up,
		Right,
		Down,
		Left,
		Diagonal
	}
public enum HeroClass : byte
{
    Warrior = 0,
    Mage = 8,
    Rogue = 16,
    Cleric = 24
}
public enum HeroMovement : byte
{
    Walk = 0,
    Run = 4,
    Dash = 8,
    Teleport = 12,
    Stationary = 16
}
public enum HeroAbility : byte
{
    None = 0,
    Shield = 4,
    Heal = 8,
    Stealth = 12,
    Rage = 16
}
public enum HeroLevel : byte
{
    Basic = 0,
    Advanced = 8,
    Elite = 16,
    Legendary = 24
}