namespace Entities;
/// <summary>
/// The Enums used for Mobs
/// </summary>
public enum MobAbility : byte
{
    None = 0,
    Poison = 4,
    Healer = 8,
    Explodes = 12,
    Aura = 16
}
public enum MobTribe : byte
{
    None = 0,
    Beast = 8,
    Undead = 16,
    Elemental = 24,
    Humanoid = 32,
    Goblinoid = 40,
    Insectoid = 48
}
public enum MobLevel : byte
{
    Basic = 0,
    Advanced = 2,
    Elite = 4,
    Boss = 6
}
public enum MobMovement : byte
{
    DashDirection = 0,
    CurvedDirection = 4,
    PlayerAttracted = 8,
    RandomDirection = 12,
    Stationary = 16,
    ZigZagSway = 20,
    CircleStrafe = 24
}