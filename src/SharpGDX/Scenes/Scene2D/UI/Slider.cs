using SharpGDX.Shims;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A slider is a horizontal indicator that allows a user to set a value. The slider has a range (min, max) and a stepping between
 * each value the slider represents.
 * <p>
 * {@link ChangeEvent} is fired when the slider knob is moved. Canceling the event will move the knob to where it was previously.
 * <p>
 * For a horizontal progress bar, its preferred height is determined by the larger of the knob and background, and the preferred
 * width is 140, a relatively arbitrary size. These parameters are reversed for a vertical progress bar.
 * @author mzechner
 * @author Nathan Sweet */
public class Slider : ProgressBar {
	int button = -1;
	int draggingPointer = -1;
	bool mouseOver;
	private Interpolation visualInterpolationInverse = Interpolation.linear;
	private float[]? snapValues;
	private float threshold;

	public Slider (float min, float max, float stepSize, bool vertical, Skin skin) 
	: this(min, max, stepSize, vertical, skin.get<SliderStyle>("default-" + (vertical ? "vertical" : "horizontal"), typeof(SliderStyle)))
	{
		
	}

	public Slider (float min, float max, float stepSize, bool vertical, Skin skin, String styleName) 
	: this(min, max, stepSize, vertical, skin.get<SliderStyle>(styleName, typeof(SliderStyle)))
	{
		
	}

	/** Creates a new slider. If horizontal, its width is determined by the prefWidth parameter, its height is determined by the
	 * maximum of the height of either the slider {@link NinePatch} or slider handle {@link TextureRegion}. The min and max values
	 * determine the range the values of this slider can take on, the stepSize parameter specifies the distance between individual
	 * values. E.g. min could be 4, max could be 10 and stepSize could be 0.2, giving you a total of 30 values, 4.0 4.2, 4.4 and so
	 * on.
	 * @param min the minimum value
	 * @param max the maximum value
	 * @param stepSize the step size between values
	 * @param style the {@link SliderStyle} */
	public Slider (float min, float max, float stepSize, bool vertical, SliderStyle style) 
	: base(min, max, stepSize, vertical, style)
	{
		

		addListener(new SliderInputListener(this) );
	}

	private class SliderInputListener: InputListener
	{
		private readonly Slider _slider;

		public SliderInputListener(Slider slider)
		{
			_slider = slider;
		}

		public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (_slider.disabled) return false;
			if (_slider.button != -1 && _slider.button != button) return false;
			if (_slider.draggingPointer != -1) return false;
			_slider.draggingPointer = pointer;
			_slider.calculatePositionAndValue(x, y);
			return true;
		}

		public override void touchUp(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (pointer != _slider.draggingPointer) return;
			_slider.draggingPointer = -1;
			// The position is invalid when focus is cancelled
			if (@event.isTouchFocusCancel() || !_slider.calculatePositionAndValue(x, y))
			{
				// Fire an event on touchUp even if the value didn't change, so listeners can see when a drag ends via isDragging.
				ChangeListener.ChangeEvent changeEvent = Pools.obtain<ChangeListener.ChangeEvent>(typeof(ChangeListener.ChangeEvent));
				_slider.fire(changeEvent);
				Pools.free(changeEvent);
			}
		}

		public override void touchDragged(InputEvent @event, float x, float y, int pointer)
		{
			_slider.calculatePositionAndValue(x, y);
		}

		public override void enter(InputEvent @event, float x, float y, int pointer, Actor? fromActor)
		{
			if (pointer == -1) _slider.mouseOver = true;
		}

		public override void exit(InputEvent @event, float x, float y, int pointer, Actor? toActor)
		{
			if (pointer == -1) _slider.mouseOver = false;
		}
	}

	/** Returns the slider's style. Modifying the returned style may not have an effect until {@link #setStyle(ProgressBarStyle)}
	 * is called. */
	public override SliderStyle getStyle () {
		return (SliderStyle)base.getStyle();
	}

	public bool isOver () {
		return mouseOver;
	}

	protected override IDrawable? getBackgroundDrawable () {
		SliderStyle style = (SliderStyle)base.getStyle();
		if (disabled && style.disabledBackground != null) return style.disabledBackground;
		if (isDragging() && style.backgroundDown != null) return style.backgroundDown;
		if (mouseOver && style.backgroundOver != null) return style.backgroundOver;
		return style.background;
	}

	protected override IDrawable? getKnobDrawable () {
		SliderStyle style = (SliderStyle)base.getStyle();
		if (disabled && style.disabledKnob != null) return style.disabledKnob;
		if (isDragging() && style.knobDown != null) return style.knobDown;
		if (mouseOver && style.knobOver != null) return style.knobOver;
		return style.knob;
	}

	protected override IDrawable getKnobBeforeDrawable () {
		SliderStyle style = (SliderStyle)base.getStyle();
		if (disabled && style.disabledKnobBefore != null) return style.disabledKnobBefore;
		if (isDragging() && style.knobBeforeDown != null) return style.knobBeforeDown;
		if (mouseOver && style.knobBeforeOver != null) return style.knobBeforeOver;
		return style.knobBefore;
	}

	protected override IDrawable getKnobAfterDrawable () {
		SliderStyle style = (SliderStyle)base.getStyle();
		if (disabled && style.disabledKnobAfter != null) return style.disabledKnobAfter;
		if (isDragging() && style.knobAfterDown != null) return style.knobAfterDown;
		if (mouseOver && style.knobAfterOver != null) return style.knobAfterOver;
		return style.knobAfter;
	}

	bool calculatePositionAndValue (float x, float y) {
		SliderStyle style = getStyle();
		IDrawable knob = style.knob;
		IDrawable bg = getBackgroundDrawable();

		float value;
		float oldPosition = position;

		float min = getMinValue();
		float max = getMaxValue();

		if (vertical) {
			float height = getHeight() - bg.getTopHeight() - bg.getBottomHeight();
			float knobHeight = knob == null ? 0 : knob.getMinHeight();
			position = y - bg.getBottomHeight() - knobHeight * 0.5f;
			value = min + (max - min) * visualInterpolationInverse.apply(position / (height - knobHeight));
			position = Math.Max(Math.Min(0, bg.getBottomHeight()), position);
			position = Math.Min(height - knobHeight, position);
		} else {
			float width = getWidth() - bg.getLeftWidth() - bg.getRightWidth();
			float knobWidth = knob == null ? 0 : knob.getMinWidth();
			position = x - bg.getLeftWidth() - knobWidth * 0.5f;
			value = min + (max - min) * visualInterpolationInverse.apply(position / (width - knobWidth));
			position = Math.Max(Math.Min(0, bg.getLeftWidth()), position);
			position = Math.Min(width - knobWidth, position);
		}

		float oldValue = value;
		if (!Gdx.input.isKeyPressed(IInput.Keys.SHIFT_LEFT) && !Gdx.input.isKeyPressed(SharpGDX.IInput.Keys.SHIFT_RIGHT)) value = snap(value);
		bool valueSet = setValue(value);
		if (value == oldValue) position = oldPosition;
		return valueSet;
	}

	/** Returns a snapped value from a value calculated from the mouse position. The default implementation uses
	 * {@link #setSnapToValues(float, float...)}. */
	protected float snap (float value) {
		if (snapValues == null || snapValues.Length == 0) return value;
		float bestDiff = -1, bestValue = 0;
		for (int i = 0; i < snapValues.Length; i++) {
			float snapValue = snapValues[i];
			float diff = Math.Abs(value - snapValue);
			if (diff <= threshold) {
				if (bestDiff == -1 || diff < bestDiff) {
					bestDiff = diff;
					bestValue = snapValue;
				}
			}
		}
		return bestDiff == -1 ? value : bestValue;
	}

	/** Makes this slider snap to the specified values when the knob is within the threshold.
	 * @param values May be null to disable snapping. */
	public void setSnapToValues (float threshold, float[]? values) {
		if (values != null && values.Length == 0) throw new IllegalArgumentException("values cannot be empty.");
		this.snapValues = values;
		this.threshold = threshold;
	}

	public float[]? getSnapToValues () {
		return snapValues;
	}

	public float getSnapToValuesThreshold () {
		return threshold;
	}

	/** Returns true if the slider is being dragged. */
	public bool isDragging () {
		return draggingPointer != -1;
	}

	/** Sets the mouse button, which can trigger a change of the slider. Is -1, so every button, by default. */
	public void setButton (int button) {
		this.button = button;
	}

	/** Sets the inverse interpolation to use for display. This should perform the inverse of the
	 * {@link #setVisualInterpolation(Interpolation) visual interpolation}. */
	public void setVisualInterpolationInverse (Interpolation interpolation) {
		this.visualInterpolationInverse = interpolation;
	}

	/** Sets the value using the specified visual percent.
	 * @see #setVisualInterpolation(Interpolation) */
	public void setVisualPercent (float percent) {
		setValue(min + (max - min) * visualInterpolationInverse.apply(percent));
	}

	/** The style for a slider, see {@link Slider}.
	 * @author mzechner
	 * @author Nathan Sweet */
	public class SliderStyle : ProgressBarStyle {
		public  IDrawable? backgroundOver, backgroundDown;
		public  IDrawable? knobOver, knobDown;
		public  IDrawable? knobBeforeOver, knobBeforeDown;
		public  IDrawable? knobAfterOver, knobAfterDown;

		public SliderStyle () {
		}

		public SliderStyle ( IDrawable? background,  IDrawable? knob) 
		: base(background, knob)
		{
			
		}

		public SliderStyle (SliderStyle style) 
		: base(style)
		{
			
			backgroundOver = style.backgroundOver;
			backgroundDown = style.backgroundDown;

			knobOver = style.knobOver;
			knobDown = style.knobDown;

			knobBeforeOver = style.knobBeforeOver;
			knobBeforeDown = style.knobBeforeDown;

			knobAfterOver = style.knobAfterOver;
			knobAfterDown = style.knobAfterDown;
		}
	}
}
