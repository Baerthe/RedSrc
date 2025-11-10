namespace Entities;

using Godot;
/// <summary>
/// Metadata is a Resource that holds common data attributes shared across various entities in the game.
/// </summary>
public partial class Metadata : Resource
{
    [Export] public Texture2D Icon { get; private set; }
    [Export] public RarityType Rarity { get; private set; } = RarityType.Common;
    [Export] public bool Unlocked { get; private set; } = false;
    public void Unlock() => Unlocked = true;
}