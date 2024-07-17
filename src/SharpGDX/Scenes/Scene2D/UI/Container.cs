using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A group with a single child that sizes and positions the child using constraints. This provides layout similar to a
 * {@link Table} with a single cell but is more lightweight.
 * @author Nathan Sweet */
public class Container<T> : WidgetGroup 
where T: Actor{
	private T? actor;
	private Value _minWidth = Value.minWidth, _minHeight = Value.minHeight;
	private Value _prefWidth = Value.prefWidth, _prefHeight = Value.prefHeight;
	private Value _maxWidth = Value.zero, _maxHeight = Value.zero;
	private Value _padTop = Value.zero, _padLeft = Value.zero, _padBottom = Value.zero, _padRight = Value.zero;
	private float _fillX, _fillY;
	private int _align;
	private IDrawable? _background;
	private bool _clip;
	private bool round = true;

	/** Creates a container with no actor. */
	public Container () {
		setTouchable(Touchable.childrenOnly);
		setTransform(false);
	}

	public Container (T? actor) 
	:this()
	{
		
		setActor(actor);
	}

	public override void draw (IBatch batch, float parentAlpha) {
		validate();
		if (isTransform()) {
			applyTransform(batch, computeTransform());
			drawBackground(batch, parentAlpha, 0, 0);
			if (_clip) {
				batch.flush();
				float padLeft = this._padLeft.get(this), padBottom = this._padBottom.get(this);
				if (clipBegin(padLeft, padBottom, getWidth() - padLeft - _padRight.get(this),
					getHeight() - padBottom - _padTop.get(this))) {
					drawChildren(batch, parentAlpha);
					batch.flush();
					clipEnd();
				}
			} else
				drawChildren(batch, parentAlpha);
			resetTransform(batch);
		} else {
			drawBackground(batch, parentAlpha, getX(), getY());
			base.draw(batch, parentAlpha);
		}
	}

	/** Called to draw the background, before clipping is applied (if enabled). Default implementation draws the background
	 * drawable. */
	protected void drawBackground (IBatch batch, float parentAlpha, float x, float y) {
		if (background == null) return;
		Color color = getColor();
		batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);
		_background.draw(batch, x, y, getWidth(), getHeight());
	}

	/** Sets the background drawable and adjusts the container's padding to match the background.
	 * @see #setBackground(Drawable, boolean) */
	public void setBackground (IDrawable? background) {
		setBackground(background, true);
	}

	/** Sets the background drawable and, if adjustPadding is true, sets the container's padding to
	 * {@link Drawable#getBottomHeight()} , {@link Drawable#getTopHeight()}, {@link Drawable#getLeftWidth()}, and
	 * {@link Drawable#getRightWidth()}.
	 * @param background If null, the background will be cleared and padding removed. */
	public void setBackground (IDrawable? background, bool adjustPadding) {
		if (this._background == background) return;
		this._background = background;
		if (adjustPadding) {
			if (background == null)
				pad(Value.zero);
			else
				pad(background.getTopHeight(), background.getLeftWidth(), background.getBottomHeight(), background.getRightWidth());
			invalidate();
		}
	}

	/** @see #setBackground(Drawable) */
	public Container<T> background (IDrawable? background) {
		setBackground(background);
		return this;
	}

	public IDrawable? getBackground () {
		return _background;
	}

	public override void layout () {
		if (actor == null) return;

		float padLeft = this._padLeft.get(this), padBottom = this._padBottom.get(this);
		float containerWidth = getWidth() - padLeft - _padRight.get(this);
		float containerHeight = getHeight() - padBottom - _padTop.get(this);
		float minWidth = this._minWidth.get(actor), minHeight = this._minHeight.get(actor);
		float prefWidth = this._prefWidth.get(actor), prefHeight = this._prefHeight.get(actor);
		float maxWidth = this._maxWidth.get(actor), maxHeight = this._maxHeight.get(actor);

		float width;
		if (_fillX > 0)
			width = containerWidth * _fillX;
		else
			width = Math.Min(prefWidth, containerWidth);
		if (width < minWidth) width = minWidth;
		if (maxWidth > 0 && width > maxWidth) width = maxWidth;

		float height;
		if (_fillY > 0)
			height = containerHeight * _fillY;
		else
			height = Math.Min(prefHeight, containerHeight);
		if (height < minHeight) height = minHeight;
		if (maxHeight > 0 && height > maxHeight) height = maxHeight;

		float x = padLeft;
		if ((_align & Align.right) != 0)
			x += containerWidth - width;
		else if ((_align & Align.left) == 0) // center
			x += (containerWidth - width) / 2;

		float y = padBottom;
		if ((_align & Align.top) != 0)
			y += containerHeight - height;
		else if ((_align & Align.bottom) == 0) // center
			y += (containerHeight - height) / 2;

		if (round) {
			x = (float)Math.Round(x);
			y = (float)Math.Round(y);
			width = (float)Math.Round(width);
			height = (float)Math.Round(height);
		}

		actor.setBounds(x, y, width, height);
		if (actor is ILayout) ((ILayout)actor).validate();
	}

	public override void setCullingArea (Rectangle cullingArea) {
		base.setCullingArea(cullingArea);
		if (_fillX == 1 && _fillY == 1 && actor is ICullable) ((ICullable)actor).setCullingArea(cullingArea);
	}

	/** @param actor May be null. */
	public void setActor (T? actor) {
		if (actor == this) throw new IllegalArgumentException("actor cannot be the Container.");
		if (actor == this.actor) return;
		if (this.actor != null) base.removeActor(this.actor);
		this.actor = actor;
		if (actor != null) base.addActor(actor);
	}

	/** @return May be null. */
	public T? getActor () {
		return actor;
	}

	public override bool removeActor (Actor actor) {
		if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
		if (actor != this.actor) return false;
		setActor(null);
		return true;
	}

	public override bool removeActor (Actor actor, bool unfocus) {
		if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
		if (actor != this.actor) return false;
		this.actor = null;
		return base.removeActor(actor, unfocus);
	}

	public override Actor removeActorAt (int index, bool unfocus) {
		Actor actor = base.removeActorAt(index, unfocus);
		if (actor == this.actor) this.actor = null;
		return actor;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified value. */
	public Container<T> size (Value size) {
		if (size == null) throw new IllegalArgumentException("size cannot be null.");
		_minWidth = size;
		_minHeight = size;
		_prefWidth = size;
		_prefHeight = size;
		_maxWidth = size;
		_maxHeight = size;
		return this;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified values. */
	public Container<T> size (Value width, Value height) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
		_minWidth = width;
		_minHeight = height;
		_prefWidth = width;
		_prefHeight = height;
		_maxWidth = width;
		_maxHeight = height;
		return this;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified value. */
	public Container<T> size (float size) {
		this.size(Value.Fixed.valueOf(size));
		return this;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified values. */
	public Container<T> size (float width, float height) {
		size(Value.Fixed.valueOf(width), Value.Fixed.valueOf(height));
		return this;
	}

	/** Sets the minWidth, prefWidth, and maxWidth to the specified value. */
	public Container<T> width (Value width) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
		_minWidth = width;
		_prefWidth = width;
		_maxWidth = width;
		return this;
	}

	/** Sets the minWidth, prefWidth, and maxWidth to the specified value. */
	public Container<T> width (float width) {
		this.width(Value.Fixed.valueOf(width));
		return this;
	}

	/** Sets the minHeight, prefHeight, and maxHeight to the specified value. */
	public Container<T> height (Value height) {
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
		_minHeight = height;
		_prefHeight = height;
		_maxHeight = height;
		return this;
	}

	/** Sets the minHeight, prefHeight, and maxHeight to the specified value. */
	public Container<T> height (float height) {
		this.height(Value.Fixed.valueOf(height));
		return this;
	}

	/** Sets the minWidth and minHeight to the specified value. */
	public Container<T> minSize (Value size) {
		if (size == null) throw new IllegalArgumentException("size cannot be null.");
		_minWidth = size;
		_minHeight = size;
		return this;
	}

	/** Sets the minWidth and minHeight to the specified values. */
	public Container<T> minSize (Value width, Value height) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
		_minWidth = width;
		_minHeight = height;
		return this;
	}

	public Container<T> minWidth (Value minWidth) {
		if (minWidth == null) throw new IllegalArgumentException("minWidth cannot be null.");
		this._minWidth = minWidth;
		return this;
	}

	public Container<T> minHeight (Value minHeight) {
		if (minHeight == null) throw new IllegalArgumentException("minHeight cannot be null.");
		this._minHeight = minHeight;
		return this;
	}

	/** Sets the minWidth and minHeight to the specified value. */
	public Container<T> minSize (float size) {
		minSize(Value.Fixed.valueOf(size));
		return this;
	}

	/** Sets the minWidth and minHeight to the specified values. */
	public Container<T> minSize (float width, float height) {
		minSize(Value.Fixed.valueOf(width), Value.Fixed.valueOf(height));
		return this;
	}

	public Container<T> minWidth (float minWidth) {
		this._minWidth = Value.Fixed.valueOf(minWidth);
		return this;
	}

	public Container<T> minHeight (float minHeight) {
		this._minHeight = Value.Fixed.valueOf(minHeight);
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified value. */
	public Container<T> prefSize (Value size) {
		if (size == null) throw new IllegalArgumentException("size cannot be null.");
		_prefWidth = size;
		_prefHeight = size;
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified values. */
	public Container<T> prefSize (Value width, Value height) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
		_prefWidth = width;
		_prefHeight = height;
		return this;
	}

	public Container<T> prefWidth (Value prefWidth) {
		if (prefWidth == null) throw new IllegalArgumentException("prefWidth cannot be null.");
		this._prefWidth = prefWidth;
		return this;
	}

	public Container<T> prefHeight (Value prefHeight) {
		if (prefHeight == null) throw new IllegalArgumentException("prefHeight cannot be null.");
		this._prefHeight = prefHeight;
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified value. */
	public Container<T> prefSize (float width, float height) {
		prefSize(Value.Fixed.valueOf(width), Value.Fixed.valueOf(height));
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified values. */
	public Container<T> prefSize (float size) {
		prefSize(Value.Fixed.valueOf(size));
		return this;
	}

	public Container<T> prefWidth (float prefWidth) {
		this._prefWidth = Value.Fixed.valueOf(prefWidth);
		return this;
	}

	public Container<T> prefHeight (float prefHeight) {
		this._prefHeight = Value.Fixed.valueOf(prefHeight);
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified value. */
	public Container<T> maxSize (Value size) {
		if (size == null) throw new IllegalArgumentException("size cannot be null.");
		_maxWidth = size;
		_maxHeight = size;
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified values. */
	public Container<T> maxSize (Value width, Value height) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
		_maxWidth = width;
		_maxHeight = height;
		return this;
	}

	public Container<T> maxWidth (Value maxWidth) {
		if (maxWidth == null) throw new IllegalArgumentException("maxWidth cannot be null.");
		this._maxWidth = maxWidth;
		return this;
	}

	public Container<T> maxHeight (Value maxHeight) {
		if (maxHeight == null) throw new IllegalArgumentException("maxHeight cannot be null.");
		this._maxHeight = maxHeight;
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified value. */
	public Container<T> maxSize (float size) {
		maxSize(Value.Fixed.valueOf(size));
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified values. */
	public Container<T> maxSize (float width, float height) {
		maxSize(Value.Fixed.valueOf(width), Value.Fixed.valueOf(height));
		return this;
	}

	public Container<T> maxWidth (float maxWidth) {
		this._maxWidth = Value.Fixed.valueOf(maxWidth);
		return this;
	}

	public Container<T> maxHeight (float maxHeight) {
		this._maxHeight = Value.Fixed.valueOf(maxHeight);
		return this;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight to the specified value. */
	public Container<T> pad (Value pad) {
		if (pad == null) throw new IllegalArgumentException("pad cannot be null.");
		_padTop = pad;
		_padLeft = pad;
		_padBottom = pad;
		_padRight = pad;
		return this;
	}

	public Container<T> pad (Value top, Value left, Value bottom, Value right) {
		if (top == null) throw new IllegalArgumentException("top cannot be null.");
		if (left == null) throw new IllegalArgumentException("left cannot be null.");
		if (bottom == null) throw new IllegalArgumentException("bottom cannot be null.");
		if (right == null) throw new IllegalArgumentException("right cannot be null.");
		_padTop = top;
		_padLeft = left;
		_padBottom = bottom;
		_padRight = right;
		return this;
	}

	public Container<T> padTop (Value padTop) {
		if (padTop == null) throw new IllegalArgumentException("padTop cannot be null.");
		this._padTop = padTop;
		return this;
	}

	public Container<T> padLeft (Value padLeft) {
		if (padLeft == null) throw new IllegalArgumentException("padLeft cannot be null.");
		this._padLeft = padLeft;
		return this;
	}

	public Container<T> padBottom (Value padBottom) {
		if (padBottom == null) throw new IllegalArgumentException("padBottom cannot be null.");
		this._padBottom = padBottom;
		return this;
	}

	public Container<T> padRight (Value padRight) {
		if (padRight == null) throw new IllegalArgumentException("padRight cannot be null.");
		this._padRight = padRight;
		return this;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight to the specified value. */
	public Container<T> pad (float pad) {
		Value value = Value.Fixed.valueOf(pad);
		_padTop = value;
		_padLeft = value;
		_padBottom = value;
		_padRight = value;
		return this;
	}

	public Container<T> pad (float top, float left, float bottom, float right) {
		_padTop = Value.Fixed.valueOf(top);
		_padLeft = Value.Fixed.valueOf(left);
		_padBottom = Value.Fixed.valueOf(bottom);
		_padRight = Value.Fixed.valueOf(right);
		return this;
	}

	public Container<T> padTop (float padTop) {
		this._padTop = Value.Fixed.valueOf(padTop);
		return this;
	}

	public Container<T> padLeft (float padLeft) {
		this._padLeft = Value.Fixed.valueOf(padLeft);
		return this;
	}

	public Container<T> padBottom (float padBottom) {
		this._padBottom = Value.Fixed.valueOf(padBottom);
		return this;
	}

	public Container<T> padRight (float padRight) {
		this._padRight = Value.Fixed.valueOf(padRight);
		return this;
	}

	/** Sets fillX and fillY to 1. */
	public Container<T> fill () {
		_fillX = 1f;
		_fillY = 1f;
		return this;
	}

	/** Sets fillX to 1. */
	public Container<T> fillX () {
		_fillX = 1f;
		return this;
	}

	/** Sets fillY to 1. */
	public Container<T> fillY () {
		_fillY = 1f;
		return this;
	}

	public Container<T> fill (float x, float y) {
		_fillX = x;
		_fillY = y;
		return this;
	}

	/** Sets fillX and fillY to 1 if true, 0 if false. */
	public Container<T> fill (bool x, bool y) {
		_fillX = x ? 1f : 0;
		_fillY = y ? 1f : 0;
		return this;
	}

	/** Sets fillX and fillY to 1 if true, 0 if false. */
	public Container<T> fill (bool fill) {
		_fillX = fill ? 1f : 0;
		_fillY = fill ? 1f : 0;
		return this;
	}

	/** Sets the alignment of the actor within the container. Set to {@link Align#center}, {@link Align#top}, {@link Align#bottom},
	 * {@link Align#left}, {@link Align#right}, or any combination of those. */
	public Container<T> align (int align) {
		this._align = align;
		return this;
	}

	/** Sets the alignment of the actor within the container to {@link Align#center}. This clears any other alignment. */
	public Container<T> center () {
		_align = Align.center;
		return this;
	}

	/** Sets {@link Align#top} and clears {@link Align#bottom} for the alignment of the actor within the container. */
	public Container<T> top () {
		_align |= Align.top;
		_align &= ~Align.bottom;
		return this;
	}

	/** Sets {@link Align#left} and clears {@link Align#right} for the alignment of the actor within the container. */
	public Container<T> left () {
		_align |= Align.left;
		_align &= ~Align.right;
		return this;
	}

	/** Sets {@link Align#bottom} and clears {@link Align#top} for the alignment of the actor within the container. */
	public Container<T> bottom () {
		_align |= Align.bottom;
		_align &= ~Align.top;
		return this;
	}

	/** Sets {@link Align#right} and clears {@link Align#left} for the alignment of the actor within the container. */
	public Container<T> right () {
		_align |= Align.right;
		_align &= ~Align.left;
		return this;
	}

	public override float getMinWidth () {
		return _minWidth.get(actor) + _padLeft.get(this) + _padRight.get(this);
	}

	public Value getMinHeightValue () {
		return _minHeight;
	}

	public override float getMinHeight () {
		return _minHeight.get(actor) + _padTop.get(this) + _padBottom.get(this);
	}

	public Value getPrefWidthValue () {
		return _prefWidth;
	}

	public override float getPrefWidth () {
		float v = _prefWidth.get(actor);
		if (_background != null) v = Math.Max(v, _background.getMinWidth());
		return Math.Max(getMinWidth(), v + _padLeft.get(this) + _padRight.get(this));
	}

	public Value getPrefHeightValue () {
		return _prefHeight;
	}

	public override float getPrefHeight () {
		float v = _prefHeight.get(actor);
		if (_background != null) v = Math.Max(v, _background.getMinHeight());
		return Math.Max(getMinHeight(), v + _padTop.get(this) + _padBottom.get(this));
	}

	public Value getMaxWidthValue () {
		return _maxWidth;
	}

	public override float getMaxWidth () {
		float v = _maxWidth.get(actor);
		if (v > 0) v += _padLeft.get(this) + _padRight.get(this);
		return v;
	}

	public Value getMaxHeightValue () {
		return _maxHeight;
	}

	public override float getMaxHeight () {
		float v = _maxHeight.get(actor);
		if (v > 0) v += _padTop.get(this) + _padBottom.get(this);
		return v;
	}

	public Value getPadTopValue () {
		return _padTop;
	}

	public float getPadTop () {
		return _padTop.get(this);
	}

	public Value getPadLeftValue () {
		return _padLeft;
	}

	public float getPadLeft () {
		return _padLeft.get(this);
	}

	public Value getPadBottomValue () {
		return _padBottom;
	}

	public float getPadBottom () {
		return _padBottom.get(this);
	}

	public Value getPadRightValue () {
		return _padRight;
	}

	public float getPadRight () {
		return _padRight.get(this);
	}

	/** Returns {@link #getPadLeft()} plus {@link #getPadRight()}. */
	public float getPadX () {
		return _padLeft.get(this) + _padRight.get(this);
	}

	/** Returns {@link #getPadTop()} plus {@link #getPadBottom()}. */
	public float getPadY () {
		return _padTop.get(this) + _padBottom.get(this);
	}

	public float getFillX () {
		return _fillX;
	}

	public float getFillY () {
		return _fillY;
	}

	public int getAlign () {
		return _align;
	}

	/** If true (the default), positions and sizes are rounded to integers. */
	public void setRound (bool round) {
		this.round = round;
	}

	/** Sets clip to true. */
	public Container<T> clip () {
		setClip(true);
		return this;
	}

	public Container<T> clip (bool enabled) {
		setClip(enabled);
		return this;
	}

	/** Causes the contents to be clipped if they exceed the container bounds. Enabling clipping will set
	 * {@link #setTransform(boolean)} to true. */
	public void setClip (bool enabled) {
		_clip = enabled;
		setTransform(enabled);
		invalidate();
	}

	public bool getClip () {
		return _clip;
	}

	public override Actor? hit (float x, float y, bool touchable) {
		if (_clip) {
			if (touchable && getTouchable() == Touchable.disabled) return null;
			if (x < 0 || x >= getWidth() || y < 0 || y >= getHeight()) return null;
		}
		return base.hit(x, y, touchable);
	}

	public override void drawDebug (ShapeRenderer shapes) {
		validate();
		if (isTransform()) {
			applyTransform(shapes, computeTransform());
			if (_clip) {
				shapes.flush();
				float padLeft = this._padLeft.get(this), padBottom = this._padBottom.get(this);
				bool draw = _background == null ? clipBegin(0, 0, getWidth(), getHeight())
					: clipBegin(padLeft, padBottom, getWidth() - padLeft - _padRight.get(this),
						getHeight() - padBottom - _padTop.get(this));
				if (draw) {
					drawDebugChildren(shapes);
					clipEnd();
				}
			} else
				drawDebugChildren(shapes);
			resetTransform(shapes);
		} else
			base.drawDebug(shapes);
	}
}
