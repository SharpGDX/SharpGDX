using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Moves an actor from its current size to a specific size.
 * @author Nathan Sweet */
public class SizeToAction : TemporalAction {
	private float startWidth, startHeight;
	private float endWidth, endHeight;

	protected override void begin () {
		startWidth = target.getWidth();
		startHeight = target.getHeight();
	}

	protected override void update (float percent) {
		float width, height;
		if (percent == 0) {
			width = startWidth;
			height = startHeight;
		} else if (percent == 1) {
			width = endWidth;
			height = endHeight;
		} else {
			width = startWidth + (endWidth - startWidth) * percent;
			height = startHeight + (endHeight - startHeight) * percent;
		}
		target.setSize(width, height);
	}

	public void setSize (float width, float height) {
		endWidth = width;
		endHeight = height;
	}

	public float getWidth () {
		return endWidth;
	}

	public void setWidth (float width) {
		endWidth = width;
	}

	public float getHeight () {
		return endHeight;
	}

	public void setHeight (float height) {
		endHeight = height;
	}
}
