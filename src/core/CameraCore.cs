namespace Core;

using Godot;
/// <summary>
/// The core camera class responsible for managing the game's camera behavior.
/// </summary>
public sealed partial class CameraCore : Camera2D
{
    public static CameraCore Instance { get; private set; }
    private double _delta;
    public override void _Ready()
    {
        Instance = this;
        GlobalPosition = Vector2.Zero;
        GD.PrintRich("[color=#000][bgcolor=#00ff00]CameraCore ready.[/bgcolor][/color]");
    }
    public override void _Process(double delta)
    {
        _delta = delta;
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
    public void MoveCamera(Vector2 direction, float speed)
    {
        GlobalPosition += direction * speed * (float)_delta;
    }
    public void MoveOnPosition(Vector2 targetPosition, float speed)
    {
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();
        MoveCamera(direction, speed);
    }
    public void MoveOnNode(Node2D targetNode, float speed)
    {
        if (targetNode != null)
        {
            MoveOnPosition(targetNode.GlobalPosition, speed);
        }
    }
    public void ResetCameraPosition()
    {
        GlobalPosition = Vector2.Zero;
    }
}