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

	public override bool Act (float delta) {
		if (capture)
			Target.addCaptureListener(listener);
		else
			Target.addListener(listener);
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

	public override void Reset () {
		base.Reset();
		listener = null;
	}
}
