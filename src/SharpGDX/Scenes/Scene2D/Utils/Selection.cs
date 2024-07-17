using System;
using System.Collections;
using SharpGDX;
using SharpGDX.Shims;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Manages selected objects. Optionally fires a {@link ChangeEvent} on an actor. Selection changes can be vetoed via
 * {@link ChangeEvent#cancel()}.
 * @author Nathan Sweet */
public class Selection<T> : IDisableable, IEnumerable<T> {
	private Actor? actor;
	protected readonly OrderedSet<T> selected = new ();
	private readonly OrderedSet<T> old = new ();
	protected bool _isDisabled;
	private bool toggle;
	protected bool multiple;
	protected bool required;
	private bool programmaticChangeEvents = true;
	T? lastSelected;

	/** @param actor An actor to fire {@link ChangeEvent} on when the selection changes, or null. */
	public void setActor ( Actor? actor) {
		this.actor = actor;
	}

	/** Selects or deselects the specified item based on how the selection is configured, whether ctrl is currently pressed, etc.
	 * This is typically invoked by user interaction. */
	public virtual void choose (T item) {
		if (item == null) throw new IllegalArgumentException("item cannot be null.");
		if (_isDisabled) return;
		snapshot();
		try {
			if ((toggle || UIUtils.ctrl()) && selected.contains(item)) {
				if (required && selected.size == 1) return;
				selected.remove(item);
				lastSelected = default;
			} else {
				bool modified = false;
				if (!multiple || (!toggle && !UIUtils.ctrl())) {
					if (selected.size == 1 && selected.contains(item)) return;
					modified = selected.size > 0;
					selected.clear(8);
				}
				if (!selected.add(item) && !modified) return;
				lastSelected = item;
			}
			if (fireChangeEvent())
				revert();
			else
				changed();
		} finally {
			cleanup();
		}
	}
		
	public bool notEmpty () {
		return selected.size > 0;
	}

	public bool isEmpty () {
		return selected.size == 0;
	}

	public int size () {
		return selected.size;
	}

	public OrderedSet<T> items () {
		return selected;
	}

	/** Returns the first selected item, or null. */
	public  T? first () {
		return selected.size == 0 ? default : selected.first();
	}

	protected void snapshot () {
		old.clear(selected.size);
		old.addAll(selected);
	}

	protected void revert () {
		selected.clear(old.size);
		selected.addAll(old);
	}

	protected void cleanup () {
		old.clear(32);
	}

	/** Sets the selection to only the specified item. */
	public void set (T item) {
		if (item == null) throw new IllegalArgumentException("item cannot be null.");
		if (selected.size == 1 && ReferenceEquals(selected.first() , item)) return;
		snapshot();
		selected.clear(8);
		selected.add(item);
		if (programmaticChangeEvents && fireChangeEvent())
			revert();
		else {
			lastSelected = item;
			changed();
		}
		cleanup();
	}

	public void setAll (Array<T> items) {
		bool added = false;
		snapshot();
		lastSelected = default;
		selected.clear(items.size);
		for (int i = 0, n = items.size; i < n; i++) {
			T item = items.get(i);
			if (item == null) throw new IllegalArgumentException("item cannot be null.");
			if (selected.add(item)) added = true;
		}
		if (added) {
			if (programmaticChangeEvents && fireChangeEvent())
				revert();
			else if (items.size > 0) {
				lastSelected = items.peek();
				changed();
			}
		}
		cleanup();
	}

	/** Adds the item to the selection. */
	public void add (T item) {
		if (item == null) throw new IllegalArgumentException("item cannot be null.");
		if (!selected.add(item)) return;
		if (programmaticChangeEvents && fireChangeEvent())
			selected.remove(item);
		else {
			lastSelected = item;
			changed();
		}
	}

	public void addAll (Array<T> items) {
		bool added = false;
		snapshot();
		for (int i = 0, n = items.size; i < n; i++) {
			T item = items.get(i);
			if (item == null) throw new IllegalArgumentException("item cannot be null.");
			if (selected.add(item)) added = true;
		}
		if (added) {
			if (programmaticChangeEvents && fireChangeEvent())
				revert();
			else {
				lastSelected = items.peek();
				changed();
			}
		}
		cleanup();
	}

	public void remove (T item) {
		if (item == null) throw new IllegalArgumentException("item cannot be null.");
		if (!selected.remove(item)) return;
		if (programmaticChangeEvents && fireChangeEvent())
			selected.add(item);
		else {
			lastSelected = default;
			changed();
		}
	}

	public void removeAll (Array<T> items) {
		bool removed = false;
		snapshot();
		for (int i = 0, n = items.size; i < n; i++) {
			T item = items.get(i);
			if (item == null) throw new IllegalArgumentException("item cannot be null.");
			if (selected.remove(item)) removed = true;
		}
		if (removed) {
			if (programmaticChangeEvents && fireChangeEvent())
				revert();
			else {
				lastSelected = default;
				changed();
			}
		}
		cleanup();
	}

	public void clear () {
		if (selected.size == 0) {
			lastSelected = default;
			return;
		}
		snapshot();
		selected.clear(8);
		if (programmaticChangeEvents && fireChangeEvent())
			revert();
		else {
			lastSelected = default;
			changed();
		}
		cleanup();
	}

	/** Called after the selection changes. The default implementation does nothing. */
	protected virtual void changed () {
	}

	/** Fires a change event on the selection's actor, if any. Called internally when the selection changes, depending on
	 * {@link #setProgrammaticChangeEvents(boolean)}.
	 * @return true if the change should be undone. */
	public virtual bool fireChangeEvent () {
		if (actor == null) return false;
		ChangeListener.ChangeEvent changeEvent = Pools.obtain<ChangeListener.ChangeEvent>(typeof(ChangeListener.ChangeEvent));
		try {
			return actor.fire(changeEvent);
		} finally {
			Pools.free(changeEvent);
		}
	}

	/** @param item May be null (returns false). */
	public bool contains ( T? item) {
		if (item == null) return false;
		return selected.contains(item);
	}

	/** Makes a best effort to return the last item selected, else returns an arbitrary item or null if the selection is empty. */
	public  T? getLastSelected () {
		if (lastSelected != null) {
			return lastSelected;
		} else if (selected.size > 0) {
			return selected.first();
		}
		return default;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<T> GetEnumerator () {
		return selected.iterator();
	}

	public Array<T> toArray () {
		return selected.iterator().toArray();
	}

	public Array<T> toArray (Array<T> array) {
		return selected.iterator().toArray(array);
	}

	/** If true, prevents {@link #choose(Object)} from changing the selection. Default is false. */
	public void setDisabled (bool isDisabled) {
		this._isDisabled = isDisabled;
	}

	public bool isDisabled () {
		return _isDisabled;
	}

	public bool getToggle () {
		return toggle;
	}

	/** If true, prevents {@link #choose(Object)} from clearing the selection. Default is false. */
	public void setToggle (bool toggle) {
		this.toggle = toggle;
	}

	public bool getMultiple () {
		return multiple;
	}

	/** If true, allows {@link #choose(Object)} to select multiple items. Default is false. */
	public void setMultiple (bool multiple) {
		this.multiple = multiple;
	}

	public bool getRequired () {
		return required;
	}

	/** If true, prevents {@link #choose(Object)} from selecting none. Default is false. */
	public void setRequired (bool required) {
		this.required = required;
	}

	/** If false, only {@link #choose(Object)} will fire a change event. Default is true. */
	public void setProgrammaticChangeEvents (bool programmaticChangeEvents) {
		this.programmaticChangeEvents = programmaticChangeEvents;
	}

	public override String ToString () {
		return selected.ToString();
	}
}
}
