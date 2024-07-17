using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Base class for an action that wraps another action.
 * @author Nathan Sweet */
abstract public class DelegateAction : Action {
	protected Action? action;

	/** Sets the wrapped action. */
	public void setAction (Action? action) {
		this.action = action;
	}

	public Action? getAction () {
		return action;
	}

	abstract protected bool @delegate (float delta);

	public override bool act (float delta) {
		var pool = getPool();
		setPool((Pool<Action>?)null); // Ensure this action can't be returned to the pool inside the delegate action.
		try {
			return @delegate(delta);
		} finally {
			setPool(pool);
		}
	}

	public override void restart () {
		if (action != null) action.restart();
	}

	public override void reset () {
		base.reset();
		action = null;
	}

	public override void setActor (Actor actor) {
		if (action != null) action.setActor(actor);
		base.setActor(actor);
	}

	public override void setTarget (Actor target) {
		if (action != null) action.setTarget(target);
		base.setTarget(target);
	}

	public override String ToString() {
		return base.ToString() + (action == null ? "" : "(" + action + ")");
	}
}
