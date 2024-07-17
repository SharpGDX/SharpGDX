using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.UI;
using Fixed = SharpGDX.Scenes.Scene2D.UI.Value.Fixed;
using SharpGDX.Shims;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Scenes.Scene2D.UI
{
	/** A group that sizes and positions children using table constraints.
 * <p>
 * Children added with {@link #add(Actor...)} (and similar methods returning a {@link Cell}) are laid out in rows and columns.
 * Other children may be added with {@link #addActor(Actor)} (and similar methods) but are not laid out automatically and don't
 * affect the preferred or minimum sizes.
 * <p>
 * By default, {@link #getTouchable()} is {@link Touchable#childrenOnly}.
 * <p>
 * The preferred and minimum sizes are that of the children laid out in columns and rows.
 * @author Nathan Sweet */
public class Table : WidgetGroup {
	static public Color debugTableColor = new Color(0, 0, 1, 1);
	static public Color debugCellColor = new Color(1, 0, 0, 1);
	static public Color debugActorColor = new Color(0, 1, 0, 1);

	private static readonly Pool<Cell> cellPool = new CellPool()
	{
	};

	private class CellPool : Pool<Cell>
	{
		protected override Cell newObject()
		{
			throw new NotImplementedException();
				// TODO: return new Cell();
		}
	}
	static private float[] columnWeightedWidth, rowWeightedHeight;

	private int columns, rows;
	private bool implicitEndRow;

	private readonly Array<Cell> cells = new (4);
	private readonly Cell cellDefaults;
	private readonly Array<Cell> _columnDefaults = new (2);
	private Cell rowDefaults;

	private bool sizeInvalid = true;
	private float[] columnMinWidth, rowMinHeight;
	private float[] columnPrefWidth, rowPrefHeight;
	private float tableMinWidth, tableMinHeight;
	private float tablePrefWidth, tablePrefHeight;
	private float[] columnWidth, rowHeight;
	private float[] expandWidth, expandHeight;

	Value _padTop = backgroundTop, _padLeft = backgroundLeft, _padBottom = backgroundBottom, _padRight = backgroundRight;
	int _align = Align.center;

	Debug _debug = Debug.none;
	Array<DebugRect> debugRects;

	 IDrawable? _background;
	private bool _clip;
	private  Skin? skin;
	bool round = true;

	public Table () 
	: this(null)
	{
		
	}

	/** Creates a table with a skin, which is required to use {@link #add(CharSequence)} or {@link #add(CharSequence, String)}. */
	public Table (Skin? skin) {
		this.skin = skin;

		cellDefaults = obtainCell();

		setTransform(false);
		setTouchable(Touchable.childrenOnly);
	}

		private Cell obtainCell () {
		Cell cell = cellPool.obtain();
		cell.setTable(this);
		return cell;
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
		protected virtual void drawBackground (IBatch batch, float parentAlpha, float x, float y) {
		if (_background == null) return;
		Color color = getColor();
		batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);
		_background.draw(batch, x, y, getWidth(), getHeight());
	}

	/** Sets the background drawable from the skin and adjusts the table's padding to match the background. This may only be called
	 * if a skin has been set with {@link Table#Table(Skin)} or {@link #setSkin(Skin)}.
	 * @see #setBackground(Drawable) */
	public void setBackground (String drawableName) {
		if (skin == null) throw new IllegalStateException("Table must have a skin set to use this method.");
		setBackground(skin.getDrawable(drawableName));
	}

	/** @param background May be null to clear the background. */
	public void setBackground (IDrawable? background) {
		if (this._background == background) return;
		float padTopOld = getPadTop(), padLeftOld = getPadLeft(), padBottomOld = getPadBottom(), padRightOld = getPadRight();
		this._background = background; // The default pad values use the background's padding.
		float padTopNew = getPadTop(), padLeftNew = getPadLeft(), padBottomNew = getPadBottom(), padRightNew = getPadRight();
		if (padTopOld + padBottomOld != padTopNew + padBottomNew || padLeftOld + padRightOld != padLeftNew + padRightNew)
			invalidateHierarchy();
		else if (padTopOld != padTopNew || padLeftOld != padLeftNew || padBottomOld != padBottomNew || padRightOld != padRightNew)
			invalidate();
	}

	/** @see #setBackground(Drawable) */
	public Table background (IDrawable? background) {
		setBackground(background);
		return this;
	}

	/** @see #setBackground(String) */
	public Table background (String drawableName) {
		setBackground(drawableName);
		return this;
	}

	public IDrawable? getBackground () {
		return _background;
	}

		public override Actor? hit (float x, float y, bool touchable) {
		if (_clip) {
			if (touchable && getTouchable() == Touchable.disabled) return null;
			if (x < 0 || x >= getWidth() || y < 0 || y >= getHeight()) return null;
		}
		return base.hit(x, y, touchable);
	}

	/** Sets {@link #setClip(boolean)} to true. */
	public Table clip () {
		setClip(true);
		return this;
	}

	public Table clip (bool enabled) {
		setClip(enabled);
		return this;
	}

	/** Causes the contents to be clipped if they exceed the table's bounds. Enabling clipping sets {@link #setTransform(boolean)}
	 * to true. */
	public void setClip (bool enabled) {
		_clip = enabled;
		setTransform(enabled);
		invalidate();
	}

	public bool getClip () {
		return _clip;
	}

		public override void invalidate () {
		sizeInvalid = true;
		base.invalidate();
	}

	/** Adds a new cell to the table with the specified actor. */
	public  Cell<T> add<T> ( T? actor)
	where T: Actor{
		// TODO: No way this works for 'T' as obtainCell is not generic. -RP
		var cell = obtainCell();
		cell.actor = actor;

		// The row was ended for layout, not by the user, so revert it.
		if (implicitEndRow) {
			implicitEndRow = false;
			rows--;
			cells.peek().endRow = false;
		}

		int cellCount = cells.size;
		if (cellCount > 0) {
			// Set cell column and row.
			Cell lastCell = cells.peek();
			if (!lastCell.endRow) {
				cell.column = lastCell.column + lastCell.colspan ?? 0;
				cell.row = lastCell.row;
			} else {
				cell.column = 0;
				cell.row = lastCell.row + 1;
			}
			// Set the index of the cell above.
			if (cell.row > 0) {
				Object[] cells = this.cells.items;
				
				for (int i = cellCount - 1; i >= 0; i--) {
					Cell other = (Cell)cells[i];
					for (int column = other.column, nn = column + other.colspan ?? 0; column < nn; column++) {
						if (column == cell.column) {
							cell.cellAboveIndex = i;
							goto outer;
						}
					}
					
				}
				outer:{}
			}
		} else {
			cell.column = 0;
			cell.row = 0;
		}
		cells.add(cell);

		cell.Set(cellDefaults);
		if (cell.column < _columnDefaults.size) cell.Merge(_columnDefaults.get(cell.column));
		cell.Merge(rowDefaults);

		if (actor != null) addActor(actor);

		return (Cell<T>)cell;
	}

	public Table add (Actor[] actors) {
		for (int i = 0, n = actors.Length; i < n; i++)
			add(actors[i]);
		return this;
	}

	/** Adds a new cell with a label. This may only be called if a skin has been set with {@link Table#Table(Skin)} or
	 * {@link #setSkin(Skin)}. */
	public Cell<Label> add (String? text) {
		if (skin == null) throw new IllegalStateException("Table must have a skin set to use this method.");
		return add(new Label(text, skin));
	}

	/** Adds a new cell with a label. This may only be called if a skin has been set with {@link Table#Table(Skin)} or
	 * {@link #setSkin(Skin)}. */
	public Cell<Label> add (String? text, String labelStyleName) {
		if (skin == null) throw new IllegalStateException("Table must have a skin set to use this method.");
		return add(new Label(text, skin.get<Label.LabelStyle>(labelStyleName, typeof(Label.LabelStyle))));
	}

	/** Adds a new cell with a label. This may only be called if a skin has been set with {@link Table#Table(Skin)} or
	 * {@link #setSkin(Skin)}. */
	public Cell<Label> add (String? text, String fontName,  Color? color) {
		if (skin == null) throw new IllegalStateException("Table must have a skin set to use this method.");
		return add(new Label(text, new Label.LabelStyle(skin.getFont(fontName), color)));
	}

	/** Adds a new cell with a label. This may only be called if a skin has been set with {@link Table#Table(Skin)} or
	 * {@link #setSkin(Skin)}. */
	public Cell<Label> add (String? text, String fontName, String colorName) {
		if (skin == null) throw new IllegalStateException("Table must have a skin set to use this method.");
		return add(new Label(text, new Label.LabelStyle(skin.getFont(fontName), skin.getColor(colorName))));
	}

	/** Adds a cell without an actor. */
	public Cell add () {
		return add((Actor)null);
	}

	/** Adds a new cell to the table with the specified actors in a {@link Stack}.
	 * @param actors May be null or empty to add a stack without any actors. */
	public Cell<Stack> stack(Actor[]? actors) {
		Stack stack = new Stack();
		if (actors != null) {
			for (int i = 0, n = actors.Length; i < n; i++)
				stack.addActor(actors[i]);
		}
		return add(stack);
	}

		public override bool removeActor (Actor actor) {
		return removeActor(actor, true);
	}

		public override bool removeActor (Actor actor, bool unfocus) {
		if (!base.removeActor(actor, unfocus)) return false;
		Cell cell = getCell(actor);
		if (cell != null) cell.actor = null;
		return true;
	}

		public override Actor removeActorAt (int index, bool unfocus) {
		Actor actor = base.removeActorAt(index, unfocus);
		Cell cell = getCell(actor);
		if (cell != null) cell.actor = null;
		return actor;
	}

		/** Removes all actors and cells from the table. */
		public override void clearChildren (bool unfocus) {
		Object[] cells = this.cells.items;
		for (int i = this.cells.size - 1; i >= 0; i--) {
			Cell cell = (Cell)cells[i];
			Actor actor = cell.actor;
			if (actor != null) actor.remove();
		}
		cellPool.freeAll(this.cells);
		this.cells.clear();
		rows = 0;
		columns = 0;
		if (rowDefaults != null) cellPool.free(rowDefaults);
		rowDefaults = null;
		implicitEndRow = false;

		base.clearChildren(unfocus);
	}

	/** Removes all actors and cells from the table (same as {@link #clearChildren()}) and additionally resets all table properties
	 * and cell, column, and row defaults. */
	public void reset () {
		clearChildren();
		_padTop = backgroundTop;
		_padLeft = backgroundLeft;
		_padBottom = backgroundBottom;
		_padRight = backgroundRight;
		_align = Align.center;
		debug(Debug.none);
		cellDefaults.reset();
		for (int i = 0, n = _columnDefaults.size; i < n; i++) {
			Cell columnCell = _columnDefaults.get(i);
			if (columnCell != null) cellPool.free(columnCell);
		}
		_columnDefaults.clear();
	}

	/** Indicates that subsequent cells should be added to a new row and returns the cell values that will be used as the defaults
	 * for all cells in the new row. */
	public Cell row () {
		if (cells.size > 0) {
			if (!implicitEndRow) {
				if (cells.peek().endRow) return rowDefaults; // Row was already ended.
				endRow();
			}
			invalidate();
		}
		implicitEndRow = false;
		if (rowDefaults != null) cellPool.free(rowDefaults);
		rowDefaults = obtainCell();
		rowDefaults.Clear();
		return rowDefaults;
	}

	private void endRow () {
		Object[] cells = this.cells.items;
		int rowColumns = 0;
		for (int i = this.cells.size - 1; i >= 0; i--) {
			Cell cell = (Cell)cells[i];
			if (cell.endRow) break;
			rowColumns += cell.colspan ?? 0;
		}
		columns = Math.Max(columns, rowColumns);
		rows++;
		this.cells.peek().endRow = true;
	}

	/** Gets the cell values that will be used as the defaults for all cells in the specified column. Columns are indexed starting
	 * at 0. */
	public Cell columnDefaults (int column) {
		Cell cell = _columnDefaults.size > column ? _columnDefaults.get(column) : null;
		if (cell == null) {
			cell = obtainCell();
			cell.Clear();
			if (column >= _columnDefaults.size) {
				for (int i = _columnDefaults.size; i < column; i++)
					_columnDefaults.add(null);
				_columnDefaults.add(cell);
			} else
				_columnDefaults.set(column, cell);
		}
		return cell;
	}

	/** Returns the cell for the specified actor in this table, or null. */
	public  Cell<T>? getCell<T> (T actor) 
	where T: Actor{
		if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
		var cells = this.cells.items;
		for (int i = 0, n = this.cells.size; i < n; i++) {
			Cell c = (Cell)cells[i];
			if (c.actor == actor) return (Cell<T>?)c;
		}
		return null;
	}

	/** Returns the cells for this table. */
	public Array<Cell> getCells () {
		return cells;
	}

		public override float getPrefWidth () {
		if (sizeInvalid) computeSize();
		float width = tablePrefWidth;
		if (_background != null) return Math.Max(width, _background.getMinWidth());
		return width;
	}

		public override float getPrefHeight () {
		if (sizeInvalid) computeSize();
		float height = tablePrefHeight;
		if (_background != null) return Math.Max(height, _background.getMinHeight());
		return height;
	}

		public override float getMinWidth () {
		if (sizeInvalid) computeSize();
		return tableMinWidth;
	}

		public override float getMinHeight () {
		if (sizeInvalid) computeSize();
		return tableMinHeight;
	}

	/** The cell values that will be used as the defaults for all cells. */
	public Cell defaults () {
		return cellDefaults;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight around the table to the specified value. */
	public Table pad (Value pad) {
		if (pad == null) throw new IllegalArgumentException("pad cannot be null.");
		_padTop = pad;
		_padLeft = pad;
		_padBottom = pad;
		_padRight = pad;
		sizeInvalid = true;
		return this;
	}

	public Table pad (Value top, Value left, Value bottom, Value right) {
		if (top == null) throw new IllegalArgumentException("top cannot be null.");
		if (left == null) throw new IllegalArgumentException("left cannot be null.");
		if (bottom == null) throw new IllegalArgumentException("bottom cannot be null.");
		if (right == null) throw new IllegalArgumentException("right cannot be null.");
		_padTop = top;
		_padLeft = left;
		_padBottom = bottom;
		_padRight = right;
		sizeInvalid = true;
		return this;
	}

	/** Padding at the top edge of the table. */
	public Table padTop (Value padTop) {
		if (padTop == null) throw new IllegalArgumentException("padTop cannot be null.");
		this._padTop = padTop;
		sizeInvalid = true;
		return this;
	}

	/** Padding at the left edge of the table. */
	public Table padLeft (Value padLeft) {
		if (padLeft == null) throw new IllegalArgumentException("padLeft cannot be null.");
		this._padLeft = padLeft;
		sizeInvalid = true;
		return this;
	}

	/** Padding at the bottom edge of the table. */
	public Table padBottom (Value padBottom) {
		if (padBottom == null) throw new IllegalArgumentException("padBottom cannot be null.");
		this._padBottom = padBottom;
		sizeInvalid = true;
		return this;
	}

	/** Padding at the right edge of the table. */
	public Table padRight (Value padRight) {
		if (padRight == null) throw new IllegalArgumentException("padRight cannot be null.");
		this._padRight = padRight;
		sizeInvalid = true;
		return this;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight around the table to the specified value. */
	public Table pad (float pad) {
		this.pad(Fixed.valueOf(pad));
		return this;
	}

	public Table pad (float top, float left, float bottom, float right) {
		_padTop = Fixed.valueOf(top);
		_padLeft = Fixed.valueOf(left);
		_padBottom = Fixed.valueOf(bottom);
		_padRight = Fixed.valueOf(right);
		sizeInvalid = true;
		return this;
	}

	/** Padding at the top edge of the table. */
	public Table padTop (float padTop) {
		this._padTop = Fixed.valueOf(padTop);
		sizeInvalid = true;
		return this;
	}

	/** Padding at the left edge of the table. */
	public Table padLeft (float padLeft) {
		this._padLeft = Fixed.valueOf(padLeft);
		sizeInvalid = true;
		return this;
	}

	/** Padding at the bottom edge of the table. */
	public Table padBottom (float padBottom) {
		this._padBottom = Fixed.valueOf(padBottom);
		sizeInvalid = true;
		return this;
	}

	/** Padding at the right edge of the table. */
	public Table padRight (float padRight) {
		this._padRight = Fixed.valueOf(padRight);
		sizeInvalid = true;
		return this;
	}

	/** Alignment of the logical table within the table actor. Set to {@link Align#center}, {@link Align#top}, {@link Align#bottom}
	 * , {@link Align#left}, {@link Align#right}, or any combination of those. */
	public Table align (int align) {
		this._align = align;
		return this;
	}

	/** Sets the alignment of the logical table within the table actor to {@link Align#center}. This clears any other alignment. */
	public Table center () {
		_align = Align.center;
		return this;
	}

	/** Adds {@link Align#top} and clears {@link Align#bottom} for the alignment of the logical table within the table actor. */
	public Table top () {
		_align |= Align.top;
		_align &= ~Align.bottom;
		return this;
	}

	/** Adds {@link Align#left} and clears {@link Align#right} for the alignment of the logical table within the table actor. */
	public Table left () {
		_align |= Align.left;
		_align &= ~Align.right;
		return this;
	}

	/** Adds {@link Align#bottom} and clears {@link Align#top} for the alignment of the logical table within the table actor. */
	public Table bottom () {
		_align |= Align.bottom;
		_align &= ~Align.top;
		return this;
	}

	/** Adds {@link Align#right} and clears {@link Align#left} for the alignment of the logical table within the table actor. */
	public Table right () {
		_align |= Align.right;
		_align &= ~Align.left;
		return this;
	}

		public override void setDebug (bool enabled) {
		debug(enabled ? Debug.all : Debug.none);
	}

		public override Table debug () {
		base.debug();
		return this;
	}

		public override Table debugAll () {
		base.debugAll();
		return this;
	}

	/** Turns on table debug lines. */
	public Table debugTable () {
		base.setDebug(true);
		if (_debug != Debug.table) {
			this._debug = Debug.table;
			invalidate();
		}
		return this;
	}

	/** Turns on cell debug lines. */
	public Table debugCell () {
		base.setDebug(true);
		if (_debug != Debug.cell) {
			this._debug = Debug.cell;
			invalidate();
		}
		return this;
	}

	/** Turns on actor debug lines. */
	public Table debugActor () {
		base.setDebug(true);
		if (_debug != Debug.actor) {
			this._debug = Debug.actor;
			invalidate();
		}
		return this;
	}

	/** Turns debug lines on or off. */
	public Table debug (Debug debug) {
		base.setDebug(debug != Debug.none);
		if (this._debug != debug) {
			this._debug = debug;
			if (debug == Debug.none)
				clearDebugRects();
			else
				invalidate();
		}
		return this;
	}

	public Debug getTableDebug () {
		return _debug;
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

	public int getAlign () {
		return _align;
	}

	/** Returns the row index for the y coordinate, or -1 if not over a row.
	 * @param y The y coordinate, where 0 is the top of the table. */
	public int getRow (float y) {
		int n = this.cells.size;
		if (n == 0) return -1;
		y += getPadTop();
		Object[] cells = this.cells.items;
		for (int i = 0, row = 0; i < n;) {
			Cell c = (Cell)cells[i++];
			if (c.actorY + c.computedPadTop < y) return row;
			if (c.endRow) row++;
		}
		return -1;
	}

	public void setSkin (Skin? skin) {
		this.skin = skin;
	}

	/** If true (the default), positions and sizes of child actors are rounded and ceiled to the nearest integer value. */
	public void setRound (bool round) {
		this.round = round;
	}

	public int getRows () {
		return rows;
	}

	public int getColumns () {
		return columns;
	}

	/** Returns the height of the specified row, or 0 if the table layout has not been validated. */
	public float getRowHeight (int rowIndex) {
		if (rowHeight == null) return 0;
		return rowHeight[rowIndex];
	}

	/** Returns the min height of the specified row. */
	public float getRowMinHeight (int rowIndex) {
		if (sizeInvalid) computeSize();
		return rowMinHeight[rowIndex];
	}

	/** Returns the pref height of the specified row. */
	public float getRowPrefHeight (int rowIndex) {
		if (sizeInvalid) computeSize();
		return rowPrefHeight[rowIndex];
	}

	/** Returns the width of the specified column, or 0 if the table layout has not been validated. */
	public float getColumnWidth (int columnIndex) {
		if (columnWidth == null) return 0;
		return columnWidth[columnIndex];
	}

	/** Returns the min height of the specified column. */
	public float getColumnMinWidth (int columnIndex) {
		if (sizeInvalid) computeSize();
		return columnMinWidth[columnIndex];
	}

	/** Returns the pref height of the specified column. */
	public float getColumnPrefWidth (int columnIndex) {
		if (sizeInvalid) computeSize();
		return columnPrefWidth[columnIndex];
	}

	private float[] ensureSize (float[] array, int size) {
		if (array == null || array.Length < size) return new float[size];
		Array.Fill(array, 0, size, 0);
		return array;
	}

	private void computeSize () {
		sizeInvalid = false;

		Object[] cells = this.cells.items;
		int cellCount = this.cells.size;

		// Implicitly end the row for layout purposes.
		if (cellCount > 0 && !((Cell)cells[cellCount - 1]).endRow) {
			endRow();
			implicitEndRow = true;
		}

		int columns = this.columns, rows = this.rows;
		float[] columnMinWidth = this.columnMinWidth = ensureSize(this.columnMinWidth, columns);
		float[] rowMinHeight = this.rowMinHeight = ensureSize(this.rowMinHeight, rows);
		float[] columnPrefWidth = this.columnPrefWidth = ensureSize(this.columnPrefWidth, columns);
		float[] rowPrefHeight = this.rowPrefHeight = ensureSize(this.rowPrefHeight, rows);
		float[] columnWidth = this.columnWidth = ensureSize(this.columnWidth, columns);
		float[] rowHeight = this.rowHeight = ensureSize(this.rowHeight, rows);
		float[] expandWidth = this.expandWidth = ensureSize(this.expandWidth, columns);
		float[] expandHeight = this.expandHeight = ensureSize(this.expandHeight, rows);

		float spaceRightLast = 0;
		for (int i = 0; i < cellCount; i++) {
			Cell c = (Cell)cells[i];
			int column = c.column, row = c.row, colspan = c.colspan ?? 0;
			Actor a = c.actor;

			// Collect rows that expand and colspan=1 columns that expand.
			if (c.expandY != 0 && expandHeight[row] == 0) expandHeight[row] = c.expandY ?? 0;
			if (colspan == 1 && c.expandX != 0 && expandWidth[column] == 0) expandWidth[column] = c.expandX ?? 0;

			// Compute combined padding/spacing for cells.
			// Spacing between actors isn't additive, the larger is used. Also, no spacing around edges.
			c.computedPadLeft = c.padLeft.get(a) + (column == 0 ? 0 : Math.Max(0, c.spaceLeft.get(a) - spaceRightLast));
			c.computedPadTop = c.padTop.get(a);
			if (c.cellAboveIndex != -1) {
				Cell above = (Cell)cells[c.cellAboveIndex];
				c.computedPadTop += Math.Max(0, c.spaceTop.get(a) - above.spaceBottom.get(a));
			}
			float spaceRight = c.spaceRight.get(a);
			c.computedPadRight = c.padRight.get(a) + ((column + colspan) == columns ? 0 : spaceRight);
			c.computedPadBottom = c.padBottom.get(a) + (row == rows - 1 ? 0 : c.spaceBottom.get(a));
			spaceRightLast = spaceRight;

			// Determine minimum and preferred cell sizes.
			float prefWidth = c.prefWidth.get(a), prefHeight = c.prefHeight.get(a);
			float minWidth = c.minWidth.get(a), minHeight = c.minHeight.get(a);
			float maxWidth = c.maxWidth.get(a), maxHeight = c.maxHeight.get(a);
			if (prefWidth < minWidth) prefWidth = minWidth;
			if (prefHeight < minHeight) prefHeight = minHeight;
			if (maxWidth > 0 && prefWidth > maxWidth) prefWidth = maxWidth;
			if (maxHeight > 0 && prefHeight > maxHeight) prefHeight = maxHeight;
			if (round) {
				minWidth = (float)Math.Ceiling(minWidth);
				minHeight = (float)Math.Ceiling(minHeight);
				prefWidth = (float)Math.Ceiling(prefWidth);
				prefHeight = (float)Math.Ceiling(prefHeight);
			}

			if (colspan == 1) { // Spanned column min and pref width is added later.
				float hpadding = c.computedPadLeft + c.computedPadRight;
				columnPrefWidth[column] = Math.Max(columnPrefWidth[column], prefWidth + hpadding);
				columnMinWidth[column] = Math.Max(columnMinWidth[column], minWidth + hpadding);
			}
			float vpadding = c.computedPadTop + c.computedPadBottom;
			rowPrefHeight[row] = Math.Max(rowPrefHeight[row], prefHeight + vpadding);
			rowMinHeight[row] = Math.Max(rowMinHeight[row], minHeight + vpadding);
		}

		float uniformMinWidth = 0, uniformMinHeight = 0;
		float uniformPrefWidth = 0, uniformPrefHeight = 0;
		for (int i = 0; i < cellCount; i++) {
			Cell c = (Cell)cells[i];
			int column = c.column;

			// Colspan with expand will expand all spanned columns if none of the spanned columns have expand.
			int expandX = c.expandX ?? 0;
			if (expandX != 0) {
				int nn = column + c.colspan ?? 0;
				for (int ii = column; ii < nn; ii++)
					if (expandWidth[ii] != 0)
						goto outer;
				for (int ii = column; ii < nn; ii++)
					expandWidth[ii] = expandX;
			}
			outer:

			// Collect uniform sizes.
			if (c.uniformX == true && c.colspan == 1) {
				float hpadding = c.computedPadLeft + c.computedPadRight;
				uniformMinWidth = Math.Max(uniformMinWidth, columnMinWidth[column] - hpadding);
				uniformPrefWidth = Math.Max(uniformPrefWidth, columnPrefWidth[column] - hpadding);
			}
			if (c.uniformY == true) {
				float vpadding = c.computedPadTop + c.computedPadBottom;
				uniformMinHeight = Math.Max(uniformMinHeight, rowMinHeight[c.row] - vpadding);
				uniformPrefHeight = Math.Max(uniformPrefHeight, rowPrefHeight[c.row] - vpadding);
			}
		}

		// Size uniform cells to the same width/height.
		if (uniformPrefWidth > 0 || uniformPrefHeight > 0) {
			for (int i = 0; i < cellCount; i++) {
				Cell c = (Cell)cells[i];
				if (uniformPrefWidth > 0 && c.uniformX == true && c.colspan == 1) {
					float hpadding = c.computedPadLeft + c.computedPadRight;
					columnMinWidth[c.column] = uniformMinWidth + hpadding;
					columnPrefWidth[c.column] = uniformPrefWidth + hpadding;
				}
				if (uniformPrefHeight > 0 && c.uniformY == true) {
					float vpadding = c.computedPadTop + c.computedPadBottom;
					rowMinHeight[c.row] = uniformMinHeight + vpadding;
					rowPrefHeight[c.row] = uniformPrefHeight + vpadding;
				}
			}
		}

		// Distribute any additional min and pref width added by colspanned cells to the columns spanned.
		for (int i = 0; i < cellCount; i++) {
			Cell c = (Cell)cells[i];
			int colspan = c.colspan ?? 0;
			if (colspan == 1) continue;
			int column = c.column;

			Actor a = c.actor;
			float minWidth = c.minWidth.get(a), prefWidth = c.prefWidth.get(a), maxWidth = c.maxWidth.get(a);
			if (prefWidth < minWidth) prefWidth = minWidth;
			if (maxWidth > 0 && prefWidth > maxWidth) prefWidth = maxWidth;
			if (round) {
				minWidth = (float)Math.Ceiling(minWidth);
				prefWidth = (float)Math.Ceiling(prefWidth);
			}

			float spannedMinWidth = -(c.computedPadLeft + c.computedPadRight), spannedPrefWidth = spannedMinWidth;
			float totalExpandWidth = 0;
			for (int ii = column, nn = ii + colspan; ii < nn; ii++) {
				spannedMinWidth += columnMinWidth[ii];
				spannedPrefWidth += columnPrefWidth[ii];
				totalExpandWidth += expandWidth[ii]; // Distribute extra space using expand, if any columns have expand.
			}

			float extraMinWidth = Math.Max(0, minWidth - spannedMinWidth);
			float extraPrefWidth = Math.Max(0, prefWidth - spannedPrefWidth);
			for (int ii = column, nn = ii + colspan; ii < nn; ii++) {
				float ratio = totalExpandWidth == 0 ? 1f / colspan : expandWidth[ii] / totalExpandWidth;
				columnMinWidth[ii] += extraMinWidth * ratio;
				columnPrefWidth[ii] += extraPrefWidth * ratio;
			}
		}

		{
			// Determine table min and pref size.
			float hpadding = _padLeft.get(this) + _padRight.get(this);
			float vpadding = _padTop.get(this) + _padBottom.get(this);
			tableMinWidth = hpadding;
			tablePrefWidth = hpadding;
			for (int i = 0; i < columns; i++)
			{
				tableMinWidth += columnMinWidth[i];
				tablePrefWidth += columnPrefWidth[i];
			}

			tableMinHeight = vpadding;
			tablePrefHeight = vpadding;
			for (int i = 0; i < rows; i++)
			{
				tableMinHeight += rowMinHeight[i];
				tablePrefHeight += Math.Max(rowMinHeight[i], rowPrefHeight[i]);
			}

			tablePrefWidth = Math.Max(tableMinWidth, tablePrefWidth);
			tablePrefHeight = Math.Max(tableMinHeight, tablePrefHeight);
		}
	}

		/** Positions and sizes children of the table using the cell associated with each child. The values given are the position
		 * within the parent and size of the table. */
		public override void layout () {
		if (sizeInvalid) computeSize();

		float layoutWidth = getWidth(), layoutHeight = getHeight();
		int columns = this.columns, rows = this.rows;
		float[] columnWidth = this.columnWidth, rowHeight = this.rowHeight;
		float padLeft = this._padLeft.get(this), hpadding = padLeft + _padRight.get(this);
		float padTop = this._padTop.get(this), vpadding = padTop + _padBottom.get(this);

		// Size columns and rows between min and pref size using (preferred - min) size to weight distribution of extra space.
		float[] columnWeightedWidth;
		float totalGrowWidth = tablePrefWidth - tableMinWidth;
		if (totalGrowWidth == 0)
			columnWeightedWidth = columnMinWidth;
		else {
			float extraWidth = Math.Min(totalGrowWidth, Math.Max(0, layoutWidth - tableMinWidth));
			columnWeightedWidth = Table.columnWeightedWidth = ensureSize(Table.columnWeightedWidth, columns);
			float[] columnMinWidth = this.columnMinWidth, columnPrefWidth = this.columnPrefWidth;
			for (int i = 0; i < columns; i++) {
				float growWidth = columnPrefWidth[i] - columnMinWidth[i];
				float growRatio = growWidth / totalGrowWidth;
				columnWeightedWidth[i] = columnMinWidth[i] + extraWidth * growRatio;
			}
		}

		float[] rowWeightedHeight;
		float totalGrowHeight = tablePrefHeight - tableMinHeight;
		if (totalGrowHeight == 0)
			rowWeightedHeight = rowMinHeight;
		else {
			rowWeightedHeight = Table.rowWeightedHeight = ensureSize(Table.rowWeightedHeight, rows);
			float extraHeight = Math.Min(totalGrowHeight, Math.Max(0, layoutHeight - tableMinHeight));
			float[] rowMinHeight = this.rowMinHeight, rowPrefHeight = this.rowPrefHeight;
			for (int i = 0; i < rows; i++) {
				float growHeight = rowPrefHeight[i] - rowMinHeight[i];
				float growRatio = growHeight / totalGrowHeight;
				rowWeightedHeight[i] = rowMinHeight[i] + extraHeight * growRatio;
			}
		}

		// Determine actor and cell sizes (before expand or fill).
		Object[] cells = this.cells.items;
		int cellCount = this.cells.size;
		for (int i = 0; i < cellCount; i++) {
			Cell c = (Cell)cells[i];
			int column = c.column, row = c.row;
			Actor a = c.actor;

			float spannedWeightedWidth = 0;
			int colspan = c.colspan ?? 0;
			for (int ii = column, nn = ii + colspan; ii < nn; ii++)
				spannedWeightedWidth += columnWeightedWidth[ii];
			float weightedHeight = rowWeightedHeight[row];

			float prefWidth = c.prefWidth.get(a), prefHeight = c.prefHeight.get(a);
			float minWidth = c.minWidth.get(a), minHeight = c.minHeight.get(a);
			float maxWidth = c.maxWidth.get(a), maxHeight = c.maxHeight.get(a);
			if (prefWidth < minWidth) prefWidth = minWidth;
			if (prefHeight < minHeight) prefHeight = minHeight;
			if (maxWidth > 0 && prefWidth > maxWidth) prefWidth = maxWidth;
			if (maxHeight > 0 && prefHeight > maxHeight) prefHeight = maxHeight;

			c.actorWidth = Math.Min(spannedWeightedWidth - c.computedPadLeft - c.computedPadRight, prefWidth);
			c.actorHeight = Math.Min(weightedHeight - c.computedPadTop - c.computedPadBottom, prefHeight);

			if (colspan == 1) columnWidth[column] = Math.Max(columnWidth[column], spannedWeightedWidth);
			rowHeight[row] = Math.Max(rowHeight[row], weightedHeight);
		}

		// Distribute remaining space to any expanding columns/rows.
		float[] expandWidth = this.expandWidth, expandHeight = this.expandHeight;
		float totalExpand = 0;
		for (int i = 0; i < columns; i++)
			totalExpand += expandWidth[i];
		if (totalExpand > 0) {
			float extra = layoutWidth - hpadding;
			for (int i = 0; i < columns; i++)
				extra -= columnWidth[i];
			if (extra > 0) { // layoutWidth < tableMinWidth.
				float used = 0;
				int lastIndex = 0;
				for (int i = 0; i < columns; i++) {
					if (expandWidth[i] == 0) continue;
					float amount = extra * expandWidth[i] / totalExpand;
					columnWidth[i] += amount;
					used += amount;
					lastIndex = i;
				}
				columnWidth[lastIndex] += extra - used;
			}
		}

		totalExpand = 0;
		for (int i = 0; i < rows; i++)
			totalExpand += expandHeight[i];
		if (totalExpand > 0) {
			float extra = layoutHeight - vpadding;
			for (int i = 0; i < rows; i++)
				extra -= rowHeight[i];
			if (extra > 0) { // layoutHeight < tableMinHeight.
				float used = 0;
				int lastIndex = 0;
				for (int i = 0; i < rows; i++) {
					if (expandHeight[i] == 0) continue;
					float amount = extra * expandHeight[i] / totalExpand;
					rowHeight[i] += amount;
					used += amount;
					lastIndex = i;
				}
				rowHeight[lastIndex] += extra - used;
			}
		}

		// Distribute any additional width added by colspanned cells to the columns spanned.
		for (int i = 0; i < cellCount; i++) {
			Cell c = (Cell)cells[i];
			int colspan = c.colspan ?? 0;
			if (colspan == 1) continue;

			float extraWidth = 0;
			for (int column = c.column, nn = column + colspan; column < nn; column++)
				extraWidth += columnWeightedWidth[column] - columnWidth[column];
			extraWidth -= Math.Max(0, c.computedPadLeft + c.computedPadRight);

			extraWidth /= colspan;
			if (extraWidth > 0) {
				for (int column = c.column, nn = column + colspan; column < nn; column++)
					columnWidth[column] += extraWidth;
			}
		}

		// Determine table size.
		float tableWidth = hpadding, tableHeight = vpadding;
		for (int i = 0; i < columns; i++)
			tableWidth += columnWidth[i];
		for (int i = 0; i < rows; i++)
			tableHeight += rowHeight[i];

		// Position table within the container.
		int align = this._align;
		float x = padLeft;
		if ((align & Align.right) != 0)
			x += layoutWidth - tableWidth;
		else if ((align & Align.left) == 0) // Center
			x += (layoutWidth - tableWidth) / 2;

		float y = padTop;
		if ((align & Align.bottom) != 0)
			y += layoutHeight - tableHeight;
		else if ((align & Align.top) == 0) // Center
			y += (layoutHeight - tableHeight) / 2;

		// Size and position actors within cells.
		float currentX = x, currentY = y;
		for (int i = 0; i < cellCount; i++) {
			Cell c = (Cell)cells[i];

			float spannedCellWidth = 0;
			for (int column = c.column, nn = column + c.colspan ?? 0; column < nn; column++)
				spannedCellWidth += columnWidth[column];
			spannedCellWidth -= c.computedPadLeft + c.computedPadRight;

			currentX += c.computedPadLeft;

			float fillX = c.fillX ?? 0, fillY = c.fillY ?? 0;
			if (fillX > 0) {
				c.actorWidth = Math.Max(spannedCellWidth * fillX, c.minWidth.get(c.actor));
				float maxWidth = c.maxWidth.get(c.actor);
				if (maxWidth > 0) c.actorWidth = Math.Min(c.actorWidth, maxWidth);
			}
			if (fillY > 0) {
				c.actorHeight = Math.Max(rowHeight[c.row] * fillY - c.computedPadTop - c.computedPadBottom, c.minHeight.get(c.actor));
				float maxHeight = c.maxHeight.get(c.actor);
				if (maxHeight > 0) c.actorHeight = Math.Min(c.actorHeight, maxHeight);
			}

			align = c.align ?? 0;
			if ((align & Align.left) != 0)
				c.actorX = currentX;
			else if ((align & Align.right) != 0)
				c.actorX = currentX + spannedCellWidth - c.actorWidth;
			else
				c.actorX = currentX + (spannedCellWidth - c.actorWidth) / 2;

			if ((align & Align.top) != 0)
				c.actorY = c.computedPadTop;
			else if ((align & Align.bottom) != 0)
				c.actorY = rowHeight[c.row] - c.actorHeight - c.computedPadBottom;
			else
				c.actorY = (rowHeight[c.row] - c.actorHeight + c.computedPadTop - c.computedPadBottom) / 2;
			c.actorY = layoutHeight - currentY - c.actorY - c.actorHeight;

			if (round) {
				c.actorWidth = (float)Math.Ceiling(c.actorWidth);
				c.actorHeight = (float)Math.Ceiling(c.actorHeight);
				c.actorX = (float)Math.Floor(c.actorX);
				c.actorY = (float)Math.Floor(c.actorY);
			}

			if (c.actor != null) c.actor.setBounds(c.actorX, c.actorY, c.actorWidth, c.actorHeight);

			if (c.endRow) {
				currentX = x;
				currentY += rowHeight[c.row];
			} else
				currentX += spannedCellWidth + c.computedPadRight;
		}

		// Validate all children (some may not be in cells).
		Array<Actor> childrenArray = getChildren();
		Actor[] children = childrenArray.items;
		for (int i = 0, n = childrenArray.size; i < n; i++) {
			Object child = children[i];
			if (child is ILayout) ((ILayout)child).validate();
		}

		// Store debug rectangles.
		if (_debug != Debug.none) addDebugRects(x, y, tableWidth - hpadding, tableHeight - vpadding);
	}

	private void addDebugRects (float currentX, float currentY, float width, float height) {
		clearDebugRects();
		if (_debug == Debug.table || _debug == Debug.all) {
			// Table actor bounds.
			addDebugRect(0, 0, getWidth(), getHeight(), debugTableColor);
			// Table bounds.
			addDebugRect(currentX, getHeight() - currentY, width, -height, debugTableColor);
		}
		float x = currentX;
		for (int i = 0, n = cells.size; i < n; i++) {
			Cell c = cells.get(i);

			// Cell actor bounds.
			if (_debug == Debug.actor || _debug == Debug.all)
				addDebugRect(c.actorX, c.actorY, c.actorWidth, c.actorHeight, debugActorColor);

			// Cell bounds.
			float spannedCellWidth = 0;
			for (int column = c.column, nn = column + c.colspan ?? 0; column < nn; column++)
				spannedCellWidth += columnWidth[column];
			spannedCellWidth -= c.computedPadLeft + c.computedPadRight;
			currentX += c.computedPadLeft;
			if (_debug == Debug.cell || _debug == Debug.all) {
				float h = rowHeight[c.row] - c.computedPadTop - c.computedPadBottom;
				float y = currentY + c.computedPadTop;
				addDebugRect(currentX, getHeight() - y, spannedCellWidth, -h, debugCellColor);
			}

			if (c.endRow) {
				currentX = x;
				currentY += rowHeight[c.row];
			} else
				currentX += spannedCellWidth + c.computedPadRight;
		}
	}

	private void clearDebugRects () {
		if (debugRects == null) debugRects = new ();
		DebugRect.pool.freeAll(debugRects);
		debugRects.clear();
	}

	private void addDebugRect (float x, float y, float w, float h, Color color) {
		DebugRect rect = DebugRect.pool.obtain();
		rect.color = color;
		rect.set(x, y, w, h);
		debugRects.add(rect);
	}

		public override void drawDebug (ShapeRenderer shapes) {
		if (isTransform()) {
			applyTransform(shapes, computeTransform());
			drawDebugRects(shapes);
			if (_clip) {
				shapes.flush();
				float x = 0, y = 0, width = getWidth(), height = getHeight();
				if (_background != null) {
					x = _padLeft.get(this);
					y = _padBottom.get(this);
					width -= x + _padRight.get(this);
					height -= y + _padTop.get(this);
				}
				if (clipBegin(x, y, width, height)) {
					drawDebugChildren(shapes);
					clipEnd();
				}
			} else
				drawDebugChildren(shapes);
			resetTransform(shapes);
		} else {
			drawDebugRects(shapes);
			base.drawDebug(shapes);
		}
	}

		protected override void drawDebugBounds (ShapeRenderer shapes) {
	}

	private void drawDebugRects (ShapeRenderer shapes) {
		if (debugRects == null || !getDebug()) return;
		shapes.set(ShapeRenderer.ShapeType.Line);
		if (getStage() != null) shapes.setColor(getStage().getDebugColor());
		float x = 0, y = 0;
		if (!isTransform()) {
			x = getX();
			y = getY();
		}
		for (int i = 0, n = debugRects.size; i < n; i++) {
			DebugRect debugRect = debugRects.get(i);
			shapes.setColor(debugRect.color);
			shapes.rect(x + debugRect.x, y + debugRect.y, debugRect.width, debugRect.height);
		}
	}

	/** @return The skin that was passed to this table in its constructor, or null if none was given. */
	public  Skin? getSkin () {
		return skin;
	}

	/** @author Nathan Sweet */
	public class DebugRect : Rectangle {
		internal static Pool<DebugRect> pool = Pools.get< DebugRect>(typeof(DebugRect));
		internal Color color;
	}

	/** @author Nathan Sweet */
	public enum Debug {
		none, all, table, cell, actor
	}

	/** Value that is the top padding of the table's background.
	 * @author Nathan Sweet */
	static public Value backgroundTop = new BackgroundTopValue() ;

	private class BackgroundTopValue : Value
		
			{
				public override float get(Actor? context)
				{
					IDrawable background = ((Table)context)._background;
					return background == null ? 0 : background.getTopHeight();
				}
			}

	/** Value that is the left padding of the table's background.
	 * @author Nathan Sweet */
	static public Value backgroundLeft = new BackgroundLeftValue() ;

	private class BackgroundLeftValue:Value
	{
		public override float get(Actor? context)
		{
			IDrawable background = ((Table)context)._background;
			return background == null ? 0 : background.getLeftWidth();
		}
	}

		/** Value that is the bottom padding of the table's background.
		 * @author Nathan Sweet */
		static public Value backgroundBottom = new BackgroundBottomValue() ;

		private class BackgroundBottomValue : Value
			{
				public override float get(Actor? context)
				{
					IDrawable background = ((Table)context)._background;
					return background == null ? 0 : background.getBottomHeight();
				}
			}

	/** Value that is the right padding of the table's background.
	 * @author Nathan Sweet */
	static public Value backgroundRight = new BackgroundRightValue() ;

	private class BackgroundRightValue:Value
	{
		public override float get(Actor? context)
		{
			IDrawable background = ((Table)context)._background;
			return background == null ? 0 : background.getRightWidth();
		}
	}
	}
}
