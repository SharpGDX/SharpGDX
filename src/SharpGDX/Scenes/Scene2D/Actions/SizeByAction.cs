using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Moves an actor from its current size to a relative size.
 * @author Nathan Sweet */
public class SizeByAction : RelativeTemporalAction {
	private float amountWidth, amountHeight;

	protected override void updateRelative (float percentDelta) {
		target.sizeBy(amountWidth * percentDelta, amountHeight * percentDelta);
	}

	public void setAmount (float width, float height) {
		amountWidth = width;
		amountHeight = height;
	}

	public float getAmountWidth () {
		return amountWidth;
	}

	public void setAmountWidth (float width) {
		amountWidth = width;
	}

	public float getAmountHeight () {
		return amountHeight;
	}

	public void setAmountHeight (float height) {
		amountHeight = height;
	}
}
