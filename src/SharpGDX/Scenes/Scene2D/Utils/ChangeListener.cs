using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Listener for {@link ChangeEvent}.
 * @author Nathan Sweet */
	abstract public class ChangeListener : IEventListener
	{
	public bool handle(Event @event)
	{
		if (!(@event is ChangeEvent)) return false;
		changed((ChangeEvent)@event, @event.getTarget());
		return false;
	}

	/** @param actor The event target, which is the actor that emitted the change event. */
	abstract public void changed(ChangeEvent @event, Actor actor);

	/** Fired when something in an actor has changed. This is a generic event, exactly what changed in an actor will vary.
	 * @author Nathan Sweet */
	public class ChangeEvent : Event
	{
	}
	}
}
