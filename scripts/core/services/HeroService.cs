namespace Core;

using Core.Interface;
using Entities;
using Godot;
/// <summary>
/// Service that manages hero loading and unloading.
/// </summary>
public sealed class HeroService : IHeroService
{
    public HeroData CurrentHero { get; private set; }
    public HeroService()
    {
        GD.PrintRich("[color=#00ff88]HeroService initialized.[/color]");
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
    public void LoadHero(HeroData heroData)
    {
        if (heroData == null)
        {
            GD.PrintErr("HeroService: LoadHero called with null heroData.");
            return;
        }
        CurrentHero = heroData;
    }
    public void UnloadHero()
    {
        if (CurrentHero != null)
        {
            CurrentHero = null;
            GD.Print("HeroService: Unloaded current hero.");
        }
        else
        {
            GD.PrintErr("HeroService: No hero is currently loaded to unload.");
        }
    }
}