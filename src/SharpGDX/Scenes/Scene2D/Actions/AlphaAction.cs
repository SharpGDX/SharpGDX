using SharpGDX;
using SharpGDX.Graphics;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Sets the alpha for an actor's color (or a specified color), from the current alpha to the new alpha. Note this action
 * transitions from the alpha at the time the action starts to the specified alpha.
 * @author Nathan Sweet */
public class AlphaAction : TemporalAction {
	private float _start, _end;
	private  Color? color;

	protected override void begin () {
		if (color == null) color = target.getColor();
		_start = color.a;
	}

	protected override void update (float percent) {
		if (percent == 0)
			color.a = _start;
		else if (percent == 1)
			color.a = _end;
		else
			color.a = _start + (_end - _start) * percent;
	}

	public override void reset () {
		base.reset();
		color = null;
	}

	public  Color? getColor () {
		return color;
	}

	/** Sets the color to modify. If null (the default), the {@link #getActor() actor's} {@link Actor#getColor() color} will be
	 * used. */
	public void setColor ( Color? color) {
		this.color = color;
	}

	public float getAlpha () {
		return _end;
	}

	public void setAlpha (float alpha) {
		this._end = alpha;
	}
}
