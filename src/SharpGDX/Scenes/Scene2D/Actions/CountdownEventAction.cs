using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** An EventAction that is complete once it receives X number of events.
 * @author JavadocMD
 * @author Nathan Sweet */
public class CountdownEventAction<T> : EventAction<T> 
where T: Event{
	int count, current;

	public CountdownEventAction (Type eventClass, int count) 
	: base(eventClass)
	{
		
		this.count = count;
	}

	public override bool handle (T @event) {
		current++;
		return current >= count;
	}
}
