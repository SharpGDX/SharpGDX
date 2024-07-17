using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Removes an action from an actor.
 * @author Nathan Sweet */
public class RemoveAction : Action {
	private Action action;

	public override bool act (float delta) {
		target.removeAction(action);
		return true;
	}

	public Action getAction () {
		return action;
	}

	public void setAction (Action action) {
		this.action = action;
	}

	public override void reset () {
		base.reset();
		action = null;
	}
}
