namespace Core;

using Core.Interface;
/// <summary>
/// Event class that delivers a request to change state.
/// </summary>
public sealed partial class StateEvent : IEvent
{
    public State RequestedState { get; private set; }
    public StateEvent(State state)
    {
        RequestedState = state;
    }
}