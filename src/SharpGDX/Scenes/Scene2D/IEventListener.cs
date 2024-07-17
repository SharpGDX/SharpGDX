using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D
{
	/** Low level interface for receiving events. Typically there is a listener class for each specific event class.
 * @see InputListener
 * @see InputEvent
 * @author Nathan Sweet */
	public interface IEventListener
	{
		/** Try to handle the given event, if it is applicable.
		 * @return true if the event should be considered {@link Event#handle() handled} by scene2d. */
		public bool handle(Event @event);
	}
	}
