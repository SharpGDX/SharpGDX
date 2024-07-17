using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Manages drag and drop operations through registered drag sources and drop targets.
 * @author Nathan Sweet */
public class DragAndDrop {
	static readonly Vector2 tmpVector = new Vector2();

	Source? dragSource;
	Payload? payload;
	Actor? dragActor;
	bool removeDragActor;
	Target target;
	bool isValidTarget;
	readonly Array<Target> targets = new (8);
	readonly ObjectMap<Source, DragListener> sourceListeners = new (8);
	private float tapSquareSize = 8;
	private int button;
	float dragActorX = 0, dragActorY = 0;
	float touchOffsetX, touchOffsetY;
	long dragValidTime;
	int dragTime = 250;
	int activePointer = -1;
	bool cancelTouchFocus = true;
	bool keepWithinStage = true;

	public void addSource ( Source source)
	{
		throw new NotImplementedException();
		//DragListener listener = new DragListener() {
		//	public void dragStart (InputEvent @event, float x, float y, int pointer) {
		//		if (activePointer != -1) {
		//			@event.stop();
		//			return;
		//		}

		//		activePointer = pointer;

		//		dragValidTime = System.currentTimeMillis() + dragTime;
		//		dragSource = source;
		//		payload = source.dragStart(@event, getTouchDownX(), getTouchDownY(), pointer);
		//		@event.stop();

		//		if (cancelTouchFocus && payload != null) {
		//			Stage stage = source.getActor().getStage();
		//			if (stage != null) stage.cancelTouchFocusExcept(this, source.getActor());
		//		}
		//	}

		//	public void drag (InputEvent event, float x, float y, int pointer) {
		//		if (payload == null) return;
		//		if (pointer != activePointer) return;

		//		source.drag(event, x, y, pointer);

		//		Stage stage = event.getStage();

		//		// Move the drag actor away, so it cannot be hit.
		//		Actor oldDragActor = dragActor;
		//		float oldDragActorX = 0, oldDragActorY = 0;
		//		if (oldDragActor != null) {
		//			oldDragActorX = oldDragActor.getX();
		//			oldDragActorY = oldDragActor.getY();
		//			oldDragActor.setPosition(Integer.MAX_VALUE, Integer.MAX_VALUE);
		//		}

		//		float stageX = event.getStageX() + touchOffsetX, stageY = event.getStageY() + touchOffsetY;
		//		Actor hit = event.getStage().hit(stageX, stageY, true); // Prefer touchable actors.
		//		if (hit == null) hit = event.getStage().hit(stageX, stageY, false);

		//		if (oldDragActor != null) oldDragActor.setPosition(oldDragActorX, oldDragActorY);

		//		// Find target.
		//		Target newTarget = null;
		//		isValidTarget = false;
		//		if (hit != null) {
		//			for (int i = 0, n = targets.size; i < n; i++) {
		//				Target target = targets.get(i);
		//				if (!target.actor.isAscendantOf(hit)) continue;
		//				newTarget = target;
		//				target.actor.stageToLocalCoordinates(tmpVector.set(stageX, stageY));
		//				break;
		//			}
		//		}

		//		// If over a new target, notify the former target that it's being left behind.
		//		if (newTarget != target) {
		//			if (target != null) target.reset(source, payload);
		//			target = newTarget;
		//		}

		//		// Notify new target of drag.
		//		if (newTarget != null) isValidTarget = newTarget.drag(source, payload, tmpVector.x, tmpVector.y, pointer);

		//		// Determine the drag actor, remove the old one if it was added by DragAndDrop, and add the new one.
		//		Actor actor = null;
		//		if (target != null) actor = isValidTarget ? payload.validDragActor : payload.invalidDragActor;
		//		if (actor == null) actor = payload.dragActor;
		//		if (actor != oldDragActor) {
		//			if (oldDragActor != null && removeDragActor) oldDragActor.remove();
		//			dragActor = actor;
		//			removeDragActor = actor.getStage() == null; // Only remove later if not already in the stage now.
		//			if (removeDragActor) stage.addActor(actor);
		//		}
		//		if (actor == null) return;

		//		// Position the drag actor.
		//		float actorX = event.getStageX() - actor.getWidth() + dragActorX;
		//		float actorY = event.getStageY() + dragActorY;
		//		if (keepWithinStage) {
		//			if (actorX < 0) actorX = 0;
		//			if (actorY < 0) actorY = 0;
		//			if (actorX + actor.getWidth() > stage.getWidth()) actorX = stage.getWidth() - actor.getWidth();
		//			if (actorY + actor.getHeight() > stage.getHeight()) actorY = stage.getHeight() - actor.getHeight();
		//		}
		//		actor.setPosition(actorX, actorY);
		//	}

		//	public void dragStop (InputEvent event, float x, float y, int pointer) {
		//		if (pointer != activePointer) return;
		//		activePointer = -1;
		//		if (payload == null) return;

		//		if (System.currentTimeMillis() < dragValidTime)
		//			isValidTarget = false;
		//		else if (!isValidTarget && target != null) {
		//			float stageX = event.getStageX() + touchOffsetX, stageY = event.getStageY() + touchOffsetY;
		//			target.actor.stageToLocalCoordinates(tmpVector.set(stageX, stageY));
		//			isValidTarget = target.drag(source, payload, tmpVector.x, tmpVector.y, pointer);
		//		}
		//		if (dragActor != null && removeDragActor) dragActor.remove();
		//		if (isValidTarget) {
		//			float stageX = event.getStageX() + touchOffsetX, stageY = event.getStageY() + touchOffsetY;
		//			target.actor.stageToLocalCoordinates(tmpVector.set(stageX, stageY));
		//			target.drop(source, payload, tmpVector.x, tmpVector.y, pointer);
		//		}
		//		source.dragStop(event, x, y, pointer, payload, isValidTarget ? target : null);
		//		if (target != null) target.reset(source, payload);
		//		dragSource = null;
		//		payload = null;
		//		target = null;
		//		isValidTarget = false;
		//		dragActor = null;
		//	}
		//};
		//listener.setTapSquareSize(tapSquareSize);
		//listener.setButton(button);
		//source.actor.addCaptureListener(listener);
		//sourceListeners.put(source, listener);
	}

	public void removeSource (Source source) {
		DragListener dragListener = sourceListeners.remove(source);
		source.actor.removeCaptureListener(dragListener);
	}

	public void addTarget (Target target) {
		targets.add(target);
	}

	public void removeTarget (Target target) {
		targets.removeValue(target, true);
	}

	/** Removes all targets and sources. */
	public void clear () {
		targets.clear();
		foreach (var entry in sourceListeners.entries())
			entry.key.actor.removeCaptureListener(entry.value);
		sourceListeners.clear(8);
	}

	/** Cancels the touch focus for everything except the specified source. */
	public void cancelTouchFocusExcept (Source except) {
		DragListener listener = sourceListeners.get(except);
		if (listener == null) return;
		Stage stage = except.getActor().getStage();
		if (stage != null) stage.cancelTouchFocusExcept(listener, except.getActor());
	}

	/** Sets the distance a touch must travel before being considered a drag. */
	public void setTapSquareSize (float halfTapSquareSize) {
		tapSquareSize = halfTapSquareSize;
	}

	/** Sets the button to listen for, all other buttons are ignored. Default is {@link Buttons#LEFT}. Use -1 for any button. */
	public void setButton (int button) {
		this.button = button;
	}

	public void setDragActorPosition (float dragActorX, float dragActorY) {
		this.dragActorX = dragActorX;
		this.dragActorY = dragActorY;
	}

	/** Sets an offset in stage coordinates from the touch position which is used to determine the drop location. Default is
	 * 0,0. */
	public void setTouchOffset (float touchOffsetX, float touchOffsetY) {
		this.touchOffsetX = touchOffsetX;
		this.touchOffsetY = touchOffsetY;
	}

	public bool isDragging () {
		return payload != null;
	}

	/** Returns the current drag actor, or null. */
	public  Actor? getDragActor () {
		return dragActor;
	}

	/** Returns the current drag payload, or null. */
	public  Payload? getDragPayload () {
		return payload;
	}

	/** Returns the current drag source, or null. */
	public  Source? getDragSource () {
		return dragSource;
	}

	/** Time in milliseconds that a drag must take before a drop will be considered valid. This ignores an accidental drag and drop
	 * that was meant to be a click. Default is 250. */
	public void setDragTime (int dragMillis) {
		this.dragTime = dragMillis;
	}

	public int getDragTime () {
		return dragTime;
	}

	/** Returns true if a drag is in progress and the {@link #setDragTime(int) drag time} has elapsed since the drag started. */
	public bool isDragValid () {
		return payload != null && TimeUtils.currentTimeMillis() >= dragValidTime;
	}

	/** When true (default), the {@link Stage#cancelTouchFocus()} touch focus} is cancelled if
	 * {@link Source#dragStart(InputEvent, float, float, int) dragStart} returns non-null. This ensures the DragAndDrop is the only
	 * touch focus listener, eg when the source is inside a {@link ScrollPane} with flick scroll enabled. */
	public void setCancelTouchFocus (bool cancelTouchFocus) {
		this.cancelTouchFocus = cancelTouchFocus;
	}

	public void setKeepWithinStage (bool keepWithinStage) {
		this.keepWithinStage = keepWithinStage;
	}

	/** A source where a payload can be dragged from.
	 * @author Nathan Sweet */
	 abstract public class Source {
		internal readonly Actor actor;

		public Source (Actor actor) {
			if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
			this.actor = actor;
		}

		/** Called when a drag is started on the source. The coordinates are in the source's local coordinate system.
		 * @return If null the drag will not affect any targets. */
		abstract public  Payload? dragStart (InputEvent @event, float x, float y, int pointer);

		/** Called repeatedly during a drag which started on this source. */
		public void drag (InputEvent @event, float x, float y, int pointer) {
		}

		/** Called when a drag for the source is stopped. The coordinates are in the source's local coordinate system.
		 * @param payload null if dragStart returned null.
		 * @param target null if not dropped on a valid target. */
		public void dragStop (InputEvent @event, float x, float y, int pointer,  Payload? payload,  Target? target) {
		}

		public Actor getActor () {
			return actor;
		}
	}

	/** A target where a payload can be dropped to.
	 * @author Nathan Sweet */
	 abstract public class Target {
		readonly Actor actor;

		public Target (Actor actor) {
			if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
			this.actor = actor;
			Stage stage = actor.getStage();
			if (stage != null && actor == stage.getRoot())
				throw new IllegalArgumentException("The stage root cannot be a drag and drop target.");
		}

		/** Called when the payload is dragged over the target. The coordinates are in the target's local coordinate system.
		 * @return true if this is a valid target for the payload. */
		abstract public bool drag (Source source, Payload payload, float x, float y, int pointer);

		/** Called when the payload is no longer over the target, whether because the touch was moved or a drop occurred. This is
		 * called even if {@link #drag(Source, Payload, float, float, int)} returned false. */
		public void reset (Source source, Payload payload) {
		}

		/** Called when the payload is dropped on the target. The coordinates are in the target's local coordinate system. This is
		 * not called if {@link #drag(Source, Payload, float, float, int)} returned false. */
		abstract public void drop (Source source, Payload payload, float x, float y, int pointer);

		public Actor getActor () {
			return actor;
		}
	}

	/** The payload of a drag and drop operation. Actors can be optionally provided to follow the cursor and change when over a
	 * target. Such actors will be added the stage automatically during the drag operation as necessary and they will only be
	 * removed from the stage if they were added automatically. A source actor can be used as a payload drag actor. */
	 public class Payload {
		 Actor? dragActor, validDragActor, invalidDragActor;
		 Object? obj;

		public void setDragActor ( Actor? dragActor) {
			this.dragActor = dragActor;
		}

		public Actor? getDragActor () {
			return dragActor;
		}

		public void setValidDragActor ( Actor? validDragActor) {
			this.validDragActor = validDragActor;
		}

		public  Actor? getValidDragActor () {
			return validDragActor;
		}

		public void setInvalidDragActor ( Actor? invalidDragActor) {
			this.invalidDragActor = invalidDragActor;
		}

		public  Actor? getInvalidDragActor () {
			return invalidDragActor;
		}

		public  Object? getObject () {
			return obj;
		}

		public void setObject ( Object? obj) {
			this.obj = obj;
		}
	}
}
}
