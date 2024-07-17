using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A progress bar is a widget that visually displays the progress of some activity or a value within given range. The progress
 * bar has a range (min, max) and a stepping between each value it represents. The percentage of completeness typically starts out
 * as an empty progress bar and gradually becomes filled in as the task or variable value progresses.
 * <p>
 * {@link ChangeEvent} is fired when the progress bar knob is moved. Cancelling the event will move the knob to where it was
 * previously.
 * <p>
 * For a horizontal progress bar, its preferred height is determined by the larger of the knob and background, and the preferred
 * width is 140, a relatively arbitrary size. These parameters are reversed for a vertical progress bar.
 * @author mzechner
 * @author Nathan Sweet */
public class ProgressBar : Widget , IDisableable {
	private ProgressBarStyle style;
	protected float min, max, stepSize;
	private float value, animateFromValue;
	protected float position;
	protected readonly bool vertical;
	private float animateDuration, animateTime;
	private Interpolation animateInterpolation = Interpolation.linear, visualInterpolation = Interpolation.linear;
	protected bool disabled;
	private bool _round = true, programmaticChangeEvents = true;

	public ProgressBar (float min, float max, float stepSize, bool vertical, Skin skin) 
	: this(min, max, stepSize, vertical, skin.get<ProgressBarStyle>("default-" + (vertical ? "vertical" : "horizontal"), typeof(ProgressBarStyle)))
	{
		
	}

	public ProgressBar (float min, float max, float stepSize, bool vertical, Skin skin, String styleName) 
	: this(min, max, stepSize, vertical, skin.get<ProgressBarStyle>(styleName, typeof(ProgressBarStyle)))
	{
		
	}

	/** Creates a new progress bar. If horizontal, its width is determined by the prefWidth parameter, and its height is determined
	 * by the maximum of the height of either the progress bar {@link NinePatch} or progress bar handle {@link TextureRegion}. The
	 * min and max values determine the range the values of this progress bar can take on, the stepSize parameter specifies the
	 * distance between individual values.
	 * <p>
	 * E.g. min could be 4, max could be 10 and stepSize could be 0.2, giving you a total of 30 values, 4.0 4.2, 4.4 and so on.
	 * @param min the minimum value
	 * @param max the maximum value
	 * @param stepSize the step size between values
	 * @param style the {@link ProgressBarStyle} */
	public ProgressBar (float min, float max, float stepSize, bool vertical, ProgressBarStyle style) {
		if (min > max) throw new IllegalArgumentException("max must be > min. min,max: " + min + ", " + max);
		if (stepSize <= 0) throw new IllegalArgumentException("stepSize must be > 0: " + stepSize);
		setStyle(style);
		this.min = min;
		this.max = max;
		this.stepSize = stepSize;
		this.vertical = vertical;
		this.value = min;
		setSize(getPrefWidth(), getPrefHeight());
	}

	public void setStyle (ProgressBarStyle style) {
		if (style == null) throw new IllegalArgumentException("style cannot be null.");
		this.style = style;
		invalidateHierarchy();
	}

	/** Returns the progress bar's style. Modifying the returned style may not have an effect until
	 * {@link #setStyle(ProgressBarStyle)} is called. */
	public virtual ProgressBarStyle getStyle () {
		return style;
	}

	public override void act (float delta) {
		base.act(delta);
		if (animateTime > 0) {
			animateTime -= delta;
			Stage stage = getStage();
			if (stage != null && stage.getActionsRequestRendering()) Gdx.graphics.requestRendering();
		}
	}

	public override void draw (IBatch batch, float parentAlpha) {
		ProgressBarStyle style = this.style;
		bool disabled = this.disabled;
		IDrawable knob = style.knob, currentKnob = getKnobDrawable();
		IDrawable bg = getBackgroundDrawable();
		IDrawable knobBefore = getKnobBeforeDrawable();
		IDrawable knobAfter = getKnobAfterDrawable();

		Color color = getColor();
		float x = getX(), y = getY();
		float width = getWidth(), height = getHeight();
		float knobHeight = knob == null ? 0 : knob.getMinHeight();
		float knobWidth = knob == null ? 0 : knob.getMinWidth();
		float percent = getVisualPercent();

		batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);

		if (vertical) {
			float bgTopHeight = 0, bgBottomHeight = 0;
			if (bg != null) {
				drawRound(batch, bg, x + (width - bg.getMinWidth()) * 0.5f, y, bg.getMinWidth(), height);
				bgTopHeight = bg.getTopHeight();
				bgBottomHeight = bg.getBottomHeight();
				height -= bgTopHeight + bgBottomHeight;
			}

			float total = height - knobHeight;
			float beforeHeight = MathUtils.clamp(total * percent, 0, total);
			position = bgBottomHeight + beforeHeight;

			float knobHeightHalf = knobHeight * 0.5f;
			if (knobBefore != null) {
				drawRound(batch, knobBefore, //
					x + (width - knobBefore.getMinWidth()) * 0.5f, //
					y + bgBottomHeight, //
					knobBefore.getMinWidth(), beforeHeight + knobHeightHalf);
			}
			if (knobAfter != null) {
				drawRound(batch, knobAfter, //
					x + (width - knobAfter.getMinWidth()) * 0.5f, //
					y + position + knobHeightHalf, //
					knobAfter.getMinWidth(),
					total - (_round ? (float)Math.Round(beforeHeight - knobHeightHalf) : beforeHeight - knobHeightHalf));
			}
			if (currentKnob != null) {
				float w = currentKnob.getMinWidth(), h = currentKnob.getMinHeight();
				drawRound(batch, currentKnob, //
					x + (width - w) * 0.5f, //
					y + position + (knobHeight - h) * 0.5f, //
					w, h);
			}
		} else {
			float bgLeftWidth = 0, bgRightWidth = 0;
			if (bg != null) {
				drawRound(batch, bg, x, (float)Math.Round(y + (height - bg.getMinHeight()) * 0.5f), width, (float)Math.Round(bg.getMinHeight()));
				bgLeftWidth = bg.getLeftWidth();
				bgRightWidth = bg.getRightWidth();
				width -= bgLeftWidth + bgRightWidth;
			}

			float total = width - knobWidth;
			float beforeWidth = MathUtils.clamp(total * percent, 0, total);
			position = bgLeftWidth + beforeWidth;

			float knobWidthHalf = knobWidth * 0.5f;
			if (knobBefore != null) {
				drawRound(batch, knobBefore, //
					x + bgLeftWidth, //
					y + (height - knobBefore.getMinHeight()) * 0.5f, //
					beforeWidth + knobWidthHalf, knobBefore.getMinHeight());
			}
			if (knobAfter != null) {
				drawRound(batch, knobAfter, //
					x + position + knobWidthHalf, //
					y + (height - knobAfter.getMinHeight()) * 0.5f, //
					total - (_round ? (float)Math.Round(beforeWidth - knobWidthHalf) : beforeWidth - knobWidthHalf), knobAfter.getMinHeight());
			}
			if (currentKnob != null) {
				float w = currentKnob.getMinWidth(), h = currentKnob.getMinHeight();
				drawRound(batch, currentKnob, //
					x + position + (knobWidth - w) * 0.5f, //
					y + (height - h) * 0.5f, //
					w, h);
			}
		}
	}

	private void drawRound (IBatch batch, IDrawable drawable, float x, float y, float w, float h) {
		if (_round) {
			x = (float)Math.Round(x);
			y = (float)Math.Round(y);
			w = (float)Math.Round(w);
			h = (float)Math.Round(h);
		}
		drawable.draw(batch, x, y, w, h);
	}

	public float getValue () {
		return value;
	}

	/** If {@link #setAnimateDuration(float) animating} the progress bar value, this returns the value current displayed. */
	public float getVisualValue () {
		if (animateTime > 0) return animateInterpolation.apply(animateFromValue, value, 1 - animateTime / animateDuration);
		return value;
	}

	/** Sets the visual value equal to the actual value. This can be used to set the value without animating. */
	public void updateVisualValue () {
		animateTime = 0;
	}

	public float getPercent () {
		if (min == max) return 0;
		return (value - min) / (max - min);
	}

	public float getVisualPercent () {
		if (min == max) return 0;
		return visualInterpolation.apply((getVisualValue() - min) / (max - min));
	}

	protected virtual IDrawable? getBackgroundDrawable () {
		if (disabled && style.disabledBackground != null) return style.disabledBackground;
		return style.background;
	}

	protected virtual IDrawable? getKnobDrawable () {
		if (disabled && style.disabledKnob != null) return style.disabledKnob;
		return style.knob;
	}

	protected virtual IDrawable getKnobBeforeDrawable () {
		if (disabled && style.disabledKnobBefore != null) return style.disabledKnobBefore;
		return style.knobBefore;
	}

	protected virtual IDrawable getKnobAfterDrawable () {
		if (disabled && style.disabledKnobAfter != null) return style.disabledKnobAfter;
		return style.knobAfter;
	}

	/** Returns progress bar visual position within the range (as it was last calculated in {@link #draw(Batch, float)}). */
	protected float getKnobPosition () {
		return this.position;
	}

	/** Sets the progress bar position, rounded to the nearest step size and clamped to the minimum and maximum values.
	 * {@link #clamp(float)} can be overridden to allow values outside of the progress bar's min/max range.
	 * @return false if the value was not changed because the progress bar already had the value or it was canceled by a
	 *         listener. */
	public bool setValue (float value) {
		value = clamp(round(value));
		float oldValue = this.value;
		if (value == oldValue) return false;
		float oldVisualValue = getVisualValue();
		this.value = value;

		if (programmaticChangeEvents) {
			ChangeListener.ChangeEvent changeEvent = Pools.obtain<ChangeListener.ChangeEvent>(typeof(ChangeListener.ChangeEvent));
			bool cancelled = fire(changeEvent);
			Pools.free(changeEvent);
			if (cancelled) {
				this.value = oldValue;
				return false;
			}
		}

		if (animateDuration > 0) {
			animateFromValue = oldVisualValue;
			animateTime = animateDuration;
		}
		return true;
	}

	/** Rouinds the value using the progress bar's step size. This can be overridden to customize or disable rounding. */
	protected float round (float value) {
		return (float)Math.Round(value / stepSize) * stepSize;
	}

	/** Clamps the value to the progress bar's min/max range. This can be overridden to allow a range different from the progress
	 * bar knob's range. */
	protected float clamp (float value) {
		return MathUtils.clamp(value, min, max);
	}

	/** Sets the range of this progress bar. The progress bar's current value is clamped to the range. */
	public void setRange (float min, float max) {
		if (min > max) throw new IllegalArgumentException("min must be <= max: " + min + " <= " + max);
		this.min = min;
		this.max = max;
		if (value < min)
			setValue(min);
		else if (value > max) //
			setValue(max);
	}

	public void setStepSize (float stepSize) {
		if (stepSize <= 0) throw new IllegalArgumentException("steps must be > 0: " + stepSize);
		this.stepSize = stepSize;
	}

	public override float getPrefWidth () {
		if (vertical) {
			IDrawable knob = style.knob, bg = getBackgroundDrawable();
			return Math.Max(knob == null ? 0 : knob.getMinWidth(), bg == null ? 0 : bg.getMinWidth());
		} else
			return 140;
	}

	public override float getPrefHeight () {
		if (vertical)
			return 140;
		else {
			IDrawable knob = style.knob, bg = getBackgroundDrawable();
			return Math.Max(knob == null ? 0 : knob.getMinHeight(), bg == null ? 0 : bg.getMinHeight());
		}
	}

	public float getMinValue () {
		return this.min;
	}

	public float getMaxValue () {
		return this.max;
	}

	public float getStepSize () {
		return this.stepSize;
	}

	/** If > 0, changes to the progress bar value via {@link #setValue(float)} will happen over this duration in seconds. */
	public void setAnimateDuration (float duration) {
		this.animateDuration = duration;
	}

	/** Sets the interpolation to use for {@link #setAnimateDuration(float)}. */
	public void setAnimateInterpolation (Interpolation animateInterpolation) {
		if (animateInterpolation == null) throw new IllegalArgumentException("animateInterpolation cannot be null.");
		this.animateInterpolation = animateInterpolation;
	}

	/** Sets the interpolation to use for display. */
	public void setVisualInterpolation (Interpolation interpolation) {
		this.visualInterpolation = interpolation;
	}

	/** If true (the default), inner Drawable positions and sizes are rounded to integers. */
	public void setRound (bool round) {
		this._round = round;
	}

	public void setDisabled (bool disabled) {
		this.disabled = disabled;
	}

	public bool isAnimating () {
		return animateTime > 0;
	}

	public bool isDisabled () {
		return disabled;
	}

	/** True if the progress bar is vertical, false if it is horizontal. **/
	public bool isVertical () {
		return vertical;
	}

	/** If false, {@link #setValue(float)} will not fire {@link ChangeEvent}. The event will only be fired when the user changes
	 * the slider. */
	public void setProgrammaticChangeEvents (bool programmaticChangeEvents) {
		this.programmaticChangeEvents = programmaticChangeEvents;
	}

	/** The style for a progress bar, see {@link ProgressBar}.
	 * @author mzechner
	 * @author Nathan Sweet */
	public class ProgressBarStyle {
		/** The progress bar background, stretched only in one direction. */
		public  IDrawable? background, disabledBackground;
		public  IDrawable? knob, disabledKnob;
		public  IDrawable? knobBefore, disabledKnobBefore;
		public  IDrawable? knobAfter, disabledKnobAfter;

		public ProgressBarStyle () {
		}

		public ProgressBarStyle ( IDrawable? background, IDrawable? knob) {
			this.background = background;
			this.knob = knob;
		}

		public ProgressBarStyle (ProgressBarStyle style) {
			background = style.background;
			disabledBackground = style.disabledBackground;

			knob = style.knob;
			disabledKnob = style.disabledKnob;

			knobBefore = style.knobBefore;
			disabledKnobBefore = style.disabledKnobBefore;

			knobAfter = style.knobAfter;
			disabledKnobAfter = style.disabledKnobAfter;
		}
	}
}
