namespace Core;

using Godot;
using Interface;
using Service;
/// <summary>
/// The core service class responsible for managing service registrations and providing access to core services.
/// </summary>
public sealed partial class ServiceCore : Node2D
{
    internal static Registry ServiceRegistry { get; private set; } = new();
    private static bool _isBuilt = false;
    public override void _Ready()
    {
        BuildServiceContainer();
    }
    public static IAudioService AudioService() => ServiceRegistry.Resolve<IAudioService>();
    public static IHeroService HeroService() => ServiceRegistry.Resolve<IHeroService>();
    public static IPrefService PrefService() => ServiceRegistry.Resolve<IPrefService>();
    public static ILevelService LevelService() => ServiceRegistry.Resolve<ILevelService>();
    /// <summary>
    /// Builds the service container with all service singletons.
    /// </summary>
    /// <remarks>
    /// The service container contains singletons that are essential for the application's core functionality.
    /// </remarks>
    private static void BuildServiceContainer()
    {
        if (_isBuilt)
            return;
        GD.PrintRich("[color=#00ff00]Registering Services to ServiceContainer...[/color]");
        ServiceRegistry.Register<IAudioService, AudioService>();
        ServiceRegistry.Register<IHeroService, HeroService>();
        ServiceRegistry.Register<IPrefService, PrefService>();
        ServiceRegistry.Register<ILevelService, LevelService>();
        ServiceRegistry.Register<IStateService, StateService>();
        GD.PrintRich("[color=#00ff00]Services Registered.[/color]");
        _isBuilt = true;
    }
}