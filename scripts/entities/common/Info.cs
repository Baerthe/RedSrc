namespace Entities;

using Godot;
/// <summary>
/// Info is a Resource that holds common information attributes shared across various entities in the game.
/// </summary>
public partial class Info : Resource
{
    [Export] public string Name { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public string Lore { get; private set; } = "";
}