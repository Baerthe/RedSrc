namespace Interface;

using Data;
/// <summary>
/// Interface for the HeroService; this Service handles hero data and management.
/// </summary>
public interface IHeroService
{
    HeroData CurrentHero { get; }
    void LoadHero(HeroData heroData);
    void UnloadHero();
}