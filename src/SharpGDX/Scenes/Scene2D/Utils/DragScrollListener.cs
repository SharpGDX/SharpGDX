using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Scenes.Scene2D.UI;
using Timer = SharpGDX.Utils.Timer;
using Task = SharpGDX.Utils.Timer.Task;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Causes a scroll pane to scroll when a drag goes outside the bounds of the scroll pane. Attach the listener to the actor which
 * will cause scrolling when dragged, usually the scroll pane or the scroll pane's actor.
 * <p>
 * If {@link ScrollPane#setFlickScroll(boolean)} is true, the scroll pane must have
 * {@link ScrollPane#setCancelTouchFocus(boolean)} false. When a drag starts that should drag rather than flick scroll, cancel the
 * scroll pane's touch focus using <code>stage.cancelTouchFocus(scrollPane);</code>. In this case the drag scroll listener must
 * not be attached to the scroll pane, else it would also lose touch focus. Instead it can be attached to the scroll pane's actor.
 * <p>
 * If using drag and drop, {@link DragAndDrop#setCancelTouchFocus(boolean)} must be false.
 * @author Nathan Sweet */
public class DragScrollListener : DragListener {
	static readonly Vector2 tmpCoords = new Vector2();

	private ScrollPane _scroll;
	private Task scrollUp, scrollDown;
	Interpolation interpolation = Interpolation.exp5In;
	float minSpeed = 15, maxSpeed = 75, tickSecs = 0.05f;
	long startTime, rampTime = 1750;
	float padTop, padBottom;

	public DragScrollListener (ScrollPane scroll) {
		this._scroll = scroll;

		throw new NotImplementedException();
		//scrollUp = new Task() {
		//	public void run () {
		//		scroll(scroll.getScrollY() - getScrollPixels());
		//	}
		//};
		//scrollDown = new Task() {
		//	public void run () {
		//		scroll(scroll.getScrollY() + getScrollPixels());
		//	}
		//};
	}

	public void setup (float minSpeedPixels, float maxSpeedPixels, float tickSecs, float rampSecs) {
		this.minSpeed = minSpeedPixels;
		this.maxSpeed = maxSpeedPixels;
		this.tickSecs = tickSecs;
		rampTime = (long)(rampSecs * 1000);
	}

	float getScrollPixels () {
		return interpolation.apply(minSpeed, maxSpeed, Math.Min(1, (TimeUtils.currentTimeMillis() - startTime) / (float)rampTime));
	}

		public override void drag (InputEvent @event, float x, float y, int pointer) {
		@event.getListenerActor().localToActorCoordinates(_scroll, tmpCoords.set(x, y));
		if (isAbove(tmpCoords.y)) {
			scrollDown.cancel();
			if (!scrollUp.isScheduled()) {
				startTime = TimeUtils.currentTimeMillis();
				Timer.schedule(scrollUp, tickSecs, tickSecs);
			}
			return;
		} else if (isBelow(tmpCoords.y)) {
			scrollUp.cancel();
			if (!scrollDown.isScheduled()) {
				startTime = TimeUtils.currentTimeMillis();
				Timer.schedule(scrollDown, tickSecs, tickSecs);
			}
			return;
		}
		scrollUp.cancel();
		scrollDown.cancel();
	}

		public override void dragStop (InputEvent @event, float x, float y, int pointer) {
		scrollUp.cancel();
		scrollDown.cancel();
	}

	protected bool isAbove (float y) {
		return y >= _scroll.getHeight() - padTop;
	}

	protected bool isBelow (float y) {
		return y < padBottom;
	}

	protected void scroll (float y) {
		_scroll.setScrollY(y);
	}

	public void setPadding (float padTop, float padBottom) {
		this.padTop = padTop;
		this.padBottom = padBottom;
	}
}
}
