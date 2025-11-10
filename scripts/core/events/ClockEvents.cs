namespace Core;

using Core.Interface;
/// Non-data events for Clock System.
public sealed class PulseTimeout : IEvent;
public sealed class SlowPulseTimeout : IEvent;
public sealed class MobSpawnTimeout : IEvent;
public sealed class ChestSpawnTimeout : IEvent;
public sealed class GameTimeout : IEvent;
public sealed class StartingTimeout : IEvent;