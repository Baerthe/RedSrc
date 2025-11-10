namespace Core.Interface;

using System;
/// <summary>
/// Interface for the EventService which manages event publishing and subscribing within the game.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Subscribes to an event of type IEvent. The handler can accept an IEvent parameter to pass data if desired.
    /// </summary>
    /// <typeparam name="IEvent"></typeparam>
    /// <param name="handler"></param>
    void Subscribe<T>(Action<IEvent> handler);
    void Subscribe<T>(Action handler);
    /// <summary>
    /// Unsubscribes from an event of type IEvent.
    /// </summary>
    /// <typeparam name="IEvent"></typeparam>
    /// <param name="handler"></param>
    void Unsubscribe<T>(Action<IEvent> handler);
    void Unsubscribe<T>(Action handler);
    /// <summary>
    /// Unsubscribes all event handlers from all events.
    /// </summary>
    void UnsubscribeAll();
    /// <summary>
    /// Publishes an event of type T to all subscribed handlers. The eventData parameter can carry data associated with the event.
    /// </summary>
    /// <typeparam name="IEvent"></typeparam>
    /// <param name="eventData"></param>
    void Publish<T>(IEvent eventData);
    void Publish<T>();
}