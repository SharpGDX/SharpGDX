using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Executes a number of actions one at a time.
 * @author Nathan Sweet */
public class SequenceAction : ParallelAction {
	private int index;

	public SequenceAction () {
	}

	public SequenceAction (Action action1) {
		addAction(action1);
	}

	public SequenceAction (Action action1, Action action2) {
		addAction(action1);
		addAction(action2);
	}

	public SequenceAction (Action action1, Action action2, Action action3) {
		addAction(action1);
		addAction(action2);
		addAction(action3);
	}

	public SequenceAction (Action action1, Action action2, Action action3, Action action4) {
		addAction(action1);
		addAction(action2);
		addAction(action3);
		addAction(action4);
	}

	public SequenceAction (Action action1, Action action2, Action action3, Action action4, Action action5) {
		addAction(action1);
		addAction(action2);
		addAction(action3);
		addAction(action4);
		addAction(action5);
	}

	public override bool Act (float delta) {
		if (index >= actions.size) return true;
		var pool = GetPool();
		SetPool((Pool<Action>?)null); // Ensure this action can't be returned to the pool while executings.
		try {
			if (actions.Get(index).Act(delta)) {
				if (Actor == null) return true; // This action was removed.
				index++;
				if (index >= actions.size) return true;
			}
			return false;
		} finally {
			SetPool(pool);
		}
	}

	public override void Restart () {
		base.Restart();
		index = 0;
	}
}
