namespace Core;

using Core.Interface;
/// Non-data events for Game Systems.
public sealed class Init : IEvent;
// Player Events
public sealed class PlayerSpawn : IEvent;
public sealed class PlayerDefeat : IEvent;
public sealed class PlayerVictory : IEvent;
public sealed class PlayerGainedXP : IEvent
{
    public uint Amount
    {
        get => _amount;
        set {
            if (value < 0)
                _amount = 0;
            _amount = value;
        }
    }
    private uint _amount;
    public PlayerGainedXP(uint amount) => Amount = amount;
}