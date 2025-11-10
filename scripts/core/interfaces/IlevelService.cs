namespace Core.Interface;

using Entities;
using Godot;
/// <summary>
/// Interface for the LevelService; this Service handles level transitions and loading.
/// </summary>
public interface ILevelService
{
    LevelData CurrentLevel { get; }
    void LoadLevel(LevelData levelData);
    void UnloadLevel();
}