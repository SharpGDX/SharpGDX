using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Repeats an action a number of times or forever.
 * @author Nathan Sweet */
public class RepeatAction : DelegateAction {
	static public readonly int FOREVER = -1;

	private int repeatCount, executedCount;
	private bool finished;

	protected override bool @delegate (float delta) {
		if (executedCount == repeatCount) return true;
		if (action.act(delta)) {
			if (finished) return true;
			if (repeatCount > 0) executedCount++;
			if (executedCount == repeatCount) return true;
			if (action != null) action.restart();
		}
		return false;
	}

	/** Causes the action to not repeat again. */
	public void finish () {
		finished = true;
	}

	public override void restart () {
		base.restart();
		executedCount = 0;
		finished = false;
	}

	/** Sets the number of times to repeat. Can be set to {@link #FOREVER}. */
	public void setCount (int count) {
		this.repeatCount = count;
	}

	public int getCount () {
		return repeatCount;
	}
}
