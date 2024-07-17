using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Adds a listener to the actor for a specific event type and does not complete until {@link #handle(Event)} returns true.
 * @author JavadocMD
 * @author Nathan Sweet */
abstract public class EventAction<T> : Action 
where T: Event{
	readonly Type eventClass;
	bool result, active;

	private readonly IEventListener listener ;

	private class EventActionEventListener : IEventListener
	{
		private readonly EventAction<T> _eventAction;

		public EventActionEventListener(EventAction<T> eventAction)
		{
			_eventAction = eventAction;
		}

		public bool handle(Event @event)
		{
			if (!_eventAction.active || !ClassReflection.isInstance(_eventAction.eventClass, @event)) return false;
			_eventAction.result = _eventAction.handle((T)@event);
			return _eventAction.result;
		}
	}

	public EventAction (Type eventClass) {
		this.eventClass = eventClass;
		listener = new EventActionEventListener(this);
	}

	public override void restart () {
		result = false;
		active = false;
	}

	public override void setTarget (Actor newTarget) {
		if (target != null) target.removeListener(listener);
		base.setTarget(newTarget);
		if (newTarget != null) newTarget.addListener(listener);
	}

	/** Called when the specific type of event occurs on the actor.
	 * @return true if the event should be considered {@link Event#handle() handled} and this EventAction considered complete. */
	abstract public bool handle (T @event);

	public override bool act (float delta) {
		active = true;
		return result;
	}

	public bool isActive () {
		return active;
	}

	public void setActive (bool active) {
		this.active = active;
	}
}
