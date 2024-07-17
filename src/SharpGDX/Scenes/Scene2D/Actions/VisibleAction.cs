using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Sets the actor's {@link Actor#setVisible(boolean) visibility}.
 * @author Nathan Sweet */
public class VisibleAction : Action {
	private bool visible;

	public override bool act (float delta) {
		target.setVisible(visible);
		return true;
	}

	public bool isVisible () {
		return visible;
	}

	public void setVisible (bool visible) {
		this.visible = visible;
	}
}
