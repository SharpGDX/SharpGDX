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

	public override bool Act (float delta) {
		var pool = GetPool();
		SetPool((Pool<Action>?)null); // Ensure this action can't be returned to the pool inside the delegate action.
		try {
			return @delegate(delta);
		} finally {
			SetPool(pool);
		}
	}

	public override void Restart () {
		if (action != null) action.Restart();
	}

	public override void Reset () {
		base.Reset();
		action = null;
	}

	public override void SetActor (Actor actor) {
		if (action != null) action.SetActor(actor);
		base.SetActor(actor);
	}

	public override void SetTarget (Actor target) {
		if (action != null) action.SetTarget(target);
		base.SetTarget(target);
	}

	public override String ToString() {
		return base.ToString() + (action == null ? "" : "(" + action + ")");
	}
}
