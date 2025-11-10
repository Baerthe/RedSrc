namespace Core;

using Core.Interface;
using Entities;
using Godot;
/// <summary>
/// Service that manages level loading and unloading.
/// </summary>
public sealed class LevelService : ILevelService
{
    public LevelData CurrentLevel { get; private set; }
    public string LevelName { get; private set; }
    public LevelService()
    {
        GD.PrintRich("[color=#00ff88]LevelService initialized.[/color]");
    }
    /// <summary>
    /// Loads a level from a PackedScene and adds it to the specified parent node.
    /// </summary>
    /// <param name="levelScene">The PackedScene of the level to load.</param>
    /// <param name="parentNode">The parent node to which the level instance will be added. If null, uses the existing ParentNode.</param>
    /// <remarks>
    /// If ParentNode is already set and a different parentNode is provided, an error is logged and the method returns without making changes.
    /// If LevelInstance already exists, it is freed before loading the new level.
    /// </remarks>
    public void LoadLevel(LevelData levelData)
    {
        if (levelData == null)
        {
            GD.PrintErr("LevelService: LoadLevel called with null levelData.");
            return;
        }
        CurrentLevel = levelData;
        LevelName = levelData.Info.Name;
    }
    public void UnloadLevel()
    {
        if (CurrentLevel != null)
        {
            CurrentLevel = null;
            LevelName = null;
            GD.Print("LevelService: Unloaded current level.");
        }
        else
        {
            GD.PrintErr("LevelService: No level is currently loaded to unload.");
        }
    }
}