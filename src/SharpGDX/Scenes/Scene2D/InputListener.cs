using System;
using SharpGDX.Shims;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Scenes.Scene2D.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D
{
	/** EventListener for low-level input events. Unpacks {@link InputEvent}s and calls the appropriate method. By default the methods
 * here do nothing with the event. Users are expected to override the methods they are interested in, like this:
 *
 * <pre>
 * actor.addListener(new InputListener() {
 * 	public boolean touchDown (InputEvent event, float x, float y, int pointer, int button) {
 * 		Gdx.app.log(&quot;Example&quot;, &quot;touch started at (&quot; + x + &quot;, &quot; + y + &quot;)&quot;);
 * 		return false;
 * 	}
 *
 * 	public void touchUp (InputEvent event, float x, float y, int pointer, int button) {
 * 		Gdx.app.log(&quot;Example&quot;, &quot;touch done at (&quot; + x + &quot;, &quot; + y + &quot;)&quot;);
 * 	}
 * });
 * </pre>
 */
	public class InputListener : IEventListener
	{
		static private readonly Vector2 tmpCoords = new Vector2();

		/** Try to handle the given event, if it is an {@link InputEvent}.
		 * <p>
		 * If the input event is of type {@link InputEvent.Type#touchDown} and {@link InputEvent#getTouchFocus()} is true and
		 * {@link #touchDown(InputEvent, float, float, int, int)} returns true (indicating the event was handled) then this listener is
		 * added to the stage's {@link Stage#addTouchFocus(EventListener, Actor, Actor, int, int) touch focus} so it will receive all
		 * touch dragged events until the next touch up event. */
		public bool handle(Event e)
		{
			if (!(e is InputEvent)) return false;
			InputEvent @event = (InputEvent)e;

			switch (@event.getType())
			{
				case InputEvent.Type.keyDown:
					return keyDown(@event, @event.getKeyCode());
				case InputEvent.Type.keyUp:
					return keyUp(@event, @event.getKeyCode());
				case InputEvent.Type.keyTyped:
					return keyTyped(@event, @event.getCharacter());
			}

			@event.toCoordinates(@event.getListenerActor(), tmpCoords);

			switch (@event.getType())
			{
				case InputEvent.Type.touchDown:
					bool handled = touchDown(@event, tmpCoords.x, tmpCoords.y, @event.getPointer(), @event.getButton());
					if (handled && @event.getTouchFocus())
					{
						@event.getStage().addTouchFocus(this, @event.getListenerActor(), @event.getTarget(),
							@event.getPointer(),
							@event.getButton());
					}

					return handled;
				case InputEvent.Type.touchUp:
					touchUp(@event, tmpCoords.x, tmpCoords.y, @event.getPointer(), @event.getButton());
					return true;
				case InputEvent.Type.touchDragged:
					touchDragged(@event, tmpCoords.x, tmpCoords.y, @event.getPointer());
					return true;
				case InputEvent.Type.mouseMoved:
					return mouseMoved(@event, tmpCoords.x, tmpCoords.y);
				case InputEvent.Type.scrolled:
					return scrolled(@event, tmpCoords.x, tmpCoords.y, @event.getScrollAmountX(),
						@event.getScrollAmountY());
				case InputEvent.Type.enter:
					enter(@event, tmpCoords.x, tmpCoords.y, @event.getPointer(), @event.getRelatedActor());
					return false;
				case InputEvent.Type.exit:
					exit(@event, tmpCoords.x, tmpCoords.y, @event.getPointer(), @event.getRelatedActor());
					return false;
			}

			return false;
		}

		/** Called when a mouse button or a finger touch goes down on the actor. If true is returned, this listener will have
		 * {@link Stage#addTouchFocus(EventListener, Actor, Actor, int, int) touch focus}, so it will receive all touchDragged and
		 * touchUp events, even those not over this actor, until touchUp is received. Also when true is returned, the event is
		 * {@link Event#handle() handled}.
		 * @see InputEvent */
		public virtual bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
		{
			return false;
		}

		/** Called when a mouse button or a finger touch goes up anywhere, but only if touchDown previously returned true for the mouse
		 * button or touch. The touchUp event is always {@link Event#handle() handled}.
		 * @see InputEvent */
		public virtual void touchUp(InputEvent @event, float x, float y, int pointer, int button)
		{
		}

		/** Called when a mouse button or a finger touch is moved anywhere, but only if touchDown previously returned true for the
		 * mouse button or touch. The touchDragged event is always {@link Event#handle() handled}.
		 * @see InputEvent */
		public virtual void touchDragged(InputEvent @event, float x, float y, int pointer)
		{
		}

		/** Called any time the mouse is moved when a button is not down. This event only occurs on the desktop. When true is returned,
		 * the event is {@link Event#handle() handled}.
		 * @see InputEvent */
		public virtual bool mouseMoved(InputEvent @event, float x, float y)
		{
			return false;
		}

		/** Called any time the mouse cursor or a finger touch is moved over an actor. On the desktop, this event occurs even when no
		 * mouse buttons are pressed (pointer will be -1).
		 * @param fromActor May be null.
		 * @see InputEvent */
		public virtual void enter(InputEvent @event, float x, float y, int pointer, Actor? fromActor)
		{
		}

		/** Called any time the mouse cursor or a finger touch is moved out of an actor. On the desktop, this event occurs even when no
		 * mouse buttons are pressed (pointer will be -1).
		 * @param toActor May be null.
		 * @see InputEvent */
		public virtual void exit(InputEvent @event, float x, float y, int pointer, Actor? toActor)
		{
		}

		/** Called when the mouse wheel has been scrolled. When true is returned, the event is {@link Event#handle() handled}. */
		public virtual bool scrolled(InputEvent @event, float x, float y, float amountX, float amountY)
		{
			return false;
		}

		/** Called when a key goes down. When true is returned, the event is {@link Event#handle() handled}. */
		public virtual bool keyDown(InputEvent @event, int keycode)
		{
			return false;
		}

		/** Called when a key goes up. When true is returned, the event is {@link Event#handle() handled}. */
		public virtual bool keyUp(InputEvent @event, int keycode)
		{
			return false;
		}

		/** Called when a key is typed. When true is returned, the event is {@link Event#handle() handled}.
		 * @param character May be 0 for key typed events that don't map to a character (ctrl, shift, etc). */
		public virtual bool keyTyped(InputEvent @event, char character)
		{
			return false;
		}
	}
}