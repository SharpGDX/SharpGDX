using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Utils
{
	/** An array that allows modification during iteration. Guarantees that array entries provided by {@link #begin()} between indexes
 * 0 and {@link #size} at the time begin was called will not be modified until {@link #end()} is called. If modification of the
 * SnapshotArray occurs between begin/end, the backing array is copied prior to the modification, ensuring that the backing array
 * that was returned by {@link #begin()} is unaffected. To avoid allocation, an attempt is made to reuse any extra array created
 * as a result of this copy on subsequent copies.
 * <p>
 * Note that SnapshotArray is not for thread safety, only for modification during iteration.
 * <p>
 * It is suggested iteration be done in this specific way:
 * 
 * <pre>
 * SnapshotArray<Item> array = new SnapshotArray();
 * // ...
 * Object[] items = array.begin();
 * for (int i = 0, n = array.size; i &lt; n; i++) {
 * 	Item item = (Item)items[i];
 * 	// ...
 * }
 * array.end();
 * </pre>
 * 
 * @author Nathan Sweet */
	public class SnapshotArray<T> : Array<T>
	{
	private T[] snapshot, recycled;
	private int snapshots;
	public SnapshotArray()
	{
	}
	public SnapshotArray(Array<T> array)
	: base(array)
		{
		
	}

	public SnapshotArray(bool ordered, int capacity, Type arrayType)
	: base(ordered, capacity, arrayType)
		{
		
	}

	public SnapshotArray(bool ordered, int capacity)
	: base(ordered, capacity)
		{
	}

	public SnapshotArray(bool ordered, T[] array, int startIndex, int count)
	: base(ordered, array, startIndex, count)
		{
		
	}

	public SnapshotArray(Type arrayType)
	: base(arrayType)
		{
		
	}

	public SnapshotArray(int capacity)
	: base(capacity)
		{
		
	}

	public SnapshotArray(T[] array)
	: base(array)
		{
		
	}

	/** Returns the backing array, which is guaranteed to not be modified before {@link #end()}. */
	public T[] begin()
	{
		modified();
		snapshot = items;
		snapshots++;
		return items;
	}

	/** Releases the guarantee that the array returned by {@link #begin()} won't be modified. */
	public void end()
	{
		snapshots = Math.Max(0, snapshots - 1);
		if (snapshot == null) return;
		if (snapshot != items && snapshots == 0)
		{
			// The backing array was copied, keep around the old array.
			recycled = snapshot;
			for (int i = 0, n = recycled.Length; i < n; i++)
				recycled[i] = default;
		}
		snapshot = null;
	}

	private void modified()
	{
		if (snapshot == null || snapshot != items) return;
		// Snapshot is in use, copy backing array to recycled array or create new backing array.
		if (recycled != null && recycled.Length >= size)
		{
			Array.Copy(items, 0, recycled, 0, size);
			items = recycled;
			recycled = null;
		}
		else
			resize(items.Length);
	}

		public override void set(int index, T value)
	{
		modified();
		base.set(index, value);
	}

		public override void insert(int index, T value)
	{
		modified();
		base.insert(index, value);
	}

		public override void insertRange(int index, int count)
	{
		modified();
		base.insertRange(index, count);
	}

		public override void swap(int first, int second)
	{
		modified();
		base.swap(first, second);
	}

		public override bool removeValue(T value, bool identity)
	{
		modified();
		return base.removeValue(value, identity);
	}

		public override T removeIndex(int index)
	{
		modified();
		return base.removeIndex(index);
	}
		public override void removeRange(int start, int end)
	{
		modified();
		base.removeRange(start, end);
	}

		public override bool removeAll(Array< T> array, bool identity)
	{
		modified();
		return base.removeAll(array, identity);
	}

		public override T pop()
	{
		modified();
		return base.pop();
	}

		public override void clear()
	{
		modified();
		base.clear();
	}

		public override void sort()
	{
		modified();
		base.sort();
	}

		public override void sort(IComparer< T> comparator)
	{
		modified();
		base.sort(comparator);
	}

		public override void reverse()
	{
		modified();
		base.reverse();
	}

		public override void shuffle()
	{
		modified();
		base.shuffle();
	}

		public override void truncate(int newSize)
	{
		modified();
		base.truncate(newSize);
	}

		public override T[] setSize(int newSize)
	{
		modified();
		return base.setSize(newSize);
	}

	/** @see #SnapshotArray(Object[]) */
	static public SnapshotArray<T> with(T[]array)
	{
		return new SnapshotArray<T>(array);
	}
}
}
