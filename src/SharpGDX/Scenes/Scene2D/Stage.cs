using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Utils.Viewports;
using SharpGDX.Scenes.Scene2D;

namespace SharpGDX.Scenes.Scene2D
{
	/** A 2D scene graph containing hierarchies of {@link Actor actors}. Stage handles the viewport and distributes input events.
 * <p>
 * {@link #setViewport(Viewport)} controls the coordinates used within the stage and sets up the camera used to convert between
 * stage coordinates and screen coordinates.
 * <p>
 * A stage must receive input events so it can distribute them to actors. This is typically done by passing the stage to
 * {@link Input#setInputProcessor(com.badlogic.gdx.InputProcessor) Gdx.input.setInputProcessor}. An {@link InputMultiplexer} may
 * be used to handle input events before or after the stage does. If an actor handles an event by returning true from the input
 * method, then the stage's input method will also return true, causing subsequent InputProcessors to not receive the event.
 * <p>
 * The Stage and its constituents (like Actors and Listeners) are not thread-safe and should only be updated and queried from a
 * single thread (presumably the main render thread). Methods should be reentrant, so you can update Actors and Stages from within
 * callbacks and handlers.
 * @author mzechner
 * @author Nathan Sweet */
public class Stage : InputAdapter , Disposable {
	/** True if any actor has ever had debug enabled. */
	internal static bool debug;

	private Viewport viewport;
	private readonly IBatch batch;
	private bool ownsBatch;
	private Group root;
	private readonly Vector2 tempCoords = new Vector2();
	private readonly Actor[] pointerOverActors = new Actor[20];
	private readonly bool[] pointerTouched = new bool[20];
	private readonly int[] pointerScreenX = new int[20], pointerScreenY = new int[20];
	private int mouseScreenX, mouseScreenY;
	private Actor? mouseOverActor;
	private Actor? keyboardFocus, scrollFocus;
	internal readonly SnapshotArray<TouchFocus> touchFocuses = new (true, 4, typeof(TouchFocus));
	private bool actionsRequestRendering = true;

	private ShapeRenderer debugShapes;
	private bool debugInvisible, debugAll, debugUnderMouse, debugParentUnderMouse;
	private Table.Debug debugTableUnderMouse = Table.Debug.none;
	private readonly Color debugColor = new Color(0, 1, 0, 0.85f);

	/** Creates a stage with a {@link ScalingViewport} set to {@link Scaling#stretch}. The stage will use its own {@link Batch}
	 * which will be disposed when the stage is disposed. */
	public Stage ()
		: this(new ScalingViewport(Scaling.stretch, Gdx.graphics.getWidth(), Gdx.graphics.getHeight(), new OrthographicCamera()),
		new SpriteBatch())
	{
		
		ownsBatch = true;
	}

	/** Creates a stage with the specified viewport. The stage will use its own {@link Batch} which will be disposed when the stage
	 * is disposed. */
	public Stage (Viewport viewport) 
	: this(viewport, new SpriteBatch())
	{
		
		ownsBatch = true;
	}

	/** Creates a stage with the specified viewport and batch. This can be used to specify an existing batch or to customize which
	 * batch implementation is used.
	 * @param batch Will not be disposed if {@link #dispose()} is called, handle disposal yourself. */
	public Stage (Viewport viewport, IBatch batch) {
		if (viewport == null) throw new IllegalArgumentException("viewport cannot be null.");
		if (batch == null) throw new IllegalArgumentException("batch cannot be null.");
		this.viewport = viewport;
		this.batch = batch;

		root = new Group();
		root.setStage(this);

		viewport.update(Gdx.graphics.getWidth(), Gdx.graphics.getHeight(), true);
	}

	public void draw () {
		Camera camera = viewport.getCamera();
		camera.update();

		if (!root.isVisible()) return;

		IBatch batch = this.batch;
		batch.setProjectionMatrix(camera.combined);
		batch.begin();
		root.draw(batch, 1);
		batch.end();

		if (debug) drawDebug();
	}

	private void drawDebug () {
		if (debugShapes == null) {
			debugShapes = new ShapeRenderer();
			debugShapes.setAutoShapeType(true);
		}

		if (debugUnderMouse || debugParentUnderMouse || debugTableUnderMouse != Table.Debug.none) {
			screenToStageCoordinates(tempCoords.set(Gdx.input.getX(), Gdx.input.getY()));
			Actor actor = hit(tempCoords.x, tempCoords.y, true);
			if (actor == null) return;

			if (debugParentUnderMouse && actor.parent != null) actor = actor.parent;

			if (debugTableUnderMouse == Table.Debug.none)
				actor.setDebug(true);
			else {
				while (actor != null) {
					if (actor is Table) break;
					actor = actor.parent;
				}
				if (actor == null) return;
				((Table)actor).debug(debugTableUnderMouse);
			}

			if (debugAll && actor is Group) ((Group)actor).debugAll();

			disableDebug(root, actor);
		} else {
			if (debugAll) root.debugAll();
		}

		Gdx.gl.glEnable(GL20.GL_BLEND);
		debugShapes.setProjectionMatrix(viewport.getCamera().combined);
		debugShapes.begin();
		root.drawDebug(debugShapes);
		debugShapes.end();
		Gdx.gl.glDisable(GL20.GL_BLEND);
	}

	/** Disables debug on all actors recursively except the specified actor and any children. */
	private void disableDebug (Actor actor, Actor except) {
		if (actor == except) return;
		actor.setDebug(false);
		if (actor is Group) {
			SnapshotArray<Actor> children = ((Group)actor).children;
			for (int i = 0, n = children.size; i < n; i++)
				disableDebug(children.get(i), except);
		}
	}

	/** Calls {@link #act(float)} with {@link Graphics#getDeltaTime()}, limited to a minimum of 30fps. */
	public void act () {
		act(Math.Min(Gdx.graphics.getDeltaTime(), 1 / 30f));
	}

	/** Calls the {@link Actor#act(float)} method on each actor in the stage. Typically called each frame. This method also fires
	 * enter and exit events.
	 * @param delta Time in seconds since the last frame. */
	public void act (float delta) {
		// Update over actors. Done in act() because actors may change position, which can fire enter/exit without an input event.
		for (int pointer = 0, n = pointerOverActors.Length; pointer < n; pointer++) {
			Actor overLast = pointerOverActors[pointer];
			if (pointerTouched[pointer]) {
				// Update the over actor for the pointer.
				pointerOverActors[pointer] = fireEnterAndExit(overLast, pointerScreenX[pointer], pointerScreenY[pointer], pointer);
			} else if (overLast != null) {
				// The pointer is gone, exit the over actor for the pointer, if any.
				pointerOverActors[pointer] = null;
				fireExit(overLast, pointerScreenX[pointer], pointerScreenY[pointer], pointer);
			}
		}

		// Update over actor for the mouse on the desktop.
		IApplication.ApplicationType type = Gdx.app.getType();
		if (type == IApplication.ApplicationType.Desktop || type == IApplication.ApplicationType.Applet || type == IApplication.ApplicationType.WebGL)
			mouseOverActor = fireEnterAndExit(mouseOverActor, mouseScreenX, mouseScreenY, -1);

		root.act(delta);
	}

	private Actor? fireEnterAndExit (Actor? overLast, int screenX, int screenY, int pointer) {
		// Find the actor under the point.
		screenToStageCoordinates(tempCoords.set(screenX, screenY));
		Actor over = hit(tempCoords.x, tempCoords.y, true);
		if (over == overLast) return overLast;

		// Exit overLast.
		if (overLast != null) {
			InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
			@event.setType(InputEvent.Type.exit);
			@event.setStage(this);
			@event.setStageX(tempCoords.x);
			@event.setStageY(tempCoords.y);
			@event.setPointer(pointer);
			@event.setRelatedActor(over);
			overLast.fire(@event);
			Pools.free(@event);
		}

		// Enter over.
		if (over != null) {
			InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
			@event.setType(InputEvent.Type.enter);
			@event.setStage(this);
			@event.setStageX(tempCoords.x);
			@event.setStageY(tempCoords.y);
			@event.setPointer(pointer);
			@event.setRelatedActor(overLast);
			over.fire(@event);
			Pools.free(@event);
		}
		return over;
	}

	private void fireExit (Actor actor, int screenX, int screenY, int pointer) {
		screenToStageCoordinates(tempCoords.set(screenX, screenY));
		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.exit);
		@event.setStage(this);
		@event.setStageX(tempCoords.x);
		@event.setStageY(tempCoords.y);
		@event.setPointer(pointer);
		@event.setRelatedActor(actor);
		actor.fire(@event);
		Pools.free(@event);
	}

	/** Applies a touch down event to the stage and returns true if an actor in the scene {@link Event#handle() handled} the
	 * event. */
	public bool touchDown (int screenX, int screenY, int pointer, int button) {
		if (!isInsideViewport(screenX, screenY)) return false;

		pointerTouched[pointer] = true;
		pointerScreenX[pointer] = screenX;
		pointerScreenY[pointer] = screenY;

		screenToStageCoordinates(tempCoords.set(screenX, screenY));

		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.touchDown);
		@event.setStage(this);
		@event.setStageX(tempCoords.x);
		@event.setStageY(tempCoords.y);
		@event.setPointer(pointer);
		@event.setButton(button);

		Actor target = hit(tempCoords.x, tempCoords.y, true);
		if (target == null) {
			if (root.getTouchable() == Touchable.enabled) root.fire(@event);
		} else
			target.fire(@event);

		bool handled = @event.isHandled();
		Pools.free(@event);
		return handled;
	}

	/** Applies a touch moved event to the stage and returns true if an actor in the scene {@link Event#handle() handled} the
	 * event. Only {@link InputListener listeners} that returned true for touchDown will receive this event. */
	public bool touchDragged (int screenX, int screenY, int pointer) {
		pointerScreenX[pointer] = screenX;
		pointerScreenY[pointer] = screenY;
		mouseScreenX = screenX;
		mouseScreenY = screenY;

		if (this.touchFocuses.size == 0) return false;

		screenToStageCoordinates(tempCoords.set(screenX, screenY));

		InputEvent @event = Pools.obtain<Scene2D.InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.touchDragged);
		@event.setStage(this);
		@event.setStageX(tempCoords.x);
		@event.setStageY(tempCoords.y);
		@event.setPointer(pointer);

		SnapshotArray<TouchFocus> touchFocuses = this.touchFocuses;
		TouchFocus[] focuses = touchFocuses.begin();
		for (int i = 0, n = touchFocuses.size; i < n; i++) {
			TouchFocus focus = focuses[i];
			if (focus.pointer != pointer) continue;
			if (!touchFocuses.contains(focus, true)) continue; // Touch focus already gone.
			@event.setTarget(focus.target);
			@event.setListenerActor(focus.listenerActor);
			if (focus.listener.handle(@event)) @event.handle();
		}
		touchFocuses.end();

		bool handled = @event.isHandled();
		Pools.free(@event);
		return handled;
	}

	/** Applies a touch up event to the stage and returns true if an actor in the scene {@link Event#handle() handled} the event.
	 * Only {@link InputListener listeners} that returned true for touchDown will receive this event. */
	public bool touchUp (int screenX, int screenY, int pointer, int button) {
		pointerTouched[pointer] = false;
		pointerScreenX[pointer] = screenX;
		pointerScreenY[pointer] = screenY;

		if (this.touchFocuses.size == 0) return false;

		screenToStageCoordinates(tempCoords.set(screenX, screenY));

		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.touchUp);
		@event.setStage(this);
		@event.setStageX(tempCoords.x);
		@event.setStageY(tempCoords.y);
		@event.setPointer(pointer);
		@event.setButton(button);

		SnapshotArray<TouchFocus> touchFocuses = this.touchFocuses;
		TouchFocus[] focuses = touchFocuses.begin();
		for (int i = 0, n = touchFocuses.size; i < n; i++) {
			TouchFocus focus = focuses[i];
			if (focus.pointer != pointer || focus.button != button) continue;
			if (!touchFocuses.removeValue(focus, true)) continue; // Touch focus already gone.
			@event.setTarget(focus.target);
			@event.setListenerActor(focus.listenerActor);
			if (focus.listener.handle(@event)) @event.handle();
			Pools.free(focus);
		}
		touchFocuses.end();

		bool handled = @event.isHandled();
		Pools.free(@event);
		return handled;
	}

	public bool touchCancelled (int screenX, int screenY, int pointer, int button) {
		cancelTouchFocus();
		return false;
	}

	/** Applies a mouse moved event to the stage and returns true if an actor in the scene {@link Event#handle() handled} the
	 * event. This event only occurs on the desktop. */
	public bool mouseMoved (int screenX, int screenY) {
		mouseScreenX = screenX;
		mouseScreenY = screenY;

		if (!isInsideViewport(screenX, screenY)) return false;

		screenToStageCoordinates(tempCoords.set(screenX, screenY));

		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.mouseMoved);
		@event.setStage(this);
		@event.setStageX(tempCoords.x);
		@event.setStageY(tempCoords.y);

		Actor target = hit(tempCoords.x, tempCoords.y, true);
		if (target == null) target = root;

		target.fire(@event);
		bool handled = @event.isHandled();
		Pools.free(@event);
		return handled;
	}

	/** Applies a mouse scroll event to the stage and returns true if an actor in the scene {@link Event#handle() handled} the
	 * event. This event only occurs on the desktop. */
	public bool scrolled (float amountX, float amountY) {
		Actor target = scrollFocus == null ? root : scrollFocus;

		screenToStageCoordinates(tempCoords.set(mouseScreenX, mouseScreenY));

		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.scrolled);
		@event.setStage(this);
		@event.setStageX(tempCoords.x);
		@event.setStageY(tempCoords.y);
		@event.setScrollAmountX(amountX);
		@event.setScrollAmountY(amountY);
		target.fire(@event);
		bool handled = @event.isHandled();
		Pools.free(@event);
		return handled;
	}

	/** Applies a key down event to the actor that has {@link Stage#setKeyboardFocus(Actor) keyboard focus}, if any, and returns
	 * true if the event was {@link Event#handle() handled}. */
	public bool keyDown (int keyCode) {
		Actor target = keyboardFocus == null ? root : keyboardFocus;
		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.keyDown);
		@event.setStage(this);
		@event.setKeyCode(keyCode);
		target.fire(@event);
		bool handled = @event.isHandled();
		Pools.free(@event);
		return handled;
	}

	/** Applies a key up event to the actor that has {@link Stage#setKeyboardFocus(Actor) keyboard focus}, if any, and returns true
	 * if the event was {@link Event#handle() handled}. */
	public bool keyUp (int keyCode) {
		Actor target = keyboardFocus == null ? root : keyboardFocus;
		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.keyUp);
		@event.setStage(this);
		@event.setKeyCode(keyCode);
		target.fire(@event);
		bool handled = @event.isHandled();
		Pools.free(@event);
		return handled;
	}

	/** Applies a key typed event to the actor that has {@link Stage#setKeyboardFocus(Actor) keyboard focus}, if any, and returns
	 * true if the event was {@link Event#handle() handled}. */
	public bool keyTyped (char character) {
		Actor target = keyboardFocus == null ? root : keyboardFocus;
		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.keyTyped);
		@event.setStage(this);
		@event.setCharacter(character);
		target.fire(@event);
		bool handled = @event.isHandled();
		Pools.free(@event);
		return handled;
	}

	/** Adds the listener to be notified for all touchDragged and touchUp events for the specified pointer and button. Touch focus
	 * is added automatically when true is returned from {@link InputListener#touchDown(InputEvent, float, float, int, int)
	 * touchDown}. The specified actors will be used as the {@link Event#getListenerActor() listener actor} and
	 * {@link Event#getTarget() target} for the touchDragged and touchUp events. */
	public void addTouchFocus (IEventListener listener, Actor listenerActor, Actor target, int pointer, int button) {
		TouchFocus focus = Pools.obtain< TouchFocus>(typeof(TouchFocus));
		focus.listenerActor = listenerActor;
		focus.target = target;
		focus.listener = listener;
		focus.pointer = pointer;
		focus.button = button;
		touchFocuses.add(focus);
	}

	/** Removes touch focus for the specified listener, pointer, and button. Note the listener will not receive a touchUp event
	 * when this method is used. */
	public void removeTouchFocus (IEventListener listener, Actor listenerActor, Actor target, int pointer, int button) {
		SnapshotArray<TouchFocus> touchFocuses = this.touchFocuses;
		for (int i = touchFocuses.size - 1; i >= 0; i--) {
			TouchFocus focus = touchFocuses.get(i);
			if (focus.listener == listener && focus.listenerActor == listenerActor && focus.target == target
				&& focus.pointer == pointer && focus.button == button) {
				touchFocuses.removeIndex(i);
				Pools.free(focus);
			}
		}
	}

	/** Cancels touch focus for all listeners with the specified listener actor.
	 * @see #cancelTouchFocus() */
	public void cancelTouchFocus (Actor listenerActor) {
		// Cancel all current touch focuses for the specified listener, allowing for concurrent modification, and never cancel the
		// same focus twice.
		InputEvent @event = null;
		SnapshotArray<TouchFocus> touchFocuses = this.touchFocuses;
		TouchFocus[] items = touchFocuses.begin();
		for (int i = 0, n = touchFocuses.size; i < n; i++) {
			TouchFocus focus = items[i];
			if (focus.listenerActor != listenerActor) continue;
			if (!touchFocuses.removeValue(focus, true)) continue; // Touch focus already gone.

			if (@event == null) {
				@event = Pools.obtain<InputEvent>(typeof(InputEvent));
				@event.setType(InputEvent.Type.touchUp);
				@event.setStage(this);
				@event.setStageX(int.MinValue);
				@event.setStageY(int.MinValue);
			}

			@event.setTarget(focus.target);
			@event.setListenerActor(focus.listenerActor);
			@event.setPointer(focus.pointer);
			@event.setButton(focus.button);
			focus.listener.handle(@event);
			// Cannot return TouchFocus to pool, as it may still be in use (eg if cancelTouchFocus is called from touchDragged).
		}
		touchFocuses.end();

		if (@event != null) Pools.free(@event);
	}

	/** Removes all touch focus listeners, sending a touchUp event to each listener. Listeners typically expect to receive a
	 * touchUp event when they have touch focus. The location of the touchUp is {@link Integer#MIN_VALUE}. Listeners can use
	 * {@link InputEvent#isTouchFocusCancel()} to ignore this event if needed. */
	public void cancelTouchFocus () {
		cancelTouchFocusExcept(null, null);
	}

	/** Cancels touch focus for all listeners except the specified listener.
	 * @see #cancelTouchFocus() */
	public void cancelTouchFocusExcept (IEventListener? exceptListener, Actor? exceptActor) {
		InputEvent @event = Pools.obtain<InputEvent>(typeof(InputEvent));
		@event.setType(InputEvent.Type.touchUp);
		@event.setStage(this);
		@event.setStageX(int.MinValue);
		@event.setStageY(int.MinValue);

		// Cancel all current touch focuses except for the specified listener, allowing for concurrent modification, and never
		// cancel the same focus twice.
		SnapshotArray<TouchFocus> touchFocuses = this.touchFocuses;
		TouchFocus[] items = touchFocuses.begin();
		for (int i = 0, n = touchFocuses.size; i < n; i++) {
			TouchFocus focus = items[i];
			if (focus.listener == exceptListener && focus.listenerActor == exceptActor) continue;
			if (!touchFocuses.removeValue(focus, true)) continue; // Touch focus already gone.
			@event.setTarget(focus.target);
			@event.setListenerActor(focus.listenerActor);
			@event.setPointer(focus.pointer);
			@event.setButton(focus.button);
			focus.listener.handle(@event);
			// Cannot return TouchFocus to pool, as it may still be in use (eg if cancelTouchFocus is called from touchDragged).
		}
		touchFocuses.end();

		Pools.free(@event);
	}

	/** Adds an actor to the root of the stage.
	 * @see Group#addActor(Actor) */
	public void addActor (Actor actor) {
		root.addActor(actor);
	}

	/** Adds an action to the root of the stage.
	 * @see Group#addAction(Action) */
	public void addAction (Action action) {
		root.addAction(action);
	}

	/** Returns the root's child actors.
	 * @see Group#getChildren() */
	public Array<Actor> getActors () {
		return root.children;
	}

	/** Adds a listener to the root.
	 * @see Actor#addListener(EventListener) */
	public bool addListener (IEventListener listener) {
		return root.addListener(listener);
	}

	/** Removes a listener from the root.
	 * @see Actor#removeListener(EventListener) */
	public bool removeListener (IEventListener listener) {
		return root.removeListener(listener);
	}

	/** Adds a capture listener to the root.
	 * @see Actor#addCaptureListener(EventListener) */
	public bool addCaptureListener (IEventListener listener) {
		return root.addCaptureListener(listener);
	}

	/** Removes a listener from the root.
	 * @see Actor#removeCaptureListener(EventListener) */
	public bool removeCaptureListener (IEventListener listener) {
		return root.removeCaptureListener(listener);
	}

	/** Called just before an actor is removed from a group.
	 * <p>
	 * The default implementation fires an {@link InputEvent.Type#exit} event if a pointer had entered the actor. */
	internal protected void actorRemoved (Actor actor) {
		for (int pointer = 0, n = pointerOverActors.Length; pointer < n; pointer++) {
			if (actor == pointerOverActors[pointer]) {
				pointerOverActors[pointer] = null;
				fireExit(actor, pointerScreenX[pointer], pointerScreenY[pointer], pointer);
			}
		}

		if (actor == mouseOverActor) {
			mouseOverActor = null;
			fireExit(actor, mouseScreenX, mouseScreenY, -1);
		}
	}

	/** Removes the root's children, actions, and listeners. */
	public void clear () {
		unfocusAll();
		root.clear();
	}

	/** Removes the touch, keyboard, and scroll focused actors. */
	public void unfocusAll () {
		setScrollFocus(null);
		setKeyboardFocus(null);
		cancelTouchFocus();
	}

	/** Removes the touch, keyboard, and scroll focus for the specified actor and any descendants. */
	public void unfocus (Actor actor) {
		cancelTouchFocus(actor);
		if (scrollFocus != null && scrollFocus.isDescendantOf(actor)) setScrollFocus(null);
		if (keyboardFocus != null && keyboardFocus.isDescendantOf(actor)) setKeyboardFocus(null);
	}

	/** Sets the actor that will receive key events.
	 * @param actor May be null.
	 * @return true if the unfocus and focus events were not cancelled by a {@link FocusListener}. */
	public bool setKeyboardFocus (Actor? actor) {
		if (keyboardFocus == actor) return true;
		FocusListener.FocusEvent @event = Pools.obtain<FocusListener.FocusEvent>(typeof(FocusListener.FocusEvent));
		@event.setStage(this);
		@event.setType(FocusListener.FocusEvent.Type.keyboard);
		Actor oldKeyboardFocus = keyboardFocus;
		if (oldKeyboardFocus != null) {
			@event.setFocused(false);
			@event.setRelatedActor(actor);
			oldKeyboardFocus.fire(@event);
		}
		bool success = !@event.isCancelled();
		if (success) {
			keyboardFocus = actor;
			if (actor != null) {
				@event.setFocused(true);
				@event.setRelatedActor(oldKeyboardFocus);
				actor.fire(@event);
				success = !@event.isCancelled();
				if (!success) keyboardFocus = oldKeyboardFocus;
			}
		}
		Pools.free(@event);
		return success;
	}

	/** Gets the actor that will receive key events.
	 * @return May be null. */
	public Actor? getKeyboardFocus () {
		return keyboardFocus;
	}

	/** Sets the actor that will receive scroll events.
	 * @param actor May be null.
	 * @return true if the unfocus and focus events were not cancelled by a {@link FocusListener}. */
	public bool setScrollFocus (Actor? actor) {
		if (scrollFocus == actor) return true;
		FocusListener.FocusEvent @event = Pools.obtain<FocusListener.FocusEvent>(typeof(FocusListener.FocusEvent));
		@event.setStage(this);
		@event.setType(FocusListener.FocusEvent.Type.scroll);
		Actor oldScrollFocus = scrollFocus;
		if (oldScrollFocus != null) {
			@event.setFocused(false);
			@event.setRelatedActor(actor);
			oldScrollFocus.fire(@event);
		}
		bool success = !@event.isCancelled();
		if (success) {
			scrollFocus = actor;
			if (actor != null) {
				@event.setFocused(true);
				@event.setRelatedActor(oldScrollFocus);
				actor.fire(@event);
				success = !@event.isCancelled();
				if (!success) scrollFocus = oldScrollFocus;
			}
		}
		Pools.free(@event);
		return success;
	}

	/** Gets the actor that will receive scroll events.
	 * @return May be null. */
	public Actor? getScrollFocus () {
		return scrollFocus;
	}

	public IBatch getBatch () {
		return batch;
	}

	public Viewport getViewport () {
		return viewport;
	}

	public void setViewport (Viewport viewport) {
		this.viewport = viewport;
	}

	/** The viewport's world width. */
	public float getWidth () {
		return viewport.getWorldWidth();
	}

	/** The viewport's world height. */
	public float getHeight () {
		return viewport.getWorldHeight();
	}

	/** The viewport's camera. */
	public Camera getCamera () {
		return viewport.getCamera();
	}

	/** Returns the root group which holds all actors in the stage. */
	public Group getRoot () {
		return root;
	}

	/** Replaces the root group. This can be useful, for example, to subclass the root group to be notified by
	 * {@link Group#childrenChanged()}. */
	public void setRoot (Group root) {
		if (root.parent != null) root.parent.removeActor(root, false);
		this.root = root;
		root.setParent(null);
		root.setStage(this);
	}

	/** Returns the {@link Actor} at the specified location in stage coordinates. Hit testing is performed in the order the actors
	 * were inserted into the stage, last inserted actors being tested first. To get stage coordinates from screen coordinates, use
	 * {@link #screenToStageCoordinates(Vector2)}.
	 * @param touchable If true, the hit detection will respect the {@link Actor#setTouchable(Touchable) touchability}.
	 * @return May be null if no actor was hit. */
	public Actor? hit (float stageX, float stageY, bool touchable) {
		root.parentToLocalCoordinates(tempCoords.set(stageX, stageY));
		return root.hit(tempCoords.x, tempCoords.y, touchable);
	}

	/** Transforms the screen coordinates to stage coordinates.
	 * @param screenCoords Input screen coordinates and output for resulting stage coordinates. */
	public Vector2 screenToStageCoordinates (Vector2 screenCoords) {
		viewport.unproject(screenCoords);
		return screenCoords;
	}

	/** Transforms the stage coordinates to screen coordinates.
	 * @param stageCoords Input stage coordinates and output for resulting screen coordinates. */
	public Vector2 stageToScreenCoordinates (Vector2 stageCoords) {
		viewport.project(stageCoords);
		stageCoords.y = Gdx.graphics.getHeight() - stageCoords.y;
		return stageCoords;
	}

	/** Transforms the coordinates to screen coordinates. The coordinates can be anywhere in the stage since the transform matrix
	 * describes how to convert them. The transform matrix is typically obtained from {@link Batch#getTransformMatrix()} during
	 * {@link Actor#draw(Batch, float)}.
	 * @see Actor#localToStageCoordinates(Vector2) */
	public Vector2 toScreenCoordinates (Vector2 coords, Matrix4 transformMatrix) {
		return viewport.toScreenCoordinates(coords, transformMatrix);
	}

	/** Calculates window scissor coordinates from local coordinates using the batch's current transformation matrix.
	 * @see ScissorStack#calculateScissors(Camera, float, float, float, float, Matrix4, Rectangle, Rectangle) */
	public void calculateScissors (Rectangle localRect, Rectangle scissorRect) {
		Matrix4 transformMatrix;
		if (debugShapes != null && debugShapes.isDrawing())
			transformMatrix = debugShapes.getTransformMatrix();
		else
			transformMatrix = batch.getTransformMatrix();
		viewport.calculateScissors(transformMatrix, localRect, scissorRect);
	}

	/** If true, any actions executed during a call to {@link #act()}) will result in a call to
	 * {@link Graphics#requestRendering()}. Widgets that animate or otherwise require additional rendering may check this setting
	 * before calling {@link Graphics#requestRendering()}. Default is true. */
	public void setActionsRequestRendering (bool actionsRequestRendering) {
		this.actionsRequestRendering = actionsRequestRendering;
	}

	public bool getActionsRequestRendering () {
		return actionsRequestRendering;
	}

	/** The default color that can be used by actors to draw debug lines. */
	public Color getDebugColor () {
		return debugColor;
	}

	/** If true, debug lines are shown for actors even when {@link Actor#isVisible()} is false. */
	public void setDebugInvisible (bool debugInvisible) {
		this.debugInvisible = debugInvisible;
	}

	/** If true, debug lines are shown for all actors. */
	public void setDebugAll (bool debugAll) {
		if (this.debugAll == debugAll) return;
		this.debugAll = debugAll;
		if (debugAll)
			debug = true;
		else
			root.setDebug(false, true);
	}

	public bool isDebugAll () {
		return debugAll;
	}

	/** If true, debug is enabled only for the actor under the mouse. Can be combined with {@link #setDebugAll(boolean)}. */
	public void setDebugUnderMouse (bool debugUnderMouse) {
		if (this.debugUnderMouse == debugUnderMouse) return;
		this.debugUnderMouse = debugUnderMouse;
		if (debugUnderMouse)
			debug = true;
		else
			root.setDebug(false, true);
	}

	/** If true, debug is enabled only for the parent of the actor under the mouse. Can be combined with
	 * {@link #setDebugAll(boolean)}. */
	public void setDebugParentUnderMouse (bool debugParentUnderMouse) {
		if (this.debugParentUnderMouse == debugParentUnderMouse) return;
		this.debugParentUnderMouse = debugParentUnderMouse;
		if (debugParentUnderMouse)
			debug = true;
		else
			root.setDebug(false, true);
	}

	/** If not {@link Debug#none}, debug is enabled only for the first ascendant of the actor under the mouse that is a table. Can
	 * be combined with {@link #setDebugAll(boolean)}.
	 * @param debugTableUnderMouse May be null for {@link Debug#none}. */
	public void setDebugTableUnderMouse (Table.Debug? debugTableUnderMouse) {
		if (debugTableUnderMouse == null) debugTableUnderMouse = Table.Debug.none;
		if (this.debugTableUnderMouse == debugTableUnderMouse) return;
		this.debugTableUnderMouse = debugTableUnderMouse ?? throw new Exception("Debug table was empty. -RP");
		if (debugTableUnderMouse != Table.Debug.none)
			debug = true;
		else
			root.setDebug(false, true);
	}

	/** If true, debug is enabled only for the first ascendant of the actor under the mouse that is a table. Can be combined with
	 * {@link #setDebugAll(boolean)}. */
	public void setDebugTableUnderMouse (bool debugTableUnderMouse) {
		setDebugTableUnderMouse(debugTableUnderMouse ? Table.Debug.all : Table.Debug.none);
	}

	public void dispose () {
		clear();
		if (ownsBatch) batch.dispose();
		if (debugShapes != null) debugShapes.dispose();
	}

	/** Check if screen coordinates are inside the viewport's screen area. */
	protected bool isInsideViewport (int screenX, int screenY) {
		int x0 = viewport.getScreenX();
		int x1 = x0 + viewport.getScreenWidth();
		int y0 = viewport.getScreenY();
		int y1 = y0 + viewport.getScreenHeight();
		screenY = Gdx.graphics.getHeight() - 1 - screenY;
		return screenX >= x0 && screenX < x1 && screenY >= y0 && screenY < y1;
	}

	/** Internal class for managing touch focus. Public only for GWT.
	 * @author Nathan Sweet */
	public sealed class TouchFocus : IPoolable {
		internal IEventListener listener;
		internal Actor listenerActor, target;
		internal int pointer, button;

		public void reset () {
			listenerActor = null;
			listener = null;
			target = null;
		}
	}
}
}
