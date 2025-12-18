namespace Core;

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
public sealed partial class MainCore : Node2D
{
	// Instance
	public static MainCore Instance { get; private set; }
	// Nodes
	[ExportGroup("Main Nodes")]
	[Export] public bool IsDebugMode { get; private set; } = false;
	[ExportSubgroup("Core")]
	[Export] public CameraCore CameraCore { get; private set; }
	[Export] public ContextCore ContextCore { get; private set; }
	[Export] public EventCore EventCore { get; private set; }
	[Export] public ServiceCore ServiceCore { get; private set; }
	[ExportGroup("Managers")]
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

	// Flags
	private bool _isGameStarted = false;
	// Engine Callbacks
	public override void _Ready()
	{
		Instance = this;
		NullCheck();
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Main node ready. Initializing game...[/bgcolor][/color]");
		// TODO: Move states to StateCore

		_eventService.Subscribe<StateEvent>(OnStateRequest);
		// TODO: Move initialization to ContextCore
		// TODO: Move manager state to StateCore
		GameManager.Initialize();
		// Start Game
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Game Initialized.[/bgcolor][/color]");
		_eventService.Publish<IndexEvent>(new IndexEvent(Heroes, EntityTemplates, Items, Levels, Weapons));
		if (IsDebugMode)
		{
			_eventService.Publish<DebugModeEvent>();
			//_eventService.Publish<StateEvent>(new StateEvent(State.Playing));
			GD.PrintRich("[color=#000][bgcolor=#ff0000]Debug Mode Enabled.[/bgcolor][/color]");
		}
		else
		{
			//_eventService.Publish<StateEvent>(new StateEvent(State.Menu));
		}
	}
	public override void _Process(double delta)
	{
	}
	/// <summary>
	/// Validates that all critical nodes are assigned in the editor. If any are missing, it logs an error and throws an exception to prevent the game from running in an invalid state.
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	private void NullCheck()
	{
		if (CameraCore == null) throw new InvalidOperationException("ERROR 201: CameraCore not set in Main. Game cannot load.");
		if (EventCore == null) throw new InvalidOperationException("ERROR 202: EventCore not set in Main. Game cannot load.");
		if (GameManager == null) throw new InvalidOperationException("ERROR 203: GameManager not set in Main. Game cannot load.");
		//if (MenuManager == null) throw new InvalidOperationException("ERROR 204: MenuManager not set in Main. Game cannot load.");
		if (UiManager == null) throw new InvalidOperationException("ERROR 204: UiManager not set in Main. Game cannot load.");
		GD.Print("We got all of our nodes! Checking Indices...");
		if (EntityTemplates == null) throw new InvalidOperationException("ERROR 205: EntityIndex not set in Main. Game cannot load.");
		if (Heroes == null) throw new InvalidOperationException("ERROR 206: HeroIndex not set in Main. Game cannot load.");
		if (Items == null) throw new InvalidOperationException("ERROR 207: ItemIndex not set in Main. Game cannot load.");
		if (Levels == null) throw new InvalidOperationException("ERROR 208: LevelIndex not set in Main. Game cannot load.");
		if (Weapons == null) throw new InvalidOperationException("ERROR 209: WeaponIndex not set in Main. Game cannot load.");
		GD.Print("We got all of our indices! NullCheck Complete");
	}
}
