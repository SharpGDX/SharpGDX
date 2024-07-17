using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Sets the actor's rotation from its current value to a specific value.
 * 
 * By default, the rotation will take you from the starting value to the specified value via simple subtraction. For example,
 * setting the start at 350 and the target at 10 will result in 340 degrees of movement.
 * 
 * If the action is instead set to useShortestDirection instead, it will rotate straight to the target angle, regardless of where
 * the angle starts and stops. For example, starting at 350 and rotating to 10 will cause 20 degrees of rotation.
 * 
 * @see com.badlogic.gdx.math.MathUtils#lerpAngleDeg(float, float, float)
 * 
 * @author Nathan Sweet */
public class RotateToAction : TemporalAction {
	private float _start, _end;

	private bool useShortestDirection = false;

	public RotateToAction () {
	}

	/** @param useShortestDirection Set to true to move directly to the closest angle */
	public RotateToAction (bool useShortestDirection) {
		this.useShortestDirection = useShortestDirection;
	}

	protected override void begin () {
		_start = target.getRotation();
	}

	protected override void update (float percent) {
		float rotation;
		if (percent == 0)
			rotation = _start;
		else if (percent == 1)
			rotation = _end;
		else if (useShortestDirection)
			rotation = MathUtils.lerpAngleDeg(this._start, this._end, percent);
		else
			rotation = _start + (_end - _start) * percent;
		target.setRotation(rotation);
	}

	public float getRotation () {
		return _end;
	}

	public void setRotation (float rotation) {
		this._end = rotation;
	}

	public bool isUseShortestDirection () {
		return useShortestDirection;
	}

	public void setUseShortestDirection (bool useShortestDirection) {
		this.useShortestDirection = useShortestDirection;
	}
}
