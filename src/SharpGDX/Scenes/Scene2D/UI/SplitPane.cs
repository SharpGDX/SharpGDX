using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A container that contains two widgets and is divided either horizontally or vertically. The user may resize the widgets. The
 * child widgets are always sized to fill their side of the SplitPane.
 * <p>
 * Minimum and maximum split amounts can be set to limit the motion of the resizing handle. The handle position is also prevented
 * from shrinking the children below their minimum sizes. If these limits over-constrain the handle, it will be locked and placed
 * at an averaged location, resulting in cropped children. The minimum child size can be ignored (allowing dynamic cropping) by
 * wrapping the child in a {@linkplain Container} with a minimum size of 0 and {@linkplain Container#fill() fill()} set, or by
 * overriding {@link #clampSplitAmount()}.
 * <p>
 * The preferred size of a SplitPane is that of the child widgets and the size of the {@link SplitPaneStyle#handle}. The widgets
 * are sized depending on the SplitPane size and the {@link #setSplitAmount(float) split position}.
 * @author mzechner
 * @author Nathan Sweet */
public class SplitPane : WidgetGroup {
	SplitPaneStyle style;
	private Actor? firstWidget, secondWidget;
	bool vertical;
	float splitAmount = 0.5f, minAmount, maxAmount = 1;

	private readonly Rectangle firstWidgetBounds = new Rectangle();
	private readonly Rectangle secondWidgetBounds = new Rectangle();
	readonly Rectangle handleBounds = new Rectangle();
	bool cursorOverHandle;
	private readonly Rectangle tempScissors = new Rectangle();

	Vector2 lastPoint = new Vector2();
	Vector2 handlePosition = new Vector2();

	/** @param firstWidget May be null.
	 * @param secondWidget May be null. */
	public SplitPane (Actor? firstWidget, Actor? secondWidget, bool vertical, Skin skin) 
	: this(firstWidget, secondWidget, vertical, skin, "default-" + (vertical ? "vertical" : "horizontal"))
	{
		
	}

	/** @param firstWidget May be null.
	 * @param secondWidget May be null. */
	public SplitPane (Actor? firstWidget, Actor? secondWidget, bool vertical, Skin skin, String styleName) 
	: this(firstWidget, secondWidget, vertical, skin.get<SplitPaneStyle>(styleName, typeof(SplitPaneStyle)))
	{
		
	}

	/** @param firstWidget May be null.
	 * @param secondWidget May be null. */
	public SplitPane (Actor? firstWidget, Actor? secondWidget, bool vertical, SplitPaneStyle style) {
		this.vertical = vertical;
		setStyle(style);
		setFirstWidget(firstWidget);
		setSecondWidget(secondWidget);
		setSize(getPrefWidth(), getPrefHeight());
		initialize();
	}

	private void initialize () {
		addListener(new SplitPaneInputListener(this) );
	}

	private class SplitPaneInputListener : InputListener
	{
		private readonly SplitPane _splitPane;

		public SplitPaneInputListener(SplitPane splitPane)
		{
			_splitPane = splitPane;
		}

			int draggingPointer = -1;

			public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
			{
				if (draggingPointer != -1) return false;
				if (pointer == 0 && button != 0) return false;
				if (_splitPane.handleBounds.contains(x, y))
				{
					draggingPointer = pointer;
					_splitPane.lastPoint.set(x, y);
					_splitPane.handlePosition.set(_splitPane.handleBounds.x, _splitPane.handleBounds.y);
					return true;
				}
				return false;
			}

			public override void touchUp(InputEvent @event, float x, float y, int pointer, int button)
			{
				if (pointer == draggingPointer) draggingPointer = -1;
			}

			public override void touchDragged(InputEvent @event, float x, float y, int pointer)
			{
				if (pointer != draggingPointer) return;

				IDrawable handle = _splitPane.style.handle;
				if (!_splitPane.vertical)
				{
					float delta = x - _splitPane.lastPoint.x;
					float availWidth = _splitPane.getWidth() - handle.getMinWidth();
					float dragX = _splitPane.handlePosition.x + delta;
					_splitPane.handlePosition.x = dragX;
					dragX = Math.Max(0, dragX);
					dragX = Math.Min(availWidth, dragX);
					_splitPane.splitAmount = dragX / availWidth;
					_splitPane.lastPoint.set(x, y);
				}
				else
				{
					float delta = y - _splitPane.lastPoint.y;
					float availHeight = _splitPane.getHeight() - handle.getMinHeight();
					float dragY = _splitPane.handlePosition.y + delta;
					_splitPane.handlePosition.y = dragY;
					dragY = Math.Max(0, dragY);
					dragY = Math.Min(availHeight, dragY);
					_splitPane.splitAmount = 1 - (dragY / availHeight);
					_splitPane.lastPoint.set(x, y);
				}
				_splitPane.invalidate();
			}

			public override bool mouseMoved(InputEvent @event, float x, float y)
			{
				_splitPane.cursorOverHandle = _splitPane.handleBounds.contains(x, y);
				return false;
			}
}

	public void setStyle (SplitPaneStyle style) {
		this.style = style;
		invalidateHierarchy();
	}

	/** Returns the split pane's style. Modifying the returned style may not have an effect until {@link #setStyle(SplitPaneStyle)}
	 * is called. */
	public SplitPaneStyle getStyle () {
		return style;
	}

	public override void layout () {
		clampSplitAmount();
		if (!vertical)
			calculateHorizBoundsAndPositions();
		else
			calculateVertBoundsAndPositions();

		Actor firstWidget = this.firstWidget;
		if (firstWidget != null) {
			Rectangle firstWidgetBounds = this.firstWidgetBounds;
			firstWidget.setBounds(firstWidgetBounds.x, firstWidgetBounds.y, firstWidgetBounds.width, firstWidgetBounds.height);
			if (firstWidget is ILayout) ((ILayout)firstWidget).validate();
		}
		Actor secondWidget = this.secondWidget;
		if (secondWidget != null) {
			Rectangle secondWidgetBounds = this.secondWidgetBounds;
			secondWidget.setBounds(secondWidgetBounds.x, secondWidgetBounds.y, secondWidgetBounds.width, secondWidgetBounds.height);
			if (secondWidget is ILayout) ((ILayout)secondWidget).validate();
		}
	}

	public override float getPrefWidth () {
		float first = firstWidget == null ? 0
			: (firstWidget is ILayout ? ((ILayout)firstWidget).getPrefWidth() : firstWidget.getWidth());
		float second = secondWidget == null ? 0
			: (secondWidget is ILayout ? ((ILayout)secondWidget).getPrefWidth() : secondWidget.getWidth());
		if (vertical) return Math.Max(first, second);
		return first + style.handle.getMinWidth() + second;
	}

	public override float getPrefHeight () {
		float first = firstWidget == null ? 0
			: (firstWidget is ILayout ? ((ILayout)firstWidget).getPrefHeight() : firstWidget.getHeight());
		float second = secondWidget == null ? 0
			: (secondWidget is ILayout ? ((ILayout)secondWidget).getPrefHeight() : secondWidget.getHeight());
		if (!vertical) return Math.Max(first, second);
		return first + style.handle.getMinHeight() + second;
	}

	public override float getMinWidth () {
		float first = firstWidget is ILayout ? ((ILayout)firstWidget).getMinWidth() : 0;
		float second = secondWidget is ILayout ? ((ILayout)secondWidget).getMinWidth() : 0;
		if (vertical) return Math.Max(first, second);
		return first + style.handle.getMinWidth() + second;
	}

	public override float getMinHeight () {
		float first = firstWidget is ILayout ? ((ILayout)firstWidget).getMinHeight() : 0;
		float second = secondWidget is ILayout ? ((ILayout)secondWidget).getMinHeight() : 0;
		if (!vertical) return Math.Max(first, second);
		return first + style.handle.getMinHeight() + second;
	}

	public void setVertical (bool vertical) {
		if (this.vertical == vertical) return;
		this.vertical = vertical;
		invalidateHierarchy();
	}

	public bool isVertical () {
		return vertical;
	}

	private void calculateHorizBoundsAndPositions () {
		IDrawable handle = style.handle;

		float height = getHeight();

		float availWidth = getWidth() - handle.getMinWidth();
		float leftAreaWidth = (int)(availWidth * splitAmount);
		float rightAreaWidth = availWidth - leftAreaWidth;
		float handleWidth = handle.getMinWidth();

		firstWidgetBounds.set(0, 0, leftAreaWidth, height);
		secondWidgetBounds.set(leftAreaWidth + handleWidth, 0, rightAreaWidth, height);
		handleBounds.set(leftAreaWidth, 0, handleWidth, height);
	}

	private void calculateVertBoundsAndPositions () {
		IDrawable handle = style.handle;

		float width = getWidth();
		float height = getHeight();

		float availHeight = height - handle.getMinHeight();
		float topAreaHeight = (int)(availHeight * splitAmount);
		float bottomAreaHeight = availHeight - topAreaHeight;
		float handleHeight = handle.getMinHeight();

		firstWidgetBounds.set(0, height - topAreaHeight, width, topAreaHeight);
		secondWidgetBounds.set(0, 0, width, bottomAreaHeight);
		handleBounds.set(0, bottomAreaHeight, width, handleHeight);
	}

	public override void draw (IBatch batch, float parentAlpha) {
		Stage stage = getStage();
		if (stage == null) return;

		validate();

		Color color = getColor();
		float alpha = color.a * parentAlpha;

		applyTransform(batch, computeTransform());
		if (firstWidget != null && firstWidget.isVisible()) {
			batch.flush();
			stage.calculateScissors(firstWidgetBounds, tempScissors);
			if (ScissorStack.pushScissors(tempScissors)) {
				firstWidget.draw(batch, alpha);
				batch.flush();
				ScissorStack.popScissors();
			}
		}
		if (secondWidget != null && secondWidget.isVisible()) {
			batch.flush();
			stage.calculateScissors(secondWidgetBounds, tempScissors);
			if (ScissorStack.pushScissors(tempScissors)) {
				secondWidget.draw(batch, alpha);
				batch.flush();
				ScissorStack.popScissors();
			}
		}
		batch.setColor(color.r, color.g, color.b, alpha);
		style.handle.draw(batch, handleBounds.x, handleBounds.y, handleBounds.width, handleBounds.height);
		resetTransform(batch);
	}

	/** @param splitAmount The split amount between the min and max amount. This parameter is clamped during layout. See
	 *           {@link #clampSplitAmount()}. */
	public void setSplitAmount (float splitAmount) {
		this.splitAmount = splitAmount; // will be clamped during layout
		invalidate();
	}

	public float getSplitAmount () {
		return splitAmount;
	}

	/** Called during layout to clamp the {@link #splitAmount} within the set limits. By default it imposes the limits of the
	 * {@linkplain #getMinSplitAmount() min amount}, {@linkplain #getMaxSplitAmount() max amount}, and min sizes of the children.
	 * This method is internally called in response to layout, so it should not call {@link #invalidate()}. */
	protected void clampSplitAmount () {
		float effectiveMinAmount = minAmount, effectiveMaxAmount = maxAmount;

		if (vertical) {
			float availableHeight = getHeight() - style.handle.getMinHeight();
			if (firstWidget is ILayout) effectiveMinAmount = Math.Max(effectiveMinAmount,
				Math.Min(((ILayout)firstWidget).getMinHeight() / availableHeight, 1));
			if (secondWidget is ILayout) effectiveMaxAmount = Math.Min(effectiveMaxAmount,
				1 - Math.Min(((ILayout)secondWidget).getMinHeight() / availableHeight, 1));
		} else {
			float availableWidth = getWidth() - style.handle.getMinWidth();
			if (firstWidget is ILayout)
				effectiveMinAmount = Math.Max(effectiveMinAmount, Math.Min(((ILayout)firstWidget).getMinWidth() / availableWidth, 1));
			if (secondWidget is ILayout) effectiveMaxAmount = Math.Min(effectiveMaxAmount,
				1 - Math.Min(((ILayout)secondWidget).getMinWidth() / availableWidth, 1));
		}

		if (effectiveMinAmount > effectiveMaxAmount) // Locked handle. Average the position.
			splitAmount = 0.5f * (effectiveMinAmount + effectiveMaxAmount);
		else
			splitAmount = Math.Max(Math.Min(splitAmount, effectiveMaxAmount), effectiveMinAmount);
	}

	public float getMinSplitAmount () {
		return minAmount;
	}

	public void setMinSplitAmount (float minAmount) {
		if (minAmount < 0 || minAmount > 1) throw new GdxRuntimeException("minAmount has to be >= 0 and <= 1");
		this.minAmount = minAmount;
	}

	public float getMaxSplitAmount () {
		return maxAmount;
	}

	public void setMaxSplitAmount (float maxAmount) {
		if (maxAmount < 0 || maxAmount > 1) throw new GdxRuntimeException("maxAmount has to be >= 0 and <= 1");
		this.maxAmount = maxAmount;
	}

	/** @param widget May be null. */
	public void setFirstWidget (Actor? widget) {
		if (firstWidget != null) base.removeActor(firstWidget);
		firstWidget = widget;
		if (widget != null) base.addActor(widget);
		invalidate();
	}

	/** @param widget May be null. */
	public void setSecondWidget (Actor? widget) {
		if (secondWidget != null) base.removeActor(secondWidget);
		secondWidget = widget;
		if (widget != null) base.addActor(widget);
		invalidate();
	}

	public override void addActor (Actor actor) {
		throw new UnsupportedOperationException("Use SplitPane#setWidget.");
	}

	public override void addActorAt (int index, Actor actor) {
		throw new UnsupportedOperationException("Use SplitPane#setWidget.");
	}

	public override void addActorBefore (Actor actorBefore, Actor actor) {
		throw new UnsupportedOperationException("Use SplitPane#setWidget.");
	}

	public override bool removeActor (Actor actor) {
		if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
		if (actor == firstWidget) {
			setFirstWidget(null);
			return true;
		}
		if (actor == secondWidget) {
			setSecondWidget(null);
			return true;
		}
		return true;
	}

	public override bool removeActor (Actor actor, bool unfocus) {
		if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
		if (actor == firstWidget) {
			base.removeActor(actor, unfocus);
			firstWidget = null;
			invalidate();
			return true;
		}
		if (actor == secondWidget) {
			base.removeActor(actor, unfocus);
			secondWidget = null;
			invalidate();
			return true;
		}
		return false;
	}

	public override Actor removeActorAt (int index, bool unfocus) {
		Actor actor = base.removeActorAt(index, unfocus);
		if (actor == firstWidget) {
			base.removeActor(actor, unfocus);
			firstWidget = null;
			invalidate();
		} else if (actor == secondWidget) {
			base.removeActor(actor, unfocus);
			secondWidget = null;
			invalidate();
		}
		return actor;
	}

	public bool isCursorOverHandle () {
		return cursorOverHandle;
	}

	/** The style for a splitpane, see {@link SplitPane}.
	 * @author mzechner
	 * @author Nathan Sweet */
	public class SplitPaneStyle {
		public IDrawable handle;

		public SplitPaneStyle () {
		}

		public SplitPaneStyle (IDrawable handle) {
			this.handle = handle;
		}

		public SplitPaneStyle (SplitPaneStyle style) {
			handle = style.handle;
		}
	}
}
