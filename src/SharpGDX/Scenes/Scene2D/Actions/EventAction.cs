using SharpGDX.Utils.Reflect;

namespace SharpGDX.Scenes.Scene2D.Actions;

/// <summary>
///     Adds a listener to the actor for a specific event type and does not complete until <see cref="Handle(T)" /> returns
///     true.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EventAction<T> : Action
    where T : Event
{
    private readonly Type _eventClass;
    private readonly IEventListener _listener;
    private bool _active;

    private bool _result;

    public EventAction(Type eventClass)
    {
        _eventClass = eventClass;
        _listener = new EventActionEventListener(this);
    }

    public override void Restart()
    {
        _result = false;
        _active = false;
    }

    public override void SetTarget(Actor? newTarget)
    {
        Target?.removeListener(_listener);
        base.SetTarget(newTarget);
        newTarget?.addListener(_listener);
    }

    /// <summary>
    ///     Called when the specific type of event occurs on the actor.
    /// </summary>
    /// <param name="event"></param>
    /// <returns>
    ///     true if the event should be considered {@link Event#handle() handled} and this EventAction considered
    ///     complete.
    /// </returns>
    public abstract bool Handle(T @event);

    public override bool Act(float delta)
    {
        _active = true;
        return _result;
    }

    public bool IsActive()
    {
        return _active;
    }

    public void SetActive(bool active)
    {
        _active = active;
    }

    private class EventActionEventListener(EventAction<T> eventAction) : IEventListener
    {
        public bool Handle(Event @event)
        {
            if (!eventAction._active || !ClassReflection.isInstance(eventAction._eventClass, @event)) return false;

            eventAction._result = eventAction.Handle((T)@event);

            return eventAction._result;
        }
    }
}