namespace Utility;

using Data;
using Entity;
using Event;
using Godot;
using Interface;
using System;
using System.Collections;
/// <summary>
/// System that handles experience point (XP) events and processing.
/// </summary>
public sealed partial class XPUtility : Node2D, IUtility
{
	public bool IsInitialized { get; private set; }
	private Queue _xpQueue = new Queue();
	private IAudioService _audioService;
	private IEventService _eventService;
	public XPUtility(IAudioService audioService, IEventService eventService)
	{
		GD.Print("XPSystem: Initializing...");
		_audioService = audioService;
		_eventService = eventService;
	}
	public override void _Ready()
	{
		_eventService.Subscribe<Init>(OnInit);
		_eventService.Subscribe<XPEvent>(OnXPEvent);
		GD.Print("XPSystem Ready.");
	}
	public override void _ExitTree()
	{
		_eventService.Unsubscribe<Init>(OnInit);
		_eventService.Unsubscribe<XPEvent>(OnXPEvent);
	}
	public void OnInit()
	{
		IsInitialized = true;
	}
	public void Update()
	{
		uint xpCount = 0;
		while (_xpQueue.Count > 0)
		{
			RarityType rarity = (RarityType)_xpQueue.Dequeue();
			xpCount += (byte)rarity + (uint)2;
		}
		_eventService.Publish<PlayerGainedXP>(new PlayerGainedXP(xpCount));
	}
	// Right now just auto enqueues XP from XPEvents.
	// TODO: create from template for xp orbs.
	private void OnXPEvent(IEvent xpEvent)
	{
		var eventData = xpEvent as XPEvent;
		_xpQueue.Enqueue(eventData.Rarity);
	}
}
