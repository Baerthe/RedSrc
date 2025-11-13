namespace Event;

using Entities;
using Interface;
/// <summary>
/// Event class that delivers a rarity of XP picked up each pulse.
/// </summary>
public sealed partial class XPEvent : IEvent
{
    public RarityType Rarity { get; private set; }
    public XPEvent(RarityType rarity)
    {
        Rarity = rarity;
    }
}