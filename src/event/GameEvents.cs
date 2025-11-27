namespace Event;

using Godot;
using Interface;
/// Game Events
public sealed class InitEvent : IEvent;
public sealed class LoadingProgress : IEvent
{
    public byte Progress
    {
        get => _progress;
        set
        {
            if (value < 0)
                _progress = 0;
            else if (value > 100)
                _progress = 100;
            else
                _progress = value;
        }
    }
    private byte _progress;
    public LoadingProgress(byte progress) => Progress = progress;
}
public sealed class DebugModeEvent : IEvent;
// Player Events
public sealed class PlayerSpawn : IEvent;
public sealed class PlayerDefeat : IEvent;
public sealed class PlayerVictory : IEvent;
public sealed class PlayerGainedXP : IEvent
{
    public uint Amount
    {
        get => _amount;
        set
        {
            if (value < 0)
                _amount = 0;
            _amount = value;
        }
    }
    private uint _amount;
    public PlayerGainedXP(uint amount) => Amount = amount;
}
public sealed class PlayerForceMove : IEvent
{
    public Vector2 TargetPosition { get; private set; }
    public PlayerForceMove(Vector2 targetPosition) => TargetPosition = targetPosition;
}