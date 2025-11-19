using Godot;
using Interface;
using Service;
/// <summary>
/// Where the magic happens; builds our dependency injection containers for Core Services Injection. ServiceProvider is a global static class that allows any other class to access core services via a simple container.
/// In usage, call ServiceProvider.[ServiceName]Service() to get the singleton instance of the service; it can be helpful to register the service instance to a local readonly variable for easier access.
/// Example: private readonly IAudioService _audioService = ServiceProvider.AudioService();
/// </summary>
public static class ServiceProvider
{
    internal static ServiceContainer ServiceContainer { get; private set; } = new();
    private static bool _isBuilt = false;
    static ServiceProvider()
    {
        BuildServiceContainer();
    }
    public static IAudioService AudioService() => ServiceContainer.Resolve<IAudioService>();
    public static IEventService EventService() => ServiceContainer.Resolve<IEventService>();
    public static IHeroService HeroService() => ServiceContainer.Resolve<IHeroService>();
    public static IPrefService PrefService() => ServiceContainer.Resolve<IPrefService>();
    public static ILevelService LevelService() => ServiceContainer.Resolve<ILevelService>();
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
        ServiceContainer.Register<IAudioService, AudioService>();
        ServiceContainer.Register<IEventService, EventService>();
        ServiceContainer.Register<IHeroService, HeroService>();
        ServiceContainer.Register<IPrefService, PrefService>();
        ServiceContainer.Register<ILevelService, LevelService>();
        GD.PrintRich("[color=#00ff00]Services Registered.[/color]");
        _isBuilt = true;
    }
}