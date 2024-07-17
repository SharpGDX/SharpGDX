using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Listener for {@link FocusEvent}.
 * @author Nathan Sweet */
abstract public class FocusListener : IEventListener {
	public bool handle (Event @event) {
		if (!(@event is FocusEvent)) return false;
		FocusEvent focusEvent = (FocusEvent) @event;
		switch (focusEvent.getType()) {
		case FocusEvent.Type.keyboard:
			keyboardFocusChanged(focusEvent, @event.getTarget(), focusEvent.isFocused());
			break;
		case FocusEvent.Type.scroll:
			scrollFocusChanged(focusEvent, @event.getTarget(), focusEvent.isFocused());
			break;
		}
		return false;
	}

	/** @param actor The event target, which is the actor that emitted the focus event. */
	public virtual void keyboardFocusChanged (FocusEvent @event, Actor actor, bool focused) {
	}

	/** @param actor The event target, which is the actor that emitted the focus event. */
	public virtual void scrollFocusChanged (FocusEvent @event, Actor actor, bool focused) {
	}

	/** Fired when an actor gains or loses keyboard or scroll focus. Can be cancelled to prevent losing or gaining focus.
	 * @author Nathan Sweet */
	public class FocusEvent : Event {
		private bool focused;
		private Type type;
		private Actor? relatedActor;

		public override void reset () {
			base.reset();
			relatedActor = null;
		}

		public bool isFocused () {
			return focused;
		}

		public void setFocused (bool focused) {
			this.focused = focused;
		}

		public Type getType () {
			return type;
		}

		public void setType (Type focusType) {
			this.type = focusType;
		}

		/** The actor related to the event. When focus is lost, this is the new actor being focused, or null. When focus is gained,
		 * this is the previous actor that was focused, or null. */
		public Actor? getRelatedActor () {
			return relatedActor;
		}

		/** @param relatedActor May be null. */
		public void setRelatedActor (Actor? relatedActor) {
			this.relatedActor = relatedActor;
		}

		/** @author Nathan Sweet */
		public enum Type {
			keyboard, scroll
		}
	}
}
}
