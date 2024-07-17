using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A group that lays out its children top to bottom vertically, with optional wrapping. {@link #getChildren()} can be sorted to
 * change the order of the actors (eg {@link Actor#setZIndex(int)}). This can be easier than using {@link Table} when actors need
 * to be inserted into or removed from the middle of the group. {@link #invalidate()} must be called after changing the children
 * order.
 * <p>
 * The preferred width is the largest preferred width of any child. The preferred height is the sum of the children's preferred
 * heights plus spacing. The preferred size is slightly different when {@link #wrap() wrap} is enabled. The min size is the
 * preferred size and the max size is 0.
 * <p>
 * Widgets are sized using their {@link Layout#getPrefWidth() preferred height}, so widgets which return 0 as their preferred
 * height will be given a height of 0.
 * @author Nathan Sweet */
public class VerticalGroup : WidgetGroup {
	private float prefWidth, prefHeight, lastPrefWidth;
	private bool sizeInvalid = true;
	private FloatArray columnSizes; // column height, column width, ...

	private int _align = Align.top, _columnAlign;
	private bool _reverse, round = true,_wrap, _expand;
	private float _space, _wrapSpace, _fill, _padTop, _padLeft, _padBottom, _padRight;

	public VerticalGroup () {
		setTouchable(Touchable.childrenOnly);
	}

	public override void invalidate () {
		base.invalidate();
		sizeInvalid = true;
	}

	private void computeSize () {
		sizeInvalid = false;
		SnapshotArray<Actor> children = getChildren();
		int n = children.size;
		prefWidth = 0;
		if (_wrap) {
			prefHeight = 0;
			if (this.columnSizes == null)
				this.columnSizes = new FloatArray();
			else
				this.columnSizes.clear();
			FloatArray columnSizes = this.columnSizes;
			float space = this._space, wrapSpace = this._wrapSpace;
			float pad = _padTop + _padBottom, groupHeight = getHeight() - pad, x = 0, y = 0, columnWidth = 0;
			int i = 0, incr = 1;
			if (_reverse) {
				i = n - 1;
				n = -1;
				incr = -1;
			}
			for (; i != n; i += incr) {
				Actor child = children.get(i);

				float width, height;
				if (child is ILayout) {
					ILayout layout = (ILayout)child;
					width = layout.getPrefWidth();
					height = layout.getPrefHeight();
					if (height > groupHeight) height = Math.Max(groupHeight, layout.getMinHeight());
				} else {
					width = child.getWidth();
					height = child.getHeight();
				}

				float incrY = height + (y > 0 ? space : 0);
				if (y + incrY > groupHeight && y > 0) {
					columnSizes.add(y);
					columnSizes.add(columnWidth);
					prefHeight = Math.Max(prefHeight, y + pad);
					if (x > 0) x += wrapSpace;
					x += columnWidth;
					columnWidth = 0;
					y = 0;
					incrY = height;
				}
				y += incrY;
				columnWidth = Math.Max(columnWidth, width);
			}
			columnSizes.add(y);
			columnSizes.add(columnWidth);
			prefHeight = Math.Max(prefHeight, y + pad);
			if (x > 0) x += wrapSpace;
			prefWidth = Math.Max(prefWidth, x + columnWidth);
		} else {
			prefHeight = _padTop + _padBottom + _space * (n - 1);
			for (int i = 0; i < n; i++) {
				Actor child = children.get(i);
				if (child is ILayout) {
					ILayout layout = (ILayout)child;
					prefWidth = Math.Max(prefWidth, layout.getPrefWidth());
					prefHeight += layout.getPrefHeight();
				} else {
					prefWidth = Math.Max(prefWidth, child.getWidth());
					prefHeight += child.getHeight();
				}
			}
		}
		prefWidth += _padLeft + _padRight;
		if (round) {
			prefWidth = (float)Math.Round(prefWidth);
			prefHeight = (float)Math.Round(prefHeight);
		}
	}

	public override void layout () {
		if (sizeInvalid) computeSize();

		if (_wrap) {
			layoutWrapped();
			return;
		}

		bool round = this.round;
		int align = this._align;
		float space = this._space, padLeft = this._padLeft, fill = this._fill;
		float columnWidth = (_expand ? getWidth() : prefWidth) - padLeft - _padRight, y = prefHeight - _padTop + space;

		if ((align & Align.top) != 0)
			y += getHeight() - prefHeight;
		else if ((align & Align.bottom) == 0) // center
			y += (getHeight() - prefHeight) / 2;

		float startX;
		if ((align & Align.left) != 0)
			startX = padLeft;
		else if ((align & Align.right) != 0)
			startX = getWidth() - _padRight - columnWidth;
		else
			startX = padLeft + (getWidth() - padLeft - _padRight - columnWidth) / 2;

		align = _columnAlign;

		SnapshotArray<Actor> children = getChildren();
		int i = 0, n = children.size, incr = 1;
		if (_reverse) {
			i = n - 1;
			n = -1;
			incr = -1;
		}
		for (int r = 0; i != n; i += incr) {
			Actor child = children.get(i);

			float width, height;
			ILayout layout = null;
			if (child is ILayout) {
				layout = (ILayout)child;
				width = layout.getPrefWidth();
				height = layout.getPrefHeight();
			} else {
				width = child.getWidth();
				height = child.getHeight();
			}

			if (fill > 0) width = columnWidth * fill;

			if (layout != null) {
				width = Math.Max(width, layout.getMinWidth());
				float maxWidth = layout.getMaxWidth();
				if (maxWidth > 0 && width > maxWidth) width = maxWidth;
			}

			float x = startX;
			if ((align & Align.right) != 0)
				x += columnWidth - width;
			else if ((align & Align.left) == 0) // center
				x += (columnWidth - width) / 2;

			y -= height + space;
			if (round)
				child.setBounds((float)Math.Round(x), (float)Math.Round(y), (float)Math.Round(width), (float)Math.Round(height));
			else
				child.setBounds(x, y, width, height);

			if (layout != null) layout.validate();
		}
	}

	private void layoutWrapped () {
		float prefWidth = getPrefWidth();
		if (prefWidth != lastPrefWidth) {
			lastPrefWidth = prefWidth;
			invalidateHierarchy();
		}

		int align = this._align;
		bool round = this.round;
		float space = this._space, padLeft = this._padLeft, fill = this._fill, wrapSpace = this._wrapSpace;
		float maxHeight = prefHeight - _padTop - _padBottom;
		float columnX = padLeft, groupHeight = getHeight();
		float yStart = prefHeight - _padTop + space, y = 0, columnWidth = 0;

		if ((align & Align.right) != 0)
			columnX += getWidth() - prefWidth;
		else if ((align & Align.left) == 0) // center
			columnX += (getWidth() - prefWidth) / 2;

		if ((align & Align.top) != 0)
			yStart += groupHeight - prefHeight;
		else if ((align & Align.bottom) == 0) // center
			yStart += (groupHeight - prefHeight) / 2;

		groupHeight -= _padTop;
		align = _columnAlign;

		FloatArray columnSizes = this.columnSizes;
		SnapshotArray<Actor> children = getChildren();
		int i = 0, n = children.size, incr = 1;
		if (_reverse) {
			i = n - 1;
			n = -1;
			incr = -1;
		}
		for (int r = 0; i != n; i += incr) {
			Actor child = children.get(i);

			float width, height;
			ILayout layout = null;
			if (child is ILayout) {
				layout = (ILayout)child;
				width = layout.getPrefWidth();
				height = layout.getPrefHeight();
				if (height > groupHeight) height = Math.Max(groupHeight, layout.getMinHeight());
			} else {
				width = child.getWidth();
				height = child.getHeight();
			}

			if (y - height - space < _padBottom || r == 0) {
				r = Math.Min(r, columnSizes.size - 2); // In case an actor changed size without invalidating this layout.
				y = yStart;
				if ((align & Align.bottom) != 0)
					y -= maxHeight - columnSizes.get(r);
				else if ((align & Align.top) == 0) // center
					y -= (maxHeight - columnSizes.get(r)) / 2;
				if (r > 0) {
					columnX += wrapSpace;
					columnX += columnWidth;
				}
				columnWidth = columnSizes.get(r + 1);
				r += 2;
			}

			if (fill > 0) width = columnWidth * fill;

			if (layout != null) {
				width = Math.Max(width, layout.getMinWidth());
				float maxWidth = layout.getMaxWidth();
				if (maxWidth > 0 && width > maxWidth) width = maxWidth;
			}

			float x = columnX;
			if ((align & Align.right) != 0)
				x += columnWidth - width;
			else if ((align & Align.left) == 0) // center
				x += (columnWidth - width) / 2;

			y -= height + space;
			if (round)
				child.setBounds((float)Math.Round(x), (float)Math.Round(y), (float)Math.Round(width), (float)Math.Round(height));
			else
				child.setBounds(x, y, width, height);

			if (layout != null) layout.validate();
		}
	}

	public override float getPrefWidth () {
		if (sizeInvalid) computeSize();
		return prefWidth;
	}

	public override float getPrefHeight () {
		if (_wrap) return 0;
		if (sizeInvalid) computeSize();
		return prefHeight;
	}

	/** When wrapping is enabled, the number of columns may be > 1. */
	public int getColumns () {
		return _wrap ? columnSizes.size >> 1 : 1;
	}

	/** If true (the default), positions and sizes are rounded to integers. */
	public void setRound (bool round) {
		this.round = round;
	}

	/** The children will be displayed last to first. */
	public VerticalGroup reverse () {
		this._reverse = true;
		return this;
	}

	/** If true, the children will be displayed last to first. */
	public VerticalGroup reverse (bool reverse) {
		this._reverse = reverse;
		return this;
	}

	public bool getReverse () {
		return _reverse;
	}

	/** Sets the vertical space between children. */
	public VerticalGroup space (float space) {
		this._space = space;
		return this;
	}

	public float getSpace () {
		return _space;
	}

	/** Sets the horizontal space between columns when wrap is enabled. */
	public VerticalGroup wrapSpace (float wrapSpace) {
		this._wrapSpace = wrapSpace;
		return this;
	}

	public float getWrapSpace () {
		return _wrapSpace;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight to the specified value. */
	public VerticalGroup pad (float pad) {
		_padTop = pad;
		_padLeft = pad;
		_padBottom = pad;
		_padRight = pad;
		return this;
	}

	public VerticalGroup pad (float top, float left, float bottom, float right) {
		_padTop = top;
		_padLeft = left;
		_padBottom = bottom;
		_padRight = right;
		return this;
	}

	public VerticalGroup padTop (float padTop) {
		this._padTop = padTop;
		return this;
	}

	public VerticalGroup padLeft (float padLeft) {
		this._padLeft = padLeft;
		return this;
	}

	public VerticalGroup padBottom (float padBottom) {
		this._padBottom = padBottom;
		return this;
	}

	public VerticalGroup padRight (float padRight) {
		this._padRight = padRight;
		return this;
	}

	public float getPadTop () {
		return _padTop;
	}

	public float getPadLeft () {
		return _padLeft;
	}

	public float getPadBottom () {
		return _padBottom;
	}

	public float getPadRight () {
		return _padRight;
	}

	/** Sets the alignment of all widgets within the vertical group. Set to {@link Align#center}, {@link Align#top},
	 * {@link Align#bottom}, {@link Align#left}, {@link Align#right}, or any combination of those. */
	public VerticalGroup align (int align) {
		this._align = align;
		return this;
	}

	/** Sets the alignment of all widgets within the vertical group to {@link Align#center}. This clears any other alignment. */
	public VerticalGroup center () {
		_align = Align.center;
		return this;
	}

	/** Sets {@link Align#top} and clears {@link Align#bottom} for the alignment of all widgets within the vertical group. */
	public VerticalGroup top () {
		_align |= Align.top;
		_align &= ~Align.bottom;
		return this;
	}

	/** Adds {@link Align#left} and clears {@link Align#right} for the alignment of all widgets within the vertical group. */
	public VerticalGroup left () {
		_align |= Align.left;
		_align &= ~Align.right;
		return this;
	}

	/** Sets {@link Align#bottom} and clears {@link Align#top} for the alignment of all widgets within the vertical group. */
	public VerticalGroup bottom () {
		_align |= Align.bottom;
		_align &= ~Align.top;
		return this;
	}

	/** Adds {@link Align#right} and clears {@link Align#left} for the alignment of all widgets within the vertical group. */
	public VerticalGroup right () {
		_align |= Align.right;
		_align &= ~Align.left;
		return this;
	}

	public int getAlign () {
		return _align;
	}

	public VerticalGroup fill () {
		_fill = 1f;
		return this;
	}

	/** @param fill 0 will use preferred height. */
	public VerticalGroup fill (float fill) {
		this._fill = fill;
		return this;
	}

	public float getFill () {
		return _fill;
	}

	public VerticalGroup expand () {
		_expand = true;
		return this;
	}

	/** When true and wrap is false, the columns will take up the entire vertical group width. */
	public VerticalGroup expand (bool expand) {
		this._expand = expand;
		return this;
	}

	public bool getExpand () {
		return _expand;
	}

	/** Sets fill to 1 and expand to true. */
	public VerticalGroup grow () {
		_expand = true;
		_fill = 1;
		return this;
	}

	/** If false, the widgets are arranged in a single column and the preferred height is the widget heights plus spacing.
	 * <p>
	 * If true, the widgets will wrap using the height of the vertical group. The preferred height of the group will be 0 as it is
	 * expected that something external will set the height of the group. Widgets are sized to their preferred height unless it is
	 * larger than the group's height, in which case they are sized to the group's height but not less than their minimum height.
	 * Default is false.
	 * <p>
	 * When wrap is enabled, the group's preferred width depends on the height of the group. In some cases the parent of the group
	 * will need to layout twice: once to set the height of the group and a second time to adjust to the group's new preferred
	 * width. */
	public VerticalGroup wrap () {
		_wrap = true;
		return this;
	}

	public VerticalGroup wrap (bool wrap) {
		this._wrap = wrap;
		return this;
	}

	public bool getWrap () {
		return _wrap;
	}

	/** Sets the vertical alignment of each column of widgets when {@link #wrap() wrapping} is enabled and sets the horizontal
	 * alignment of widgets within each column. Set to {@link Align#center}, {@link Align#top}, {@link Align#bottom},
	 * {@link Align#left}, {@link Align#right}, or any combination of those. */
	public VerticalGroup columnAlign (int columnAlign) {
		this._columnAlign = columnAlign;
		return this;
	}

	/** Sets the alignment of widgets within each column to {@link Align#center}. This clears any other alignment. */
	public VerticalGroup columnCenter () {
		_columnAlign = Align.center;
		return this;
	}

	/** Adds {@link Align#top} and clears {@link Align#bottom} for the alignment of each column of widgets when {@link #wrap()
	 * wrapping} is enabled. */
	public VerticalGroup columnTop () {
		_columnAlign |= Align.top;
		_columnAlign &= ~Align.bottom;
		return this;
	}

	/** Adds {@link Align#left} and clears {@link Align#right} for the alignment of widgets within each column. */
	public VerticalGroup columnLeft () {
		_columnAlign |= Align.left;
		_columnAlign &= ~Align.right;
		return this;
	}

	/** Adds {@link Align#bottom} and clears {@link Align#top} for the alignment of each column of widgets when {@link #wrap()
	 * wrapping} is enabled. */
	public VerticalGroup columnBottom () {
		_columnAlign |= Align.bottom;
		_columnAlign &= ~Align.top;
		return this;
	}

	/** Adds {@link Align#right} and clears {@link Align#left} for the alignment of widgets within each column. */
	public VerticalGroup columnRight () {
		_columnAlign |= Align.right;
		_columnAlign &= ~Align.left;
		return this;
	}

	protected override void drawDebugBounds (ShapeRenderer shapes) {
		base.drawDebugBounds(shapes);
		if (!getDebug()) return;
		shapes.set(ShapeRenderer.ShapeType.Line);
		if (getStage() != null) shapes.setColor(getStage().getDebugColor());
		shapes.rect(getX() + _padLeft, getY() + _padBottom, getOriginX(), getOriginY(), getWidth() - _padLeft - _padRight,
			getHeight() - _padBottom - _padTop, getScaleX(), getScaleY(), getRotation());
	}
}
