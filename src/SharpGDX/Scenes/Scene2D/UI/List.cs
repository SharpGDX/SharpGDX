using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A list (aka list box) displays textual items and highlights the currently selected item.
 * <p>
 * {@link ChangeEvent} is fired when the list selection changes.
 * <p>
 * The preferred size of the list is determined by the text bounds of the items and the size of the {@link ListStyle#selection}.
 * @author mzechner
 * @author Nathan Sweet */
public class List<T> : Widget , ICullable {
	ListStyle style;
	readonly Array<T> items = new ();
	internal ArraySelection<T> selection ;
	private Rectangle cullingArea;
	private float prefWidth, prefHeight;
	float itemHeight;
	private int alignment = Align.left;
	int pressedIndex = -1, overIndex = -1;
	private InputListener keyListener;
	bool typeToSelect;

	public List (Skin skin) 
	: this(skin.get<ListStyle>(typeof(ListStyle)))
	{
		
	}

	public List (Skin skin, String styleName) 
	: this(skin.get<ListStyle>(styleName, typeof(ListStyle)))
	{
		
	}

	private class KeyInputListener : InputListener
	{
		long typeTimeout;
		String prefix;

		private readonly List<T> _list;

		public KeyInputListener(List<T> list)
		{
			_list = list;
		}

		public override bool keyDown(InputEvent @event, int keycode)
		{
			if (_list.items.isEmpty()) return false;
			int index;
			switch (keycode)
			{
				case IInput.Keys.A:
					if (UIUtils.ctrl() && _list.selection.getMultiple())
					{
						_list.selection.clear();
						_list.selection.addAll(_list.items);
						return true;
					}
					break;
				case IInput.Keys.HOME:
					_list.setSelectedIndex(0);
					return true;
				case IInput.Keys.END:
					_list.setSelectedIndex(_list.items.size - 1);
					return true;
				case IInput.Keys.DOWN:
					index = _list.items.indexOf(_list.getSelected(), false) + 1;
					if (index >= _list.items.size) index = 0;
					_list.setSelectedIndex(index);
					return true;
				case IInput.Keys.UP:
					index = _list.items.indexOf(_list.getSelected(), false) - 1;
					if (index < 0) index = _list.items.size - 1;
					_list.setSelectedIndex(index);
					return true;
				case IInput.Keys.ESCAPE:
					if (_list.getStage() != null) _list.getStage().setKeyboardFocus(null);
					return true;
			}
			return false;
		}

		public override bool keyTyped(InputEvent @event, char character)
		{
			if (!_list.typeToSelect) return false;
			long time = TimeUtils.currentTimeMillis();
			if (time > typeTimeout) prefix = "";
			typeTimeout = time + 300;
			prefix += Char.ToLower(character);
			for (int i = 0, n = _list.items.size; i < n; i++)
			{
				if (_list.toString(_list.items.get(i)).ToLower().StartsWith(prefix)) {
					_list.setSelectedIndex(i);
					break;
				}
			}
			return false;
		}
	}

	private class TouchInputListener:InputListener
	{
		private readonly List<T> _list;

		public TouchInputListener(List<T> list)
		{
			_list = list;
		}

		public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (pointer != 0 || button != 0) return true;
			if (_list.selection.isDisabled()) return true;
			if (_list.getStage() != null) _list.getStage().setKeyboardFocus(_list);
			if (_list.items.size == 0) return true;
			int index = _list.getItemIndexAt(y);
			if (index == -1) return true;
			_list.selection.choose(_list.items.get(index));
			_list.pressedIndex = index;
			return true;
		}

		public override void touchUp(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (pointer != 0 || button != 0) return;
			_list.pressedIndex = -1;
		}

		public override void touchDragged(InputEvent @event, float x, float y, int pointer)
		{
			_list.overIndex = _list.getItemIndexAt(y);
		}

		public override bool mouseMoved(InputEvent @event, float x, float y)
		{
			_list.overIndex = _list.getItemIndexAt(y);
			return false;
		}

		public override void exit(InputEvent @event, float x, float y, int pointer, Actor toActor)
		{
			if (pointer == 0) _list.pressedIndex = -1;
			if (pointer == -1) _list.overIndex = -1;
		}
	}

public List (ListStyle style)
{
	selection = new(items);
		selection.setActor(this);
		selection.setRequired(true);

		setStyle(style);
		setSize(getPrefWidth(), getPrefHeight());

		addListener(keyListener = new KeyInputListener(this) );

		addListener(new TouchInputListener(this) );
	}

	public void setStyle (ListStyle style) {
		if (style == null) throw new IllegalArgumentException("style cannot be null.");
		this.style = style;
		invalidateHierarchy();
	}

	/** Returns the list's style. Modifying the returned style may not have an effect until {@link #setStyle(ListStyle)} is
	 * called. */
	public ListStyle getStyle () {
		return style;
	}

	public override void layout () {
		BitmapFont font = style.font;
		IDrawable selectedDrawable = style.selection;

		itemHeight = font.getCapHeight() - font.getDescent() * 2;
		itemHeight += selectedDrawable.getTopHeight() + selectedDrawable.getBottomHeight();

		prefWidth = 0;
		Pool<GlyphLayout> layoutPool = Pools.get<GlyphLayout>(typeof(GlyphLayout));
		GlyphLayout layout = layoutPool.obtain();
		for (int i = 0; i < items.size; i++) {
			layout.setText(font, toString(items.get(i)));
			prefWidth = Math.Max(layout.width, prefWidth);
		}
		layoutPool.free(layout);
		prefWidth += selectedDrawable.getLeftWidth() + selectedDrawable.getRightWidth();
		prefHeight = items.size * itemHeight;

		IDrawable background = style.background;
		if (background != null) {
			prefWidth = Math.Max(prefWidth + background.getLeftWidth() + background.getRightWidth(), background.getMinWidth());
			prefHeight = Math.Max(prefHeight + background.getTopHeight() + background.getBottomHeight(), background.getMinHeight());
		}
	}

	public override void draw (IBatch batch, float parentAlpha) {
		validate();

		drawBackground(batch, parentAlpha);

		BitmapFont font = style.font;
		IDrawable selectedDrawable = style.selection;
		Color fontColorSelected = style.fontColorSelected;
		Color fontColorUnselected = style.fontColorUnselected;

		Color color = getColor();
		batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);

		float x = getX(), y = getY(), width = getWidth(), height = getHeight();
		float itemY = height;

		IDrawable background = style.background;
		if (background != null) {
			float leftWidth = background.getLeftWidth();
			x += leftWidth;
			itemY -= background.getTopHeight();
			width -= leftWidth + background.getRightWidth();
		}

		float textOffsetX = selectedDrawable.getLeftWidth(), textWidth = width - textOffsetX - selectedDrawable.getRightWidth();
		float textOffsetY = selectedDrawable.getTopHeight() - font.getDescent();

		font.setColor(fontColorUnselected.r, fontColorUnselected.g, fontColorUnselected.b, fontColorUnselected.a * parentAlpha);
		for (int i = 0; i < items.size; i++) {
			if (cullingArea == null || (itemY - itemHeight <= cullingArea.y + cullingArea.height && itemY >= cullingArea.y)) {
				T item = items.get(i);
				bool selected = selection.contains(item);
				IDrawable drawable = null;
				if (pressedIndex == i && style.down != null)
					drawable = style.down;
				else if (selected) {
					drawable = selectedDrawable;
					font.setColor(fontColorSelected.r, fontColorSelected.g, fontColorSelected.b, fontColorSelected.a * parentAlpha);
				} else if (overIndex == i && style.over != null) //
					drawable = style.over;
				drawSelection(batch, drawable, x, y + itemY - itemHeight, width, itemHeight);
				drawItem(batch, font, i, item, x + textOffsetX, y + itemY - textOffsetY, textWidth);
				if (selected) {
					font.setColor(fontColorUnselected.r, fontColorUnselected.g, fontColorUnselected.b,
						fontColorUnselected.a * parentAlpha);
				}
			} else if (itemY < cullingArea.y) {
				break;
			}
			itemY -= itemHeight;
		}
	}

	protected void drawSelection (IBatch batch, IDrawable? drawable, float x, float y, float width, float height) {
		if (drawable != null) drawable.draw(batch, x, y, width, height);
	}

	/** Called to draw the background. Default implementation draws the style background drawable. */
	protected void drawBackground (IBatch batch, float parentAlpha) {
		if (style.background != null) {
			Color color = getColor();
			batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);
			style.background.draw(batch, getX(), getY(), getWidth(), getHeight());
		}
	}

	protected GlyphLayout drawItem (IBatch batch, BitmapFont font, int index, T item, float x, float y, float width) {
		String @string = toString(item);
		return font.draw(batch, @string, x, y, 0, @string.Length, width, alignment, false, "...");
	}

	public ArraySelection<T> getSelection () {
		return selection;
	}

	public void setSelection (ArraySelection<T> selection) {
		this.selection = selection;
	}

	/** Returns the first selected item, or null. */
	public T? getSelected () {
		return selection.first();
	}

	/** Sets the selection to only the passed item, if it is a possible choice.
	 * @param item May be null. */
	public void setSelected (T? item) {
		if (items.contains(item, false))
			selection.set(item);
		else if (selection.getRequired() && items.size > 0)
			selection.set(items.first());
		else
			selection.clear();
	}

	/** @return The index of the first selected item. The top item has an index of 0. Nothing selected has an index of -1. */
	public int getSelectedIndex () {
		ObjectSet<T> selected = selection.items();
		return selected.size == 0 ? -1 : items.indexOf(selected.first(), false);
	}

	/** Sets the selection to only the selected index.
	 * @param index -1 to clear the selection. */
	public void setSelectedIndex (int index) {
		if (index < -1 || index >= items.size)
			throw new IllegalArgumentException("index must be >= -1 and < " + items.size + ": " + index);
		if (index == -1) {
			selection.clear();
		} else {
			selection.set(items.get(index));
		}
	}

	/** @return May be null. */
	public T? getOverItem () {
		return overIndex == -1 ? default : items.get(overIndex);
	}

	/** @return May be null. */
	public T? getPressedItem () {
		return pressedIndex == -1 ? default : items.get(pressedIndex);
	}

	/** @return null if not over an item. */
	public T? getItemAt (float y) {
		int index = getItemIndexAt(y);
		if (index == -1) return default;
		return items.get(index);
	}

	/** @return -1 if not over an item. */
	public int getItemIndexAt (float y) {
		float height = getHeight();
		IDrawable background = this.style.background;
		if (background != null) {
			height -= background.getTopHeight() + background.getBottomHeight();
			y -= background.getBottomHeight();
		}
		int index = (int)((height - y) / itemHeight);
		if (index < 0 || index >= items.size) return -1;
		return index;
	}

	public void setItems (T[] newItems) {
		if (newItems == null) throw new IllegalArgumentException("newItems cannot be null.");
		float oldPrefWidth = getPrefWidth(), oldPrefHeight = getPrefHeight();

		items.clear();
		items.addAll(newItems);
		overIndex = -1;
		pressedIndex = -1;
		selection.validate();

		invalidate();
		if (oldPrefWidth != getPrefWidth() || oldPrefHeight != getPrefHeight()) invalidateHierarchy();
	}

	/** Sets the items visible in the list, clearing the selection if it is no longer valid. If a selection is
	 * {@link ArraySelection#getRequired()}, the first item is selected. This can safely be called with a (modified) array returned
	 * from {@link #getItems()}. */
	public void setItems (Array<T> newItems) {
		if (newItems == null) throw new IllegalArgumentException("newItems cannot be null.");
		float oldPrefWidth = getPrefWidth(), oldPrefHeight = getPrefHeight();

		if (newItems != items) {
			items.clear();
			items.addAll(newItems);
		}
		overIndex = -1;
		pressedIndex = -1;
		selection.validate();

		invalidate();
		if (oldPrefWidth != getPrefWidth() || oldPrefHeight != getPrefHeight()) invalidateHierarchy();
	}

	public void clearItems () {
		if (items.size == 0) return;
		items.clear();
		overIndex = -1;
		pressedIndex = -1;
		selection.clear();
		invalidateHierarchy();
	}

	/** Returns the internal items array. If modified, {@link #setItems(Array)} must be called to reflect the changes. */
	public Array<T> getItems () {
		return items;
	}

	public float getItemHeight () {
		return itemHeight;
	}

	public override float getPrefWidth () {
		validate();
		return prefWidth;
	}

	public override float getPrefHeight () {
		validate();
		return prefHeight;
	}

	public String toString (T obj) {
		return obj.ToString();
	}

	public void setCullingArea (Rectangle? cullingArea) {
		this.cullingArea = cullingArea;
	}

	/** @return May be null.
	 * @see #setCullingArea(Rectangle) */
	public Rectangle getCullingArea () {
		return cullingArea;
	}

	/** Sets the horizontal alignment of the list items.
	 * @param alignment See {@link Align}. */
	public void setAlignment (int alignment) {
		this.alignment = alignment;
	}

	public int getAlignment () {
		return alignment;
	}

	public void setTypeToSelect (bool typeToSelect) {
		this.typeToSelect = typeToSelect;
	}

	public InputListener getKeyListener () {
		return keyListener;
	}

	/** The style for a list, see {@link List}.
	 * @author mzechner
	 * @author Nathan Sweet */
	public class ListStyle {
		public BitmapFont font;
		public Color fontColorSelected = new Color(1, 1, 1, 1);
		public Color fontColorUnselected = new Color(1, 1, 1, 1);
		public IDrawable selection;
		public IDrawable? down, over, background;

		public ListStyle () {
		}

		public ListStyle (BitmapFont font, Color fontColorSelected, Color fontColorUnselected, IDrawable selection) {
			this.font = font;
			this.fontColorSelected.set(fontColorSelected);
			this.fontColorUnselected.set(fontColorUnselected);
			this.selection = selection;
		}

		public ListStyle (ListStyle style) {
			font = style.font;
			fontColorSelected.set(style.fontColorSelected);
			fontColorUnselected.set(style.fontColorUnselected);
			selection = style.selection;

			down = style.down;
			over = style.over;
			background = style.background;
		}
	}
}
