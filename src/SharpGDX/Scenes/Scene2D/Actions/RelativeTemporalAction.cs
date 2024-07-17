using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Base class for actions that transition over time using the percent complete since the last frame.
 * @author Nathan Sweet */
abstract public class RelativeTemporalAction : TemporalAction {
	private float lastPercent;

	protected override void begin () {
		lastPercent = 0;
	}

	protected override void update (float percent) {
		updateRelative(percent - lastPercent);
		lastPercent = percent;
	}

	abstract protected void updateRelative (float percentDelta);
}
