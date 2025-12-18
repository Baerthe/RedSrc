namespace Event;

using Interface;
/// Non-data events for heartbeats and timeouts
public sealed class PulseTimeout : IEvent;
public sealed class SlowPulseTimeout : IEvent;
public sealed class MobSpawnTimeout : IEvent;
public sealed class ChestSpawnTimeout : IEvent;
public sealed class GameTimeout : IEvent;
public sealed class StartingTimeout : IEvent;