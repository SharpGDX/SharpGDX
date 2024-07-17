using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Sets the actor's rotation from its current value to a relative value.
 * @author Nathan Sweet */
public class RotateByAction : RelativeTemporalAction {
	private float amount;

	protected override void updateRelative (float percentDelta) {
		target.rotateBy(amount * percentDelta);
	}

	public float getAmount () {
		return amount;
	}

	public void setAmount (float rotationAmount) {
		amount = rotationAmount;
	}
}
