using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Sets the actor's {@link Actor#setTouchable(Touchable) touchability}.
 * @author Nathan Sweet */
public class TouchableAction : Action {
	private Touchable touchable;

	public override bool act (float delta) {
		target.setTouchable(touchable);
		return true;
	}

	public Touchable getTouchable () {
		return touchable;
	}

	public void setTouchable (Touchable touchable) {
		this.touchable = touchable;
	}
}
