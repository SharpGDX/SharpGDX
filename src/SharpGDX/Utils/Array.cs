﻿using System.Collections;
using SharpGDX.Shims;
using System.Text;
using SharpGDX.Utils;
using System.Drawing;
using System.Security.Claims;
using System;
using SharpGDX.Mathematics;
using SharpGDX.Utils.Reflect;

namespace SharpGDX.Utils
{
	/** A resizable, ordered or unordered array of objects. If unordered, this class avoids a memory copy when removing elements (the
 * last element is moved to the removed element's position).
 * @author Nathan Sweet */
	[Obsolete("Use List<T> where applicable.")]
	public class Array<T> : IEnumerable<T>
	{
	/** Provides direct access to the underlying array. If the Array's generic type is not Object, this field may only be accessed
	 * if the {@link Array#Array(boolean, int, Class)} constructor was used. */
	public T[] items;

	public int size;
	public bool ordered;

	private ArrayIterable iterable;
	private IPredicate<T>.PredicateIterable<T> predicateIterable;

	/** Creates an ordered array with a capacity of 16. */
	public Array()
	: this(true, 16)
		{
	}

	/** Creates an ordered array with the specified capacity. */
	public Array(int capacity)
	: this(true, capacity)
		{
	}

	/** @param ordered If false, methods that remove elements may change the order of other elements in the array, which avoids a
	 *           memory copy.
	 * @param capacity Any elements added beyond this will cause the backing array to be grown. */
	public Array(bool ordered, int capacity)
	{
		this.ordered = ordered;
		items = (T[])new T[capacity];
	}

	/** Creates a new array with {@link #items} of the specified type.
	 * @param ordered If false, methods that remove elements may change the order of other elements in the array, which avoids a
	 *           memory copy.
	 * @param capacity Any elements added beyond this will cause the backing array to be grown. */
	public Array(bool ordered, int capacity, Type arrayType)
	{
		// TODO: Remove type as it is not needed in C#
			this.ordered = ordered;
		items = (T[])ArrayReflection.newInstance(arrayType, capacity);
	}

	/** Creates an ordered array with {@link #items} of the specified type and a capacity of 16. */
	public Array(Type arrayType)
	: this(true, 16, arrayType)
		{
	}

	/** Creates a new array containing the elements in the specified array. The new array will have the same type of backing array
	 * and will be ordered if the specified array is ordered. The capacity is set to the number of elements, so any subsequent
	 * elements added will cause the backing array to be grown. */
	public Array(Array<T> array)
	: this(array.ordered, array.size, array.items.GetType().GetGenericTypeDefinition())
		{
		size = array.size;
		Array.Copy(array.items, 0, items, 0, size);
	}

	/** Creates a new ordered array containing the elements in the specified array. The new array will have the same type of
	 * backing array. The capacity is set to the number of elements, so any subsequent elements added will cause the backing array
	 * to be grown. */
	public Array(T[] array)
	: this(true, array, 0, array.Length)
		{
	}

	/** Creates a new array containing the elements in the specified array. The new array will have the same type of backing array.
	 * The capacity is set to the number of elements, so any subsequent elements added will cause the backing array to be grown.
	 * @param ordered If false, methods that remove elements may change the order of other elements in the array, which avoids a
	 *           memory copy. */
	public Array(bool ordered, T[] array, int start, int count)
	: this(ordered, count, array.GetType().GetElementType())
		{
		size = count;
		Array.Copy(array, start, items, 0, size);
	}

	public void add(T value)
	{
		T[] items = this.items;
		if (size == items.Length) items = resize(Math.Max(8, (int)(size * 1.75f)));
		items[size++] = value;
	}

	public void add(T value1, T value2)
	{
		T[] items = this.items;
		if (size + 1 >= items.Length) items = resize(Math.Max(8, (int)(size * 1.75f)));
		items[size] = value1;
		items[size + 1] = value2;
		size += 2;
	}

	public void add(T value1, T value2, T value3)
	{
		T[] items = this.items;
		if (size + 2 >= items.Length) items = resize(Math.Max(8, (int)(size * 1.75f)));
		items[size] = value1;
		items[size + 1] = value2;
		items[size + 2] = value3;
		size += 3;
	}
	public void add(T value1, T value2, T value3, T value4)
	{
		T[] items = this.items;
		if (size + 3 >= items.Length) items = resize(Math.Max(8, (int)(size * 1.8f))); // 1.75 isn't enough when size=5.
		items[size] = value1;
		items[size + 1] = value2;
		items[size + 2] = value3;
		items[size + 3] = value4;
		size += 4;
	}

	public void addAll(Array< T> array)
	{
		addAll(array.items, 0, array.size);
	}

	public void addAll(Array< T> array, int start, int count)
	{
		if (start + count > array.size)
			throw new IllegalArgumentException("start + count must be <= size: " + start + " + " + count + " <= " + array.size);
		addAll(array.items, start, count);
	}

	public void addAll(T[]array)
	{
		addAll(array, 0, array.Length);
	}

	public void addAll(T[] array, int start, int count)
	{
		T[] items = this.items;
		int sizeNeeded = size + count;
		if (sizeNeeded > items.Length) items = resize(Math.Max(Math.Max(8, sizeNeeded), (int)(size * 1.75f)));
		Array.Copy(array, start, items, size, count);
		size = sizeNeeded;
	}

	public T get(int index)
	{
		if (index >= size) throw new IndexOutOfBoundsException("index can't be >= size: " + index + " >= " + size);
		return items[index];
	}

	public virtual void set(int index, T value)
	{
		if (index >= size) throw new IndexOutOfBoundsException("index can't be >= size: " + index + " >= " + size);
		items[index] = value;
	}

	public virtual void insert(int index, T value)
	{
		if (index > size) throw new IndexOutOfBoundsException("index can't be > size: " + index + " > " + size);
		T[] items = this.items;
		if (size == items.Length) items = resize(Math.Max(8, (int)(size * 1.75f)));
		if (ordered)
			Array.Copy(items, index, items, index + 1, size - index);
		else
			items[size] = items[index];
		size++;
		items[index] = value;
	}

	/** Inserts the specified number of items at the specified index. The new items will have values equal to the values at those
	 * indices before the insertion. */
	public virtual void insertRange(int index, int count)
	{
		if (index > size) throw new IndexOutOfBoundsException("index can't be > size: " + index + " > " + size);
		int sizeNeeded = size + count;
		if (sizeNeeded > items.Length) items = resize(Math.Max(Math.Max(8, sizeNeeded), (int)(size * 1.75f)));
		Array.Copy(items, index, items, index + count, size - index);
		size = sizeNeeded;
	}

	public virtual void swap(int first, int second)
	{
		if (first >= size) throw new IndexOutOfBoundsException("first can't be >= size: " + first + " >= " + size);
		if (second >= size) throw new IndexOutOfBoundsException("second can't be >= size: " + second + " >= " + size);
		T[] items = this.items;
		T firstValue = items[first];
		items[first] = items[second];
		items[second] = firstValue;
	}

	/** Returns true if this array contains the specified value.
	 * @param value May be null.
	 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used. */
	public bool contains(T? value, bool identity)
	{
		T[] items = this.items;
		int i = size - 1;
		if (identity || value == null)
		{
			while (i >= 0)
				if (ReferenceEquals(items[i--] , value)) return true;
		}
		else
		{
			while (i >= 0)
				if (value.Equals(items[i--])) return true;
		}
		return false;
	}

	/** Returns true if this array contains all the specified values.
	 * @param values May contains nulls.
	 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used. */
	public bool containsAll(Array<T> values, bool identity)
	{
		T[] items = values.items;
		for (int i = 0, n = values.size; i < n; i++)
			if (!contains(items[i], identity)) return false;
		return true;
	}

	/** Returns true if this array contains any the specified values.
	 * @param values May contains nulls.
	 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used. */
	public bool containsAny(Array<T> values, bool identity)
	{
		T[] items = values.items;
		for (int i = 0, n = values.size; i < n; i++)
			if (contains(items[i], identity)) return true;
		return false;
	}

	/** Returns the index of first occurrence of value in the array, or -1 if no such value exists.
	 * @param value May be null.
	 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used.
	 * @return An index of first occurrence of value in array or -1 if no such value exists */
	public int indexOf(T? value, bool identity)
	{
		T[] items = this.items;
		if (identity || value == null)
		{
			for (int i = 0, n = size; i < n; i++)
				if (ReferenceEquals(items[i] , value)) return i;
		}
		else
		{
			for (int i = 0, n = size; i < n; i++)
				if (value.Equals(items[i])) return i;
		}
		return -1;
	}

	/** Returns an index of last occurrence of value in array or -1 if no such value exists. Search is started from the end of an
	 * array.
	 * @param value May be null.
	 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used.
	 * @return An index of last occurrence of value in array or -1 if no such value exists */
	public int lastIndexOf(T? value, bool identity)
	{
		T[] items = this.items;
		if (identity || value == null)
		{
			for (int i = size - 1; i >= 0; i--)
				if (ReferenceEquals(items[i] , value)) return i;
		}
		else
		{
			for (int i = size - 1; i >= 0; i--)
				if (value.Equals(items[i])) return i;
		}
		return -1;
	}

	/** Removes the first instance of the specified value in the array.
	 * @param value May be null.
	 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used.
	 * @return true if value was found and removed, false otherwise */
	public virtual bool removeValue(T? value, bool identity)
	{
		T[] items = this.items;
		if (identity || value == null)
		{
			for (int i = 0, n = size; i < n; i++)
			{
				if (ReferenceEquals(items[i] , value))
				{
					removeIndex(i);
					return true;
				}
			}
		}
		else
		{
			for (int i = 0, n = size; i < n; i++)
			{
				if (value.Equals(items[i]))
				{
					removeIndex(i);
					return true;
				}
			}
		}
		return false;
	}

	/** Removes and returns the item at the specified index. */
	public virtual T removeIndex(int index)
	{
		if (index >= size) throw new IndexOutOfBoundsException("index can't be >= size: " + index + " >= " + size);
		T[] items = this.items;
		T value = items[index];
		size--;
		if (ordered)
			Array.Copy(items, index + 1, items, index, size - index);
		else
			items[index] = items[size];
		items[size] = default;
		return value;
	}

		/** Removes the items between the specified indices, inclusive. */
		public virtual void removeRange(int start, int end)
	{
		int n = size;
		if (end >= n) throw new IndexOutOfBoundsException("end can't be >= size: " + end + " >= " + size);
		if (start > end) throw new IndexOutOfBoundsException("start can't be > end: " + start + " > " + end);
		T[] items = this.items;
		int count = end - start + 1, lastIndex = n - count;
		if (ordered)
			Array.Copy(items, start + count, items, start, n - (start + count));
		else
		{
			int i = Math.Max(lastIndex, end + 1);
			Array.Copy(items, i, items, start, n - i);
		}
		for (int i = lastIndex; i < n; i++)
			items[i] = default;
		size = n - count;
	}

	/** Removes from this array all of elements contained in the specified array.
	 * @param identity True to use ==, false to use .equals().
	 * @return true if this array was modified. */
	public virtual bool removeAll(Array<T> array, bool identity)
	{
		int size = this.size;
		int startSize = size;
		T[] items = this.items;
		if (identity)
		{
			for (int i = 0, n = array.size; i < n; i++)
			{
				T item = array.get(i);
				for (int ii = 0; ii < size; ii++)
				{
					if (ReferenceEquals(item , items[ii]))
					{
						removeIndex(ii);
						size--;
						break;
					}
				}
			}
		}
		else
		{
			for (int i = 0, n = array.size; i < n; i++)
			{
				T item = array.get(i);
				for (int ii = 0; ii < size; ii++)
				{
					if (item.Equals(items[ii]))
					{
						removeIndex(ii);
						size--;
						break;
					}
				}
			}
		}
		return size != startSize;
	}

	/** Removes and returns the last item. */
	public virtual T pop()
	{
		if (size == 0) throw new IllegalStateException("Array is empty.");
		--size;
		T item = items[size];
		items[size] = default;
		return item;
	}

	/** Returns the last item. */
	public T peek()
	{
		if (size == 0) throw new IllegalStateException("Array is empty.");
		return items[size - 1];
	}

	/** Returns the first item. */
	public T first()
	{
		if (size == 0) throw new IllegalStateException("Array is empty.");
		return items[0];
	}

	/** Returns true if the array has one or more items. */
	public bool notEmpty()
	{
		return size > 0;
	}

	/** Returns true if the array is empty. */
	public bool isEmpty()
	{
		return size == 0;
	}

	public virtual void clear()
	{
		Array.Fill(items,default, 0, size);
		size = 0;
	}

	/** Reduces the size of the backing array to the size of the actual items. This is useful to release memory when many items
	 * have been removed, or if it is known that more items will not be added.
	 * @return {@link #items} */
	public T[] shrink()
	{
		if (items.Length != size) resize(size);
		return items;
	}

	/** Increases the size of the backing array to accommodate the specified number of additional items. Useful before adding many
	 * items to avoid multiple backing array resizes.
	 * @return {@link #items} */
	public T[] ensureCapacity(int additionalCapacity)
	{
		if (additionalCapacity < 0) throw new IllegalArgumentException("additionalCapacity must be >= 0: " + additionalCapacity);
		int sizeNeeded = size + additionalCapacity;
		if (sizeNeeded > items.Length) resize(Math.Max(Math.Max(8, sizeNeeded), (int)(size * 1.75f)));
		return items;
	}

	/** Sets the array size, leaving any values beyond the current size null.
	 * @return {@link #items} */
	public virtual T[] setSize(int newSize)
	{
		truncate(newSize);
		if (newSize > items.Length) resize(Math.Max(8, newSize));
		size = newSize;
		return items;
	}

	/** Creates a new backing array with the specified size containing the current items. */
	protected T[] resize(int newSize)
	{
		T[] items = this.items;
		T[] newItems = (T[])ArrayReflection.newInstance(items.GetType().GetElementType(), newSize);
		Array.Copy(items, 0, newItems, 0, Math.Min(size, newItems.Length));
		this.items = newItems;
		return newItems;
	}

	/** Sorts this array. The array elements must implement {@link Comparable}. This method is not thread safe (uses
	 * {@link Sort#instance()}). */
	public virtual void sort()
	{
		Sort<T>.instance().sort(items, 0, size);
	}

	/** Sorts the array. This method is not thread safe (uses {@link Sort#instance()}). */
	public virtual void sort(IComparer<T> comparator)
	{
		Sort<T>.instance().sort(items, comparator, 0, size);
	}

	/** Selects the nth-lowest element from the Array according to Comparator ranking. This might partially sort the Array. The
	 * array must have a size greater than 0, or a {@link com.badlogic.gdx.utils.GdxRuntimeException} will be thrown.
	 * @see Select
	 * @param comparator used for comparison
	 * @param kthLowest rank of desired object according to comparison, n is based on ordinal numbers, not array indices. for min
	 *           value use 1, for max value use size of array, using 0 results in runtime exception.
	 * @return the value of the Nth lowest ranked object. */
	public T selectRanked(IComparer<T> comparator, int kthLowest)
	{
		if (kthLowest < 1)
		{
			throw new GdxRuntimeException("nth_lowest must be greater than 0, 1 = first, 2 = second...");
		}
		return Select<T>.instance().select(items, comparator, kthLowest, size);
	}

	/** @see Array#selectRanked(java.util.Comparator, int)
	 * @param comparator used for comparison
	 * @param kthLowest rank of desired object according to comparison, n is based on ordinal numbers, not array indices. for min
	 *           value use 1, for max value use size of array, using 0 results in runtime exception.
	 * @return the index of the Nth lowest ranked object. */
	public int selectRankedIndex(IComparer<T> comparator, int kthLowest)
	{
		if (kthLowest < 1)
		{
			throw new GdxRuntimeException("nth_lowest must be greater than 0, 1 = first, 2 = second...");
		}
		return Select<T>.instance().selectIndex(items, comparator, kthLowest, size);
	}

	public virtual void reverse()
	{
		T[] items = this.items;
		for (int i = 0, lastIndex = size - 1, n = size / 2; i < n; i++)
		{
			int ii = lastIndex - i;
			T temp = items[i];
			items[i] = items[ii];
			items[ii] = temp;
		}
	}

	public virtual void shuffle()
	{
		T[] items = this.items;
		for (int i = size - 1; i >= 0; i--)
		{
			int ii = MathUtils.random(i);
			T temp = items[i];
			items[i] = items[ii];
			items[ii] = temp;
		}
	}

	/** Returns an iterator for the items in the array. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link ArrayIterator} constructor for nested or multithreaded iteration. */
	public IEnumerator<T> GetEnumerator()
	{
		if (Collections.allocateIterators) return new ArrayIterator(this, true);
		if (iterable == null) iterable = new ArrayIterable(this);
		return iterable.GetEnumerator();
	}

	/** Returns an iterable for the selected items in the array. Remove is supported, but not between hasNext() and next().
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterable instance is returned each time this method is called.
	 * Use the {@link Predicate.PredicateIterable} constructor for nested or multithreaded iteration. */
	public IEnumerable<T> select(IPredicate<T> predicate)
	{
		if (Collections.allocateIterators) return new IPredicate<T>.PredicateIterable<T>(this, predicate);
		if (predicateIterable == null)
			predicateIterable = new IPredicate<T>.PredicateIterable<T>(this, predicate);
		else
			predicateIterable.set(this, predicate);
		return predicateIterable;
	}

	/** Reduces the size of the array to the specified size. If the array is already smaller than the specified size, no action is
	 * taken. */
	public virtual void truncate(int newSize)
	{
		if (newSize < 0) throw new IllegalArgumentException("newSize must be >= 0: " + newSize);
		if (size <= newSize) return;
		for (int i = newSize; i < size; i++)
			items[i] = default;
		size = newSize;
	}

	/** Returns a random item from the array, or null if the array is empty. */
	public T? random()
	{
		if (size == 0) return default;
		return items[MathUtils.random(0, size - 1)];
	}

	/** Returns the items as an array. Note the array is typed, so the {@link #Array(Class)} constructor must have been used.
	 * Otherwise use {@link #toArray(Class)} to specify the array type. */
	public T[] toArray()
	{
		return (T[])toArray<T>(items.GetType().GetGenericTypeDefinition());
	}

	public  V[] toArray<V>(Type type)
	{
		V[] result = (V[])ArrayReflection.newInstance(type, size);
		Array.Copy(items, 0, result, 0, size);
		return result;
	}

	public override int GetHashCode()
	{
		if (!ordered) return base.GetHashCode();
		T[] items = this.items;
		int h = 1;
		for (int i = 0, n = size; i < n; i++)
		{
			h *= 31;
			Object item = items[i];
			if (item != null) h += item.GetHashCode();
		}
		return h;
	}

	/** Returns false if either array is unordered. */
	public bool equals(Object? obj)
	{
		if (obj == this) return true;
		if (!ordered) return false;
		if (!(obj is Array<T>)) return false;
		Array<T> array = (Array<T>)obj;
		if (!array.ordered) return false;
		int n = size;
		if (n != array.size) return false;
		T[] items1 = this.items, items2 = array.items;
		for (int i = 0; i < n; i++)
		{
			Object o1 = items1[i], o2 = items2[i];
			if (!(o1 == null ? o2 == null : o1.Equals(o2))) return false;
		}
		return true;
	}

	/** Uses == for comparison of each item. Returns false if either array is unordered. */
	public bool equalsIdentity(Object obj)
	{
		if (obj == this) return true;
		if (!ordered) return false;
		if (!(obj is Array<T>)) return false;
		Array<T> array = (Array<T>)obj;
		if (!array.ordered) return false;
		int n = size;
		if (n != array.size) return false;
		T[] items1 = this.items, items2 = array.items;
		for (int i = 0; i < n; i++)
			if (!ReferenceEquals(items1[i] , items2[i])) return false;
		return true;
	}

	public override String ToString()
	{
		if (size == 0) return "[]";
		T[] items = this.items;
		StringBuilder buffer = new StringBuilder(32);
		buffer.Append('[');
		buffer.Append(items[0]);
		for (int i = 1; i < size; i++)
		{
			buffer.Append(", ");
			buffer.Append(items[i]);
		}
		buffer.Append(']');
		return buffer.ToString();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public String toString(String separator)
	{
		if (size == 0) return "";
		T[] items = this.items;
		StringBuilder buffer = new StringBuilder(32);
		buffer.Append(items[0]);
		for (int i = 1; i < size; i++)
		{
			buffer.Append(separator);
			buffer.Append(items[i]);
		}
		return buffer.ToString();
	}

	/** @see #Array(Class) */
	static public  Array<T> of(Type arrayType)
	{
		// TODO: Remove type as it is not needed in C#
			return new Array<T>(arrayType);
	}

	/** @see #Array(boolean, int, Class) */
	static public  Array<T> of(bool ordered, int capacity, Type arrayType)
	{
		// TODO: Remove type as it is not needed in C#
			return new Array<T>(ordered, capacity, arrayType);
	}

	/** @see #Array(Object[]) */
	static public  Array<T> with(T[]array)
	{
		return new Array<T>(array);
	}

	public class ArrayIterator : IEnumerator<T>, IEnumerable<T> {
		private readonly Array<T> array;
		private readonly bool allowRemove;
		internal int index;
	internal bool valid = true;

	// ArrayIterable<T> iterable;

	public ArrayIterator(Array<T> array)
	: this(array, true)
			{
	}

	public ArrayIterator(Array<T> array, bool allowRemove)
	{
		this.array = array;
		this.allowRemove = allowRemove;
	}

	public bool MoveNext()
	{
		if (!valid)
		{
			// System.out.println(iterable.lastAcquire);
			throw new GdxRuntimeException("#iterator() cannot be used nested.");
		}
		return index < array.size;
	}

	public T Current
	{
		get
		{
			if (index >= array.size) throw new NoSuchElementException(index.ToString());
			if (!valid)
			{
				// System.out.println(iterable.lastAcquire);
				throw new GdxRuntimeException("#iterator() cannot be used nested.");
			}

			return array.items[index++];
		}
	}

	public void remove()
	{
		if (!allowRemove) throw new GdxRuntimeException("Remove not allowed.");
		index--;
		array.removeIndex(index);
	}

			public void Dispose(){}

	public void Reset()
	{
		index = 0;
	}

	object IEnumerator.Current => Current;

	public IEnumerator<T> GetEnumerator()
	{
		return this;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	}

public class ArrayIterable : IEnumerable<T> {
		private readonly Array<T> array;
private readonly bool allowRemove;
private ArrayIterator iterator1, iterator2;

// java.io.StringWriter lastAcquire = new java.io.StringWriter();

public ArrayIterable (Array<T> array)
: this(array, true)
			{
}

public ArrayIterable (Array<T> array, bool allowRemove)
{
	this.array = array;
	this.allowRemove = allowRemove;
}

/** @see Collections#allocateIterators */
public IEnumerator<T> GetEnumerator()
{
	if (Collections.allocateIterators) return new ArrayIterator(array, allowRemove);
	// lastAcquire.getBuffer().setLength(0);
	// new Throwable().printStackTrace(new java.io.PrintWriter(lastAcquire));
	if (iterator1 == null)
	{
		iterator1 = new ArrayIterator(array, allowRemove);
		iterator2 = new ArrayIterator(array, allowRemove);
		// iterator1.iterable = this;
		// iterator2.iterable = this;
	}
	if (!iterator1.valid)
	{
		iterator1.index = 0;
		iterator1.valid = true;
		iterator2.valid = false;
		return iterator1;
	}
	iterator2.index = 0;
	iterator2.valid = true;
	iterator1.valid = false;
	return iterator2;
}

IEnumerator IEnumerable.GetEnumerator()
{
	return GetEnumerator();
}
}
}
}
