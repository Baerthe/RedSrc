using Data;
using Event;
using Interface;
using Manager;
using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// The main class that handles orchestration and dependency management of the game.
/// </summary>
public partial class Main : Node2D
{
	[ExportGroup("Main Nodes")]
	[ExportSubgroup("Core")]
	[Export] public Camera2D MainCamera { get; private set; }
	[Export] public GameManager GameManager { get; private set; }
	//	[Export] public MenuManager MenuManager { get; private set; }
	[Export] public UiManager UiManager { get; private set; }
	[ExportGroup("Indices")]
	[Export] public EntityIndex EntityTemplates { get; private set; }
	[Export] public HeroIndex Heroes { get; private set; }
	[Export] public ItemIndex Items { get; private set; }
	[Export] public LevelIndex Levels { get; private set; }
	[Export] public WeaponIndex Weapons { get; private set; }
	// State
	public State CurrentState { get; private set; } = State.Menu;
	private Dictionary<State, Action> _stateActions;
	// Services
	private IEventService _eventService;
	private ILevelService _levelService;
	// Flags
	private bool _isGameStarted = false;
	// Engine Callbacks
	public override void _Ready()
	{
		NullCheck();
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Main node ready. Initializing game...[/bgcolor][/color]");
		_eventService = ServiceProvider.EventService();
		_levelService = ServiceProvider.LevelService();
		_stateActions = new Dictionary<State, Action>
		{
			{ State.Menu, CallMenuState },
			{ State.LevelSelect, CallLevelSelectState },
			{ State.Paused, CallPausedState },
			{ State.Playing, CallPlayingState },
			{ State.GameOver, CallGameOverState }
		};
		_eventService.Subscribe<StateEvent>(OnStateRequest);
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Game Initialized.[/bgcolor][/color]");
		_eventService.Publish<IndexEvent>(new IndexEvent(Heroes, EntityTemplates, Items, Levels, Weapons));
	}
	/// <summary>
	/// Validates that all critical nodes are assigned in the editor. If any are missing, it logs an error and throws an exception to prevent the game from running in an invalid state.
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	private void NullCheck()
	{
		if (UiManager == null) throw new InvalidOperationException("ERROR 201: HUD node not set in Main. Game cannot load.");
		if (MainCamera == null) throw new InvalidOperationException("ERROR 202: Camera node not set in Main. Game cannot load.");
		//if (MenuManager == null) throw new InvalidOperationException("ERROR 203: Menu node not set in Main. Game cannot load.");
		if (GameManager == null) throw new InvalidOperationException("ERROR 204: Game node not set in Main. Game cannot load.");
		GD.Print("We got all of our nodes! Checking Indices...");
		if (EntityTemplates == null) throw new InvalidOperationException("ERROR 205: EntityIndex not set in Main. Game cannot load.");
		if (Heroes == null) throw new InvalidOperationException("ERROR 206: HeroIndex not set in Main. Game cannot load.");
		if (Items == null) throw new InvalidOperationException("ERROR 207: ItemIndex not set in Main. Game cannot load.");
		if (Levels == null) throw new InvalidOperationException("ERROR 208: LevelIndex not set in Main. Game cannot load.");
		if (Weapons == null) throw new InvalidOperationException("ERROR 209: WeaponIndex not set in Main. Game cannot load.");
		GD.Print("We got all of our indices! NullCheck Complete");
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
