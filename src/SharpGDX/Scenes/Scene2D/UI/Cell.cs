using SharpGDX.Shims;
using Fixed = SharpGDX.Scenes.Scene2D.UI.Value.Fixed;
using SharpGDX.Files;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A cell for a {@link Table}.
 * @author Nathan Sweet */
public class Cell: IPoolable {
	static private readonly float zerof = 0f, onef = 1f;
	static private readonly int zeroi = 0, onei = 1;
	static private readonly int centeri = onei, topi = Align.top, bottomi = Align.bottom, lefti = Align.left,
		righti = Align.right;

	static private IFiles files;
	static private Cell _defaults;

	internal Value _minWidth, _minHeight;
	internal Value _prefWidth, _prefHeight;
	internal Value _maxWidth, _maxHeight;
	internal Value _spaceTop, _spaceLeft, _spaceBottom, _spaceRight;
	internal Value _padTop, _padLeft, _padBottom, _padRight;
	internal float? _fillX, _fillY;
	internal int? _align;
	internal int? _expandX, _expandY;
	internal int? _colspan;
	internal bool? _uniformX, _uniformY;

	internal Actor? actor;
	internal float actorX, actorY;
	internal float actorWidth, actorHeight;

	private Table table;
	internal bool endRow;
	internal int _column, _row;
	internal int cellAboveIndex;
	internal float computedPadTop, computedPadLeft, computedPadBottom, computedPadRight;

	public Cell () {
		cellAboveIndex = -1;
		Cell defaults = Cell.defaults();
		if (defaults != null) Set(defaults);
	}

	public void setTable (Table table) {
		this.table = table;
	}

	/** Sets the actor in this cell and adds the actor to the cell's table. If null, removes any current actor. */
	public Cell SetActor(Actor? newActor){
		if (actor != newActor) {
			if (actor != null && actor.getParent() == table) actor.remove();
			actor = newActor;
			if (newActor != null) table.addActor(newActor);
		}
		return (Cell)this;
	}

	/** Removes the current actor for the cell, if any. */
	public Cell clearActor<T> ()
    where T: Actor{
		SetActor(null);
		return this;
	}

	/** Returns the actor for this cell, or null. */
	public T? getActor<T> () 
    where T: Actor{
		return (T)actor;
	}

	/** Returns true if the cell's actor is not null. */
	public bool hasActor () {
		return actor != null;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified value. */
	public Cell size (Value size) {
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
	public Cell size (Value width, Value height) {
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
	public Cell size (float size) {
		this.size(Fixed.valueOf(size));
		return this;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified values. */
	public Cell size (float width, float height) {
		size(Fixed.valueOf(width), Fixed.valueOf(height));
		return this;
	}

	/** Sets the minWidth, prefWidth, and maxWidth to the specified value. */
	public Cell width (Value width) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
        _minWidth = width;
        _prefWidth = width;
        _maxWidth = width;
		return this;
	}

	/** Sets the minWidth, prefWidth, and maxWidth to the specified value. */
	public Cell width (float width) {
		this.width(Fixed.valueOf(width));
		return this;
	}

	/** Sets the minHeight, prefHeight, and maxHeight to the specified value. */
	public Cell height (Value height) {
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
        _minHeight = height;
        _prefHeight = height;
        _maxHeight = height;
		return this;
	}

	/** Sets the minHeight, prefHeight, and maxHeight to the specified value. */
	public Cell height (float height) {
		this.height(Fixed.valueOf(height));
		return this;
	}

	/** Sets the minWidth and minHeight to the specified value. */
	public Cell minSize (Value size) {
		if (size == null) throw new IllegalArgumentException("size cannot be null.");
        _minWidth = size;
        _minHeight = size;
		return this;
	}

	/** Sets the minWidth and minHeight to the specified values. */
	public Cell minSize (Value width, Value height) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
        _minWidth = width;
        _minHeight = height;
		return this;
	}

	public Cell minWidth (Value minWidth) {
		if (minWidth == null) throw new IllegalArgumentException("minWidth cannot be null.");
		this._minWidth = minWidth;
		return this;
	}

	public Cell minHeight (Value minHeight) {
		if (minHeight == null) throw new IllegalArgumentException("minHeight cannot be null.");
		this._minHeight = minHeight;
		return this;
	}

	/** Sets the minWidth and minHeight to the specified value. */
	public Cell minSize (float size) {
		minSize(Fixed.valueOf(size));
		return this;
	}

	/** Sets the minWidth and minHeight to the specified values. */
	public Cell minSize (float width, float height) {
		minSize(Fixed.valueOf(width), Fixed.valueOf(height));
		return this;
	}

	public Cell MinWidth (float minWidth) {
		this._minWidth = Fixed.valueOf(minWidth);
		return this;
	}

	public Cell MinHeight (float minHeight) {
		this._minHeight = Fixed.valueOf(minHeight);
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified value. */
	public Cell prefSize (Value size) {
		if (size == null) throw new IllegalArgumentException("size cannot be null.");
        _prefWidth = size;
        _prefHeight = size;
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified values. */
	public Cell prefSize (Value width, Value height) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
        _prefWidth = width;
        _prefHeight = height;
		return this;
	}

	public Cell prefWidth (Value prefWidth) {
		if (prefWidth == null) throw new IllegalArgumentException("prefWidth cannot be null.");
		this._prefWidth = prefWidth;
		return this;
	}

	public Cell prefHeight (Value prefHeight) {
		if (prefHeight == null) throw new IllegalArgumentException("prefHeight cannot be null.");
		this._prefHeight = prefHeight;
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified value. */
	public Cell prefSize (float width, float height) {
		prefSize(Fixed.valueOf(width), Fixed.valueOf(height));
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified values. */
	public Cell prefSize (float size) {
		prefSize(Fixed.valueOf(size));
		return this;
	}

	public Cell prefWidth (float prefWidth) {
		this._prefWidth = Fixed.valueOf(prefWidth);
		return this;
	}

	public Cell prefHeight (float prefHeight) {
		this._prefHeight = Fixed.valueOf(prefHeight);
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified value. If the max size is 0, no maximum size is used. */
	public Cell maxSize (Value size) {
		if (size == null) throw new IllegalArgumentException("size cannot be null.");
        _maxWidth = size;
        _maxHeight = size;
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified values. If the max size is 0, no maximum size is used. */
	public Cell maxSize (Value width, Value height) {
		if (width == null) throw new IllegalArgumentException("width cannot be null.");
		if (height == null) throw new IllegalArgumentException("height cannot be null.");
        _maxWidth = width;
        _maxHeight = height;
		return this;
	}

	/** If the maxWidth is 0, no maximum width is used. */
	public Cell maxWidth (Value maxWidth) {
		if (maxWidth == null) throw new IllegalArgumentException("maxWidth cannot be null.");
		this._maxWidth = maxWidth;
		return this;
	}

	/** If the maxHeight is 0, no maximum height is used. */
	public Cell maxHeight (Value maxHeight) {
		if (maxHeight == null) throw new IllegalArgumentException("maxHeight cannot be null.");
		this._maxHeight = maxHeight;
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified value. If the max size is 0, no maximum size is used. */
	public Cell maxSize (float size) {
		maxSize(Fixed.valueOf(size));
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified values. If the max size is 0, no maximum size is used. */
	public Cell maxSize (float width, float height) {
		maxSize(Fixed.valueOf(width), Fixed.valueOf(height));
		return this;
	}

	/** If the maxWidth is 0, no maximum width is used. */
	public Cell maxWidth (float maxWidth) {
		this._maxWidth = Fixed.valueOf(maxWidth);
		return this;
	}

	/** If the maxHeight is 0, no maximum height is used. */
	public Cell maxHeight (float maxHeight) {
		this._maxHeight = Fixed.valueOf(maxHeight);
		return this;
	}

	/** Sets the spaceTop, spaceLeft, spaceBottom, and spaceRight to the specified value. */
	public Cell Space (Value space) {
		if (space == null) throw new IllegalArgumentException("space cannot be null.");
        _spaceTop = space;
        _spaceLeft = space;
        _spaceBottom = space;
        _spaceRight = space;
		return this;
	}

	public Cell Space (Value top, Value left, Value bottom, Value right) {
		if (top == null) throw new IllegalArgumentException("top cannot be null.");
		if (left == null) throw new IllegalArgumentException("left cannot be null.");
		if (bottom == null) throw new IllegalArgumentException("bottom cannot be null.");
		if (right == null) throw new IllegalArgumentException("right cannot be null.");
        _spaceTop = top;
        _spaceLeft = left;
        _spaceBottom = bottom;
        _spaceRight = right;
		return this;
	}

	public Cell spaceTop (Value spaceTop) {
		if (spaceTop == null) throw new IllegalArgumentException("spaceTop cannot be null.");
		this._spaceTop = spaceTop;
		return this;
	}

	public Cell spaceLeft (Value spaceLeft) {
		if (spaceLeft == null) throw new IllegalArgumentException("spaceLeft cannot be null.");
		this._spaceLeft = spaceLeft;
		return this;
	}

	public Cell spaceBottom (Value spaceBottom) {
		if (spaceBottom == null) throw new IllegalArgumentException("spaceBottom cannot be null.");
		this._spaceBottom = spaceBottom;
		return this;
	}

	public Cell spaceRight (Value spaceRight) {
		if (spaceRight == null) throw new IllegalArgumentException("spaceRight cannot be null.");
		this._spaceRight = spaceRight;
		return this;
	}

	/** Sets the spaceTop, spaceLeft, spaceBottom, and spaceRight to the specified value. The space cannot be < 0. */
	public Cell Space (float space) {
		if (space < 0) throw new IllegalArgumentException("space cannot be < 0: " + space);
		this.Space(Fixed.valueOf(space));
		return this;
	}

	/** The space cannot be < 0. */
	public Cell space (float top, float left, float bottom, float right) {
		if (top < 0) throw new IllegalArgumentException("top cannot be < 0: " + top);
		if (left < 0) throw new IllegalArgumentException("left cannot be < 0: " + left);
		if (bottom < 0) throw new IllegalArgumentException("bottom cannot be < 0: " + bottom);
		if (right < 0) throw new IllegalArgumentException("right cannot be < 0: " + right);
		Space(Fixed.valueOf(top), Fixed.valueOf(left), Fixed.valueOf(bottom), Fixed.valueOf(right));
		return this;
	}

	/** The space cannot be < 0. */
	public Cell spaceTop (float spaceTop) {
		if (spaceTop < 0) throw new IllegalArgumentException("spaceTop cannot be < 0: " + spaceTop);
		this._spaceTop = Fixed.valueOf(spaceTop);
		return this;
	}

	/** The space cannot be < 0. */
	public Cell spaceLeft (float spaceLeft) {
		if (spaceLeft < 0) throw new IllegalArgumentException("spaceLeft cannot be < 0: " + spaceLeft);
		this._spaceLeft = Fixed.valueOf(spaceLeft);
		return this;
	}

	/** The space cannot be < 0. */
	public Cell spaceBottom (float spaceBottom) {
		if (spaceBottom < 0) throw new IllegalArgumentException("spaceBottom cannot be < 0: " + spaceBottom);
		this._spaceBottom = Fixed.valueOf(spaceBottom);
		return this;
	}

	/** The space cannot be < 0. */
	public Cell spaceRight (float spaceRight) {
		if (spaceRight < 0) throw new IllegalArgumentException("spaceRight cannot be < 0: " + spaceRight);
		this._spaceRight = Fixed.valueOf(spaceRight);
		return this;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight to the specified value. */
	public Cell pad (Value pad) {
		if (pad == null) throw new IllegalArgumentException("pad cannot be null.");
        _padTop = pad;
        _padLeft = pad;
        _padBottom = pad;
        _padRight = pad;
		return this;
	}

	public Cell pad (Value top, Value left, Value bottom, Value right) {
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

	public Cell padTop (Value padTop) {
		if (padTop == null) throw new IllegalArgumentException("padTop cannot be null.");
		this._padTop = padTop;
		return this;
	}

	public Cell padLeft (Value padLeft) {
		if (padLeft == null) throw new IllegalArgumentException("padLeft cannot be null.");
		this._padLeft = padLeft;
		return this;
	}

	public Cell padBottom (Value padBottom) {
		if (padBottom == null) throw new IllegalArgumentException("padBottom cannot be null.");
		this._padBottom = padBottom;
		return this;
	}

	public Cell padRight (Value padRight) {
		if (padRight == null) throw new IllegalArgumentException("padRight cannot be null.");
		this._padRight = padRight;
		return this;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight to the specified value. */
	public Cell pad (float pad) {
		this.pad(Fixed.valueOf(pad));
		return this;
	}

	public Cell pad (float top, float left, float bottom, float right) {
		pad(Fixed.valueOf(top), Fixed.valueOf(left), Fixed.valueOf(bottom), Fixed.valueOf(right));
		return this;
	}

	public Cell padTop (float padTop) {
		this._padTop = Fixed.valueOf(padTop);
		return this;
	}

	public Cell padLeft (float padLeft) {
		this._padLeft = Fixed.valueOf(padLeft);
		return this;
	}

	public Cell padBottom (float padBottom) {
		this._padBottom = Fixed.valueOf(padBottom);
		return this;
	}

	public Cell padRight (float padRight) {
		this._padRight = Fixed.valueOf(padRight);
		return this;
	}

	/** Sets fillX and fillY to 1. */
	public Cell Fill () {
        _fillX = onef;
        _fillY = onef;
		return this;
	}

	/** Sets fillX to 1. */
	public Cell FillX () {
        _fillX = onef;
		return this;
	}

	/** Sets fillY to 1. */
	public Cell FillY () {
        _fillY = onef;
		return this;
	}

	public Cell Fill (float x, float y) {
        _fillX = x;
        _fillY = y;
		return this;
	}

	/** Sets fillX and fillY to 1 if true, 0 if false. */
	public Cell Fill (bool x, bool y) {
        _fillX = x ? onef : zerof;
        _fillY = y ? onef : zerof;
		return this;
	}

	/** Sets fillX and fillY to 1 if true, 0 if false. */
	public Cell Fill (bool fill) {
        _fillX = fill ? onef : zerof;
        _fillY = fill ? onef : zerof;
		return this;
	}

	/** Sets the alignment of the actor within the cell. Set to {@link Align#center}, {@link Align#top}, {@link Align#bottom},
	 * {@link Align#left}, {@link Align#right}, or any combination of those. */
	public Cell align (int align) {
		this._align = align;
		return this;
	}

	/** Sets the alignment of the actor within the cell to {@link Align#center}. This clears any other alignment. */
	public Cell Center () {
        _align = centeri;
		return this;
	}

	/** Adds {@link Align#top} and clears {@link Align#bottom} for the alignment of the actor within the cell. */
	public Cell Top () {
		if (align == null)
            _align = topi;
		else
            _align = (_align | Align.top) & ~Align.bottom;
		return this;
	}

	/** Adds {@link Align#left} and clears {@link Align#right} for the alignment of the actor within the cell. */
	public Cell Left () {
		if (align == null)
            _align = lefti;
		else
            _align = (_align | Align.left) & ~Align.right;
		return this;
	}

	/** Adds {@link Align#bottom} and clears {@link Align#top} for the alignment of the actor within the cell. */
	public Cell Bottom () {
		if (_align == null)
            _align = bottomi;
		else
            _align = (_align | Align.bottom) & ~Align.top;
		return this;
	}

	/** Adds {@link Align#right} and clears {@link Align#left} for the alignment of the actor within the cell. */
	public Cell Right () {
		if (_align == null)
            _align = righti;
		else
            _align = (_align | Align.right) & ~Align.left;
		return this;
	}

	/** Sets expandX, expandY, fillX, and fillY to 1. */
	public Cell Grow () {
        _expandX = onei;
        _expandY = onei;
        _fillX = onef;
        _fillY = onef;
		return this;
	}

	/** Sets expandX and fillX to 1. */
	public Cell GrowX () {
        _expandX = onei;
        _fillX = onef;
		return this;
	}

	/** Sets expandY and fillY to 1. */
	public Cell GrowY () {
        _expandY = onei;
        _fillY = onef;
		return this;
	}

	/** Sets expandX and expandY to 1. */
	public Cell Expand () {
        _expandX = onei;
        _expandY = onei;
		return this;
	}

	/** Sets expandX to 1. */
	public Cell ExpandX () {
        _expandX = onei;
		return this;
	}

	/** Sets expandY to 1. */
	public Cell ExpandY () {
        _expandY = onei;
		return this;
	}

	public Cell Expand (int x, int y) {
        _expandX = x;
        _expandY = y;
		return this;
	}

	/** Sets expandX and expandY to 1 if true, 0 if false. */
	public Cell expand (bool x, bool y) {
        _expandX = x ? onei : zeroi;
        _expandY = y ? onei : zeroi;
		return this;
	}

	public Cell colspan (int colspan) {
		this._colspan = colspan;
		return this;
	}

	/** Sets uniformX and uniformY to true. */
	public Cell uniform () {
		_uniformX = true;
        _uniformY = true;
		return this;
	}

	/** Sets uniformX to true. */
	public Cell uniformX () {
        _uniformX = true;
		return this;
	}

	/** Sets uniformY to true. */
	public Cell uniformY () {
        _uniformY = true;
		return this;
	}

	public Cell uniform (bool uniform) {
        _uniformX = uniform;
        _uniformY = uniform;
		return this;
	}

	public Cell uniform (bool x, bool y) {
        _uniformX = x;
        _uniformY = y;
		return this;
	}

	public void setActorBounds (float x, float y, float width, float height) {
		actorX = x;
		actorY = y;
		actorWidth = width;
		actorHeight = height;
	}

	public float getActorX () {
		return actorX;
	}

	public void setActorX (float actorX) {
		this.actorX = actorX;
	}

	public float getActorY () {
		return actorY;
	}

	public void setActorY (float actorY) {
		this.actorY = actorY;
	}

	public float getActorWidth () {
		return actorWidth;
	}

	public void setActorWidth (float actorWidth) {
		this.actorWidth = actorWidth;
	}

	public float getActorHeight () {
		return actorHeight;
	}

	public void setActorHeight (float actorHeight) {
		this.actorHeight = actorHeight;
	}

	public int getColumn () {
		return _column;
	}

	public int getRow () {
		return _row;
	}

	/** @return May be null if this cell is row defaults. */
	public Value? getMinWidthValue () {
		return _minWidth;
	}

	public float getMinWidth () {
		return _minWidth.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? getMinHeightValue () {
		return _minHeight;
	}

	public float getMinHeight () {
		return _minHeight.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public  Value? getPrefWidthValue () {
		return _prefWidth;
	}

	public float getPrefWidth () {
		return _prefWidth.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? getPrefHeightValue () {
		return _prefHeight;
	}

	public float getPrefHeight () {
		return _prefHeight.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? getMaxWidthValue () {
		return _maxWidth;
	}

	public float getMaxWidth () {
		return _maxWidth.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? getMaxHeightValue () {
		return _maxHeight;
	}

	public float getMaxHeight () {
		return _maxHeight.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? getSpaceTopValue () {
		return _spaceTop;
	}

	public float getSpaceTop () {
		return _spaceTop.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? getSpaceLeftValue () {
		return _spaceLeft;
	}

	public float getSpaceLeft () {
		return _spaceLeft.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? getSpaceBottomValue () {
		return _spaceBottom;
	}

	public float getSpaceBottom () {
		return _spaceBottom.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? getSpaceRightValue () {
		return _spaceRight;
	}

	public float getSpaceRight () {
		return _spaceRight.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? getPadTopValue () {
		return _padTop;
	}

	public float getPadTop () {
		return _padTop.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? getPadLeftValue () {
		return _padLeft;
	}

	public float getPadLeft () {
		return _padLeft.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? getPadBottomValue () {
		return _padBottom;
	}

	public float getPadBottom () {
		return _padBottom.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? getPadRightValue () {
		return _padRight;
	}

	public float getPadRight () {
		return _padRight.get(actor);
	}

	/** Returns {@link #getPadLeft()} plus {@link #getPadRight()}. */
	public float getPadX () {
		return _padLeft.get(actor) + _padRight.get(actor);
	}

	/** Returns {@link #getPadTop()} plus {@link #getPadBottom()}. */
	public float getPadY () {
		return _padTop.get(actor) + _padBottom.get(actor);
	}

	public float? getFillX () {
		return _fillX;
	}

	public float? getFillY () {
		return _fillY;
	}

	public int? getAlign () {
		return _align;
	}

	public int? getExpandX () {
		return _expandX;
	}

	public int? getExpandY () {
		return _expandY;
	}

	public int? getColspan () {
		return _colspan;
	}

	public bool? getUniformX () {
		return _uniformX;
	}

	public bool? getUniformY () {
		return _uniformY;
	}

	/** Returns true if this cell is the last cell in the row. */
	public bool isEndRow () {
		return endRow;
	}

	/** The actual amount of combined padding and spacing from the last layout. */
	public float getComputedPadTop () {
		return computedPadTop;
	}

	/** The actual amount of combined padding and spacing from the last layout. */
	public float getComputedPadLeft () {
		return computedPadLeft;
	}

	/** The actual amount of combined padding and spacing from the last layout. */
	public float getComputedPadBottom () {
		return computedPadBottom;
	}

	/** The actual amount of combined padding and spacing from the last layout. */
	public float getComputedPadRight () {
		return computedPadRight;
	}

	public void Row () {
		table.row();
	}

	public Table getTable () {
		return table;
	}

	/** Sets all constraint fields to null. */
	internal void Clear () {
        _minWidth = null;
        _minHeight = null;
        _prefWidth = null;
        _prefHeight = null;
        _maxWidth = null;
        _maxHeight = null;
        _spaceTop = null;
        _spaceLeft = null;
        _spaceBottom = null;
        _spaceRight = null;
        _padTop = null;
        _padLeft = null;
        _padBottom = null;
        _padRight = null;
        _fillX = null;
        _fillY = null;
        _align = null;
        _expandX = null;
        _expandY = null;
        _colspan = null;
        _uniformX = null;
        _uniformY = null;
	}

	/** Reset state so the cell can be reused, setting all constraints to their {@link #defaults() default} values. */
	public void reset () {
		actor = null;
		table = null;
		endRow = false;
		cellAboveIndex = -1;
		Set(defaults());
	}

	internal void Set (Cell cell) {
        _minWidth = cell._minWidth;
        _minHeight = cell._minHeight;
        _prefWidth = cell._prefWidth;
        _prefHeight = cell._prefHeight;
        _maxWidth = cell._maxWidth;
        _maxHeight = cell._maxHeight;
        _spaceTop = cell._spaceTop;
        _spaceLeft = cell._spaceLeft;
        _spaceBottom = cell._spaceBottom;
        _spaceRight = cell._spaceRight;
        _padTop = cell._padTop;
        _padLeft = cell._padLeft;
        _padBottom = cell._padBottom;
        _padRight = cell._padRight;
        _fillX = cell._fillX;
        _fillY = cell._fillY;
        _align = cell._align;
        _expandX = cell._expandX;
        _expandY = cell._expandY;
        _colspan = cell._colspan;
        _uniformX = cell._uniformX;
        _uniformY = cell._uniformY;
	}

	internal void Merge (Cell? cell) {
		if (cell == null) return;
		if (cell._minWidth != null) _minWidth = cell._minWidth;
		if (cell._minHeight != null) _minHeight = cell._minHeight;
		if (cell._prefWidth != null) _prefWidth = cell._prefWidth;
		if (cell._prefHeight != null) _prefHeight = cell._prefHeight;
		if (cell._maxWidth != null) _maxWidth = cell._maxWidth;
		if (cell._maxHeight != null) _maxHeight = cell._maxHeight;
		if (cell._spaceTop != null) _spaceTop = cell._spaceTop;
		if (cell._spaceLeft != null) _spaceLeft = cell._spaceLeft;
		if (cell._spaceBottom != null) _spaceBottom = cell._spaceBottom;
		if (cell._spaceRight != null) _spaceRight = cell._spaceRight;
		if (cell._padTop != null) _padTop = cell._padTop;
		if (cell._padLeft != null) _padLeft = cell._padLeft;
		if (cell._padBottom != null) _padBottom = cell._padBottom;
		if (cell._padRight != null) _padRight = cell._padRight;
		if (cell._fillX != null) _fillX = cell._fillX;
		if (cell._fillY != null) _fillY = cell._fillY;
		if (cell._align != null) _align = cell._align;
		if (cell._expandX != null) _expandX = cell._expandX;
		if (cell._expandY != null) _expandY = cell._expandY;
		if (cell._colspan != null) _colspan = cell._colspan;
		if (cell._uniformX != null) _uniformX = cell._uniformX;
		if (cell._uniformY != null) _uniformY = cell._uniformY;
	}

	public override String ToString () {
		return actor != null ? actor.ToString() : base.ToString();
	}

	/** Returns the defaults to use for all cells. This can be used to avoid needing to set the same defaults for every table (eg,
	 * for spacing). */
	static public Cell defaults () {
		if (files == null || files != GDX.Files) {
			files = GDX.Files;
            _defaults = new Cell();
            _defaults._minWidth = Value.minWidth;
            _defaults._minHeight = Value.minHeight;
            _defaults._prefWidth = Value.prefWidth;
            _defaults._prefHeight = Value.prefHeight;
            _defaults._maxWidth = Value.maxWidth;
            _defaults._maxHeight = Value.maxHeight;
            _defaults._spaceTop = Value.zero;
            _defaults._spaceLeft = Value.zero;
            _defaults._spaceBottom = Value.zero;
            _defaults._spaceRight = Value.zero;
            _defaults._padTop = Value.zero;
            _defaults._padLeft = Value.zero;
            _defaults._padBottom = Value.zero;
            _defaults._padRight = Value.zero;
            _defaults._fillX = zerof;
            _defaults._fillY = zerof;
            _defaults._align = centeri;
            _defaults._expandX = zeroi;
            _defaults._expandY = zeroi;
            _defaults._colspan = onei;
            _defaults._uniformX = null;
            _defaults._uniformY = null;
		}
		return _defaults;
	}
}
