using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Multiplies the delta of an action.
 * @author Nathan Sweet */
public class TimeScaleAction : DelegateAction {
	private float scale;

	protected override bool @delegate (float delta) {
		if (action == null) return true;
		return action.act(delta * scale);
	}

	public float getScale () {
		return scale;
	}

	public void setScale (float scale) {
		this.scale = scale;
	}
}
