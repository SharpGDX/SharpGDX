using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Detects mouse or finger touch drags on an actor. A touch must go down over the actor and a drag won't start until it is moved
 * outside the {@link #setTapSquareSize(float) tap square}. Any touch (not just the first) will trigger this listener. While
 * pressed, other touch downs are ignored.
 * @author Nathan Sweet */
	public class DragListener : InputListener
	{
		private float tapSquareSize = 14, touchDownX = -1, touchDownY = -1, stageTouchDownX = -1, stageTouchDownY = -1;
		private float dragStartX, dragStartY, dragLastX, dragLastY, dragX, dragY;
		private int pressedPointer = -1;
		private int _button;
		private bool dragging;

		public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (pressedPointer != -1) return false;
			if (pointer == 0 && this._button != -1 && button != this._button) return false;
			pressedPointer = pointer;
			touchDownX = x;
			touchDownY = y;
			stageTouchDownX = @event.getStageX();
			stageTouchDownY = @event.getStageY();
			return true;
		}

		public override void touchDragged(InputEvent @event, float x, float y, int pointer)
		{
			if (pointer != pressedPointer) return;
			if (!dragging && (Math.Abs(touchDownX - x) > tapSquareSize || Math.Abs(touchDownY - y) > tapSquareSize))
			{
				dragging = true;
				dragStartX = x;
				dragStartY = y;
				dragStart(@event, x, y, pointer);
				dragX = x;
				dragY = y;
			}

			if (dragging)
			{
				dragLastX = dragX;
				dragLastY = dragY;
				dragX = x;
				dragY = y;
				drag(@event, x, y, pointer);
			}
		}

		public override void touchUp(InputEvent @event, float x, float y, int pointer, int button)
		{
			if (pointer == pressedPointer)
			{
				if (dragging) dragStop(@event, x, y, pointer);
				cancel();
			}
		}

		public void dragStart(InputEvent @event, float x, float y, int pointer)
		{
		}

		public virtual void drag(InputEvent @event, float x, float y, int pointer)
		{
		}

		public virtual void dragStop(InputEvent @event, float x, float y, int pointer)
		{
		}

		/* If a drag is in progress, no further drag methods will be called until a new drag is started. */
		public void cancel()
		{
			dragging = false;
			pressedPointer = -1;
		}

		/** Returns true if a touch has been dragged outside the tap square. */
		public bool isDragging()
		{
			return dragging;
		}

		public void setTapSquareSize(float halfTapSquareSize)
		{
			tapSquareSize = halfTapSquareSize;
		}

		public float getTapSquareSize()
		{
			return tapSquareSize;
		}

		public float getTouchDownX()
		{
			return touchDownX;
		}

		public float getTouchDownY()
		{
			return touchDownY;
		}

		public float getStageTouchDownX()
		{
			return stageTouchDownX;
		}

		public float getStageTouchDownY()
		{
			return stageTouchDownY;
		}

		public float getDragStartX()
		{
			return dragStartX;
		}

		public void setDragStartX(float dragStartX)
		{
			this.dragStartX = dragStartX;
		}

		public float getDragStartY()
		{
			return dragStartY;
		}

		public void setDragStartY(float dragStartY)
		{
			this.dragStartY = dragStartY;
		}

		public float getDragX()
		{
			return dragX;
		}

		public float getDragY()
		{
			return dragY;
		}

		/** The distance from drag start to the current drag position. */
		public float getDragDistance()
		{
			return Vector2.len(dragX - dragStartX, dragY - dragStartY);
		}

		/** Returns the amount on the x axis that the touch has been dragged since the last drag event. */
		public float getDeltaX()
		{
			return dragX - dragLastX;
		}

		/** Returns the amount on the y axis that the touch has been dragged since the last drag event. */
		public float getDeltaY()
		{
			return dragY - dragLastY;
		}

		public int getButton()
		{
			return _button;
		}

		/** Sets the button to listen for, all other buttons are ignored. Default is {@link Buttons#LEFT}. Use -1 for any button. */
		public void setButton(int button)
		{
			this._button = button;
		}
	}
}