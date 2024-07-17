using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Sets the actor's scale from its current value to a specific value.
 * @author Nathan Sweet */
public class ScaleToAction : TemporalAction {
	private float startX, startY;
	private float endX, endY;

	protected override void begin () {
		startX = target.getScaleX();
		startY = target.getScaleY();
	}

	protected override void update (float percent) {
		float x, y;
		if (percent == 0) {
			x = startX;
			y = startY;
		} else if (percent == 1) {
			x = endX;
			y = endY;
		} else {
			x = startX + (endX - startX) * percent;
			y = startY + (endY - startY) * percent;
		}
		target.setScale(x, y);
	}

	public void setScale (float x, float y) {
		endX = x;
		endY = y;
	}

	public void setScale (float scale) {
		endX = scale;
		endY = scale;
	}

	public float getX () {
		return endX;
	}

	public void setX (float x) {
		this.endX = x;
	}

	public float getY () {
		return endY;
	}

	public void setY (float y) {
		this.endY = y;
	}
}
