using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Removes an actor from the stage.
 * @author Nathan Sweet */
public class RemoveActorAction : Action {
	private bool removed;

	public override bool Act (float delta) {
		if (!removed) {
			removed = true;
			Target.remove();
		}
		return true;
	}

	public override void Restart () {
		removed = false;
	}
}
