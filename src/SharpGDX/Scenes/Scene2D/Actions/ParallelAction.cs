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

	public override bool Act (float delta) {
		if (complete) return true;
		complete = true;
		Pool<Action>? pool = GetPool();
		SetPool((Pool<Action>?)null); // Ensure this action can't be returned to the pool while executing.
		try {
			Array<Action> actions = this.actions;
			for (int i = 0, n = actions.size; i < n && Actor != null; i++) {
				Action currentAction = actions.Get(i);
				if (currentAction.GetActor() != null && !currentAction.Act(delta)) complete = false;
				if (Actor == null) return true; // This action was removed.
			}
			return complete;
		} finally {
			SetPool(pool);
		}
	}

	public override void Restart () {
		complete = false;
		Array<Action> actions = this.actions;
		for (int i = 0, n = actions.size; i < n; i++)
			actions.Get(i).Restart();
	}

	public override void Reset () {
		base.Reset();
		actions.clear();
	}

	public void addAction (Action action) {
		actions.Add(action);
		if (Actor != null) action.SetActor(Actor);
	}

	public override void SetActor (Actor actor) {
		Array<Action> actions = this.actions;
		for (int i = 0, n = actions.size; i < n; i++)
			actions.Get(i).SetActor(actor);
		base.SetActor(actor);
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
			buffer.Append(actions.Get(i));
		}
		buffer.Append(')');
		return buffer.ToString();
	}
}
