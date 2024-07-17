using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.UI;
using SharpGDX;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A group that scrolls a child actor using scrollbars and/or mouse or touch dragging.
 * <p>
 * The actor is sized to its preferred size. If the actor's preferred width or height is less than the size of this scroll pane,
 * it is set to the size of this scroll pane. Scrollbars appear when the actor is larger than the scroll pane.
 * <p>
 * The scroll pane's preferred size is that of the child actor. At this size, the child actor will not need to scroll, so the
 * scroll pane is typically sized by ignoring the preferred size in one or both directions.
 * @author mzechner
 * @author Nathan Sweet */
public class ScrollPane : WidgetGroup {
	private ScrollPaneStyle style;
	private Actor actor;

	readonly Rectangle actorArea = new Rectangle();
	readonly Rectangle hScrollBounds = new Rectangle(), hKnobBounds = new Rectangle();
	readonly Rectangle vScrollBounds = new Rectangle(), vKnobBounds = new Rectangle();
	private readonly Rectangle actorCullingArea = new Rectangle();
	private ActorGestureListener flickScrollListener;

	bool _scrollX, _scrollY;
	bool vScrollOnRight = true, hScrollOnBottom = true;
	float amountX, amountY;
	float visualAmountX, visualAmountY;
	float maxX, maxY;
	bool touchScrollH, touchScrollV;
	readonly Vector2 lastPoint = new Vector2();
	bool fadeScrollBars = true, smoothScrolling = true, scrollBarTouch = true;
	float fadeAlpha, fadeAlphaSeconds = 1, fadeDelay, fadeDelaySeconds = 1;
	bool _cancelTouchFocus = true;

	bool flickScroll = true;
	float flingTime = 1f, flingTimer, velocityX, velocityY;
	private bool overscrollX = true, overscrollY = true;
	private float overscrollDistance = 50, overscrollSpeedMin = 30, overscrollSpeedMax = 200;
	private bool forceScrollX, forceScrollY;
	internal bool disableX, disableY;
	private bool _clamp = true;
	private bool scrollbarsOnTop;
	private bool variableSizeKnobs = true;
	int draggingPointer = -1;

	/** @param actor May be null. */
	public ScrollPane (Actor? actor) 
	: this(actor, new ScrollPaneStyle())
	{
		
	}

	/** @param actor May be null. */
	public ScrollPane (Actor? actor, Skin skin) 
	: this(actor, skin.get<ScrollPaneStyle>(typeof(ScrollPaneStyle))){
		
	}

	/** @param actor May be null. */
	public ScrollPane (Actor? actor, Skin skin, String styleName) 
	: this(actor, skin.get<ScrollPaneStyle>(styleName, typeof(ScrollPaneStyle)))
	{
		
	}

	/** @param actor May be null. */
	public ScrollPane (Actor? actor, ScrollPaneStyle style) {
		if (style == null) throw new IllegalArgumentException("style cannot be null.");
		this.style = style;
		setActor(actor);
		setSize(150, 150);

		addCaptureListener();
		flickScrollListener = getFlickScrollListener();
		addListener(flickScrollListener);
		addScrollListener();
	}

	protected void addCaptureListener () {
		addCaptureListener(new ScrollPaneInputListener(this) );
	}

	private class ScrollPaneInputListener : InputListener
	{
		private readonly ScrollPane _scrollPane;

		public ScrollPaneInputListener(ScrollPane scrollPane)
		{
			_scrollPane = scrollPane;
		}

		private float handlePosition;

		public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (_scrollPane.draggingPointer != -1) return false;
			if (pointer == 0 && button != 0) return false;
			if (_scrollPane.getStage() != null) _scrollPane.getStage().setScrollFocus(_scrollPane);

			if (!_scrollPane.flickScroll) _scrollPane.setScrollbarsVisible(true);

			if (_scrollPane.fadeAlpha == 0) return false;

			if (_scrollPane.scrollBarTouch && _scrollPane._scrollX && _scrollPane.hScrollBounds.contains(x, y))
			{
				@event.stop();
				_scrollPane.setScrollbarsVisible(true);
				if (_scrollPane.hKnobBounds.contains(x, y))
				{
					_scrollPane.lastPoint.set(x, y);
					handlePosition = _scrollPane.hKnobBounds.x;
					_scrollPane.touchScrollH = true;
					_scrollPane.draggingPointer = pointer;
					return true;
				}
				_scrollPane.setScrollX(_scrollPane.amountX + _scrollPane.actorArea.width * (x < _scrollPane.hKnobBounds.x ? -1 : 1));
				return true;
			}
			if (_scrollPane.scrollBarTouch && _scrollPane._scrollY && _scrollPane.vScrollBounds.contains(x, y))
			{
				@event.stop();
				_scrollPane.setScrollbarsVisible(true);
				if (_scrollPane.vKnobBounds.contains(x, y))
				{
					_scrollPane.lastPoint.set(x, y);
					handlePosition = _scrollPane.vKnobBounds.y;
					_scrollPane.touchScrollV = true;
					_scrollPane.draggingPointer = pointer;
					return true;
				}
				_scrollPane.setScrollY(_scrollPane.amountY + _scrollPane.actorArea.height * (y < _scrollPane.vKnobBounds.y ? 1 : -1));
				return true;
			}
			return false;
		}

		public override void touchUp(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (pointer != _scrollPane.draggingPointer) return;
			_scrollPane.cancel();
		}

		public override void touchDragged(InputEvent @event, float x, float y, int pointer)
		{
			if (pointer != _scrollPane.draggingPointer) return;
			if (_scrollPane.touchScrollH)
			{
				float delta = x - _scrollPane.lastPoint.x;
				float scrollH = handlePosition + delta;
				handlePosition = scrollH;
				scrollH = Math.Max(_scrollPane.hScrollBounds.x, scrollH);
				scrollH = Math.Min(_scrollPane.hScrollBounds.x + _scrollPane.hScrollBounds.width - _scrollPane.hKnobBounds.width, scrollH);
				float total = _scrollPane.hScrollBounds.width - _scrollPane.hKnobBounds.width;
				if (total != 0) _scrollPane.setScrollPercentX((scrollH - _scrollPane.hScrollBounds.x) / total);
				_scrollPane.lastPoint.set(x, y);
			}
			else if (_scrollPane.touchScrollV)
			{
				float delta = y - _scrollPane.lastPoint.y;
				float scrollV = handlePosition + delta;
				handlePosition = scrollV;
				scrollV = Math.Max(_scrollPane.vScrollBounds.y, scrollV);
				scrollV = Math.Min(_scrollPane.vScrollBounds.y + _scrollPane.vScrollBounds.height - _scrollPane.vKnobBounds.height, scrollV);
				float total = _scrollPane.vScrollBounds.height - _scrollPane.vKnobBounds.height;
				if (total != 0) _scrollPane.setScrollPercentY(1 - (scrollV - _scrollPane.vScrollBounds.y) / total);
				_scrollPane.lastPoint.set(x, y);
			}
		}

		public override bool mouseMoved(InputEvent @event, float x, float y)
		{
			if (!_scrollPane.flickScroll) _scrollPane.setScrollbarsVisible(true);
			return false;
		}
	}

	/** Called by constructor. */
	protected ActorGestureListener getFlickScrollListener () {
		return new FlickScrollListener(this) ;
	}

	private class FlickScrollListener : ActorGestureListener
	{
		private readonly ScrollPane _scrollPane;

		public FlickScrollListener(ScrollPane scrollPane)
		{
			_scrollPane = scrollPane;
		}

		public override void pan(InputEvent @event, float x, float y, float deltaX, float deltaY)
		{
			_scrollPane.setScrollbarsVisible(true);
			if (!_scrollPane._scrollX) deltaX = 0;
			if (!_scrollPane._scrollY) deltaY = 0;
			_scrollPane.amountX -= deltaX;
			_scrollPane.amountY += deltaY;
			_scrollPane.clamp();
			if (_scrollPane._cancelTouchFocus && (deltaX != 0 || deltaY != 0)) _scrollPane.cancelTouchFocus();
		}

		public override void fling(InputEvent @event, float x, float y, int button)
		{
			float velocityX = Math.Abs(x) > 150 && _scrollPane._scrollX ? x : 0;
			float velocityY = Math.Abs(y) > 150 && _scrollPane._scrollY ? -y : 0;
			if (velocityX != 0 || velocityY != 0)
			{
				if (_scrollPane._cancelTouchFocus) _scrollPane.cancelTouchFocus();
				_scrollPane.fling(_scrollPane.flingTime, velocityX, velocityY);
			}
		}

		public override bool handle(Event @event)
		{
			if (base.handle(@event))
			{
				if (((InputEvent)@event).getType() == InputEvent.Type.touchDown) _scrollPane.flingTimer = 0;
				return true;
			}
			else if (@event is InputEvent && ((InputEvent)@event).isTouchFocusCancel()) //
				_scrollPane.cancel();
			return false;
		}
	}

	protected void addScrollListener () {
		addListener(new ScrollListener(this) );
	}

	private class ScrollListener : InputListener
	{
		private readonly ScrollPane _scrollPane;

		public ScrollListener(ScrollPane scrollPane)
		{
			_scrollPane = scrollPane;
		}

		public override bool scrolled(InputEvent @event, float x, float y, float scrollAmountX, float scrollAmountY)
		{
			_scrollPane.setScrollbarsVisible(true);
			if (_scrollPane._scrollY || _scrollPane._scrollX)
			{
				if (_scrollPane._scrollY)
				{
					if (!_scrollPane._scrollX && scrollAmountY == 0) scrollAmountY = scrollAmountX;
				}
				else
				{
					if (_scrollPane._scrollX && scrollAmountX == 0) scrollAmountX = scrollAmountY;
				}
				_scrollPane.setScrollY(_scrollPane.amountY + _scrollPane.getMouseWheelY() * scrollAmountY);
				_scrollPane.setScrollX(_scrollPane.amountX + _scrollPane.getMouseWheelX() * scrollAmountX);
			}
			else
				return false;
			return true;
		}
	}

	/** Shows or hides the scrollbars for when using {@link #setFadeScrollBars(boolean)}. */
	public void setScrollbarsVisible (bool visible) {
		if (visible) {
			fadeAlpha = fadeAlphaSeconds;
			fadeDelay = fadeDelaySeconds;
		} else {
			fadeAlpha = 0;
			fadeDelay = 0;
		}
	}

	/** Cancels the stage's touch focus for all listeners except this scroll pane's flick scroll listener. This causes any actors
	 * inside the scrollpane that have received touchDown to receive touchUp.
	 * @see #setCancelTouchFocus(boolean) */
	public void cancelTouchFocus () {
		Stage stage = getStage();
		if (stage != null) stage.cancelTouchFocusExcept(flickScrollListener, this);
	}

	/** If currently scrolling by tracking a touch down, stop scrolling. */
	public void cancel () {
		draggingPointer = -1;
		touchScrollH = false;
		touchScrollV = false;
		flickScrollListener.getGestureDetector().cancel();
	}

	void clamp () {
		if (!_clamp) return;
		scrollX(overscrollX ? MathUtils.clamp(amountX, -overscrollDistance, maxX + overscrollDistance)
			: MathUtils.clamp(amountX, 0, maxX));
		scrollY(overscrollY ? MathUtils.clamp(amountY, -overscrollDistance, maxY + overscrollDistance)
			: MathUtils.clamp(amountY, 0, maxY));
	}

	public void setStyle (ScrollPaneStyle style) {
		if (style == null) throw new IllegalArgumentException("style cannot be null.");
		this.style = style;
		invalidateHierarchy();
	}

	/** Returns the scroll pane's style. Modifying the returned style may not have an effect until
	 * {@link #setStyle(ScrollPaneStyle)} is called. */
	public ScrollPaneStyle getStyle () {
		return style;
	}

	public override void act (float delta) {
		base.act(delta);

		bool panning = flickScrollListener.getGestureDetector().isPanning();
		bool animating = false;

		if (fadeAlpha > 0 && fadeScrollBars && !panning && !touchScrollH && !touchScrollV) {
			fadeDelay -= delta;
			if (fadeDelay <= 0) fadeAlpha = Math.Max(0, fadeAlpha - delta);
			animating = true;
		}

		if (flingTimer > 0) {
			setScrollbarsVisible(true);

			float alpha = flingTimer / flingTime;
			amountX -= velocityX * alpha * delta;
			amountY -= velocityY * alpha * delta;
			clamp();

			// Stop fling if hit overscroll distance.
			if (amountX == -overscrollDistance) velocityX = 0;
			if (amountX >= maxX + overscrollDistance) velocityX = 0;
			if (amountY == -overscrollDistance) velocityY = 0;
			if (amountY >= maxY + overscrollDistance) velocityY = 0;

			flingTimer -= delta;
			if (flingTimer <= 0) {
				velocityX = 0;
				velocityY = 0;
			}

			animating = true;
		}

		if (smoothScrolling && flingTimer <= 0 && !panning && //
		// Scroll smoothly when grabbing the scrollbar if one pixel of scrollbar movement is > 10% of the scroll area.
			((!touchScrollH || (_scrollX && maxX / (hScrollBounds.width - hKnobBounds.width) > actorArea.width * 0.1f)) && //
				(!touchScrollV || (_scrollY && maxY / (vScrollBounds.height - vKnobBounds.height) > actorArea.height * 0.1f))) //
		) {
			if (visualAmountX != amountX) {
				if (visualAmountX < amountX)
					visualScrollX(Math.Min(amountX, visualAmountX + Math.Max(200 * delta, (amountX - visualAmountX) * 7 * delta)));
				else
					visualScrollX(Math.Max(amountX, visualAmountX - Math.Max(200 * delta, (visualAmountX - amountX) * 7 * delta)));
				animating = true;
			}
			if (visualAmountY != amountY) {
				if (visualAmountY < amountY)
					visualScrollY(Math.Min(amountY, visualAmountY + Math.Max(200 * delta, (amountY - visualAmountY) * 7 * delta)));
				else
					visualScrollY(Math.Max(amountY, visualAmountY - Math.Max(200 * delta, (visualAmountY - amountY) * 7 * delta)));
				animating = true;
			}
		} else {
			if (visualAmountX != amountX) visualScrollX(amountX);
			if (visualAmountY != amountY) visualScrollY(amountY);
		}

		if (!panning) {
			if (overscrollX && _scrollX) {
				if (amountX < 0) {
					setScrollbarsVisible(true);
					amountX += (overscrollSpeedMin + (overscrollSpeedMax - overscrollSpeedMin) * -amountX / overscrollDistance)
						* delta;
					if (amountX > 0) scrollX(0);
					animating = true;
				} else if (amountX > maxX) {
					setScrollbarsVisible(true);
					amountX -= (overscrollSpeedMin
						+ (overscrollSpeedMax - overscrollSpeedMin) * -(maxX - amountX) / overscrollDistance) * delta;
					if (amountX < maxX) scrollX(maxX);
					animating = true;
				}
			}
			if (overscrollY && _scrollY) {
				if (amountY < 0) {
					setScrollbarsVisible(true);
					amountY += (overscrollSpeedMin + (overscrollSpeedMax - overscrollSpeedMin) * -amountY / overscrollDistance)
						* delta;
					if (amountY > 0) scrollY(0);
					animating = true;
				} else if (amountY > maxY) {
					setScrollbarsVisible(true);
					amountY -= (overscrollSpeedMin
						+ (overscrollSpeedMax - overscrollSpeedMin) * -(maxY - amountY) / overscrollDistance) * delta;
					if (amountY < maxY) scrollY(maxY);
					animating = true;
				}
			}
		}

		if (animating) {
			Stage stage = getStage();
			if (stage != null && stage.getActionsRequestRendering()) Gdx.graphics.requestRendering();
		}
	}

	public override void layout () {
		IDrawable bg = style.background, hScrollKnob = style.hScrollKnob, vScrollKnob = style.vScrollKnob;
		float bgLeftWidth = 0, bgRightWidth = 0, bgTopHeight = 0, bgBottomHeight = 0;
		if (bg != null) {
			bgLeftWidth = bg.getLeftWidth();
			bgRightWidth = bg.getRightWidth();
			bgTopHeight = bg.getTopHeight();
			bgBottomHeight = bg.getBottomHeight();
		}
		float width = getWidth(), height = getHeight();
		actorArea.set(bgLeftWidth, bgBottomHeight, width - bgLeftWidth - bgRightWidth, height - bgTopHeight - bgBottomHeight);

		if (actor == null) return;

		float scrollbarHeight = 0, scrollbarWidth = 0;
		if (hScrollKnob != null) scrollbarHeight = hScrollKnob.getMinHeight();
		if (style.hScroll != null) scrollbarHeight = Math.Max(scrollbarHeight, style.hScroll.getMinHeight());
		if (vScrollKnob != null) scrollbarWidth = vScrollKnob.getMinWidth();
		if (style.vScroll != null) scrollbarWidth = Math.Max(scrollbarWidth, style.vScroll.getMinWidth());

		// Get actor's desired width.
		float actorWidth, actorHeight;
		if (actor is ILayout) {
			ILayout layout = (ILayout)actor;
			actorWidth = layout.getPrefWidth();
			actorHeight = layout.getPrefHeight();
		} else {
			actorWidth = actor.getWidth();
			actorHeight = actor.getHeight();
		}

	// Determine if horizontal/vertical scrollbars are needed.
	_scrollX = forceScrollX || (actorWidth > actorArea.width && !disableX);
	_scrollY = forceScrollY || (actorHeight > actorArea.height && !disableY);

		// Adjust actor area for scrollbar sizes and check if it causes the other scrollbar to show.
		if (!scrollbarsOnTop) {
			if (_scrollY) {
				actorArea.width -= scrollbarWidth;
				if (!vScrollOnRight) actorArea.x += scrollbarWidth;
				// Horizontal scrollbar may cause vertical scrollbar to show.
				if (!_scrollX && actorWidth > actorArea.width && !disableX) _scrollX = true;
			}
			if (_scrollX) {
				actorArea.height -= scrollbarHeight;
				if (hScrollOnBottom) actorArea.y += scrollbarHeight;
				// Vertical scrollbar may cause horizontal scrollbar to show.
				if (!_scrollY && actorHeight > actorArea.height && !disableY) {
					_scrollY = true;
					actorArea.width -= scrollbarWidth;
					if (!vScrollOnRight) actorArea.x += scrollbarWidth;
				}
			}
		}

		// If the actor is smaller than the available space, make it take up the available space.
		actorWidth = disableX ? actorArea.width : Math.Max(actorArea.width, actorWidth);
		actorHeight = disableY ? actorArea.height : Math.Max(actorArea.height, actorHeight);

		maxX = actorWidth - actorArea.width;
		maxY = actorHeight - actorArea.height;
		scrollX(MathUtils.clamp(amountX, 0, maxX));
		scrollY(MathUtils.clamp(amountY, 0, maxY));

		// Set the scrollbar and knob bounds.
		if (_scrollX) {
			if (hScrollKnob != null) {
				float x = scrollbarsOnTop ? bgLeftWidth : actorArea.x;
				float y = hScrollOnBottom ? bgBottomHeight : height - bgTopHeight - scrollbarHeight;
				hScrollBounds.set(x, y, actorArea.width, scrollbarHeight);
				if (_scrollY && scrollbarsOnTop) {
					hScrollBounds.width -= scrollbarWidth;
					if (!vScrollOnRight) hScrollBounds.x += scrollbarWidth;
				}

				if (variableSizeKnobs)
					hKnobBounds.width = Math.Max(hScrollKnob.getMinWidth(), (int)(hScrollBounds.width * actorArea.width / actorWidth));
				else
					hKnobBounds.width = hScrollKnob.getMinWidth();
				if (hKnobBounds.width > actorWidth) hKnobBounds.width = 0;
				hKnobBounds.height = hScrollKnob.getMinHeight();
				hKnobBounds.x = hScrollBounds.x + (int)((hScrollBounds.width - hKnobBounds.width) * getScrollPercentX());
				hKnobBounds.y = hScrollBounds.y;
			} else {
				hScrollBounds.set(0, 0, 0, 0);
				hKnobBounds.set(0, 0, 0, 0);
			}
		}
		if (_scrollY) {
			if (vScrollKnob != null) {
				float x = vScrollOnRight ? width - bgRightWidth - scrollbarWidth : bgLeftWidth;
				float y = scrollbarsOnTop ? bgBottomHeight : actorArea.y;
				vScrollBounds.set(x, y, scrollbarWidth, actorArea.height);
				if (_scrollX && scrollbarsOnTop) {
					vScrollBounds.height -= scrollbarHeight;
					if (hScrollOnBottom) vScrollBounds.y += scrollbarHeight;
				}

				vKnobBounds.width = vScrollKnob.getMinWidth();
				if (variableSizeKnobs)
					vKnobBounds.height = Math.Max(vScrollKnob.getMinHeight(),
						(int)(vScrollBounds.height * actorArea.height / actorHeight));
				else
					vKnobBounds.height = vScrollKnob.getMinHeight();
				if (vKnobBounds.height > actorHeight) vKnobBounds.height = 0;
				vKnobBounds.x = vScrollOnRight ? width - bgRightWidth - vScrollKnob.getMinWidth() : bgLeftWidth;
				vKnobBounds.y = vScrollBounds.y + (int)((vScrollBounds.height - vKnobBounds.height) * (1 - getScrollPercentY()));
			} else {
				vScrollBounds.set(0, 0, 0, 0);
				vKnobBounds.set(0, 0, 0, 0);
			}
		}

		updateActorPosition();
		if (actor is ILayout) {
			actor.setSize(actorWidth, actorHeight);
			((ILayout)actor).validate();
		}
	}

	private void updateActorPosition () {
		// Calculate the actor's position depending on the scroll state and available actor area.
		float x = actorArea.x - (_scrollX ? (int)visualAmountX : 0);
		float y = actorArea.y - (int)(_scrollY ? maxY - visualAmountY : maxY);
		actor.setPosition(x, y);

		if (actor is ICullable) {
			actorCullingArea.x = actorArea.x - x;
			actorCullingArea.y = actorArea.y - y;
			actorCullingArea.width = actorArea.width;
			actorCullingArea.height = actorArea.height;
			((ICullable)actor).setCullingArea(actorCullingArea);
		}
	}

	public override void draw (IBatch batch, float parentAlpha) {
		if (actor == null) return;

		validate();

		// Setup transform for this group.
		applyTransform(batch, computeTransform());

		if (_scrollX) hKnobBounds.x = hScrollBounds.x + (int)((hScrollBounds.width - hKnobBounds.width) * getVisualScrollPercentX());
		if (_scrollY)
			vKnobBounds.y = vScrollBounds.y + (int)((vScrollBounds.height - vKnobBounds.height) * (1 - getVisualScrollPercentY()));

		updateActorPosition();

		// Draw the background ninepatch.
		Color color = getColor();
		float alpha = color.a * parentAlpha;
		if (style.background != null) {
			batch.setColor(color.r, color.g, color.b, alpha);
			style.background.draw(batch, 0, 0, getWidth(), getHeight());
		}

		batch.flush();
		if (clipBegin(actorArea.x, actorArea.y, actorArea.width, actorArea.height)) {
			drawChildren(batch, parentAlpha);
			batch.flush();
			clipEnd();
		}

		// Render scrollbars and knobs on top if they will be visible.
		batch.setColor(color.r, color.g, color.b, alpha);
		if (fadeScrollBars) alpha *= Interpolation.fade.apply(fadeAlpha / fadeAlphaSeconds);
		drawScrollBars(batch, color.r, color.g, color.b, alpha);

		resetTransform(batch);
	}

	/** Renders the scrollbars after the children have been drawn. If the scrollbars faded out, a is zero and rendering can be
	 * skipped. */
	protected void drawScrollBars (IBatch batch, float r, float g, float b, float a) {
		if (a <= 0) return;
		batch.setColor(r, g, b, a);

		bool x = _scrollX && hKnobBounds.width > 0;
		bool y = _scrollY && vKnobBounds.height > 0;
		if (x) {
			if (y && style.corner != null) style.corner.draw(batch, hScrollBounds.x + hScrollBounds.width, hScrollBounds.y,
				vScrollBounds.width, vScrollBounds.y);

			if (style.hScroll != null)
				style.hScroll.draw(batch, hScrollBounds.x, hScrollBounds.y, hScrollBounds.width, hScrollBounds.height);
			if (style.hScrollKnob != null)
				style.hScrollKnob.draw(batch, hKnobBounds.x, hKnobBounds.y, hKnobBounds.width, hKnobBounds.height);
		}
		if (y) {
			if (style.vScroll != null)
				style.vScroll.draw(batch, vScrollBounds.x, vScrollBounds.y, vScrollBounds.width, vScrollBounds.height);
			if (style.vScrollKnob != null)
				style.vScrollKnob.draw(batch, vKnobBounds.x, vKnobBounds.y, vKnobBounds.width, vKnobBounds.height);
		}
	}

	/** Generate fling gesture.
	 * @param flingTime Time in seconds for which you want to fling last.
	 * @param velocityX Velocity for horizontal direction.
	 * @param velocityY Velocity for vertical direction. */
	public void fling (float flingTime, float velocityX, float velocityY) {
		this.flingTimer = flingTime;
		this.velocityX = velocityX;
		this.velocityY = velocityY;
	}

	public override float getPrefWidth () {
		float width = 0;
		if (actor is ILayout)
			width = ((ILayout)actor).getPrefWidth();
		else if (actor != null) //
			width = actor.getWidth();

		IDrawable background = style.background;
		if (background != null)
			width = Math.Max(width + background.getLeftWidth() + background.getRightWidth(), background.getMinWidth());

		if (_scrollY) {
			float scrollbarWidth = 0;
			if (style.vScrollKnob != null) scrollbarWidth = style.vScrollKnob.getMinWidth();
			if (style.vScroll != null) scrollbarWidth = Math.Max(scrollbarWidth, style.vScroll.getMinWidth());
			width += scrollbarWidth;
		}
		return width;
	}

	public override float getPrefHeight () {
		float height = 0;
		if (actor is ILayout)
			height = ((ILayout)actor).getPrefHeight();
		else if (actor != null) //
			height = actor.getHeight();

		IDrawable background = style.background;
		if (background != null)
			height = Math.Max(height + background.getTopHeight() + background.getBottomHeight(), background.getMinHeight());

		if (_scrollX) {
			float scrollbarHeight = 0;
			if (style.hScrollKnob != null) scrollbarHeight = style.hScrollKnob.getMinHeight();
			if (style.hScroll != null) scrollbarHeight = Math.Max(scrollbarHeight, style.hScroll.getMinHeight());
			height += scrollbarHeight;
		}
		return height;
	}

	public override float getMinWidth () {
		return 0;
	}

	public override float getMinHeight () {
		return 0;
	}

	/** Sets the {@link Actor} embedded in this scroll pane.
	 * @param actor May be null to remove any current actor. */
	public void setActor (Actor? actor) {
		if (this.actor == this) throw new IllegalArgumentException("actor cannot be the ScrollPane.");
		if (this.actor != null) base.removeActor(this.actor);
		this.actor = actor;
		if (actor != null) base.addActor(actor);
	}

	/** Returns the actor embedded in this scroll pane, or null. */
	public Actor? getActor () {
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

	public override Actor? hit (float x, float y, bool touchable) {
		if (x < 0 || x >= getWidth() || y < 0 || y >= getHeight()) return null;
		if (touchable && getTouchable() == Touchable.enabled && isVisible()) {
			if (_scrollX && touchScrollH && hScrollBounds.contains(x, y)) return this;
			if (_scrollY && touchScrollV && vScrollBounds.contains(x, y)) return this;
		}
		return base.hit(x, y, touchable);
	}

	/** Called whenever the x scroll amount is changed. */
	protected void scrollX (float pixelsX) {
		this.amountX = pixelsX;
	}

	/** Called whenever the y scroll amount is changed. */
	protected void scrollY (float pixelsY) {
		this.amountY = pixelsY;
	}

	/** Called whenever the visual x scroll amount is changed. */
	protected void visualScrollX (float pixelsX) {
		this.visualAmountX = pixelsX;
	}

	/** Called whenever the visual y scroll amount is changed. */
	protected void visualScrollY (float pixelsY) {
		this.visualAmountY = pixelsY;
	}

	/** Returns the amount to scroll horizontally when the mouse wheel is scrolled. */
	protected float getMouseWheelX () {
		return Math.Min(actorArea.width, Math.Max(actorArea.width * 0.9f, maxX * 0.1f) / 4);
	}

	/** Returns the amount to scroll vertically when the mouse wheel is scrolled. */
	protected float getMouseWheelY () {
		return Math.Min(actorArea.height, Math.Max(actorArea.height * 0.9f, maxY * 0.1f) / 4);
	}

	public void setScrollX (float pixels) {
		scrollX(MathUtils.clamp(pixels, 0, maxX));
	}

	/** Returns the x scroll position in pixels, where 0 is the left of the scroll pane. */
	public float getScrollX () {
		return amountX;
	}

	public void setScrollY (float pixels) {
		scrollY(MathUtils.clamp(pixels, 0, maxY));
	}

	/** Returns the y scroll position in pixels, where 0 is the top of the scroll pane. */
	public float getScrollY () {
		return amountY;
	}

	/** Sets the visual scroll amount equal to the scroll amount. This can be used when setting the scroll amount without
	 * animating. */
	public void updateVisualScroll () {
		visualAmountX = amountX;
		visualAmountY = amountY;
	}

	public float getVisualScrollX () {
		return !_scrollX ? 0 : visualAmountX;
	}

	public float getVisualScrollY () {
		return !_scrollY ? 0 : visualAmountY;
	}

	public float getVisualScrollPercentX () {
		if (maxX == 0) return 0;
		return MathUtils.clamp(visualAmountX / maxX, 0, 1);
	}

	public float getVisualScrollPercentY () {
		if (maxY == 0) return 0;
		return MathUtils.clamp(visualAmountY / maxY, 0, 1);
	}

	public float getScrollPercentX () {
		if (maxX == 0) return 0;
		return MathUtils.clamp(amountX / maxX, 0, 1);
	}

	public void setScrollPercentX (float percentX) {
		scrollX(maxX * MathUtils.clamp(percentX, 0, 1));
	}

	public float getScrollPercentY () {
		if (maxY == 0) return 0;
		return MathUtils.clamp(amountY / maxY, 0, 1);
	}

	public void setScrollPercentY (float percentY) {
		scrollY(maxY * MathUtils.clamp(percentY, 0, 1));
	}

	public void setFlickScroll (bool flickScroll) {
		if (this.flickScroll == flickScroll) return;
		this.flickScroll = flickScroll;
		if (flickScroll)
			addListener(flickScrollListener);
		else
			removeListener(flickScrollListener);
		invalidate();
	}

	public void setFlickScrollTapSquareSize (float halfTapSquareSize) {
		flickScrollListener.getGestureDetector().setTapSquareSize(halfTapSquareSize);
	}

	/** Sets the scroll offset so the specified rectangle is fully in view, if possible. Coordinates are in the scroll pane actor's
	 * coordinate system. */
	public void scrollTo (float x, float y, float width, float height) {
		scrollTo(x, y, width, height, false, false);
	}

	/** Sets the scroll offset so the specified rectangle is fully in view, and optionally centered vertically and/or horizontally,
	 * if possible. Coordinates are in the scroll pane actor's coordinate system. */
	public void scrollTo (float x, float y, float width, float height, bool centerHorizontal, bool centerVertical) {
		validate();

		float amountX = this.amountX;
		if (centerHorizontal)
			amountX = x + (width - actorArea.width) / 2;
		else
			amountX = MathUtils.clamp(amountX, x + width - actorArea.width, x);
		scrollX(MathUtils.clamp(amountX, 0, maxX));

		float amountY = this.amountY;
		y = maxY - y;
		if (centerVertical)
			amountY = y + (actorArea.height + height) / 2;
		else
			amountY = MathUtils.clamp(amountY, y + height, y + actorArea.height);
		scrollY(MathUtils.clamp(amountY, 0, maxY));
	}

	/** Returns the maximum scroll value in the x direction. */
	public float getMaxX () {
		return maxX;
	}

	/** Returns the maximum scroll value in the y direction. */
	public float getMaxY () {
		return maxY;
	}

	public float getScrollBarHeight () {
		if (!_scrollX) return 0;
		float height = 0;
		if (style.hScrollKnob != null) height = style.hScrollKnob.getMinHeight();
		if (style.hScroll != null) height = Math.Max(height, style.hScroll.getMinHeight());
		return height;
	}

	public float getScrollBarWidth () {
		if (!_scrollY) return 0;
		float width = 0;
		if (style.vScrollKnob != null) width = style.vScrollKnob.getMinWidth();
		if (style.vScroll != null) width = Math.Max(width, style.vScroll.getMinWidth());
		return width;
	}

	/** Returns the width of the scrolled viewport. */
	public float getScrollWidth () {
		return actorArea.width;
	}

	/** Returns the height of the scrolled viewport. */
	public float getScrollHeight () {
		return actorArea.height;
	}

	/** Returns true if the actor is larger than the scroll pane horizontally. */
	public bool isScrollX () {
		return _scrollX;
	}

	/** Returns true if the actor is larger than the scroll pane vertically. */
	public bool isScrollY () {
		return _scrollY;
	}

	/** Disables scrolling in a direction. The actor will be sized to the FlickScrollPane in the disabled direction. */
	public void setScrollingDisabled (bool x, bool y) {
		if (x == disableX && y == disableY) return;
		disableX = x;
		disableY = y;
		invalidate();
	}

	public bool isScrollingDisabledX () {
		return disableX;
	}

	public bool isScrollingDisabledY () {
		return disableY;
	}

	public bool isLeftEdge () {
		return !_scrollX || amountX <= 0;
	}

	public bool isRightEdge () {
		return !_scrollX || amountX >= maxX;
	}

	public bool isTopEdge () {
		return !_scrollY || amountY <= 0;
	}

	public bool isBottomEdge () {
		return !_scrollY || amountY >= maxY;
	}

	public bool isDragging () {
		return draggingPointer != -1;
	}

	public bool isPanning () {
		return flickScrollListener.getGestureDetector().isPanning();
	}

	public bool isFlinging () {
		return flingTimer > 0;
	}

	public void setVelocityX (float velocityX) {
		this.velocityX = velocityX;
	}

	/** Gets the flick scroll x velocity. */
	public float getVelocityX () {
		return velocityX;
	}

	public void setVelocityY (float velocityY) {
		this.velocityY = velocityY;
	}

	/** Gets the flick scroll y velocity. */
	public float getVelocityY () {
		return velocityY;
	}

	/** For flick scroll, if true the actor can be scrolled slightly past its bounds and will animate back to its bounds when
	 * scrolling is stopped. Default is true. */
	public void setOverscroll (bool overscrollX, bool overscrollY) {
		this.overscrollX = overscrollX;
		this.overscrollY = overscrollY;
	}

	/** For flick scroll, sets the overscroll distance in pixels and the speed it returns to the actor's bounds in seconds. Default
	 * is 50, 30, 200. */
	public void setupOverscroll (float distance, float speedMin, float speedMax) {
		overscrollDistance = distance;
		overscrollSpeedMin = speedMin;
		overscrollSpeedMax = speedMax;
	}

	public float getOverscrollDistance () {
		return overscrollDistance;
	}

	/** Forces enabling scrollbars (for non-flick scroll) and overscrolling (for flick scroll) in a direction, even if the contents
	 * do not exceed the bounds in that direction. */
	public void setForceScroll (bool x, bool y) {
		forceScrollX = x;
		forceScrollY = y;
	}

	public bool isForceScrollX () {
		return forceScrollX;
	}

	public bool isForceScrollY () {
		return forceScrollY;
	}

	/** For flick scroll, sets the amount of time in seconds that a fling will continue to scroll. Default is 1. */
	public void setFlingTime (float flingTime) {
		this.flingTime = flingTime;
	}

	/** For flick scroll, prevents scrolling out of the actor's bounds. Default is true. */
	public void setClamp (bool clamp) {
		this._clamp = clamp;
	}

	/** Set the position of the vertical and horizontal scroll bars. */
	public void setScrollBarPositions (bool bottom, bool right) {
		hScrollOnBottom = bottom;
		vScrollOnRight = right;
	}

	/** When true the scrollbars don't reduce the scrollable size and fade out after some time of not being used. */
	public void setFadeScrollBars (bool fadeScrollBars) {
		if (this.fadeScrollBars == fadeScrollBars) return;
		this.fadeScrollBars = fadeScrollBars;
		if (!fadeScrollBars) fadeAlpha = fadeAlphaSeconds;
		invalidate();
	}

	public void setupFadeScrollBars (float fadeAlphaSeconds, float fadeDelaySeconds) {
		this.fadeAlphaSeconds = fadeAlphaSeconds;
		this.fadeDelaySeconds = fadeDelaySeconds;
	}

	public bool getFadeScrollBars () {
		return fadeScrollBars;
	}

	/** When false, the scroll bars don't respond to touch or mouse events. Default is true. */
	public void setScrollBarTouch (bool scrollBarTouch) {
		this.scrollBarTouch = scrollBarTouch;
	}

	public void setSmoothScrolling (bool smoothScrolling) {
		this.smoothScrolling = smoothScrolling;
	}

	/** When false (the default), the actor is clipped so it is not drawn under the scrollbars. When true, the actor is clipped to
	 * the entire scroll pane bounds and the scrollbars are drawn on top of the actor. If {@link #setFadeScrollBars(boolean)} is
	 * true, the scroll bars are always drawn on top. */
	public void setScrollbarsOnTop (bool scrollbarsOnTop) {
		this.scrollbarsOnTop = scrollbarsOnTop;
		invalidate();
	}

	public bool getVariableSizeKnobs () {
		return variableSizeKnobs;
	}

	/** If true, the scroll knobs are sized based on {@link #getMaxX()} or {@link #getMaxY()}. If false, the scroll knobs are sized
	 * based on {@link Drawable#getMinWidth()} or {@link Drawable#getMinHeight()}. Default is true. */
	public void setVariableSizeKnobs (bool variableSizeKnobs) {
		this.variableSizeKnobs = variableSizeKnobs;
	}

	/** When true (default) and flick scrolling begins, {@link #cancelTouchFocus()} is called. This causes any actors inside the
	 * scrollpane that have received touchDown to receive touchUp when flick scrolling begins. */
	public void setCancelTouchFocus (bool cancelTouchFocus) {
		this._cancelTouchFocus = cancelTouchFocus;
	}

	public override void drawDebug (ShapeRenderer shapes) {
		drawDebugBounds(shapes);
		applyTransform(shapes, computeTransform());
		if (clipBegin(actorArea.x, actorArea.y, actorArea.width, actorArea.height)) {
			drawDebugChildren(shapes);
			shapes.flush();
			clipEnd();
		}
		resetTransform(shapes);
	}

	/** The style for a scroll pane, see {@link ScrollPane}.
	 * @author mzechner
	 * @author Nathan Sweet */
	public class ScrollPaneStyle {
		public IDrawable? background, corner;
		public IDrawable? hScroll, hScrollKnob;
		public IDrawable? vScroll, vScrollKnob;

		public ScrollPaneStyle () {
		}

		public ScrollPaneStyle (IDrawable? background, IDrawable? hScroll, IDrawable? hScrollKnob,
			IDrawable? vScroll, IDrawable? vScrollKnob) {
			this.background = background;
			this.hScroll = hScroll;
			this.hScrollKnob = hScrollKnob;
			this.vScroll = vScroll;
			this.vScrollKnob = vScrollKnob;
		}

		public ScrollPaneStyle (ScrollPaneStyle style) {
			background = style.background;
			corner = style.corner;

			hScroll = style.hScroll;
			hScrollKnob = style.hScrollKnob;

			vScroll = style.vScroll;
			vScrollKnob = style.vScrollKnob;
		}
	}
}
