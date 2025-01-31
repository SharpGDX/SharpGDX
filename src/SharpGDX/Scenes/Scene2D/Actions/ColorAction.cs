using SharpGDX;
using SharpGDX.Graphics;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.Actions;

/** Sets the actor's color (or a specified color), from the current to the new color. Note this action transitions from the color
 * at the time the action starts to the specified color.
 * @author Nathan Sweet */
public class ColorAction : TemporalAction {
	private float startR, startG, startB, startA;
	private  Color? color;
	private readonly Color _end = new Color();

	protected override void begin () {
		if (color == null) color = Target.getColor();
		startR = color.R;
		startG = color.G;
		startB = color.B;
		startA = color.A;
	}

	protected override void update (float percent) {
		if (percent == 0)
			color.Set(startR, startG, startB, startA);
		else if (percent == 1)
			color.Set(_end);
		else {
			float r = startR + (_end.R - startR) * percent;
			float g = startG + (_end.G - startG) * percent;
			float b = startB + (_end.B - startB) * percent;
			float a = startA + (_end.A - startA) * percent;
			color.Set(r, g, b, a);
		}
	}

	public override void Reset () {
		base.Reset();
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

	public Color getEndColor () {
		return _end;
	}

	/** Sets the color to transition to. Required. */
	public void setEndColor (Color color) {
		_end.Set(color);
	}
}
