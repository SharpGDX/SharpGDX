using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using static SharpGDX.Scenes.Scene2D.Actions.Actions;
using SharpGDX;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.UI;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A select box (aka a drop-down list) allows a user to choose one of a number of values from a list. When inactive, the selected
 * value is displayed. When activated, it shows the list of values that may be selected.
 * <p>
 * {@link ChangeEvent} is fired when the selectbox selection changes.
 * <p>
 * The preferred size of the select box is determined by the maximum text bounds of the items and the size of the
 * {@link SelectBoxStyle#background}.
 * @author mzechner
 * @author Nathan Sweet */
public class SelectBox<T> : Widget , IDisableable {
	static readonly Vector2 temp = new Vector2();

	SelectBoxStyle style;
	readonly Array<T> items = new ();
	SelectBoxScrollPane scrollPane;
	private float prefWidth, prefHeight;
	private ClickListener clickListener;
	bool disabled;
	private int alignment = Align.left;
	bool selectedPrefWidth;

	readonly ArraySelection<T> selection;

private class SelectBoxSelection : ArraySelection<T>
{
	private readonly SelectBox<T> _selectBox;

	public SelectBoxSelection(SelectBox<T> selectBox, Array<T> items)
	:base(items)
	{
		_selectBox = selectBox;
	}

	public override bool fireChangeEvent()
	{
		if (_selectBox.selectedPrefWidth) _selectBox.invalidateHierarchy();
		return base.fireChangeEvent();
	}
	}

	public SelectBox (Skin skin) 
: this(skin.get<SelectBoxStyle>(typeof(SelectBoxStyle)))
	{
		
	}

	public SelectBox (Skin skin, String styleName) 
	: this(skin.get<SelectBoxStyle>(styleName, typeof(SelectBoxStyle)))
	{
		
	}

	public SelectBox (SelectBoxStyle style)
	{
		selection = new SelectBoxSelection(this, items);
		setStyle(style);
		setSize(getPrefWidth(), getPrefHeight());

		selection.setActor(this);
		selection.setRequired(true);

		scrollPane = newScrollPane();

		addListener(clickListener = new SelectBoxSelectionClickListener(this) );
	}

	private class SelectBoxSelectionClickListener : ClickListener
	{
		private readonly SelectBox<T> _selectBox;

		public SelectBoxSelectionClickListener(SelectBox<T> selectBox)
		{
			_selectBox = selectBox;
		}

		public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (pointer == 0 && button != 0) return false;
			if (_selectBox.isDisabled()) return false;
			if (_selectBox.scrollPane.hasParent())
				_selectBox.hideScrollPane();
			else
				_selectBox.showScrollPane();
			return true;
		}
	}

	/** Allows a subclass to customize the scroll pane shown when the select box is open. */
	protected SelectBoxScrollPane newScrollPane () {
		return new SelectBoxScrollPane(this);
	}

	/** Set the max number of items to display when the select box is opened. Set to 0 (the default) to display as many as fit in
	 * the stage height. */
	public void setMaxListCount (int maxListCount) {
		scrollPane.maxListCount = maxListCount;
	}

	/** @return Max number of items to display when the box is opened, or <= 0 to display them all. */
	public int getMaxListCount () {
		return scrollPane.maxListCount;
	}

	internal protected override void setStage (Stage stage) {
		if (stage == null) scrollPane.hide();
		base.setStage(stage);
	}

	public void setStyle (SelectBoxStyle style) {
		if (style == null) throw new IllegalArgumentException("style cannot be null.");
		this.style = style;

		if (scrollPane != null) {
			scrollPane.setStyle(style.scrollStyle);
			scrollPane.list.setStyle(style.listStyle);
		}
		invalidateHierarchy();
	}

	/** Returns the select box's style. Modifying the returned style may not have an effect until {@link #setStyle(SelectBoxStyle)}
	 * is called. */
	public SelectBoxStyle getStyle () {
		return style;
	}

	/** Set the backing Array that makes up the choices available in the SelectBox */
	public void setItems (T[] newItems) {
		if (newItems == null) throw new IllegalArgumentException("newItems cannot be null.");
		float oldPrefWidth = getPrefWidth();

		items.clear();
		items.addAll(newItems);
		selection.validate();
		scrollPane.list.setItems(items);

		invalidate();
		if (oldPrefWidth != getPrefWidth()) invalidateHierarchy();
	}

	/** Sets the items visible in the select box. */
	public void setItems (Array<T> newItems) {
		if (newItems == null) throw new IllegalArgumentException("newItems cannot be null.");
		float oldPrefWidth = getPrefWidth();

		if (newItems != items) {
			items.clear();
			items.addAll(newItems);
		}
		selection.validate();
		scrollPane.list.setItems(items);

		invalidate();
		if (oldPrefWidth != getPrefWidth()) invalidateHierarchy();
	}

	public void clearItems () {
		if (items.size == 0) return;
		items.clear();
		selection.clear();
		scrollPane.list.clearItems();
		invalidateHierarchy();
	}

	/** Returns the internal items array. If modified, {@link #setItems(Array)} must be called to reflect the changes. */
	public Array<T> getItems () {
		return items;
	}

	public override void layout () {
		IDrawable bg = style.background;
		BitmapFont font = style.font;

		if (bg != null) {
			prefHeight = Math.Max(bg.getTopHeight() + bg.getBottomHeight() + font.getCapHeight() - font.getDescent() * 2,
				bg.getMinHeight());
		} else
			prefHeight = font.getCapHeight() - font.getDescent() * 2;

		Pool<GlyphLayout> layoutPool = Pools.get<GlyphLayout>(typeof(GlyphLayout));
		GlyphLayout layout = layoutPool.obtain();
		if (selectedPrefWidth) {
			prefWidth = 0;
			if (bg != null) prefWidth = bg.getLeftWidth() + bg.getRightWidth();
			T selected = getSelected();
			if (selected != null) {
				layout.setText(font, toString(selected));
				prefWidth += layout.width;
			}
		} else {
			float maxItemWidth = 0;
			for (int i = 0; i < items.size; i++) {
				layout.setText(font, toString(items.get(i)));
				maxItemWidth = Math.Max(layout.width, maxItemWidth);
			}

			prefWidth = maxItemWidth;
			if (bg != null) prefWidth = Math.Max(prefWidth + bg.getLeftWidth() + bg.getRightWidth(), bg.getMinWidth());

			List<T>.ListStyle listStyle = style.listStyle;
			ScrollPane.ScrollPaneStyle scrollStyle = style.scrollStyle;
			float scrollWidth = maxItemWidth + listStyle.selection.getLeftWidth() + listStyle.selection.getRightWidth();
			bg = scrollStyle.background;
			if (bg != null) scrollWidth = Math.Max(scrollWidth + bg.getLeftWidth() + bg.getRightWidth(), bg.getMinWidth());
			if (scrollPane == null || !scrollPane.disableY) {
				scrollWidth += Math.Max(style.scrollStyle.vScroll != null ? style.scrollStyle.vScroll.getMinWidth() : 0,
					style.scrollStyle.vScrollKnob != null ? style.scrollStyle.vScrollKnob.getMinWidth() : 0);
			}
			prefWidth = Math.Max(prefWidth, scrollWidth);
		}
		layoutPool.free(layout);
	}

	/** Returns appropriate background drawable from the style based on the current select box state. */
	protected IDrawable? getBackgroundDrawable () {
		if (isDisabled() && style.backgroundDisabled != null) return style.backgroundDisabled;
		if (scrollPane.hasParent() && style.backgroundOpen != null) return style.backgroundOpen;
		if (isOver() && style.backgroundOver != null) return style.backgroundOver;
		return style.background;
	}

	/** Returns the appropriate label font color from the style based on the current button state. */
	protected Color getFontColor () {
		if (isDisabled() && style.disabledFontColor != null) return style.disabledFontColor;
		if (style.overFontColor != null && (isOver() || scrollPane.hasParent())) return style.overFontColor;
		return style.fontColor;
	}

	public override void draw (IBatch batch, float parentAlpha) {
		validate();

		IDrawable background = getBackgroundDrawable();
		Color fontColor = getFontColor();
		BitmapFont font = style.font;

		Color color = getColor();
		float x = getX(), y = getY();
		float width = getWidth(), height = getHeight();

		batch.setColor(color.r, color.g, color.b, color.a * parentAlpha);
		if (background != null) background.draw(batch, x, y, width, height);

		T selected = selection.first();
		if (selected != null) {
			if (background != null) {
				width -= background.getLeftWidth() + background.getRightWidth();
				height -= background.getBottomHeight() + background.getTopHeight();
				x += background.getLeftWidth();
				y += (int)(height / 2 + background.getBottomHeight() + font.getData().capHeight / 2);
			} else {
				y += (int)(height / 2 + font.getData().capHeight / 2);
			}
			font.setColor(fontColor.r, fontColor.g, fontColor.b, fontColor.a * parentAlpha);
			drawItem(batch, font, selected, x, y, width);
		}
	}

	protected GlyphLayout drawItem (IBatch batch, BitmapFont font, T item, float x, float y, float width) {
		String @string = toString(item);
		return font.draw(batch, @string, x, y, 0, @string.Length, width, alignment, false, "...");
	}

	/** Sets the alignment of the selected item in the select box. See {@link #getList()} and {@link List#setAlignment(int)} to set
	 * the alignment in the list shown when the select box is open.
	 * @param alignment See {@link Align}. */
	public void setAlignment (int alignment) {
		this.alignment = alignment;
	}

	/** Get the set of selected items, useful when multiple items are selected
	 * @return a Selection object containing the selected elements */
	public ArraySelection<T> getSelection () {
		return selection;
	}

	/** Returns the first selected item, or null. For multiple selections use {@link SelectBox#getSelection()}. */
	public T? getSelected () {
		return selection.first();
	}

	/** Sets the selection to only the passed item, if it is a possible choice, else selects the first item. */
	public void setSelected (T? item) {
		if (items.contains(item, false))
			selection.set(item);
		else if (items.size > 0)
			selection.set(items.first());
		else
			selection.clear();
	}

	/** @return The index of the first selected item. The top item has an index of 0. Nothing selected has an index of -1. */
	public int getSelectedIndex () {
		ObjectSet<T> selected = selection.items();
		return selected.size == 0 ? -1 : items.indexOf(selected.first(), false);
	}

	/** Sets the selection to only the selected index. */
	public void setSelectedIndex (int index) {
		selection.set(items.get(index));
	}

	/** When true the pref width is based on the selected item. */
	public void setSelectedPrefWidth (bool selectedPrefWidth) {
		this.selectedPrefWidth = selectedPrefWidth;
	}

	public bool getSelectedPrefWidth () {
		return selectedPrefWidth;
	}

	/** Returns the pref width of the select box if the widest item was selected, for use when
	 * {@link #setSelectedPrefWidth(bool)} is true. */
	public float getMaxSelectedPrefWidth () {
		Pool<GlyphLayout> layoutPool = Pools.get<GlyphLayout>(typeof(GlyphLayout));
		GlyphLayout layout = layoutPool.obtain();
		float width = 0;
		for (int i = 0; i < items.size; i++) {
			layout.setText(style.font, toString(items.get(i)));
			width = Math.Max(layout.width, width);
		}
		IDrawable bg = style.background;
		if (bg != null) width = Math.Max(width + bg.getLeftWidth() + bg.getRightWidth(), bg.getMinWidth());
		return width;
	}

	public void setDisabled (bool disabled) {
		if (disabled && !this.disabled) hideScrollPane();
		this.disabled = disabled;
	}

	public bool isDisabled () {
		return disabled;
	}

	public override float getPrefWidth () {
		validate();
		return prefWidth;
	}

	public override float getPrefHeight () {
		validate();
		return prefHeight;
	}

	protected String toString (T item) {
		return item.ToString();
	}

	public void showScrollPane () {
		if (items.size == 0) return;
		if (getStage() != null) scrollPane.show(getStage());
	}

	public void hideScrollPane () {
		scrollPane.hide();
	}

	/** Returns the list shown when the select box is open. */
	public List<T> getList () {
		return scrollPane.list;
	}

	/** Disables scrolling of the list shown when the select box is open. */
	public void setScrollingDisabled (bool y) {
		scrollPane.setScrollingDisabled(true, y);
		invalidateHierarchy();
	}

	/** Returns the scroll pane containing the list that is shown when the select box is open. */
	public SelectBoxScrollPane getScrollPane () {
		return scrollPane;
	}

	public bool isOver () {
		return clickListener.isOver();
	}

	public ClickListener getClickListener () {
		return clickListener;
	}

	protected void onShow (Actor scrollPane, bool below) {
		scrollPane.getColor().a = 0;
		scrollPane.addAction(fadeIn(0.3f, Interpolation.fade));
	}

	protected void onHide (Actor scrollPane) {
		scrollPane.getColor().a = 1;
		scrollPane.addAction(sequence(fadeOut(0.15f, Interpolation.fade), removeActor()));
	}

	/** The scroll pane shown when a select box is open.
	 * @author Nathan Sweet */
	public class SelectBoxScrollPane : ScrollPane {
		readonly SelectBox<T> selectBox;
		internal int maxListCount;
		private readonly Vector2 stagePosition = new Vector2();
		internal readonly List<T> list;
		private InputListener hideListener;
		private Actor previousScrollFocus;

		public SelectBoxScrollPane (SelectBox<T> selectBox) 
		: base(null, selectBox.style.scrollStyle)
		{
			
			this.selectBox = selectBox;

			setOverscroll(false, false);
			setFadeScrollBars(false);
			setScrollingDisabled(true, false);

			list = newList();
			list.setTouchable(Touchable.disabled);
			list.setTypeToSelect(true);
			setActor(list);

			list.addListener(new SelectBoxScrollPaneClickListener(this) );

			addListener(new ExitListener(this) );
		
			hideListener = new HideListener(this) ;
		}

		private class SelectBoxScrollPaneClickListener: ClickListener
		{
			private readonly SelectBoxScrollPane _selectBoxScrollPane;

			public SelectBoxScrollPaneClickListener(SelectBoxScrollPane selectBoxScrollPane)
			{
				_selectBoxScrollPane = selectBoxScrollPane;
			}
		public override void clicked(InputEvent @event, float x, float y)
			{
				T selected = _selectBoxScrollPane.list.getSelected();
				// Force clicking the already selected item to trigger a change event.
				if (selected != null) _selectBoxScrollPane.selectBox.selection.items().clear(51);
				_selectBoxScrollPane.selectBox.selection.choose(selected);
				_selectBoxScrollPane.hide();
			}

		public override bool mouseMoved(InputEvent @event, float x, float y)
			{
				int index = _selectBoxScrollPane.list.getItemIndexAt(y);
				if (index != -1) _selectBoxScrollPane.list.setSelectedIndex(index);
				return true;
			}
		}

	private class HideListener : InputListener
		{
			private readonly SelectBoxScrollPane _selectBoxScrollPane;

			public HideListener(SelectBoxScrollPane selectBoxScrollPane)
			{
				_selectBoxScrollPane = selectBoxScrollPane;
			}
	public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
			{
				Actor target = @event.getTarget();
				if (_selectBoxScrollPane.isAscendantOf(target)) return false;
				_selectBoxScrollPane.list.selection.set(_selectBoxScrollPane.selectBox.getSelected());
				_selectBoxScrollPane.hide();
				return false;
			}

		public override bool keyDown(InputEvent @event, int keycode)
			{
				switch (keycode)
				{
					case IInput.Keys.NUMPAD_ENTER:
					case IInput.Keys.ENTER:
						_selectBoxScrollPane.selectBox.selection.choose(_selectBoxScrollPane.list.getSelected());
						// TODO: This is ugly, need to condense code, C# can't fall thru.
				// Fall thru.
						_selectBoxScrollPane.hide();
						@event.stop();
						return true;
			case IInput.Keys.ESCAPE:
						_selectBoxScrollPane.hide();
						@event.stop();
						return true;
				}
				return false;
			}
		}

		private class ExitListener : InputListener
		{
			private readonly SelectBoxScrollPane _selectBoxScrollPane;
	
			public ExitListener(SelectBoxScrollPane selectBoxScrollPane)
			{
				_selectBoxScrollPane = selectBoxScrollPane;
			}

		public override void exit(InputEvent @event, float x, float y, int pointer, Actor? toActor)
			{
				if (toActor == null || !_selectBoxScrollPane.isAscendantOf(toActor))
				{
					T selected = _selectBoxScrollPane.selectBox.getSelected();
					if (selected != null) _selectBoxScrollPane.list.selection.set(selected);
				}
			}
		}

/** Allows a subclass to customize the select box list. The default implementation returns a list that delegates
 * {@link List#toString(Object)} to {@link SelectBox#toString(Object)}. */
protected List<T> newList () {
			return new SelectBoxList(selectBox.style.listStyle, this) ;
		}

		private class SelectBoxList : List<T>
		{
			private readonly SelectBoxScrollPane _selectBoxScrollPane;

			public SelectBoxList(ListStyle listStyle, SelectBoxScrollPane selectBoxScrollPane) : base(listStyle)
			{
				_selectBoxScrollPane = selectBoxScrollPane;
			}

				public new String toString(T obj)
				{
					return _selectBoxScrollPane.selectBox.toString(obj);
				}
	}

		public void show (Stage stage) {
			if (list.isTouchable()) return;

			stage.addActor(this);
			stage.addCaptureListener(hideListener);
			stage.addListener(list.getKeyListener());

			selectBox.localToStageCoordinates(stagePosition.set(0, 0));

			// Show the list above or below the select box, limited to a number of items and the available height in the stage.
			float itemHeight = list.getItemHeight();
			float height = itemHeight * (maxListCount <= 0 ? selectBox.items.size : Math.Min(maxListCount, selectBox.items.size));
			IDrawable scrollPaneBackground = getStyle().background;
			if (scrollPaneBackground != null) height += scrollPaneBackground.getTopHeight() + scrollPaneBackground.getBottomHeight();
			IDrawable listBackground = list.getStyle().background;
			if (listBackground != null) height += listBackground.getTopHeight() + listBackground.getBottomHeight();

			float heightBelow = stagePosition.y;
			float heightAbove = stage.getHeight() - heightBelow - selectBox.getHeight();
			bool below = true;
			if (height > heightBelow) {
				if (heightAbove > heightBelow) {
					below = false;
					height = Math.Min(height, heightAbove);
				} else
					height = heightBelow;
			}

			if (below)
				setY(stagePosition.y - height);
			else
				setY(stagePosition.y + selectBox.getHeight());
			setX(stagePosition.x);
			setHeight(height);
			validate();
			float width = Math.Max(getPrefWidth(), selectBox.getWidth());
			setWidth(width);

			validate();
			scrollTo(0, list.getHeight() - selectBox.getSelectedIndex() * itemHeight - itemHeight / 2, 0, 0, true, true);
			updateVisualScroll();

			previousScrollFocus = null;
			Actor actor = stage.getScrollFocus();
			if (actor != null && !actor.isDescendantOf(this)) previousScrollFocus = actor;
			stage.setScrollFocus(this);

			list.selection.set(selectBox.getSelected());
			list.setTouchable(Touchable.enabled);
			clearActions();
			selectBox.onShow(this, below);
		}

		public void hide () {
			if (!list.isTouchable() || !hasParent()) return;
			list.setTouchable(Touchable.disabled);

			Stage stage = getStage();
			if (stage != null) {
				stage.removeCaptureListener(hideListener);
				stage.removeListener(list.getKeyListener());
				if (previousScrollFocus != null && previousScrollFocus.getStage() == null) previousScrollFocus = null;
				Actor actor = stage.getScrollFocus();
				if (actor == null || isAscendantOf(actor)) stage.setScrollFocus(previousScrollFocus);
			}

			clearActions();
			selectBox.onHide(this);
		}

		public override void draw (IBatch batch, float parentAlpha) {
			selectBox.localToStageCoordinates(temp.set(0, 0));
			if (!temp.Equals(stagePosition)) hide();
			base.draw(batch, parentAlpha);
		}

		public override void act (float delta) {
			base.act(delta);
			toFront();
		}

		internal protected override void setStage (Stage stage) {
			Stage oldStage = getStage();
			if (oldStage != null) {
				oldStage.removeCaptureListener(hideListener);
				oldStage.removeListener(list.getKeyListener());
			}
			base.setStage(stage);
		}

		public List<T> getList () {
			return list;
		}

		public SelectBox<T> getSelectBox () {
			return selectBox;
		}
	}

	/** The style for a select box, see {@link SelectBox}.
	 * @author mzechner
	 * @author Nathan Sweet */
	 public class SelectBoxStyle {
		public BitmapFont font;
		public Color fontColor = new Color(1, 1, 1, 1);
		public Color? overFontColor, disabledFontColor;
		public IDrawable? background;
		public ScrollPane.ScrollPaneStyle scrollStyle;
		public List<T>.ListStyle listStyle;
		public IDrawable? backgroundOver, backgroundOpen, backgroundDisabled;

		public SelectBoxStyle () {
		}

		public SelectBoxStyle (BitmapFont font, Color fontColor, IDrawable? background, ScrollPane.ScrollPaneStyle scrollStyle,
			List<T>.ListStyle listStyle) {
			this.font = font;
			this.fontColor.set(fontColor);
			this.background = background;
			this.scrollStyle = scrollStyle;
			this.listStyle = listStyle;
		}

		public SelectBoxStyle (SelectBoxStyle style) {
			font = style.font;
			fontColor.set(style.fontColor);

			if (style.overFontColor != null) overFontColor = new Color(style.overFontColor);
			if (style.disabledFontColor != null) disabledFontColor = new Color(style.disabledFontColor);

			background = style.background;
			scrollStyle = new ScrollPane.ScrollPaneStyle(style.scrollStyle);
			listStyle = new List<T>.ListStyle(style.listStyle);

			backgroundOver = style.backgroundOver;
			backgroundOpen = style.backgroundOpen;
			backgroundDisabled = style.backgroundDisabled;
		}
	}
}
