using System;
using SharpGDX.Shims;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** A selection that supports range selection by knowing about the array of items being selected.
 * @author Nathan Sweet */
public class ArraySelection<T> : Selection<T> {
	private Array<T> array;
	private bool rangeSelect = true;
	private T rangeStart;

	public ArraySelection (Array<T> array) {
		this.array = array;
	}

		public override void choose (T item) {
		if (item == null) throw new IllegalArgumentException("item cannot be null.");
		if (_isDisabled) return;

		if (!rangeSelect || !multiple) {
			base.choose(item);
			return;
		}

		if (selected.size > 0 && UIUtils.shift()) {
			int rangeStartIndex = rangeStart == null ? -1 : array.indexOf(rangeStart, false);
			if (rangeStartIndex != -1) {
				T oldRangeStart = rangeStart;
				snapshot();
				// Select new range.
				int start = rangeStartIndex, end = array.indexOf(item, false);
				if (start > end) {
					int temp = end;
					end = start;
					start = temp;
				}
				if (!UIUtils.ctrl()) selected.clear(8);
				for (int i = start; i <= end; i++)
					selected.add(array.get(i));
				if (fireChangeEvent())
					revert();
				else
					changed();
				rangeStart = oldRangeStart;
				cleanup();
				return;
			}
		}
		base.choose(item);
		rangeStart = item;
	}

	/** Called after the selection changes, clears the range start item. */
	protected override void changed () {
		rangeStart = default;
	}

	public bool getRangeSelect () {
		return rangeSelect;
	}

	public void setRangeSelect (bool rangeSelect) {
		this.rangeSelect = rangeSelect;
	}

	/** Removes objects from the selection that are no longer in the items array. If {@link #getRequired()} is true and there is no
	 * selected item, the first item is selected. */
	public void validate () {
		Array<T> array = this.array;
		if (array.size == 0) {
			clear();
			return;
		}
		bool changed = false;
		for (var iter = items().iterator(); iter.MoveNext();) {
			T selected = iter.next();
			if (!array.contains(selected, false)) {
				iter.remove();
				changed = true;
			}
		}
		if (required && selected.size == 0)
			set(array.first());
		else if (changed) //
			this.changed();
	}
}
}
