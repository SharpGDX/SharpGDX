using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** An action that has a float, whose value is transitioned over time.
 * @author Nathan Sweet */
public class FloatAction : TemporalAction {
	private float _start, _end;
	private float value;

	/** Creates a FloatAction that transitions from 0 to 1. */
	public FloatAction () {
		_start = 0;
		_end = 1;
	}

	/** Creates a FloatAction that transitions from start to end. */
	public FloatAction (float start, float end) {
		this._start = start;
		this._end = end;
	}

	/** Creates a FloatAction that transitions from start to end. */
	public FloatAction (float start, float end, float duration) 
	: base(duration)
	{
		
		this._start = start;
		this._end = end;
	}

	/** Creates a FloatAction that transitions from start to end. */
	public FloatAction (float start, float end, float duration,  Interpolation? interpolation) 
	: base(duration, interpolation)
	{
		
		this._start = start;
		this._end = end;
	}

	protected override void begin () {
		value = _start;
	}

	protected override void update (float percent) {
		if (percent == 0)
			value = _start;
		else if (percent == 1)
			value = _end;
		else
			value = _start + (_end - _start) * percent;
	}

	/** Gets the current float value. */
	public float getValue () {
		return value;
	}

	/** Sets the current float value. */
	public void setValue (float value) {
		this.value = value;
	}

	public float getStart () {
		return _start;
	}

	/** Sets the value to transition from. */
	public void setStart (float start) {
		this._start = start;
	}

	public float getEnd () {
		return _end;
	}

	/** Sets the value to transition to. */
	public void setEnd (float end) {
		this._end = end;
	}
}
