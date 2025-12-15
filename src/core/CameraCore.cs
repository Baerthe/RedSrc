namespace Core;

using Godot;
/// <summary>
/// The core camera class responsible for managing the game's camera behavior.
/// </summary>
public sealed partial class CameraCore : Camera2D
{
    public static CameraCore Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        GlobalPosition = Vector2.Zero;
        GD.PrintRich("[color=#000][bgcolor=#00ff00]CameraCore ready.[/bgcolor][/color]");
    }
    public void FocusOnPosition(Vector2 position)
    {
        GlobalPosition = position;
    }
    public void FocusOnNode(Node2D node)
    {
        if (node != null)
        {
            GlobalPosition = node.GlobalPosition;
        }
    }
}