namespace Core;

using Godot;
using Interface;
using Service;
/// <summary>
/// Where the magic happens; builds our dependency injection containers for Core Services Injection. ServiceProvider is a global static class that allows any other class to access core services via a simple container.
/// In usage, call ServiceProvider.[ServiceName]Service() to get the singleton instance of the service; it can be helpful to register the service instance to a local readonly variable for easier access.
/// Example: private readonly IAudioService _audioService = ServiceProvider.AudioService();
/// </summary>
public sealed partial class ContextCore : Node2D
{
    public static ContextCore Instance { get; private set; }
    internal static Registry ServiceRegistry { get; private set; } = new();
    private static bool _isBuilt = false;
    public override void _Ready()
    {
        Instance = this;
        BuildServiceContainer();
    }
    public static IAudioService AudioService() => ServiceRegistry.Resolve<IAudioService>();
    public static IEventService EventService() => ServiceRegistry.Resolve<IEventService>();
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
        ServiceRegistry.Register<IEventService, EventService>();
        ServiceRegistry.Register<IHeroService, HeroService>();
        ServiceRegistry.Register<IPrefService, PrefService>();
        ServiceRegistry.Register<ILevelService, LevelService>();
        GD.PrintRich("[color=#00ff00]Services Registered.[/color]");
        _isBuilt = true;
    }
}