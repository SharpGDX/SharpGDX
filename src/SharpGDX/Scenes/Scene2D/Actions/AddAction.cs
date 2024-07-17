using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Adds an action to an actor.
 * @author Nathan Sweet */
public class AddAction : Action {
	private Action action;

	public override bool act (float delta) {
		target.addAction(action);
		return true;
	}

	public Action getAction () {
		return action;
	}

	public void setAction (Action action) {
		this.action = action;
	}

	public override void restart () {
		if (action != null) action.restart();
	}

	public override void reset () {
		base.reset();
		action = null;
	}
}
