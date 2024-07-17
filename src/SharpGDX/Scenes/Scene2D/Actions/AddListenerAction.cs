using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Adds a listener to an actor.
 * @author Nathan Sweet */
public class AddListenerAction : Action {
	private IEventListener listener;
	private bool capture;

	public override bool act (float delta) {
		if (capture)
			target.addCaptureListener(listener);
		else
			target.addListener(listener);
		return true;
	}

	public IEventListener getListener () {
		return listener;
	}

	public void setListener (IEventListener listener) {
		this.listener = listener;
	}

	public bool getCapture () {
		return capture;
	}

	public void setCapture (bool capture) {
		this.capture = capture;
	}

	public override void reset () {
		base.reset();
		listener = null;
	}
}
