namespace Core;

using Godot;
using Interface;
using System;
using System.Collections.Generic;
/// <summary>
/// The Heart for the EventCore, a heart is a bundle of timers that sync into the EvenCore to publish events on their timeouts. This allows for easy access to global timers through the EventCore.
/// </summary>
public partial class Heart : Node2D
{
	private Dictionary<string, Timer> _timers = new();
	private EventCore _eventCore;
	public override void _Ready()
    {
        _eventCore = GetParent<EventCore>();
    }
	/// <summary>
	/// Pauses all timers. Used when pausing the game.
	/// </summary>
	public void PauseTimers()
	{
		foreach (var timer in _timers.Values)
		{
			timer.Paused = true;
		}
	}
	/// <summary>
	/// Resumes all timers. Used when unpausing the game.
	/// </summary>
	public void ResumeTimers()
	{
		foreach (var timer in _timers.Values)
		{
			timer.Paused = false;
		}
	}
	/// <summary>
	/// Starts all timers. Used when initializing the game.
	/// </summary>
	public void StartTimers()
	{
		foreach (var timer in _timers.Values)
		{
			if (timer.GetParent() == null)
			{
				AddChild(timer);
			}
			timer.Start();
		}
	}
	/// <summary>
	/// Stops all timers. Used when resetting the game.
	/// </summary>
	public void StopTimers()
	{
		foreach (var timer in _timers.Values)
		{
			timer.Stop();
		}
	}
	/// <summary>
	/// Removes a timer from the Heart by name (string).
	/// </summary>	/// <param name="name"></param>
	public void RemoveTimer(string name)
	{
		if (_timers.ContainsKey(name))
		{
			var timer = _timers[name];
			_timers.Remove(name);
			timer.QueueFree();
			GD.PrintRich($"[color=#ff0044]Heart: Removed Timer {name}.[/color]");
		}
		else
		{
			GD.PrintErr($"Heart: Timer {name} does not exist, thus cannot remove.");
		}
	}
	/// <summary>
	/// Builds a Timer with the specified parameters and attaches the onTimeout action to its Timeout signal.
	/// </summary>
	/// <param name="waitTime"></param>
	/// <param name="oneShot"></param>
	/// <param name="autostart"></param>
	/// <param name="onTimeout"></param>
	/// <param name="sender"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Thrown if the Timer fails to initialize. Lists which createTimer method was called.</exception>
	public void BuildTimer<T>(string name, float waitTime, bool oneShot, bool autostart) where T : IEvent
	{
		if (_timers.ContainsKey(name))
		{
			GD.PrintErr($"Timer with name {name} already exists in Heart.");
			throw new InvalidOperationException($"ERROR: Timer with name {name} already exists in Heart.");
		}
		Timer timer = new Timer
		{
			WaitTime = waitTime,
			OneShot = oneShot,
			Autostart = autostart
		};
		if (timer == null)
		{
			GD.PrintErr("Timer is null after creation!");
			throw new InvalidOperationException($"ERROR: Timer failed to initialize in Heart.");
		}
		_timers.Add(name, timer);
		timer.Name = name;
		timer.Timeout += _eventCore.Publish<IEvent>;
		AddChild(timer);
		GD.PrintRich($"[color=#00ff88]Heart: Built Timer {name} with wait time {waitTime}s / {1.0f / waitTime} hrz.[/color]");	}
}
