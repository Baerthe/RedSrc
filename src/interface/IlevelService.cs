namespace Interface;

using Data;
using Entity;
using Godot;
/// <summary>
/// Interface for the LevelService; this Service handles level transitions and loading.
/// </summary>
public interface ILevelService : IService
{
    LevelData CurrentLevel { get; }
    void LoadLevel(LevelData levelData);
    void UnloadLevel();
}