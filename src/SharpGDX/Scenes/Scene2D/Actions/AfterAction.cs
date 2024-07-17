using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Executes an action only after all other actions on the actor at the time this action's target was set have finished.
 * @author Nathan Sweet */
public class AfterAction : DelegateAction {
	private Array<Action> waitForActions = new (false, 4);

	public override void setTarget (Actor target) {
		if (target != null) waitForActions.addAll(target.getActions());
		base.setTarget(target);
	}

	public override void restart () {
		base.restart();
		waitForActions.clear();
	}

	protected override bool @delegate (float delta) {
		Array<Action> currentActions = target.getActions();
		if (currentActions.size == 1) waitForActions.clear();
		for (int i = waitForActions.size - 1; i >= 0; i--) {
			Action action = waitForActions.get(i);
			int index = currentActions.indexOf(action, true);
			if (index == -1) waitForActions.removeIndex(i);
		}
		if (waitForActions.size > 0) return false;
		return action.act(delta);
	}
}
