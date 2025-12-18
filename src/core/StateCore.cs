namespace Core;

using Godot;
using Event;
using Interface;
using System;
using System.Collections.Generic;
/// <summary>
/// The core class that handles state management of the game.
/// </summary>
public sealed partial class StateCore : Node2D
{
    public State CurrentState { get; private set; } = State.Menu;
    public Manager CurrentManager { get; private set; } = Manager.Menu;
	private Dictionary<State, Action> _stateActions;
    public override void _Ready()
    {
        _stateActions = new Dictionary<State, Action>
        {
            { State.Menu, CallMenuState },
            { State.LevelSelect, CallLevelSelectState },
            { State.Paused, CallPausedState },
            { State.Playing, CallPlayingState },
            { State.GameOver, CallGameOverState }
        };
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
		//MenuManager.Show();
		UiManager.Hide();
		if (CurrentState == State.GameOver || _isGameStarted)
		{
			GameManager.UnloadLevel();
			_isGameStarted = false;
		}
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
		//MenuManager.Hide();
		UiManager.Show();
		if (!_isGameStarted)
		{
			_isGameStarted = true;
		}
	}
	private void CallGameOverState()
	{
		//
	}
}
public enum State : byte
{
    Menu, LevelSelect, Paused, Playing, GameOver
}
public enum Manager : byte
{
    Game, Menu
}
