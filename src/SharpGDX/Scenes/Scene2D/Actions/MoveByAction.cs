using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Moves an actor to a relative position.
 * @author Nathan Sweet */
public class MoveByAction : RelativeTemporalAction {
	private float amountX, amountY;

	protected override void updateRelative (float percentDelta) {
		target.moveBy(amountX * percentDelta, amountY * percentDelta);
	}

	public void setAmount (float x, float y) {
		amountX = x;
		amountY = y;
	}

	public float getAmountX () {
		return amountX;
	}

	public void setAmountX (float x) {
		amountX = x;
	}

	public float getAmountY () {
		return amountY;
	}

	public void setAmountY (float y) {
		amountY = y;
	}
}
