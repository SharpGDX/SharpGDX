using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** An action that runs a {@link Runnable}. Alternatively, the {@link #run()} method can be overridden instead of setting a
 * runnable.
 * @author Nathan Sweet */
public class RunnableAction : Action {
	private Runnable runnable;
	private bool ran;

	public override bool Act (float delta) {
		if (!ran) {
			ran = true;
			run();
		}
		return true;
	}

	/** Called to run the runnable. */
	public void run () {
		var pool = GetPool();
		SetPool((Pool<Action>?)null); // Ensure this action can't be returned to the pool inside the runnable.
		try {
			runnable.Invoke();
		} finally {
			SetPool(pool);
		}
	}

	public override void Restart () {
		ran = false;
	}

	public override void Reset () {
		base.Reset();
		runnable = null;
	}

	public Runnable getRunnable () {
		return runnable;
	}

	public void setRunnable (Runnable runnable) {
		this.runnable = runnable;
	}
}
