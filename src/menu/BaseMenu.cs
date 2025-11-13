namespace Menu;

using Godot;
using Data;
using Interface;
/// <summary>
/// The base menu class provides common functionality for all menu types.
/// </summary>
public abstract partial class BaseMenu : Control, IMenu
{
    private IAudioService _audioService;
    private IEventService _eventService;
    public override void _EnterTree()
    {
        _audioService = CoreProvider.AudioService();
        _eventService = CoreProvider.EventService();
        base._EnterTree();
    }
}