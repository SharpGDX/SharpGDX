using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** An on-screen joystick. The movement area of the joystick is circular, centered on the touchpad, and its size determined by the
 * smaller touchpad dimension.
 * <p>
 * The preferred size of the touchpad is determined by the background.
 * <p>
 * {@link ChangeEvent} is fired when the touchpad knob is moved. Cancelling the event will move the knob to where it was
 * previously.
 * @author Josh Street */
public class Touchpad : Widget {
	private TouchpadStyle style;
	bool touched;
	bool resetOnTouchUp = true;
	private float deadzoneRadius;
	private readonly Circle knobBounds = new Circle(0, 0, 0);
	private readonly Circle touchBounds = new Circle(0, 0, 0);
	private readonly Circle deadzoneBounds = new Circle(0, 0, 0);
	private readonly Vector2 knobPosition = new Vector2();
	private readonly Vector2 knobPercent = new Vector2();

	/** @param deadzoneRadius The distance in pixels from the center of the touchpad required for the knob to be moved. */
	public Touchpad (float deadzoneRadius, Skin skin) 
	: this(deadzoneRadius, skin.get<TouchpadStyle>(typeof(TouchpadStyle)))
	{
		
	}

	/** @param deadzoneRadius The distance in pixels from the center of the touchpad required for the knob to be moved. */
	public Touchpad (float deadzoneRadius, Skin skin, String styleName) 
	: this(deadzoneRadius, skin.get<TouchpadStyle>(styleName, typeof(TouchpadStyle)))
	{
		
	}

	/** @param deadzoneRadius The distance in pixels from the center of the touchpad required for the knob to be moved. */
	public Touchpad (float deadzoneRadius, TouchpadStyle style) {
		if (deadzoneRadius < 0) throw new IllegalArgumentException("deadzoneRadius must be > 0");
		this.deadzoneRadius = deadzoneRadius;

		knobPosition.set(getWidth() / 2f, getHeight() / 2f);

		setStyle(style);
		setSize(getPrefWidth(), getPrefHeight());

		addListener(new TouchpadListener(this) );
	}

	private class TouchpadListener : InputListener
	{
		private readonly Touchpad _touchpad;

		public TouchpadListener(Touchpad touchpad)
		{
			_touchpad = touchpad;
		}

		public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (_touchpad.touched) return false;
			_touchpad.touched = true;
			_touchpad.calculatePositionAndValue(x, y, false);
			return true;
		}

		public override void touchDragged(InputEvent @event, float x, float y, int pointer)
		{
			_touchpad.calculatePositionAndValue(x, y, false);
		}

		public override void touchUp(InputEvent @event, float x, float y, int pointer, int button)
		{
			_touchpad.touched = false;
			_touchpad.calculatePositionAndValue(x, y, _touchpad.resetOnTouchUp);
		}
	}

	void calculatePositionAndValue (float x, float y, bool isTouchUp) {
		float oldPositionX = knobPosition.x;
		float oldPositionY = knobPosition.y;
		float oldPercentX = knobPercent.x;
		float oldPercentY = knobPercent.y;
		float centerX = knobBounds.x;
		float centerY = knobBounds.y;
		knobPosition.set(centerX, centerY);
		knobPercent.set(0f, 0f);
		if (!isTouchUp) {
			if (!deadzoneBounds.contains(x, y)) {
				knobPercent.set((x - centerX) / knobBounds.radius, (y - centerY) / knobBounds.radius);
				float length = knobPercent.len();
				if (length > 1) knobPercent.scl(1 / length);
				if (knobBounds.contains(x, y)) {
					knobPosition.set(x, y);
				} else {
					knobPosition.set(knobPercent).nor().scl(knobBounds.radius).add(knobBounds.x, knobBounds.y);
				}
			}
		}
		if (oldPercentX != knobPercent.x || oldPercentY != knobPercent.y) {
			ChangeListener.ChangeEvent changeEvent = Pools.obtain<ChangeListener.ChangeEvent>(typeof(ChangeListener.ChangeEvent));
			if (fire(changeEvent)) {
				knobPercent.set(oldPercentX, oldPercentY);
				knobPosition.set(oldPositionX, oldPositionY);
			}
			Pools.free(changeEvent);
		}
	}

	public void setStyle (TouchpadStyle style) {
		if (style == null) throw new IllegalArgumentException("style cannot be null");
		this.style = style;
		invalidateHierarchy();
	}

	/** Returns the touchpad's style. Modifying the returned style may not have an effect until {@link #setStyle(TouchpadStyle)} is
	 * called. */
	public TouchpadStyle getStyle () {
		return style;
	}

	public override Actor hit (float x, float y, bool touchable) {
		if (touchable && this.getTouchable() != Touchable.enabled) return null;
		if (!isVisible()) return null;
		return touchBounds.contains(x, y) ? this : null;
	}

	public override void layout () {
		// Recalc pad and deadzone bounds
		float halfWidth = getWidth() / 2;
		float halfHeight = getHeight() / 2;
		float radius = Math.Min(halfWidth, halfHeight);
		touchBounds.set(halfWidth, halfHeight, radius);
		if (style.knob != null) radius -= Math.Max(style.knob.getMinWidth(), style.knob.getMinHeight()) / 2;
		knobBounds.set(halfWidth, halfHeight, radius);
		deadzoneBounds.set(halfWidth, halfHeight, deadzoneRadius);
		// Recalc pad values and knob position
		knobPosition.set(halfWidth, halfHeight);
		knobPercent.set(0, 0);
	}

	public override void draw (IBatch batch, float parentAlpha) {
		validate();

		Color c = getColor();
		batch.setColor(c.r, c.g, c.b, c.a * parentAlpha);

		float x = getX();
		float y = getY();
		float w = getWidth();
		float h = getHeight();

		 IDrawable bg = style.background;
		if (bg != null) bg.draw(batch, x, y, w, h);

		 IDrawable knob = style.knob;
		if (knob != null) {
			x += knobPosition.x - knob.getMinWidth() / 2f;
			y += knobPosition.y - knob.getMinHeight() / 2f;
			knob.draw(batch, x, y, knob.getMinWidth(), knob.getMinHeight());
		}
	}

	public override float getPrefWidth () {
		return style.background != null ? style.background.getMinWidth() : 0;
	}

	public override float getPrefHeight () {
		return style.background != null ? style.background.getMinHeight() : 0;
	}

	public bool isTouched () {
		return touched;
	}

	public bool getResetOnTouchUp () {
		return resetOnTouchUp;
	}

	/** @param reset Whether to reset the knob to the center on touch up. */
	public void setResetOnTouchUp (bool reset) {
		this.resetOnTouchUp = reset;
	}

	/** @param deadzoneRadius The distance in pixels from the center of the touchpad required for the knob to be moved. */
	public void setDeadzone (float deadzoneRadius) {
		if (deadzoneRadius < 0) throw new IllegalArgumentException("deadzoneRadius must be > 0");
		this.deadzoneRadius = deadzoneRadius;
		invalidate();
	}

	/** Returns the x-position of the knob relative to the center of the widget. The positive direction is right. */
	public float getKnobX () {
		return knobPosition.x;
	}

	/** Returns the y-position of the knob relative to the center of the widget. The positive direction is up. */
	public float getKnobY () {
		return knobPosition.y;
	}

	/** Returns the x-position of the knob as a percentage from the center of the touchpad to the edge of the circular movement
	 * area. The positive direction is right. */
	public float getKnobPercentX () {
		return knobPercent.x;
	}

	/** Returns the y-position of the knob as a percentage from the center of the touchpad to the edge of the circular movement
	 * area. The positive direction is up. */
	public float getKnobPercentY () {
		return knobPercent.y;
	}

	/** The style for a {@link Touchpad}.
	 * @author Josh Street */
	public  class TouchpadStyle {
		/** Stretched in both directions. */
		public IDrawable? background;
		public IDrawable? knob;

		public TouchpadStyle () {
		}

		public TouchpadStyle (IDrawable? background, IDrawable? knob) {
			this.background = background;
			this.knob = knob;
		}

		public TouchpadStyle (TouchpadStyle style) {
			background = style.background;
			knob = style.knob;
		}
	}
}
