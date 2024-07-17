using SharpGDX.Shims;
using SharpGDX.Input;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Utils;
using SharpGDX;
using SharpGDX.Mathematics;
using System.Reflection;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Detects tap, long press, fling, pan, zoom, and pinch gestures on an actor. If there is only a need to detect tap, use
 * {@link ClickListener}.
 * @see GestureDetector
 * @author Nathan Sweet */
public class ActorGestureListener : IEventListener {
	static readonly Vector2 tmpCoords = new Vector2(), tmpCoords2 = new Vector2();

	private readonly GestureDetector detector;
	InputEvent @event;
	Actor actor, touchDownTarget;

	/** @see GestureDetector#GestureDetector(com.badlogic.gdx.input.GestureDetector.GestureListener) */
	public ActorGestureListener () 
	: this(20, 0.4f, 1.1f, int.MaxValue)
	{
		
	}

	/** @see GestureDetector#GestureDetector(float, float, float, float, com.badlogic.gdx.input.GestureDetector.GestureListener) */
	public ActorGestureListener (float halfTapSquareSize, float tapCountInterval, float longPressDuration, float maxFlingDelay) {
		detector = new GestureDetector(halfTapSquareSize, tapCountInterval, longPressDuration, maxFlingDelay, new ActorGestureListenerGestureAdapter(this) );
	}

	private class ActorGestureListenerGestureAdapter : GestureDetector.GestureAdapter
		{
			private readonly ActorGestureListener _actorGestureListener;

			public ActorGestureListenerGestureAdapter(ActorGestureListener actorGestureListener)
			{
				_actorGestureListener = actorGestureListener;
			}

			private readonly Vector2 initialPointer1 = new Vector2(), initialPointer2 = new Vector2();
			private readonly Vector2 pointer1 = new Vector2(), pointer2 = new Vector2();

			public override bool tap(float stageX, float stageY, int count, int button)
			{
				_actorGestureListener.actor.stageToLocalCoordinates(tmpCoords.set(stageX, stageY));
				_actorGestureListener.tap(_actorGestureListener.@event, tmpCoords.x, tmpCoords.y, count, button);
				return true;
			}

			public override bool longPress(float stageX, float stageY)
			{
				_actorGestureListener.actor.stageToLocalCoordinates(tmpCoords.set(stageX, stageY));
				return _actorGestureListener.longPress(_actorGestureListener.actor, tmpCoords.x, tmpCoords.y);
			}

			public override bool fling(float velocityX, float velocityY, int button)
			{
				stageToLocalAmount(tmpCoords.set(velocityX, velocityY));
				_actorGestureListener.fling(_actorGestureListener.@event, tmpCoords.x, tmpCoords.y, button);
				return true;
			}

			public override bool pan(float stageX, float stageY, float deltaX, float deltaY)
			{
				stageToLocalAmount(tmpCoords.set(deltaX, deltaY));
				deltaX = tmpCoords.x;
				deltaY = tmpCoords.y;
				_actorGestureListener.actor.stageToLocalCoordinates(tmpCoords.set(stageX, stageY));
				_actorGestureListener.pan(_actorGestureListener.@event, tmpCoords.x, tmpCoords.y, deltaX, deltaY);
				return true;
			}

			public override bool panStop(float stageX, float stageY, int pointer, int button)
			{
				_actorGestureListener.actor.stageToLocalCoordinates(tmpCoords.set(stageX, stageY));
				_actorGestureListener.panStop(_actorGestureListener.@event, tmpCoords.x, tmpCoords.y, pointer, button);
				return true;
			}

			public override bool zoom(float initialDistance, float distance)
			{
				_actorGestureListener.zoom(_actorGestureListener.@event, initialDistance, distance);
				return true;
			}

			public override bool pinch(Vector2 stageInitialPointer1, Vector2 stageInitialPointer2, Vector2 stagePointer1,
				Vector2 stagePointer2)
			{
				_actorGestureListener.actor.stageToLocalCoordinates(initialPointer1.set(stageInitialPointer1));
				_actorGestureListener.actor.stageToLocalCoordinates(initialPointer2.set(stageInitialPointer2));
				_actorGestureListener.actor.stageToLocalCoordinates(pointer1.set(stagePointer1));
				_actorGestureListener.actor.stageToLocalCoordinates(pointer2.set(stagePointer2));
				_actorGestureListener.pinch(_actorGestureListener.@event, initialPointer1, initialPointer2, pointer1, pointer2);
				return true;
			}

			private void stageToLocalAmount(Vector2 amount)
			{
				_actorGestureListener.actor.stageToLocalCoordinates(amount);
				amount.sub(_actorGestureListener.actor.stageToLocalCoordinates(tmpCoords2.set(0, 0)));
			}
		}

		public virtual bool handle (Event e) {
		if (!(e is InputEvent)) return false;
		InputEvent @event = (InputEvent)e;

		switch (@event.getType()) {
		case InputEvent.Type.touchDown:
			actor = @event.getListenerActor();
			touchDownTarget = @event.getTarget();
			detector.touchDown(@event.getStageX(), @event.getStageY(), @event.getPointer(), @event.getButton());
			actor.stageToLocalCoordinates(tmpCoords.set(@event.getStageX(), @event.getStageY()));
			touchDown(@event, tmpCoords.x, tmpCoords.y, @event.getPointer(), @event.getButton());
			if (@event.getTouchFocus()) @event.getStage().addTouchFocus(this, @event.getListenerActor(), @event.getTarget(),
				@event.getPointer(), @event.getButton());
			return true;
		case InputEvent.Type.touchUp:
			if (@event.isTouchFocusCancel()) {
				detector.reset();
				return false;
			}
			this.@event = @event;
			actor = @event.getListenerActor();
			detector.touchUp(@event.getStageX(), @event.getStageY(), @event.getPointer(), @event.getButton());
			actor.stageToLocalCoordinates(tmpCoords.set(@event.getStageX(), @event.getStageY()));
			touchUp(@event, tmpCoords.x, tmpCoords.y, @event.getPointer(), @event.getButton());
			return true;
		case InputEvent.Type.touchDragged:
			this.@event = @event;
			actor = @event.getListenerActor();
			detector.touchDragged(@event.getStageX(), @event.getStageY(), @event.getPointer());
			return true;
		}
		return false;
	}

		public virtual void touchDown (InputEvent @event, float x, float y, int pointer, int button) {
	}

		public virtual void touchUp (InputEvent @event, float x, float y, int pointer, int button) {
	}

		public virtual void tap (InputEvent @event, float x, float y, int count, int button) {
	}

		/** If true is returned, additional gestures will not be triggered. No event is provided because this event is triggered by
		 * time passing, not by an InputEvent. */
		public virtual bool longPress (Actor actor, float x, float y) {
		return false;
	}

	public virtual void fling (InputEvent @event, float velocityX, float velocityY, int button) {
	}

		/** The delta is the difference in stage coordinates since the last pan. */
		public virtual void pan (InputEvent @event, float x, float y, float deltaX, float deltaY) {
	}

		public virtual void panStop (InputEvent @event, float x, float y, int pointer, int button) {
	}

		public virtual void zoom (InputEvent @event, float initialDistance, float distance) {
	}

		public virtual void pinch (InputEvent @event, Vector2 initialPointer1, Vector2 initialPointer2, Vector2 pointer1, Vector2 pointer2) {
	}

	public GestureDetector getGestureDetector () {
		return detector;
	}

	public Actor? getTouchDownTarget () {
		return touchDownTarget;
	}
}
}
