namespace Core.Service;

using Godot;
using Core;
using Event;
using Interface;
using System;
using System.Collections.Generic;
/// <summary>
/// The core class that handles state management of the game.
/// </summary>
public sealed class StateService : IStateService
{
    public StateSelection CurrentState { get; private set; } = StateSelection.Menu;
    public ManagerSelection CurrentManager { get; private set; } = ManagerSelection.Menu;
	private Dictionary<StateSelection, Action> _stateActions;
    private EventCore _eventCore = MainCore.Instance.EventCore;
    public StateService()
    {
        _stateActions = new Dictionary<StateSelection, Action>
        {
            { StateSelection.Menu, CallMenuState },
            { StateSelection.LevelSelect, CallLevelSelectState },
            { StateSelection.Paused, CallPausedState },
            { StateSelection.Playing, CallPlayingState },
            { StateSelection.GameOver, CallGameOverState }
        };
		_eventCore.Subscribe<StateEvent>(OnStateRequest);
    }
	private void OnStateRequest(IEvent eventData)
	{
		var stateData = (StateEvent)eventData;
		if (stateData.RequestedState == CurrentState) return;
		Action action = _stateActions.ContainsKey(stateData.RequestedState) ? _stateActions[stateData.RequestedState] : null;
		CurrentState = stateData.RequestedState;
		if (action != null) action();
	}
	private void CallMenuState()
	{
		_eventCore.Publish<MenuToggleEvent>();
	}
	private void CallLevelSelectState()
	{
		// Level Select State Logic Here
	}
	private void CallPausedState()
	{
		// Paused State Logic Here
	}
	private void CallPlayingState()
	{

	}
	private void CallGameOverState()
	{
		//
	}
}
public enum StateSelection : byte
{
    Menu, LevelSelect, Paused, Playing, GameOver
}
public enum ManagerSelection : byte
{
    Game, Menu
}
