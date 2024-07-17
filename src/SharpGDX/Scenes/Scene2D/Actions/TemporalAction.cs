using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Base class for actions that transition over time using the percent complete.
 * @author Nathan Sweet */
abstract public class TemporalAction : Action {
	private float duration, time;
	private  Interpolation? interpolation;
	private bool reverse, began, complete;

	public TemporalAction () {
	}

	public TemporalAction (float duration) {
		this.duration = duration;
	}

	public TemporalAction (float duration, Interpolation? interpolation) {
		this.duration = duration;
		this.interpolation = interpolation;
	}

	public override bool act (float delta) {
		if (complete) return true;
		var pool = getPool();
		setPool((Pool<Action>?)null); // Ensure this action can't be returned to the pool while executing.
		try {
			if (!began) {
				begin();
				began = true;
			}
			time += delta;
			complete = time >= duration;
			float percent = complete ? 1 : time / duration;
			if (interpolation != null) percent = interpolation.apply(percent);
			update(reverse ? 1 - percent : percent);
			if (complete) end();
			return complete;
		} finally {
			setPool(pool);
		}
	}

	/** Called the first time {@link #act(float)} is called. This is a good place to query the {@link #actor actor's} starting
	 * state. */
	protected virtual void begin () {
	}

	/** Called the last time {@link #act(float)} is called. */
	protected virtual void end () {
	}

	/** Called each frame.
	 * @param percent The percentage of completion for this action, growing from 0 to 1 over the duration. If
	 *           {@link #setReverse(bool) reversed}, this will shrink from 1 to 0. */
	protected abstract void update (float percent);

	/** Skips to the end of the transition. */
	public void finish () {
		time = duration;
	}

	public override void restart () {
		time = 0;
		began = false;
		complete = false;
	}

	public override void reset () {
		base.reset();
		reverse = false;
		interpolation = null;
	}

	/** Gets the transition time so far. */
	public float getTime () {
		return time;
	}

	/** Sets the transition time so far. */
	public void setTime (float time) {
		this.time = time;
	}

	public float getDuration () {
		return duration;
	}

	/** Sets the length of the transition in seconds. */
	public void setDuration (float duration) {
		this.duration = duration;
	}

	public Interpolation? getInterpolation () {
		return interpolation;
	}

	public void setInterpolation (Interpolation? interpolation) {
		this.interpolation = interpolation;
	}

	public bool isReverse () {
		return reverse;
	}

	/** When true, the action's progress will go from 100% to 0%. */
	public void setReverse (bool reverse) {
		this.reverse = reverse;
	}

	/** Returns true after {@link #act(float)} has been called where time >= duration. */
	public bool isComplete () {
		return complete;
	}
}
