using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Moves an actor from its current position to a specific position.
 * @author Nathan Sweet */
public class MoveToAction : TemporalAction {
	private float startX, startY;
	private float endX, endY;
	private int alignment = Align.bottomLeft;

	protected override void begin () {
		startX = target.getX(alignment);
		startY = target.getY(alignment);
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
		target.setPosition(x, y, alignment);
	}

	public override void reset () {
		base.reset();
		alignment = Align.bottomLeft;
	}

	public void setStartPosition (float x, float y) {
		startX = x;
		startY = y;
	}

	public void setPosition (float x, float y) {
		endX = x;
		endY = y;
	}

	public void setPosition (float x, float y, int alignment) {
		endX = x;
		endY = y;
		this.alignment = alignment;
	}

	public float getX () {
		return endX;
	}

	public void setX (float x) {
		endX = x;
	}

	public float getY () {
		return endY;
	}

	public void setY (float y) {
		endY = y;
	}

	/** Gets the starting X value, set in {@link #begin()}. */
	public float getStartX () {
		return startX;
	}

	/** Gets the starting Y value, set in {@link #begin()}. */
	public float getStartY () {
		return startY;
	}

	public int getAlignment () {
		return alignment;
	}

	public void setAlignment (int alignment) {
		this.alignment = alignment;
	}
}
