namespace Event;

using Core.Service;
using Interface;
/// <summary>
/// Event class that delivers a request to change state.
/// </summary>
public sealed partial class StateEvent : IEvent
{
    public StateSelection RequestedState { get; private set; }
    public StateEvent(StateSelection state)
    {
        RequestedState = state;
    }
}
public sealed partial class MenuToggleEvent : IEvent {}