using SharpGDX.Mathematics;
using SharpGDX.Shims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** An unordered set where the keys are objects. Null keys are not allowed. No allocation is done except when growing the table
 * size.
 * <p>
 * This class performs fast contains and remove (typically O(1), worst case O(n) but that is rare in practice). Add may be
 * slightly slower, depending on hash collisions. Hashcodes are rehashed to reduce collisions and the need to resize. Load factors
 * greater than 0.91 greatly increase the chances to resize to the next higher POT size.
 * <p>
 * Unordered sets and maps are not designed to provide especially fast iteration. Iteration is faster with OrderedSet and
 * OrderedMap.
 * <p>
 * This implementation uses linear probing with the backward shift algorithm for removal. Hashcodes are rehashed using Fibonacci
 * hashing, instead of the more common power-of-two mask, to better distribute poor hashCodes (see <a href=
 * "https://probablydance.com/2018/06/16/fibonacci-hashing-the-optimization-that-the-world-forgot-or-a-better-alternative-to-integer-modulo/">Malte
 * Skarupke's blog post</a>). Linear probing continues to work even when all hashCodes collide, just more slowly.
 * @author Nathan Sweet
 * @author Tommy Ettinger */
	public class ObjectSet<T> : IEnumerable<T> {
	public int size;

	T[] keyTable;

	float loadFactor;
	int threshold;

	/** Used by {@link #place(Object)} to bit shift the upper bits of a {@code long} into a usable range (&gt;= 0 and &lt;=
	 * {@link #mask}). The shift can be negative, which is convenient to match the number of bits in mask: if mask is a 7-bit
	 * number, a shift of -7 shifts the upper 7 bits into the lowest 7 positions. This class sets the shift &gt; 32 and &lt; 64,
	 * which if used with an int will still move the upper bits of an int to the lower bits due to Java's implicit modulus on
	 * shifts.
	 * <p>
	 * {@link #mask} can also be used to mask the low bits of a number, which may be faster for some hashcodes, if
	 * {@link #place(Object)} is overridden. */
	protected int shift;

	/** A bitmask used to confine hashcodes to the size of the table. Must be all 1 bits in its low positions, ie a power of two
	 * minus 1. If {@link #place(Object)} is overriden, this can be used instead of {@link #shift} to isolate usable bits of a
	 * hash. */
	protected int mask;

		[NonSerialized]
	internal ObjectSetIterator iterator1, iterator2;

	/** Creates a new set with an initial capacity of 51 and a load factor of 0.8. */
	public ObjectSet()
	: this(51, 0.8f)
		{
		
	}

	/** Creates a new set with a load factor of 0.8.
	 * @param initialCapacity The backing array size is initialCapacity / loadFactor, increased to the next power of two. */
	public ObjectSet(int initialCapacity)
	: this(initialCapacity, 0.8f)
		{
		
	}

	/** Creates a new set with the specified initial capacity and load factor. This set will hold initialCapacity items before
	 * growing the backing table.
	 * @param initialCapacity The backing array size is initialCapacity / loadFactor, increased to the next power of two. */
	public ObjectSet(int initialCapacity, float loadFactor)
	{
		if (loadFactor <= 0f || loadFactor >= 1f)
			throw new IllegalArgumentException("loadFactor must be > 0 and < 1: " + loadFactor);
		this.loadFactor = loadFactor;

		int tableSize = ObjectSet<T>.tableSize(initialCapacity, loadFactor);
		threshold = (int)(tableSize * loadFactor);
		mask = tableSize - 1;
		shift = BitOperations.LeadingZeroCount((ulong)mask);

		keyTable = (T[])new T[tableSize];
	}

	/** Creates a new set identical to the specified set. */
	public ObjectSet(ObjectSet<T> set)
	: this((int)(set.keyTable.Length * set.loadFactor), set.loadFactor)
		{
		Array.Copy(set.keyTable, 0, keyTable, 0, set.keyTable.Length);
		size = set.size;
	}

	/** Returns an index >= 0 and <= {@link #mask} for the specified {@code item}.
	 * <p>
	 * The default implementation uses Fibonacci hashing on the item's {@link Object#hashCode()}: the hashcode is multiplied by a
	 * long constant (2 to the 64th, divided by the golden ratio) then the uppermost bits are shifted into the lowest positions to
	 * obtain an index in the desired range. Multiplication by a long may be slower than int (eg on GWT) but greatly improves
	 * rehashing, allowing even very poor hashcodes, such as those that only differ in their upper bits, to be used without high
	 * collision rates. Fibonacci hashing has increased collision rates when all or most hashcodes are multiples of larger
	 * Fibonacci numbers (see <a href=
	 * "https://probablydance.com/2018/06/16/fibonacci-hashing-the-optimization-that-the-world-forgot-or-a-better-alternative-to-integer-modulo/">Malte
	 * Skarupke's blog post</a>).
	 * <p>
	 * This method can be overriden to customizing hashing. This may be useful eg in the unlikely event that most hashcodes are
	 * Fibonacci numbers, if keys provide poor or incorrect hashcodes, or to simplify hashing if keys provide high quality
	 * hashcodes and don't need Fibonacci hashing: {@code return item.hashCode() & mask;} */
	protected int place(T item)
	{
		return (int)((ulong)item.GetHashCode() * 0x9E3779B97F4A7C15L >>> shift);
	}

	/** Returns the index of the key if already present, else -(index + 1) for the next empty index. This can be overridden in this
	 * pacakge to compare for equality differently than {@link Object#equals(Object)}. */
	int locateKey(T key)
	{
		if (key == null) throw new IllegalArgumentException("key cannot be null.");
		T[] keyTable = this.keyTable;
		for (int i = place(key); ; i = i + 1 & mask)
		{
			T other = keyTable[i];
			if (other == null) return -(i + 1); // Empty space is available.
			if (other.Equals(key)) return i; // Same key was found.
		}
	}

	/** Returns true if the key was added to the set or false if it was already in the set. If this set already contains the key,
	 * the call leaves the set unchanged and returns false. */
	public virtual bool add(T key)
	{
		int i = locateKey(key);
		if (i >= 0) return false; // Existing key was found.
		i = -(i + 1); // Empty space was found.
		keyTable[i] = key;
		if (++size >= threshold) resize(keyTable.Length << 1);
		return true;
	}

	public void addAll(Array<T> array)
	{
		addAll(array.items, 0, array.size);
	}

	public void addAll(Array<T> array, int offset, int length)
	{
		if (offset + length > array.size)
			throw new IllegalArgumentException("offset + length must be <= size: " + offset + " + " + length + " <= " + array.size);
		addAll(array.items, offset, length);
	}

	public bool addAll(T[]array)
	{
		return addAll(array, 0, array.Length);
	}

	public bool addAll(T[] array, int offset, int length)
	{
		ensureCapacity(length);
		int oldSize = size;
		for (int i = offset, n = i + length; i < n; i++)
			add(array[i]);
		return oldSize != size;
	}

	public void addAll(ObjectSet<T> set)
	{
		ensureCapacity(set.size);
		T[] keyTable = set.keyTable;
		for (int i = 0, n = keyTable.Length; i < n; i++)
		{
			T key = keyTable[i];
			if (key != null) add(key);
		}
	}

	/** Skips checks for existing keys, doesn't increment size. */
	private void addResize(T key)
	{
		T[] keyTable = this.keyTable;
		for (int i = place(key); ; i = (i + 1) & mask)
		{
			if (keyTable[i] == null)
			{
				keyTable[i] = key;
				return;
			}
		}
	}

	/** Returns true if the key was removed. */
	public virtual bool remove(T key)
	{
		int i = locateKey(key);
		if (i < 0) return false;
		T[] keyTable = this.keyTable;
		int mask = this.mask, next = i + 1 & mask;
		while ((key = keyTable[next]) != null)
		{
			int placement = place(key);
			if ((next - placement & mask) > (i - placement & mask))
			{
				keyTable[i] = key;
				i = next;
			}
			next = next + 1 & mask;
		}
		keyTable[i] = default;
		size--;
		return true;
	}

	/** Returns true if the set has one or more items. */
	public bool notEmpty()
	{
		return size > 0;
	}

	/** Returns true if the set is empty. */
	public bool isEmpty()
	{
		return size == 0;
	}

	/** Reduces the size of the backing arrays to be the specified capacity / loadFactor, or less. If the capacity is already less,
	 * nothing is done. If the set contains more items than the specified capacity, the next highest power of two capacity is used
	 * instead. */
	public void shrink(int maximumCapacity)
	{
		if (maximumCapacity < 0) throw new IllegalArgumentException("maximumCapacity must be >= 0: " + maximumCapacity);
		int tableSize = ObjectSet < T > .tableSize(maximumCapacity, loadFactor);
		if (keyTable.Length > tableSize) resize(tableSize);
	}

	/** Clears the set and reduces the size of the backing arrays to be the specified capacity / loadFactor, if they are larger.
	 * The reduction is done by allocating new arrays, though for large arrays this can be faster than clearing the existing
	 * array. */
	public virtual void clear(int maximumCapacity)
	{
		int tableSize = ObjectSet<T>.tableSize(maximumCapacity, loadFactor);
		if (keyTable.Length <= tableSize)
		{
			clear();
			return;
		}
		size = 0;
		resize(tableSize);
	}

	/** Clears the set, leaving the backing arrays at the current capacity. When the capacity is high and the population is low,
	 * iteration can be unnecessarily slow. {@link #clear(int)} can be used to reduce the capacity. */
	public virtual void clear()
	{
		if (size == 0) return;
		size = 0;
		Array.Fill(keyTable, default);
	}

	public bool contains(T key)
	{
		return locateKey(key) >= 0;
	}

	public T? get(T key)
	{
		int i = locateKey(key);
		return i < 0 ? default : keyTable[i];
	}

	public T first()
	{
		T[] keyTable = this.keyTable;
		for (int i = 0, n = keyTable.Length; i < n; i++)
			if (keyTable[i] != null) return keyTable[i];
		throw new IllegalStateException("ObjectSet is empty.");
	}

	/** Increases the size of the backing array to accommodate the specified number of additional items / loadFactor. Useful before
	 * adding many items to avoid multiple backing array resizes. */
	public void ensureCapacity(int additionalCapacity)
	{
		int tableSize = ObjectSet<T>.tableSize(size + additionalCapacity, loadFactor);
		if (keyTable.Length < tableSize) resize(tableSize);
	}

	private void resize(int newSize)
	{
		int oldCapacity = keyTable.Length;
		threshold = (int)(newSize * loadFactor);
		mask = newSize - 1;
		shift = BitOperations.LeadingZeroCount((ulong)mask);
		T[] oldKeyTable = keyTable;

		keyTable = (T[])(new T[newSize]);

		if (size > 0)
		{
			for (int i = 0; i < oldCapacity; i++)
			{
				T key = oldKeyTable[i];
				if (key != null) addResize(key);
			}
		}
	}

	public override int GetHashCode()
	{
		int h = size;
		T[] keyTable = this.keyTable;
		for (int i = 0, n = keyTable.Length; i < n; i++)
		{
			T key = keyTable[i];
			if (key != null) h += key.GetHashCode();
		}
		return h;
	}

	public override bool Equals(Object? obj)
	{
		if (!(obj is ObjectSet<T>)) return false;
		ObjectSet<T> other = (ObjectSet<T>)obj;
		if (other.size != size) return false;
		T[] keyTable = this.keyTable;
		for (int i = 0, n = keyTable.Length; i < n; i++)
			if (keyTable[i] != null && !other.contains(keyTable[i])) return false;
		return true;
	}

	public override String ToString()
	{
		return '{' + toString(", ") + '}';
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public virtual String toString(String separator)
	{
		if (size == 0) return "";
		StringBuilder buffer = new StringBuilder(32);
		T[] keyTable = this.keyTable;
		int i = keyTable.Length;
		while (i-- > 0)
		{
			T key = keyTable[i];
			if (key == null) continue;
			buffer.Append(ReferenceEquals(key , this) ? "(this)" : key);
			break;
		}
		while (i-- > 0)
		{
			T key = keyTable[i];
			if (key == null) continue;
			buffer.Append(separator);
			buffer.Append(ReferenceEquals(key , this) ? "(this)" : key);
		}
		return buffer.ToString();
	}

	/** Returns an iterator for the keys in the set. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link ObjectSetIterator} constructor for nested or multithreaded iteration. */
	public IEnumerator<T> GetEnumerator()
	{
		if (Collections.allocateIterators) return new ObjectSetIterator(this);
		if (iterator1 == null)
		{
			iterator1 = new ObjectSetIterator(this);
			iterator2 = new ObjectSetIterator(this);
		}
		if (!iterator1.valid)
		{
			iterator1.Reset();
			iterator1.valid = true;
			iterator2.valid = false;
			return iterator1;
		}
		iterator2.Reset();
		iterator2.valid = true;
		iterator1.valid = false;
		return iterator2;
	}

	static public ObjectSet<T> with(T[]array)
	{
		ObjectSet<T> set = new ObjectSet<T>();
		set.addAll(array);
		return set;
	}

	internal static int tableSize(int capacity, float loadFactor)
	{
		if (capacity < 0) throw new IllegalArgumentException("capacity must be >= 0: " + capacity);
		int tableSize = MathUtils.nextPowerOfTwo(Math.Max(2, (int)Math.Ceiling(capacity / loadFactor)));
		if (tableSize > 1 << 30) throw new IllegalArgumentException("The required capacity is too large: " + capacity);
		return tableSize;
	}

	public class ObjectSetIterator : IEnumerable<T>, IEnumerator<T> {
		public bool _hasNext;

	protected readonly ObjectSet<T> set;
	protected int nextIndex, currentIndex;
	internal bool valid = true;

	public ObjectSetIterator(ObjectSet<T> set)
	{
		this.set = set;
		Reset();
	}

	public void Reset()
	{
		currentIndex = -1;
		nextIndex = -1;
		findNextIndex();
	}

	object IEnumerator.Current => Current;

	private void findNextIndex()
	{
		T[] keyTable = set.keyTable;
		for (int n = set.keyTable.Length; ++nextIndex < n;)
		{
			if (keyTable[nextIndex] != null)
			{
				_hasNext = true;
				return;
			}
		}
		_hasNext = false;
	}

	public virtual void remove()
	{
		int i = currentIndex;
		if (i < 0) throw new IllegalStateException("next must be called before remove.");
		T[] keyTable = set.keyTable;
		int mask = set.mask, next = i + 1 & mask;
		T key;
		while ((key = keyTable[next]) != null)
		{
			int placement = set.place(key);
			if ((next - placement & mask) > (i - placement & mask))
			{
				keyTable[i] = key;
				i = next;
			}
			next = next + 1 & mask;
		}
		keyTable[i] = default;
		set.size--;
		if (i != currentIndex) --nextIndex;
		currentIndex = -1;
	}

	public bool MoveNext()
	{
		if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
		return _hasNext;
	}

	public T Current
	{
		get
		{
			if (!_hasNext) throw new NoSuchElementException();
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			T key = set.keyTable[nextIndex];
			currentIndex = nextIndex;
			findNextIndex();
			return key;
		}
	}
	
	public IEnumerator<T> GetEnumerator()
	{
		return this;
	}

	public void Dispose(){}

	/** Adds the remaining values to the array. */
	public virtual Array<T> toArray(Array<T> array)
	{
		while (_hasNext)
			array.add(Current);
		return array;
	}

	/** Returns a new array containing the remaining values. */
	public virtual Array<T> toArray()
	{
		return toArray(new Array<T>(true, set.size));
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	}
}
}
