using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** An action that has an int, whose value is transitioned over time.
 * @author Nathan Sweet */
public class IntAction : TemporalAction {
	private int _start, _end;
	private int value;

	/** Creates an IntAction that transitions from 0 to 1. */
	public IntAction () {
		_start = 0;
		_end = 1;
	}

	/** Creates an IntAction that transitions from start to end. */
	public IntAction (int start, int end) {
		this._start = start;
		this._end = end;
	}

	/** Creates a FloatAction that transitions from start to end. */
	public IntAction (int start, int end, float duration) 
	: base(duration)
	{
		
		this._start = start;
		this._end = end;
	}

	/** Creates a FloatAction that transitions from start to end. */
	public IntAction (int start, int end, float duration,  Interpolation? interpolation) 
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
			value = (int)(_start + (_end - _start) * percent);
	}

	/** Gets the current int value. */
	public int getValue () {
		return value;
	}

	/** Sets the current int value. */
	public void setValue (int value) {
		this.value = value;
	}

	public int getStart () {
		return _start;
	}

	/** Sets the value to transition from. */
	public void setStart (int start) {
		this._start = start;
	}

	public int getEnd () {
		return _end;
	}

	/** Sets the value to transition to. */
	public void setEnd (int end) {
		this._end = end;
	}
}
