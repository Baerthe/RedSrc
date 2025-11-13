namespace Menu;

using Godot;
/// <summary>
/// The menu class handles the main menu UI and interactions.
/// </summary>
[GlobalClass]
public partial class MenuManager : BaseMenu
{
	[Signal] public delegate void StartGameEventHandler();
	[Export] private Button _startButton;
	[Export] private Button _quitButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startButton.Pressed += () => EmitSignal(SignalName.StartGame);
		_quitButton.Pressed += () => GetTree().Quit();
	}
}
