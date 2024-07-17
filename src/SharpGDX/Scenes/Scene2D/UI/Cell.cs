using System;
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
	// TODO: Had to shim this to get things to work. -RP
	public abstract class Cell
	{
		internal Cell()
		{

		}

		/** Sets the actor in this cell and adds the actor to the cell's table. If null, removes any current actor. */
		// TODO: This was originally SetActor<A> where A : Actor. Why? -RP
		public Cell<T> SetActor<T>(T? newActor)
		where T: Actor
		{
			if (actor != newActor)
			{
				if (actor != null && actor.getParent() == table) actor.remove();
				actor = newActor;
				if (newActor != null) table.addActor(newActor);
			}
			return (Cell<T>)this;
		}

		protected Table table;

		public void setTable(Table table)
		{
			this.table = table;
		}

		internal void Merge(Cell? cell)
		{
			if (cell == null) return;
			if (cell.minWidth != null) minWidth = cell.minWidth;
			if (cell.minHeight != null) minHeight = cell.minHeight;
			if (cell.prefWidth != null) prefWidth = cell.prefWidth;
			if (cell.prefHeight != null) prefHeight = cell.prefHeight;
			if (cell.maxWidth != null) maxWidth = cell.maxWidth;
			if (cell.maxHeight != null) maxHeight = cell.maxHeight;
			if (cell.spaceTop != null) spaceTop = cell.spaceTop;
			if (cell.spaceLeft != null) spaceLeft = cell.spaceLeft;
			if (cell.spaceBottom != null) spaceBottom = cell.spaceBottom;
			if (cell.spaceRight != null) spaceRight = cell.spaceRight;
			if (cell.padTop != null) padTop = cell.padTop;
			if (cell.padLeft != null) padLeft = cell.padLeft;
			if (cell.padBottom != null) padBottom = cell.padBottom;
			if (cell.padRight != null) padRight = cell.padRight;
			if (cell.fillX != null) fillX = cell.fillX;
			if (cell.fillY != null) fillY = cell.fillY;
			if (cell.align != null) align = cell.align;
			if (cell.expandX != null) expandX = cell.expandX;
			if (cell.expandY != null) expandY = cell.expandY;
			if (cell.colspan != null) colspan = cell.colspan;
			if (cell.uniformX != null) uniformX = cell.uniformX;
			if (cell.uniformY != null) uniformY = cell.uniformY;
		}

		internal void Set(Cell cell)
		{
			minWidth = cell.minWidth;
			minHeight = cell.minHeight;
			prefWidth = cell.prefWidth;
			prefHeight = cell.prefHeight;
			maxWidth = cell.maxWidth;
			maxHeight = cell.maxHeight;
			spaceTop = cell.spaceTop;
			spaceLeft = cell.spaceLeft;
			spaceBottom = cell.spaceBottom;
			spaceRight = cell.spaceRight;
			padTop = cell.padTop;
			padLeft = cell.padLeft;
			padBottom = cell.padBottom;
			padRight = cell.padRight;
			fillX = cell.fillX;
			fillY = cell.fillY;
			align = cell.align;
			expandX = cell.expandX;
			expandY = cell.expandY;
			colspan = cell.colspan;
			uniformX = cell.uniformX;
			uniformY = cell.uniformY;
		}

		internal float actorX, actorY;
		internal float computedPadTop, computedPadLeft, computedPadBottom, computedPadRight;
		internal int? expandX, expandY;
		internal Boolean? uniformX, uniformY;
		internal float actorWidth, actorHeight;
		internal float? fillX, fillY;
		internal int? align;

		public abstract void reset();
		internal abstract void Clear();

		internal int cellAboveIndex;
		internal int? colspan;
		internal int column, row;
		internal Actor? actor;
		internal bool endRow;
		internal Value? minWidth, minHeight;
		internal Value? prefWidth, prefHeight;
		internal Value? maxWidth, maxHeight;
		internal Value? spaceTop, spaceLeft, spaceBottom, spaceRight;
		internal Value? padTop, padLeft, padBottom, padRight;

/** Sets the spaceTop, spaceLeft, spaceBottom, and spaceRight to the specified value. The space cannot be < 0. */
public virtual Cell Space(float space)
{
	if (space < 0) throw new IllegalArgumentException("space cannot be < 0: " + space);
	Space(Fixed.valueOf(space));
	return this;
}

		/** Sets the spaceTop, spaceLeft, spaceBottom, and spaceRight to the specified value. */
		public Cell Space(Value space)
		{
			if (space == null) throw new IllegalArgumentException("space cannot be null.");
			spaceTop = space;
			spaceLeft = space;
			spaceBottom = space;
			spaceRight = space;
			return this;
		}
	}

	/** A cell for a {@link Table}.
 * @author Nathan Sweet */
public class Cell<T> : Cell,IPoolable
	where T: Actor{
	static private readonly float zerof = 0f, onef = 1f;
	static private readonly int zeroi = 0, onei = 1;
	static private readonly int centeri = onei, topi = SharpGDX.Utils.Align.top, bottomi = SharpGDX.Utils.Align.bottom, lefti = SharpGDX.Utils.Align.left,
		righti = SharpGDX.Utils.Align.right;

	static private IFiles files;
	static private Cell<T> defaults;

	public Cell () {
		cellAboveIndex = -1;
		Cell<T> defaults = Defaults();
		if (defaults != null) Set(defaults);
	}

	

	

	/** Removes the current actor for the cell, if any. */
	public Cell<T> ClearActor () {
		SetActor<T>(null);
		return this;
	}

	/** Returns the actor for this cell, or null. */
	public T? GetActor () {
		return (T)actor;
	}

	/** Returns true if the cell's actor is not null. */
	public bool HasActor () {
		return actor != null;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified value. */
	public Cell<T> Size (Value size) {
		minWidth = size ?? throw new IllegalArgumentException("size cannot be null.");
		minHeight = size;
		prefWidth = size;
		prefHeight = size;
		maxWidth = size;
		maxHeight = size;
		return this;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified values. */
	public Cell<T> Size (Value width, Value height) {
		minWidth = width ?? throw new IllegalArgumentException("width cannot be null.");
		minHeight = height ?? throw new IllegalArgumentException("height cannot be null.");
		prefWidth = width;
		prefHeight = height;
		maxWidth = width;
		maxHeight = height;
		return this;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified value. */
	public Cell<T> Size (float size) {
		Size(Fixed.valueOf(size));
		return this;
	}

	/** Sets the minWidth, prefWidth, maxWidth, minHeight, prefHeight, and maxHeight to the specified values. */
	public Cell<T> Size (float width, float height) {
		Size(Fixed.valueOf(width), Fixed.valueOf(height));
		return this;
	}

	/** Sets the minWidth, prefWidth, and maxWidth to the specified value. */
	public Cell<T> Width (Value width) {
		minWidth = width ?? throw new IllegalArgumentException("width cannot be null.");
		prefWidth = width;
		maxWidth = width;
		return this;
	}

	/** Sets the minWidth, prefWidth, and maxWidth to the specified value. */
	public Cell<T> Width (float width) {
		Width(Fixed.valueOf(width));
		return this;
	}

	/** Sets the minHeight, prefHeight, and maxHeight to the specified value. */
	public Cell<T> Height (Value height) {
		minHeight = height ?? throw new IllegalArgumentException("height cannot be null.");
		prefHeight = height;
		maxHeight = height;
		return this;
	}

	/** Sets the minHeight, prefHeight, and maxHeight to the specified value. */
	public Cell<T> Height (float height) {
		Height(Fixed.valueOf(height));
		return this;
	}

	/** Sets the minWidth and minHeight to the specified value. */
	public Cell<T> MinSize (Value size) {
		minWidth = size ?? throw new IllegalArgumentException("size cannot be null.");
		minHeight = size;
		return this;
	}

	/** Sets the minWidth and minHeight to the specified values. */
	public Cell<T> MinSize (Value width, Value height) {
		minWidth = width ?? throw new IllegalArgumentException("width cannot be null.");
		minHeight = height ?? throw new IllegalArgumentException("height cannot be null.");
		return this;
	}

	public Cell<T> MinWidth (Value minWidth) {
		this.minWidth = minWidth ?? throw new IllegalArgumentException("minWidth cannot be null.");
		return this;
	}

	public Cell<T> MinHeight (Value minHeight) {
		this.minHeight = minHeight ?? throw new IllegalArgumentException("minHeight cannot be null.");
		return this;
	}

	/** Sets the minWidth and minHeight to the specified value. */
	public Cell<T> MinSize (float size) {
		MinSize(Fixed.valueOf(size));
		return this;
	}

	/** Sets the minWidth and minHeight to the specified values. */
	public Cell<T> MinSize (float width, float height) {
		MinSize(Fixed.valueOf(width), Fixed.valueOf(height));
		return this;
	}

	public Cell<T> MinWidth (float minWidth) {
		this.minWidth = Fixed.valueOf(minWidth);
		return this;
	}

	public Cell<T> MinHeight (float minHeight) {
		this.minHeight = Fixed.valueOf(minHeight);
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified value. */
	public Cell<T> PrefSize (Value size) {
		prefWidth = size ?? throw new IllegalArgumentException("size cannot be null.");
		prefHeight = size;
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified values. */
	public Cell<T> PrefSize (Value width, Value height) {
		prefWidth = width ?? throw new IllegalArgumentException("width cannot be null.");
		prefHeight = height ?? throw new IllegalArgumentException("height cannot be null.");
		return this;
	}

	public Cell<T> PrefWidth (Value prefWidth) {
		this.prefWidth = prefWidth ?? throw new IllegalArgumentException("prefWidth cannot be null.");
		return this;
	}

	public Cell<T> PrefHeight (Value prefHeight) {
		this.prefHeight = prefHeight ?? throw new IllegalArgumentException("prefHeight cannot be null.");
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified value. */
	public Cell<T> PrefSize (float width, float height) {
		PrefSize(Fixed.valueOf(width), Fixed.valueOf(height));
		return this;
	}

	/** Sets the prefWidth and prefHeight to the specified values. */
	public Cell<T> PrefSize (float size) {
		PrefSize(Fixed.valueOf(size));
		return this;
	}

	public Cell<T> PrefWidth (float prefWidth) {
		this.prefWidth = Fixed.valueOf(prefWidth);
		return this;
	}

	public Cell<T> PrefHeight (float prefHeight) {
		this.prefHeight = Fixed.valueOf(prefHeight);
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified value. If the max size is 0, no maximum size is used. */
	public Cell<T> MaxSize (Value size) {
		maxWidth = size ?? throw new IllegalArgumentException("size cannot be null.");
		maxHeight = size;
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified values. If the max size is 0, no maximum size is used. */
	public Cell<T> MaxSize (Value width, Value height) {
		maxWidth = width ?? throw new IllegalArgumentException("width cannot be null.");
		maxHeight = height ?? throw new IllegalArgumentException("height cannot be null.");
		return this;
	}

	/** If the maxWidth is 0, no maximum width is used. */
	public Cell<T> MaxWidth (Value maxWidth) {
		this.maxWidth = maxWidth ?? throw new IllegalArgumentException("maxWidth cannot be null.");
		return this;
	}

	/** If the maxHeight is 0, no maximum height is used. */
	public Cell<T> MaxHeight (Value maxHeight) {
		this.maxHeight = maxHeight ?? throw new IllegalArgumentException("maxHeight cannot be null.");
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified value. If the max size is 0, no maximum size is used. */
	public Cell<T> MaxSize (float size) {
		MaxSize(Fixed.valueOf(size));
		return this;
	}

	/** Sets the maxWidth and maxHeight to the specified values. If the max size is 0, no maximum size is used. */
	public Cell<T> MaxSize (float width, float height) {
		MaxSize(Fixed.valueOf(width), Fixed.valueOf(height));
		return this;
	}

	/** If the maxWidth is 0, no maximum width is used. */
	public Cell<T> MaxWidth (float maxWidth) {
		this.maxWidth = Fixed.valueOf(maxWidth);
		return this;
	}

	/** If the maxHeight is 0, no maximum height is used. */
	public Cell<T> MaxHeight (float maxHeight) {
		this.maxHeight = Fixed.valueOf(maxHeight);
		return this;
	}

	/** Sets the spaceTop, spaceLeft, spaceBottom, and spaceRight to the specified value. */
	public new Cell<T> Space (Value space) {
		spaceTop = space ?? throw new IllegalArgumentException("space cannot be null.");
		spaceLeft = space;
		spaceBottom = space;
		spaceRight = space;
		return this;
	}

	public Cell<T> Space (Value top, Value left, Value bottom, Value right) {
		spaceTop = top ?? throw new IllegalArgumentException("top cannot be null.");
		spaceLeft = left ?? throw new IllegalArgumentException("left cannot be null.");
		spaceBottom = bottom ?? throw new IllegalArgumentException("bottom cannot be null.");
		spaceRight = right ?? throw new IllegalArgumentException("right cannot be null.");
		return this;
	}

	public Cell<T> SpaceTop (Value spaceTop) {
		this.spaceTop = spaceTop ?? throw new IllegalArgumentException("spaceTop cannot be null.");
		return this;
	}

	public Cell<T> SpaceLeft (Value spaceLeft) {
		this.spaceLeft = spaceLeft ?? throw new IllegalArgumentException("spaceLeft cannot be null.");
		return this;
	}

	public Cell<T> SpaceBottom (Value spaceBottom) {
		this.spaceBottom = spaceBottom ?? throw new IllegalArgumentException("spaceBottom cannot be null.");
		return this;
	}

	public Cell<T> SpaceRight (Value spaceRight) {
		this.spaceRight = spaceRight ?? throw new IllegalArgumentException("spaceRight cannot be null.");
		return this;
	}

		/** Sets the spaceTop, spaceLeft, spaceBottom, and spaceRight to the specified value. The space cannot be < 0. */
		public override Cell<T> Space (float space) {
		if (space < 0) throw new IllegalArgumentException("space cannot be < 0: " + space);
		Space(Fixed.valueOf(space));
		return this;
	}

	/** The space cannot be < 0. */
	public Cell<T> Space (float top, float left, float bottom, float right) {
		if (top < 0) throw new IllegalArgumentException("top cannot be < 0: " + top);
		if (left < 0) throw new IllegalArgumentException("left cannot be < 0: " + left);
		if (bottom < 0) throw new IllegalArgumentException("bottom cannot be < 0: " + bottom);
		if (right < 0) throw new IllegalArgumentException("right cannot be < 0: " + right);
		Space(Fixed.valueOf(top), Fixed.valueOf(left), Fixed.valueOf(bottom), Fixed.valueOf(right));
		return this;
	}

	/** The space cannot be < 0. */
	public Cell<T> SpaceTop (float spaceTop) {
		if (spaceTop < 0) throw new IllegalArgumentException("spaceTop cannot be < 0: " + spaceTop);
		this.spaceTop = Fixed.valueOf(spaceTop);
		return this;
	}

	/** The space cannot be < 0. */
	public Cell<T> SpaceLeft (float spaceLeft) {
		if (spaceLeft < 0) throw new IllegalArgumentException("spaceLeft cannot be < 0: " + spaceLeft);
		this.spaceLeft = Fixed.valueOf(spaceLeft);
		return this;
	}

	/** The space cannot be < 0. */
	public Cell<T> SpaceBottom (float spaceBottom) {
		if (spaceBottom < 0) throw new IllegalArgumentException("spaceBottom cannot be < 0: " + spaceBottom);
		this.spaceBottom = Fixed.valueOf(spaceBottom);
		return this;
	}

	/** The space cannot be < 0. */
	public Cell<T> SpaceRight (float spaceRight) {
		if (spaceRight < 0) throw new IllegalArgumentException("spaceRight cannot be < 0: " + spaceRight);
		this.spaceRight = Fixed.valueOf(spaceRight);
		return this;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight to the specified value. */
	public Cell<T> Pad (Value pad) {
		padTop = pad ?? throw new IllegalArgumentException("pad cannot be null.");
		padLeft = pad;
		padBottom = pad;
		padRight = pad;
		return this;
	}

	public Cell<T> Pad (Value top, Value left, Value bottom, Value right) {
		padTop = top ?? throw new IllegalArgumentException("top cannot be null.");
		padLeft = left ?? throw new IllegalArgumentException("left cannot be null.");
		padBottom = bottom ?? throw new IllegalArgumentException("bottom cannot be null.");
		padRight = right ?? throw new IllegalArgumentException("right cannot be null.");
		return this;
	}

	public Cell<T> PadTop (Value padTop) {
		this.padTop = padTop ?? throw new IllegalArgumentException("padTop cannot be null.");
		return this;
	}

	public Cell<T> PadLeft (Value padLeft) {
		this.padLeft = padLeft ?? throw new IllegalArgumentException("padLeft cannot be null.");
		return this;
	}

	public Cell<T> PadBottom (Value padBottom) {
		this.padBottom = padBottom ?? throw new IllegalArgumentException("padBottom cannot be null.");
		return this;
	}

	public Cell<T> PadRight (Value padRight) {
		this.padRight = padRight ?? throw new IllegalArgumentException("padRight cannot be null.");
		return this;
	}

	/** Sets the padTop, padLeft, padBottom, and padRight to the specified value. */
	public Cell<T> Pad (float pad) {
		Pad(Fixed.valueOf(pad));
		return this;
	}

	public Cell<T> Pad (float top, float left, float bottom, float right) {
		Pad(Fixed.valueOf(top), Fixed.valueOf(left), Fixed.valueOf(bottom), Fixed.valueOf(right));
		return this;
	}

	public Cell<T> PadTop (float padTop) {
		this.padTop = Fixed.valueOf(padTop);
		return this;
	}

	public Cell<T> PadLeft (float padLeft) {
		this.padLeft = Fixed.valueOf(padLeft);
		return this;
	}

	public Cell<T> PadBottom (float padBottom) {
		this.padBottom = Fixed.valueOf(padBottom);
		return this;
	}

	public Cell<T> PadRight (float padRight) {
		this.padRight = Fixed.valueOf(padRight);
		return this;
	}

	/** Sets fillX and fillY to 1. */
	public Cell<T> Fill () {
		fillX = onef;
		fillY = onef;
		return this;
	}

	/** Sets fillX to 1. */
	public Cell<T> FillX () {
		fillX = onef;
		return this;
	}

	/** Sets fillY to 1. */
	public Cell<T> FillY () {
		fillY = onef;
		return this;
	}

	public Cell<T> Fill (float x, float y) {
		fillX = x;
		fillY = y;
		return this;
	}

	/** Sets fillX and fillY to 1 if true, 0 if false. */
	public Cell<T> Fill (bool x, bool y) {
		fillX = x ? onef : zerof;
		fillY = y ? onef : zerof;
		return this;
	}

	/** Sets fillX and fillY to 1 if true, 0 if false. */
	public Cell<T> Fill (bool fill) {
		fillX = fill ? onef : zerof;
		fillY = fill ? onef : zerof;
		return this;
	}

	/** Sets the alignment of the actor within the cell. Set to {@link Align#center}, {@link Align#top}, {@link Align#bottom},
	 * {@link Align#left}, {@link Align#right}, or any combination of those. */
	public Cell<T> Align (int align) {
		this.align = align;
		return this;
	}

	/** Sets the alignment of the actor within the cell to {@link Align#center}. This clears any other alignment. */
	public Cell<T> Center () {
		align = centeri;
		return this;
	}

	/** Adds {@link Align#top} and clears {@link Align#bottom} for the alignment of the actor within the cell. */
	public Cell<T> Top () {
		if (align == null)
			align = topi;
		else
			align = (align | SharpGDX.Utils.Align.top) & ~SharpGDX.Utils.Align.bottom;
		return this;
	}

	/** Adds {@link Align#left} and clears {@link Align#right} for the alignment of the actor within the cell. */
	public Cell<T> Left () {
		if (align == null)
			align = lefti;
		else
			align = (align | SharpGDX.Utils.Align.left) & ~SharpGDX.Utils.Align.right;
		return this;
	}

	/** Adds {@link Align#bottom} and clears {@link Align#top} for the alignment of the actor within the cell. */
	public Cell<T> Bottom () {
		if (align == null)
			align = bottomi;
		else
			align = (align | SharpGDX.Utils.Align.bottom) & ~SharpGDX.Utils.Align.top;
		return this;
	}

	/** Adds {@link Align#right} and clears {@link Align#left} for the alignment of the actor within the cell. */
	public Cell<T> Right () {
		if (align == null)
			align = righti;
		else
			align = (align | SharpGDX.Utils.Align.right) & ~SharpGDX.Utils.Align.left;
		return this;
	}

	/** Sets expandX, expandY, fillX, and fillY to 1. */
	public Cell<T> Grow () {
		expandX = onei;
		expandY = onei;
		fillX = onef;
		fillY = onef;
		return this;
	}

	/** Sets expandX and fillX to 1. */
	public Cell<T> GrowX () {
		expandX = onei;
		fillX = onef;
		return this;
	}

	/** Sets expandY and fillY to 1. */
	public Cell<T> GrowY () {
		expandY = onei;
		fillY = onef;
		return this;
	}

	/** Sets expandX and expandY to 1. */
	public Cell<T> Expand () {
		expandX = onei;
		expandY = onei;
		return this;
	}

	/** Sets expandX to 1. */
	public Cell<T> ExpandX () {
		expandX = onei;
		return this;
	}

	/** Sets expandY to 1. */
	public Cell<T> ExpandY () {
		expandY = onei;
		return this;
	}

	public Cell<T> Expand (int x, int y) {
		expandX = x;
		expandY = y;
		return this;
	}

	/** Sets expandX and expandY to 1 if true, 0 if false. */
	public Cell<T> Expand (bool x, bool y) {
		expandX = x ? onei : zeroi;
		expandY = y ? onei : zeroi;
		return this;
	}

	public Cell<T> Colspan (int colspan) {
		this.colspan = colspan;
		return this;
	}

	/** Sets uniformX and uniformY to true. */
	public Cell<T> Uniform () {
		uniformX = true;
		uniformY = true;
		return this;
	}

	/** Sets uniformX to true. */
	public Cell<T> UniformX () {
		uniformX = true;
		return this;
	}

	/** Sets uniformY to true. */
	public Cell<T> UniformY () {
		uniformY = true;
		return this;
	}

	public Cell<T> Uniform (bool uniform) {
		uniformX = uniform;
		uniformY = uniform;
		return this;
	}

	public Cell<T> Uniform (bool x, bool y) {
		uniformX = x;
		uniformY = y;
		return this;
	}

	public void SetActorBounds (float x, float y, float width, float height) {
		actorX = x;
		actorY = y;
		actorWidth = width;
		actorHeight = height;
	}

	public float GetActorX () {
		return actorX;
	}

	public void SetActorX (float actorX) {
		this.actorX = actorX;
	}

	public float GetActorY () {
		return actorY;
	}

	public void SetActorY (float actorY) {
		this.actorY = actorY;
	}

	public float GetActorWidth () {
		return actorWidth;
	}

	public void SetActorWidth (float actorWidth) {
		this.actorWidth = actorWidth;
	}

	public float GetActorHeight () {
		return actorHeight;
	}

	public void SetActorHeight (float actorHeight) {
		this.actorHeight = actorHeight;
	}

	public int GetColumn () {
		return column;
	}

	public int GetRow () {
		return row;
	}

	/** @return May be null if this cell is row defaults. */
	public Value? GetMinWidthValue () {
		return minWidth;
	}

	public float? GetMinWidth () {
		return minWidth?.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? GetMinHeightValue () {
		return minHeight;
	}

	public float? GetMinHeight () {
		return minHeight?.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? GetPrefWidthValue () {
		return prefWidth;
	}

	public float? GetPrefWidth () {
		return prefWidth?.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? GetPrefHeightValue () {
		return prefHeight;
	}

	public float? GetPrefHeight () {
		return prefHeight?.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? GetMaxWidthValue () {
		return maxWidth;
	}

	public float? GetMaxWidth () {
		return maxWidth?.get(actor);
	}

	/** @return May be null if this cell is row defaults. */
	public Value? GetMaxHeightValue () {
		return maxHeight;
	}

	public float? GetMaxHeight () {
		return maxHeight?.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? GetSpaceTopValue () {
		return spaceTop;
	}

	public float? GetSpaceTop () {
		return spaceTop?.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? GetSpaceLeftValue () {
		return spaceLeft;
	}

	public float? GetSpaceLeft () {
		return spaceLeft?.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? GetSpaceBottomValue () {
		return spaceBottom;
	}

	public float? GetSpaceBottom () {
		return spaceBottom?.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? GetSpaceRightValue () {
		return spaceRight;
	}

	public float? GetSpaceRight () {
		return spaceRight?.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? GetPadTopValue () {
		return padTop;
	}

	public float? GetPadTop () {
		return padTop?.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? GetPadLeftValue () {
		return padLeft;
	}

	public float? GetPadLeft () {
		return padLeft?.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? GetPadBottomValue () {
		return padBottom;
	}

	public float? GetPadBottom () {
		return padBottom?.get(actor);
	}

	/** @return May be null if this value is not set. */
	public Value? GetPadRightValue () {
		return padRight;
	}

	public float ?GetPadRight () {
		return padRight?.get(actor);
	}

	/** Returns {@link #getPadLeft()} plus {@link #getPadRight()}. */
	public float? GetPadX () {
		return padLeft?.get(actor) + padRight?.get(actor);
	}

	/** Returns {@link #getPadTop()} plus {@link #getPadBottom()}. */
	public float? GetPadY () {
		return padTop?.get(actor) + padBottom?.get(actor);
	}

	public float? GetFillX () {
		return fillX;
	}

	public float? GetFillY () {
		return fillY;
	}

	public int? GetAlign () {
		return align;
	}

	public int? GetExpandX () {
		return expandX;
	}

	public int? GetExpandY () {
		return expandY;
	}

		public int? GetColspan () {
		return colspan;
	}

	public bool? GetUniformX () {
		return uniformX;
	}

	public bool? GetUniformY () {
		return uniformY;
	}

	/** Returns true if this cell is the last cell in the row. */
	public bool IsEndRow () {
		return endRow;
	}

	/** The actual amount of combined padding and spacing from the last layout. */
	public float GetComputedPadTop () {
		return computedPadTop;
	}

	/** The actual amount of combined padding and spacing from the last layout. */
	public float GetComputedPadLeft () {
		return computedPadLeft;
	}

	/** The actual amount of combined padding and spacing from the last layout. */
	public float GetComputedPadBottom () {
		return computedPadBottom;
	}

	/** The actual amount of combined padding and spacing from the last layout. */
	public float GetComputedPadRight () {
		return computedPadRight;
	}

	public void Row () {
		table.row();
	}

	public Table GetTable () {
		return table;
	}

	/** Sets all constraint fields to null. */
	internal override void Clear () {
		minWidth = null;
		minHeight = null;
		prefWidth = null;
		prefHeight = null;
		maxWidth = null;
		maxHeight = null;
		spaceTop = null;
		spaceLeft = null;
		spaceBottom = null;
		spaceRight = null;
		padTop = null;
		padLeft = null;
		padBottom = null;
		padRight = null;
		fillX = null;
		fillY = null;
		align = null;
		expandX = null;
		expandY = null;
		colspan = null;
		uniformX = null;
		uniformY = null;
	}

	/** Reset state so the cell can be reused, setting all constraints to their {@link #defaults() default} values. */
	public override void reset () {
		actor = null;
		table = null;
		endRow = false;
		cellAboveIndex = -1;
		Set(Defaults());
	}
		
	public override String ToString () {
		return actor != null ? actor.ToString() : base.ToString();
	}

	/** Returns the defaults to use for all cells. This can be used to avoid needing to set the same defaults for every table (eg,
	 * for spacing). */
	static public Cell<T> Defaults () {
		if (files == null || files != Gdx.files) {
			files = Gdx.files;
			defaults = new Cell<T>();
			defaults.minWidth = Value.minWidth;
			defaults.minHeight = Value.minHeight;
			defaults.prefWidth = Value.prefWidth;
			defaults.prefHeight = Value.prefHeight;
			defaults.maxWidth = Value.maxWidth;
			defaults.maxHeight = Value.maxHeight;
			defaults.spaceTop = Value.zero;
			defaults.spaceLeft = Value.zero;
			defaults.spaceBottom = Value.zero;
			defaults.spaceRight = Value.zero;
			defaults.padTop = Value.zero;
			defaults.padLeft = Value.zero;
			defaults.padBottom = Value.zero;
			defaults.padRight = Value.zero;
			defaults.fillX = zerof;
			defaults.fillY = zerof;
			defaults.align = centeri;
			defaults.expandX = zeroi;
			defaults.expandY = zeroi;
			defaults.colspan = onei;
			defaults.uniformX = null;
			defaults.uniformY = null;
		}
		return defaults;
	}
}
}
