using SharpGDX;
using System.Text;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Executes a number of actions at the same time.
 * @author Nathan Sweet */
public class ParallelAction : Action {
	protected Array<Action> actions = new (4);
	private bool complete;

	public ParallelAction () {
	}

	public ParallelAction (Action action1) {
		addAction(action1);
	}

	public ParallelAction (Action action1, Action action2) {
		addAction(action1);
		addAction(action2);
	}

	public ParallelAction (Action action1, Action action2, Action action3) {
		addAction(action1);
		addAction(action2);
		addAction(action3);
	}

	public ParallelAction (Action action1, Action action2, Action action3, Action action4) {
		addAction(action1);
		addAction(action2);
		addAction(action3);
		addAction(action4);
	}

	public ParallelAction (Action action1, Action action2, Action action3, Action action4, Action action5) {
		addAction(action1);
		addAction(action2);
		addAction(action3);
		addAction(action4);
		addAction(action5);
	}

	public override bool act (float delta) {
		if (complete) return true;
		complete = true;
		Pool<Action>? pool = getPool();
		setPool((Pool<Action>?)null); // Ensure this action can't be returned to the pool while executing.
		try {
			Array<Action> actions = this.actions;
			for (int i = 0, n = actions.size; i < n && actor != null; i++) {
				Action currentAction = actions.get(i);
				if (currentAction.getActor() != null && !currentAction.act(delta)) complete = false;
				if (actor == null) return true; // This action was removed.
			}
			return complete;
		} finally {
			setPool(pool);
		}
	}

	public override void restart () {
		complete = false;
		Array<Action> actions = this.actions;
		for (int i = 0, n = actions.size; i < n; i++)
			actions.get(i).restart();
	}

	public override void reset () {
		base.reset();
		actions.clear();
	}

	public void addAction (Action action) {
		actions.add(action);
		if (actor != null) action.setActor(actor);
	}

	public override void setActor (Actor actor) {
		Array<Action> actions = this.actions;
		for (int i = 0, n = actions.size; i < n; i++)
			actions.get(i).setActor(actor);
		base.setActor(actor);
	}

	public Array<Action> getActions () {
		return actions;
	}

	public override String ToString () {
		StringBuilder buffer = new StringBuilder(64);
		buffer.Append(base.ToString());
		buffer.Append('(');
		Array<Action> actions = this.actions;
		for (int i = 0, n = actions.size; i < n; i++) {
			if (i > 0) buffer.Append(", ");
			buffer.Append(actions.get(i));
		}
		buffer.Append(')');
		return buffer.ToString();
	}
}
