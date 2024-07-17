using System;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** An array that queues removal during iteration until the iteration has completed. Queues any removals done after
 * {@link #begin()} is called to occur once {@link #end()} is called. This can allow code out of your control to remove items
 * without affecting iteration. Between begin and end, most mutator methods will throw IllegalStateException. Only
 * {@link #removeIndex(int)}, {@link #removeValue(Object, boolean)}, {@link #removeRange(int, int)}, {@link #clear()}, and add
 * methods are allowed.
 * <p>
 * Note that DelayedRemovalArray is not for thread safety, only for removal during iteration.
 * <p>
 * Code using this class must not rely on items being removed immediately. Consider using {@link SnapshotArray} if this is a
 * problem.
 * @author Nathan Sweet */
public class DelayedRemovalArray<T> : Array<T> {
	private int iterating;
	private IntArray _remove = new IntArray(0);
	private int _clear;

	public DelayedRemovalArray () {
		}

	public DelayedRemovalArray (Array<T> array) 
	: base(array)
	{
		
	}

	public DelayedRemovalArray (bool ordered, int capacity, Type arrayType) 
	: base(ordered, capacity, arrayType)
	{
		
	}

	public DelayedRemovalArray (bool ordered, int capacity) 
	: base(ordered, capacity)
	{
		
	}

	public DelayedRemovalArray (bool ordered, T[] array, int startIndex, int count) 
	: base(ordered, array, startIndex, count)
	{
		
	}

	public DelayedRemovalArray (Type arrayType) 
	: base(arrayType)
	{
		
	}

	public DelayedRemovalArray (int capacity) 
	: base(capacity)
	{
		
	}

	public DelayedRemovalArray (T[] array) 
	: base(array)
	{
		
	}

	public void begin () {
		iterating++;
	}

	public void end () {
		if (iterating == 0) throw new IllegalStateException("begin must be called before end.");
		iterating--;
		if (iterating == 0) {
			if (_clear > 0 && _clear == size) {
				_remove.clear();
				clear();
			} else {
				for (int i = 0, n = _remove.size; i < n; i++) {
					int index = _remove.pop();
					if (index >= _clear) removeIndex(index);
				}
				for (int i = _clear - 1; i >= 0; i--)
					removeIndex(i);
			}
			_clear = 0;
		}
	}

	private void remove (int index) {
		if (index < _clear) return;
		for (int i = 0, n = _remove.size; i < n; i++) {
			int removeIndex = _remove.get(i);
			if (index == removeIndex) return;
			if (index < removeIndex) {
				_remove.insert(i, index);
				return;
			}
		}
		_remove.add(index);
	}

		public override bool removeValue (T value, bool identity) {
		if (iterating > 0) {
			int index = indexOf(value, identity);
			if (index == -1) return false;
			remove(index);
			return true;
		}
		return base.removeValue(value, identity);
	}

		public override T removeIndex (int index) {
		if (iterating > 0) {
			remove(index);
			return get(index);
		}
		return base.removeIndex(index);
	}

	public override void removeRange (int start, int end) {
		if (iterating > 0) {
			for (int i = end; i >= start; i--)
				remove(i);
		} else
			base.removeRange(start, end);
	}

		public override void clear () {
		if (iterating > 0) {
			_clear = size;
			return;
		}
		base.clear();
	}

		public override void set (int index, T value) {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.set(index, value);
	}

		public override void insert (int index, T value) {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.insert(index, value);
	}

		public override void insertRange (int index, int count) {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.insertRange(index, count);
	}

		public override void swap (int first, int second) {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.swap(first, second);
	}

		public override T pop () {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		return base.pop();
	}

		public override void sort () {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.sort();
	}

		public override void sort (IComparer<T> comparator) {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.sort(comparator);
	}

		public override void reverse () {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.reverse();
	}

		public override void shuffle () {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.shuffle();
	}

		public override void truncate (int newSize) {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		base.truncate(newSize);
	}

		public override T[] setSize (int newSize) {
		if (iterating > 0) throw new IllegalStateException("Invalid between begin/end.");
		return base.setSize(newSize);
	}

	/** @see #DelayedRemovalArray(Object[]) */
	static public DelayedRemovalArray<T> with (T[] array) {
		return new DelayedRemovalArray<T>(array);
	}
}
}
