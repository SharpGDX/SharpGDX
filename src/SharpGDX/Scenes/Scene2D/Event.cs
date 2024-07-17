using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D
{
	/** The base class for all events.
 * <p>
 * By default an event will "bubble" up through an actor's parent's handlers (see {@link #setBubbles(boolean)}).
 * <p>
 * An actor's capture listeners can {@link #stop()} an event to prevent child actors from seeing it.
 * <p>
 * An Event may be marked as "handled" which will end its propagation outside of the Stage (see {@link #handle()}). The default
 * {@link Actor#fire(Event)} will mark events handled if an {@link EventListener} returns true.
 * <p>
 * A cancelled event will be stopped and handled. Additionally, many actors will undo the side-effects of a canceled event. (See
 * {@link #cancel()}.)
 *
 * @see InputEvent
 * @see Actor#fire(Event) */
public class Event : IPoolable {
	private Stage stage;
	private Actor targetActor;
	private Actor listenerActor;
	private bool capture; // true means event occurred during the capture phase
	private bool bubbles = true; // true means propagate to target's parents
	private bool handled; // true means the event was handled (the stage will eat the input)
	private bool stopped; // true means event propagation was stopped
	private bool cancelled; // true means propagation was stopped and any action that this event would cause should not happen

	/** Marks this event as handled. This does not affect event propagation inside scene2d, but causes the {@link Stage}
	 * {@link InputProcessor} methods to return true, which will eat the event so it is not passed on to the application under the
	 * stage. */
	public void handle () {
		handled = true;
	}

	/** Marks this event cancelled. This {@link #handle() handles} the event and {@link #stop() stops} the event propagation. It
	 * also cancels any default action that would have been taken by the code that fired the event. Eg, if the event is for a
	 * checkbox being checked, cancelling the event could uncheck the checkbox. */
	public void cancel () {
		cancelled = true;
		stopped = true;
		handled = true;
	}

	/** Marks this event has being stopped. This halts event propagation. Any other listeners on the {@link #getListenerActor()
	 * listener actor} are notified, but after that no other listeners are notified. */
	public void stop () {
		stopped = true;
	}

	public virtual void reset () {
		stage = null;
		targetActor = null;
		listenerActor = null;
		capture = false;
		bubbles = true;
		handled = false;
		stopped = false;
		cancelled = false;
	}

	/** Returns the actor that the event originated from. */
	public Actor getTarget () {
		return targetActor;
	}

	public void setTarget (Actor targetActor) {
		this.targetActor = targetActor;
	}

	/** Returns the actor that this listener is attached to. */
	public Actor getListenerActor () {
		return listenerActor;
	}

	public void setListenerActor (Actor listenerActor) {
		this.listenerActor = listenerActor;
	}

	public bool getBubbles () {
		return bubbles;
	}

	/** If true, after the event is fired on the target actor, it will also be fired on each of the parent actors, all the way to
	 * the root. */
	public void setBubbles (bool bubbles) {
		this.bubbles = bubbles;
	}

	/** {@link #handle()} */
	public bool isHandled () {
		return handled;
	}

	/** @see #stop() */
	public bool isStopped () {
		return stopped;
	}

	/** @see #cancel() */
	public bool isCancelled () {
		return cancelled;
	}

	public void setCapture (bool capture) {
		this.capture = capture;
	}

	/** If true, the event was fired during the capture phase.
	 * @see Actor#fire(Event) */
	public bool isCapture () {
		return capture;
	}

	public void setStage (Stage stage) {
		this.stage = stage;
	}

	/** The stage for the actor the event was fired on. */
	public Stage getStage () {
		return stage;
	}
}
}
