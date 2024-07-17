using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A group that lays out its children side by side horizontally, with optional wrapping. This can be easier than using
 * {@link Table} when actors need to be inserted into or removed from the middle of the group. {@link #getChildren()} can be
 * sorted to change the order of the actors (eg {@link Actor#setZIndex(int)}). {@link #invalidate()} must be called after changing
 * the children order.
 * <p>
 * The preferred width is the sum of the children's preferred widths plus spacing. The preferred height is the largest preferred
 * height of any child. The preferred size is slightly different when {@link #wrap() wrap} is enabled. The min size is the
 * preferred size and the max size is 0.
 * <p>
 * Widgets are sized using their {@link Layout#getPrefWidth() preferred width}, so widgets which return 0 as their preferred width
 * will be given a width of 0 (eg, a label with {@link Label#setWrap(boolean) word wrap} enabled).
 * @author Nathan Sweet */
public class HorizontalGroup : WidgetGroup {
	private float prefWidth, prefHeight, lastPrefHeight;
	private bool sizeInvalid = true;
	private FloatArray rowSizes; // row width, row height, ...

	private int _align = Align.left, _rowAlign;
	private bool _reverse, _round = true, _wrap, _wrapReverse, _expand;
	private float _space, _wrapSpace, _fill, _padTop, _padLeft, _padBottom, _padRight;

	public HorizontalGroup () {
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
		prefHeight = 0;
		if (_wrap) {
			prefWidth = 0;
			if (this.rowSizes == null)
				this.rowSizes = new FloatArray();
			else
				this.rowSizes.clear();
			FloatArray rowSizes = this.rowSizes;
			float space = this._space, wrapSpace = this._wrapSpace;
			float pad = _padLeft + _padRight, groupWidth = getWidth() - pad, x = 0, y = 0, rowHeight = 0;
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
					if (width > groupWidth) width = Math.Max(groupWidth, layout.getMinWidth());
					height = layout.getPrefHeight();
				} else {
					width = child.getWidth();
					height = child.getHeight();
				}

				float incrX = width + (x > 0 ? space : 0);
				if (x + incrX > groupWidth && x > 0) {
					rowSizes.add(x);
					rowSizes.add(rowHeight);
					prefWidth = Math.Max(prefWidth, x + pad);
					if (y > 0) y += wrapSpace;
					y += rowHeight;
					rowHeight = 0;
					x = 0;
					incrX = width;
				}
				x += incrX;
				rowHeight = Math.Max(rowHeight, height);
			}
			rowSizes.add(x);
			rowSizes.add(rowHeight);
			prefWidth = Math.Max(prefWidth, x + pad);
			if (y > 0) y += wrapSpace;
			prefHeight = Math.Max(prefHeight, y + rowHeight);
		} else {
			prefWidth = _padLeft + _padRight + _space * (n - 1);
			for (int i = 0; i < n; i++) {
				Actor child = children.get(i);
				if (child is ILayout) {
					ILayout layout = (ILayout)child;
					prefWidth += layout.getPrefWidth();
					prefHeight = Math.Max(prefHeight, layout.getPrefHeight());
				} else {
					prefWidth += child.getWidth();
					prefHeight = Math.Max(prefHeight, child.getHeight());
				}
			}
		}
		prefHeight += _padTop + _padBottom;
		if (_round) {
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

		bool round = this._round;
		int align = this._align;
		float space = this._space, padBottom = this._padBottom, fill = this._fill;
		float rowHeight = (_expand ? getHeight() : prefHeight) - _padTop - padBottom, x = _padLeft;

		if ((align & Align.right) != 0)
			x += getWidth() - prefWidth;
		else if ((align & Align.left) == 0) // center
			x += (getWidth() - prefWidth) / 2;

		float startY;
		if ((align & Align.bottom) != 0)
			startY = padBottom;
		else if ((align & Align.top) != 0)
			startY = getHeight() - _padTop - rowHeight;
		else
			startY = padBottom + (getHeight() - padBottom - _padTop - rowHeight) / 2;

		align = _rowAlign;

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

			if (fill > 0) height = rowHeight * fill;

			if (layout != null) {
				height = Math.Max(height, layout.getMinHeight());
				float maxHeight = layout.getMaxHeight();
				if (maxHeight > 0 && height > maxHeight) height = maxHeight;
			}

			float y = startY;
			if ((align & Align.top) != 0)
				y += rowHeight - height;
			else if ((align & Align.bottom) == 0) // center
				y += (rowHeight - height) / 2;

			if (round)
				child.setBounds((float)Math.Round(x), (float)Math.Round(y), (float)Math.Round(width), (float)Math.Round(height));
			else
				child.setBounds(x, y, width, height);
			x += width + space;

			if (layout != null) layout.validate();
		}
	}

	private void layoutWrapped () {
		float prefHeight = getPrefHeight();
		if (prefHeight != lastPrefHeight) {
			lastPrefHeight = prefHeight;
			invalidateHierarchy();
		}

		int align = this._align;
		bool round = this._round;
		float space = this._space, fill = this._fill, wrapSpace = this._wrapSpace;
		float maxWidth = prefWidth - _padLeft - _padRight;
		float rowY = prefHeight - _padTop, groupWidth = getWidth(), xStart = _padLeft, x = 0, rowHeight = 0, rowDir = -1;

		if ((align & Align.top) != 0)
			rowY += getHeight() - prefHeight;
		else if ((align & Align.bottom) == 0) // center
			rowY += (getHeight() - prefHeight) / 2;
		if (_wrapReverse) {
			rowY -= prefHeight + this.rowSizes.get(1);
			rowDir = 1;
		}

		if ((align & Align.right) != 0)
			xStart += groupWidth - prefWidth;
		else if ((align & Align.left) == 0) // center
			xStart += (groupWidth - prefWidth) / 2;

		groupWidth -= _padRight;
		align = this._rowAlign;

		FloatArray rowSizes = this.rowSizes;
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
				if (width > groupWidth) width = Math.Max(groupWidth, layout.getMinWidth());
				height = layout.getPrefHeight();
			} else {
				width = child.getWidth();
				height = child.getHeight();
			}

			if (x + width > groupWidth || r == 0) {
				r = Math.Min(r, rowSizes.size - 2); // In case an actor changed size without invalidating this layout.
				x = xStart;
				if ((align & Align.right) != 0)
					x += maxWidth - rowSizes.get(r);
				else if ((align & Align.left) == 0) // center
					x += (maxWidth - rowSizes.get(r)) / 2;
				rowHeight = rowSizes.get(r + 1);
				if (r > 0) rowY += wrapSpace * rowDir;
				rowY += rowHeight * rowDir;
				r += 2;
			}

			if (fill > 0) height = rowHeight * fill;

			if (layout != null) {
				height = Math.Max(height, layout.getMinHeight());
				float maxHeight = layout.getMaxHeight();
				if (maxHeight > 0 && height > maxHeight) height = maxHeight;
			}

			float y = rowY;
			if ((align & Align.top) != 0)
				y += rowHeight - height;
			else if ((align & Align.bottom) == 0) // center
				y += (rowHeight - height) / 2;

			if (round)
				child.setBounds((float)Math.Round(x), (float)Math.Round(y), (float)Math.Round(width), (float)Math.Round(height));
			else
				child.setBounds(x, y, width, height);
			x += width + space;

			if (layout != null) layout.validate();
		}
	}

	public override float getPrefWidth () {
		if (_wrap) return 0;
		if (sizeInvalid) computeSize();
		return prefWidth;
	}

	public override float getPrefHeight () {
		if (sizeInvalid) computeSize();
		return prefHeight;
	}

	/** When wrapping is enabled, the number of rows may be > 1. */
	public int getRows () {
		return _wrap ? rowSizes.size >> 1 : 1;
	}

	/** If true (the default), positions and sizes are rounded to integers. */
	public void setRound (bool round) {
		this._round = round;
	}

	/** The children will be displayed last to first. */
	public HorizontalGroup reverse () {
		_reverse = true;
		return this;
	}

	/** If true, the children will be displayed last to first. */
	public HorizontalGroup reverse (bool reverse) {
		this._reverse = reverse;
		return this;
	}

	public bool getReverse () {
		return _reverse;
	}

	/** Rows will wrap above the previous rows. */
	public HorizontalGroup wrapReverse () {
		_wrapReverse = true;
		return this;
	}

	/** If true, rows will wrap above the previous rows. */
	public HorizontalGroup wrapReverse (bool wrapReverse) {
		this._wrapReverse = wrapReverse;
		return this;
	}

	public bool getWrapReverse () {
		return _wrapReverse;
	}

	/** Sets the horizontal space between children. */
	public HorizontalGroup space (float space) {
		this._space = space;
		return this;
	}

	public float getSpace () {
		return _space;
	}

	/** Sets the vertical space between rows when wrap is enabled. */
	public HorizontalGroup wrapSpace (float wrapSpace) {
		this._wrapSpace = wrapSpace;
		return this;
	}

	public float getWrapSpace () {
		return _wrapSpace;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight to the specified value. */
	public HorizontalGroup pad (float pad) {
		_padTop = pad;
		_padLeft = pad;
		_padBottom = pad;
		_padRight = pad;
		return this;
	}

	public HorizontalGroup pad (float top, float left, float bottom, float right) {
		_padTop = top;
		_padLeft = left;
		_padBottom = bottom;
		_padRight = right;
		return this;
	}

	public HorizontalGroup padTop (float padTop) {
		this._padTop = padTop;
		return this;
	}

	public HorizontalGroup padLeft (float padLeft) {
		this._padLeft = padLeft;
		return this;
	}

	public HorizontalGroup padBottom (float padBottom) {
		this._padBottom = padBottom;
		return this;
	}

	public HorizontalGroup padRight (float padRight) {
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

	/** Sets the alignment of all widgets within the horizontal group. Set to {@link Align#center}, {@link Align#top},
	 * {@link Align#bottom}, {@link Align#left}, {@link Align#right}, or any combination of those. */
	public HorizontalGroup align (int align) {
		this._align = align;
		return this;
	}

	/** Sets the alignment of all widgets within the horizontal group to {@link Align#center}. This clears any other alignment. */
	public HorizontalGroup center () {
		_align = Align.center;
		return this;
	}

	/** Sets {@link Align#top} and clears {@link Align#bottom} for the alignment of all widgets within the horizontal group. */
	public HorizontalGroup top () {
		_align |= Align.top;
		_align &= ~Align.bottom;
		return this;
	}

	/** Adds {@link Align#left} and clears {@link Align#right} for the alignment of all widgets within the horizontal group. */
	public HorizontalGroup left () {
		_align |= Align.left;
		_align &= ~Align.right;
		return this;
	}

	/** Sets {@link Align#bottom} and clears {@link Align#top} for the alignment of all widgets within the horizontal group. */
	public HorizontalGroup bottom () {
		_align |= Align.bottom;
		_align &= ~Align.top;
		return this;
	}

	/** Adds {@link Align#right} and clears {@link Align#left} for the alignment of all widgets within the horizontal group. */
	public HorizontalGroup right () {
		_align |= Align.right;
		_align &= ~Align.left;
		return this;
	}

	public int getAlign () {
		return _align;
	}

	public HorizontalGroup fill () {
		_fill = 1f;
		return this;
	}

	/** @param fill 0 will use preferred width. */
	public HorizontalGroup fill (float fill) {
		this._fill = fill;
		return this;
	}

	public float getFill () {
		return _fill;
	}

	public HorizontalGroup expand () {
		_expand = true;
		return this;
	}

	/** When true and wrap is false, the rows will take up the entire horizontal group height. */
	public HorizontalGroup expand (bool expand) {
		this._expand = expand;
		return this;
	}

	public bool getExpand () {
		return _expand;
	}

	/** Sets fill to 1 and expand to true. */
	public HorizontalGroup grow () {
		_expand = true;
		_fill = 1;
		return this;
	}

	/** If false, the widgets are arranged in a single row and the preferred width is the widget widths plus spacing.
	 * <p>
	 * If true, the widgets will wrap using the width of the horizontal group. The preferred width of the group will be 0 as it is
	 * expected that something external will set the width of the group. Widgets are sized to their preferred width unless it is
	 * larger than the group's width, in which case they are sized to the group's width but not less than their minimum width.
	 * Default is false.
	 * <p>
	 * When wrap is enabled, the group's preferred height depends on the width of the group. In some cases the parent of the group
	 * will need to layout twice: once to set the width of the group and a second time to adjust to the group's new preferred
	 * height. */
	public HorizontalGroup wrap () {
		_wrap = true;
		return this;
	}

	public HorizontalGroup wrap (bool wrap) {
		this._wrap = wrap;
		return this;
	}

	public bool getWrap () {
		return _wrap;
	}

	/** Sets the horizontal alignment of each row of widgets when {@link #wrap() wrapping} is enabled and sets the vertical
	 * alignment of widgets within each row. Set to {@link Align#center}, {@link Align#top}, {@link Align#bottom},
	 * {@link Align#left}, {@link Align#right}, or any combination of those. */
	public HorizontalGroup rowAlign (int rowAlign) {
		this._rowAlign = rowAlign;
		return this;
	}

	/** Sets the alignment of widgets within each row to {@link Align#center}. This clears any other alignment. */
	public HorizontalGroup rowCenter () {
		_rowAlign = Align.center;
		return this;
	}

	/** Sets {@link Align#top} and clears {@link Align#bottom} for the alignment of widgets within each row. */
	public HorizontalGroup rowTop () {
		_rowAlign |= Align.top;
		_rowAlign &= ~Align.bottom;
		return this;
	}

	/** Adds {@link Align#left} and clears {@link Align#right} for the alignment of each row of widgets when {@link #wrap()
	 * wrapping} is enabled. */
	public HorizontalGroup rowLeft () {
		_rowAlign |= Align.left;
		_rowAlign &= ~Align.right;
		return this;
	}

	/** Sets {@link Align#bottom} and clears {@link Align#top} for the alignment of widgets within each row. */
	public HorizontalGroup rowBottom () {
		_rowAlign |= Align.bottom;
		_rowAlign &= ~Align.top;
		return this;
	}

	/** Adds {@link Align#right} and clears {@link Align#left} for the alignment of each row of widgets when {@link #wrap()
	 * wrapping} is enabled. */
	public HorizontalGroup rowRight () {
		_rowAlign |= Align.right;
		_rowAlign &= ~Align.left;
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
