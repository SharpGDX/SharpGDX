using System;
using System.Collections;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
    /** An unordered map where the keys and values are unboxed ints. No allocation is done except when growing the table size.
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
public class IntIntMap : IEnumerable<IntIntMap.Entry> {
	public int size;

	int[] keyTable;
	int[] valueTable;

	int zeroValue;
	bool hasZeroValue;

	private readonly float loadFactor;
	private int threshold;

	/** Used by {@link #place(int)} to bit shift the upper bits of a {@code long} into a usable range (&gt;= 0 and &lt;=
	 * {@link #mask}). The shift can be negative, which is convenient to match the number of bits in mask: if mask is a 7-bit
	 * number, a shift of -7 shifts the upper 7 bits into the lowest 7 positions. This class sets the shift &gt; 32 and &lt; 64,
	 * which if used with an int will still move the upper bits of an int to the lower bits due to Java's implicit modulus on
	 * shifts.
	 * <p>
	 * {@link #mask} can also be used to mask the low bits of a number, which may be faster for some hashcodes, if
	 * {@link #place(int)} is overridden. */
	protected int shift;

	/** A bitmask used to confine hashcodes to the size of the table. Must be all 1 bits in its low positions, ie a power of two
	 * minus 1. If {@link #place(int)} is overriden, this can be used instead of {@link #shift} to isolate usable bits of a
	 * hash. */
	protected int mask;

		[NonSerialized]
	private  Entries entries1, entries2;
    [NonSerialized]
        private  Values values1, values2;
    [NonSerialized]
        private  Keys keys1, keys2;

	/** Creates a new map with an initial capacity of 51 and a load factor of 0.8. */
	public IntIntMap () 
    : this(51, 0.8f)
    {
		
	}

	/** Creates a new map with a load factor of 0.8.
	 * @param initialCapacity The backing array size is initialCapacity / loadFactor, increased to the next power of two. */
	public IntIntMap (int initialCapacity) 
    : this(initialCapacity, 0.8f)
    {
		
	}

	/** Creates a new map with the specified initial capacity and load factor. This map will hold initialCapacity items before
	 * growing the backing table.
	 * @param initialCapacity The backing array size is initialCapacity / loadFactor, increased to the next power of two. */
	public IntIntMap (int initialCapacity, float loadFactor) {
		if (loadFactor <= 0f || loadFactor >= 1f)
			throw new IllegalArgumentException("loadFactor must be > 0 and < 1: " + loadFactor);
		this.loadFactor = loadFactor;

		// TODO: Should this be ObjectSet<int>?
		int tableSize = ObjectSet<int>.tableSize(initialCapacity, loadFactor);
		threshold = (int)(tableSize * loadFactor);
		mask = tableSize - 1;
		shift = BitOperations.LeadingZeroCount((uint)mask);

		keyTable = new int[tableSize];
		valueTable = new int[tableSize];
	}

	/** Creates a new map identical to the specified map. */
	public IntIntMap (IntIntMap map) 
    : this((int)(map.keyTable.Length * map.loadFactor), map.loadFactor)
    {
		
		Array.Copy(map.keyTable, 0, keyTable, 0, map.keyTable.Length);
		Array.Copy(map.valueTable, 0, valueTable, 0, map.valueTable.Length);
		size = map.size;
		zeroValue = map.zeroValue;
		hasZeroValue = map.hasZeroValue;
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
	protected int place (int item) {
		// TODO: No clue if this is correct. -RP
		return (int)((uint)item * 0x9E3779B97F4A7C15L >>> shift);
	}

	/** Returns the index of the key if already present, else -(index + 1) for the next empty index. This can be overridden in this
	 * pacakge to compare for equality differently than {@link Object#equals(Object)}. */
	private int locateKey (int key) {
		int[] keyTable = this.keyTable;
		for (int i = place(key);; i = i + 1 & mask) {
			int other = keyTable[i];
			if (other == 0) return -(i + 1); // Empty space is available.
			if (other == key) return i; // Same key was found.
		}
	}

	public void put (int key, int value) {
		if (key == 0) {
			zeroValue = value;
			if (!hasZeroValue) {
				hasZeroValue = true;
				size++;
			}
			return;
		}
		int i = locateKey(key);
		if (i >= 0) { // Existing key was found.
			valueTable[i] = value;
			return;
		}
		i = -(i + 1); // Empty space was found.
		keyTable[i] = key;
		valueTable[i] = value;
		if (++size >= threshold) resize(keyTable.Length << 1);
	}

	/** Returns the old value associated with the specified key, or the specified default value. */
	public int put (int key, int value, int defaultValue) {
		if (key == 0) {
			int oldValue = zeroValue;
			zeroValue = value;
			if (!hasZeroValue) {
				hasZeroValue = true;
				size++;
				return defaultValue;
			}
			return oldValue;
		}
		int i = locateKey(key);
		if (i >= 0) { // Existing key was found.
			int oldValue = valueTable[i];
			valueTable[i] = value;
			return oldValue;
		}
		i = -(i + 1); // Empty space was found.
		keyTable[i] = key;
		valueTable[i] = value;
		if (++size >= threshold) resize(keyTable.Length << 1);
		return defaultValue;
	}

	public void putAll (IntIntMap map) {
		ensureCapacity(map.size);
		if (map.hasZeroValue) put(0, map.zeroValue);
		int[] keyTable = map.keyTable;
		int[] valueTable = map.valueTable;
		for (int i = 0, n = keyTable.Length; i < n; i++) {
			int key = keyTable[i];
			if (key != 0) put(key, valueTable[i]);
		}
	}

	/** Skips checks for existing keys, doesn't increment size, doesn't need to handle key 0. */
	private void putResize (int key, int value) {
		int[] keyTable = this.keyTable;
		for (int i = place(key);; i = (i + 1) & mask) {
			if (keyTable[i] == 0) {
				keyTable[i] = key;
				valueTable[i] = value;
				return;
			}
		}
	}

	public int get (int key, int defaultValue) {
		if (key == 0) return hasZeroValue ? zeroValue : defaultValue;
		int i = locateKey(key);
		return i >= 0 ? valueTable[i] : defaultValue;
	}

	/** Returns the key's current value and increments the stored value. If the key is not in the map, defaultValue + increment is
	 * put into the map and defaultValue is returned. */
	public int getAndIncrement (int key, int defaultValue, int increment) {
		if (key == 0) {
			if (!hasZeroValue) {
				hasZeroValue = true;
				zeroValue = defaultValue + increment;
				size++;
				return defaultValue;
			}
			int oldValue = zeroValue;
			zeroValue += increment;
			return oldValue;
		}
		int i = locateKey(key);
		if (i >= 0) { // Existing key was found.
			int oldValue = valueTable[i];
			valueTable[i] += increment;
			return oldValue;
		}
		i = -(i + 1); // Empty space was found.
		keyTable[i] = key;
		valueTable[i] = defaultValue + increment;
		if (++size >= threshold) resize(keyTable.Length << 1);
		return defaultValue;
	}

	/** Returns the value for the removed key, or the default value if the key is not in the map. */
	public int remove (int key, int defaultValue) {
		if (key == 0) {
			if (!hasZeroValue) return defaultValue;
			hasZeroValue = false;
			size--;
			return zeroValue;
		}

		int i = locateKey(key);
		if (i < 0) return defaultValue;
		int[] keyTable = this.keyTable;
		int[] valueTable = this.valueTable;
		int oldValue = valueTable[i], mask = this.mask, next = i + 1 & mask;
		while ((key = keyTable[next]) != 0) {
			int placement = place(key);
			if ((next - placement & mask) > (i - placement & mask)) {
				keyTable[i] = key;
				valueTable[i] = valueTable[next];
				i = next;
			}
			next = next + 1 & mask;
		}
		keyTable[i] = 0;
		size--;
		return oldValue;
	}

	/** Returns true if the map has one or more items. */
	public bool notEmpty () {
		return size > 0;
	}

	/** Returns true if the map is empty. */
	public bool isEmpty () {
		return size == 0;
	}

	/** Reduces the size of the backing arrays to be the specified capacity / locateKey, or less. If the capacity is already less,
	 * nothing is done. If the map contains more items than the specified capacity, the next highest power of two capacity is used
	 * instead. */
	public void shrink (int maximumCapacity) {
		if (maximumCapacity < 0) throw new IllegalArgumentException("maximumCapacity must be >= 0: " + maximumCapacity);
        // TODO: Should this be ObjectSet<int>?
        int tableSize = ObjectSet<int>.tableSize(maximumCapacity, loadFactor);
		if (keyTable.Length > tableSize) resize(tableSize);
	}

	/** Clears the map and reduces the size of the backing arrays to be the specified capacity / loadFactor, if they are larger. */
	public void clear (int maximumCapacity) {
        // TODO: Should this be ObjectSet<int>?
        int tableSize = ObjectSet<int>.tableSize(maximumCapacity, loadFactor);
		if (keyTable.Length <= tableSize) {
			clear();
			return;
		}
		size = 0;
		hasZeroValue = false;
		resize(tableSize);
	}

	public void clear () {
		if (size == 0) return;
		Array.Fill(keyTable, 0);
		size = 0;
		hasZeroValue = false;
	}

	/** Returns true if the specified value is in the map. Note this traverses the entire map and compares every value, which may
	 * be an expensive operation. */
	public bool containsValue (int value) {
		if (hasZeroValue && zeroValue == value) return true;
		int[] keyTable = this.keyTable;
		int[] valueTable = this.valueTable;
		for (int i = valueTable.Length - 1; i >= 0; i--)
			if (keyTable[i] != 0 && valueTable[i] == value) return true;
		return false;
	}

	public bool containsKey (int key) {
		if (key == 0) return hasZeroValue;
		return locateKey(key) >= 0;
	}

	/** Returns the key for the specified value, or null if it is not in the map. Note this traverses the entire map and compares
	 * every value, which may be an expensive operation. */
	public int findKey (int value, int notFound) {
		if (hasZeroValue && zeroValue == value) return 0;
		int[] keyTable = this.keyTable;
		int[] valueTable = this.valueTable;
		for (int i = valueTable.Length - 1; i >= 0; i--) {
			int key = keyTable[i];
			if (key != 0 && valueTable[i] == value) return key;
		}
		return notFound;
	}

	/** Increases the size of the backing array to accommodate the specified number of additional items / loadFactor. Useful before
	 * adding many items to avoid multiple backing array resizes. */
	public void ensureCapacity (int additionalCapacity) {
        // TODO: Should this be ObjectSet<int>?
        int tableSize = ObjectSet<int>.tableSize(size + additionalCapacity, loadFactor);
		if (keyTable.Length < tableSize) resize(tableSize);
	}

	private void resize (int newSize) {
		int oldCapacity = keyTable.Length;
		threshold = (int)(newSize * loadFactor);
		mask = newSize - 1;
		shift = BitOperations.LeadingZeroCount((uint)mask);

		int[] oldKeyTable = keyTable;
		int[] oldValueTable = valueTable;

		keyTable = new int[newSize];
		valueTable = new int[newSize];

		if (size > 0) {
			for (int i = 0; i < oldCapacity; i++) {
				int key = oldKeyTable[i];
				if (key != 0) putResize(key, oldValueTable[i]);
			}
		}
	}

	public int hashCode () {
		int h = size;
		if (hasZeroValue) h += zeroValue;
		int[] keyTable = this.keyTable;
		int[] valueTable = this.valueTable;
		for (int i = 0, n = keyTable.Length; i < n; i++) {
			int key = keyTable[i];
			if (key != 0) h += key * 31 + valueTable[i];
		}
		return h;
	}

	public override bool Equals (Object? obj) {
		if (obj == this) return true;
		if (!(obj is IntIntMap)) return false;
		IntIntMap other = (IntIntMap)obj;
		if (other.size != size) return false;
		if (other.hasZeroValue != hasZeroValue) return false;
		if (hasZeroValue) {
			if (other.zeroValue != zeroValue) return false;
		}
		int[] keyTable = this.keyTable;
		int[] valueTable = this.valueTable;
		for (int i = 0, n = keyTable.Length; i < n; i++) {
			int key = keyTable[i];
			if (key != 0) {
				int otherValue = other.get(key, 0);
				if (otherValue == 0 && !other.containsKey(key)) return false;
				if (otherValue != valueTable[i]) return false;
			}
		}
		return true;
	}

	public override String ToString () {
		if (size == 0) return "[]";
		StringBuilder buffer = new StringBuilder(32);
		buffer.Append('[');
		int[] keyTable = this.keyTable;
		int[] valueTable = this.valueTable;
		int i = keyTable.Length;
		if (hasZeroValue) {
			buffer.Append("0=");
			buffer.Append(zeroValue);
		} else {
			while (i-- > 0) {
				int key = keyTable[i];
				if (key == 0) continue;
				buffer.Append(key);
				buffer.Append('=');
				buffer.Append(valueTable[i]);
				break;
			}
		}
		while (i-- > 0) {
			int key = keyTable[i];
			if (key == 0) continue;
			buffer.Append(", ");
			buffer.Append(key);
			buffer.Append('=');
			buffer.Append(valueTable[i]);
		}
		buffer.Append(']');
		return buffer.ToString();
	}

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<Entry> GetEnumerator () {
		return entries();
	}

	/** Returns an iterator for the entries in the map. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link Entries} constructor for nested or multithreaded iteration. */
	public Entries entries () {
		if (Collections.allocateIterators) return new Entries(this);
		if (entries1 == null) {
			entries1 = new Entries(this);
			entries2 = new Entries(this);
		}
		if (!entries1.valid) {
			entries1.Reset();
			entries1.valid = true;
			entries2.valid = false;
			return entries1;
		}
		entries2.Reset();
		entries2.valid = true;
		entries1.valid = false;
		return entries2;
	}

	/** Returns an iterator for the values in the map. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link Entries} constructor for nested or multithreaded iteration. */
	public Values values () {
		if (Collections.allocateIterators) return new Values(this);
		if (values1 == null) {
			values1 = new Values(this);
			values2 = new Values(this);
		}
		if (!values1.valid) {
			values1.Reset();
			values1.valid = true;
			values2.valid = false;
			return values1;
		}
		values2.Reset();
		values2.valid = true;
		values1.valid = false;
		return values2;
	}

	/** Returns an iterator for the keys in the map. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link Entries} constructor for nested or multithreaded iteration. */
	public Keys keys () {
		if (Collections.allocateIterators) return new Keys(this);
		if (keys1 == null) {
			keys1 = new Keys(this);
			keys2 = new Keys(this);
		}
		if (!keys1.valid) {
			keys1.Reset();
			keys1.valid = true;
			keys2.valid = false;
			return keys1;
		}
		keys2.Reset();
		keys2.valid = true;
		keys1.valid = false;
		return keys2;
	}

	public class Entry {
		public int key;
		public int value;

		public String toString () {
			return key + "=" + value;
		}
	}

	// TODO: This was originally private
	public class MapIterator {
		static private readonly int INDEX_ILLEGAL = -2;
        internal static readonly int INDEX_ZERO = -1;

		public bool _hasNext;

        internal readonly IntIntMap map;
        internal int nextIndex, currentIndex;
		internal bool valid = true;

		public MapIterator (IntIntMap map) {
			this.map = map;
			Reset();
		}

		public void Reset () {
			currentIndex = INDEX_ILLEGAL;
			nextIndex = INDEX_ZERO;
			if (map.hasZeroValue)
				_hasNext = true;
			else
				findNextIndex();
		}

		protected void findNextIndex () {
			int[] keyTable = map.keyTable;
			for (int n = keyTable.Length; ++nextIndex < n;) {
				if (keyTable[nextIndex] != 0) {
					_hasNext = true;
					return;
				}
			}
			_hasNext = false;
		}

		public void remove () {
			int i = currentIndex;
			if (i == INDEX_ZERO && map.hasZeroValue) {
				map.hasZeroValue = false;
			} else if (i < 0) {
				throw new IllegalStateException("next must be called before remove.");
			} else {
				int[] keyTable = map.keyTable;
				int[] valueTable = map.valueTable;
				int mask = map.mask, next = i + 1 & mask, key;
				while ((key = keyTable[next]) != 0) {
					int placement = map.place(key);
					if ((next - placement & mask) > (i - placement & mask)) {
						keyTable[i] = key;
						valueTable[i] = valueTable[next];
						i = next;
					}
					next = next + 1 & mask;
				}
				keyTable[i] = 0;
				if (i != currentIndex) --nextIndex;
			}
			currentIndex = INDEX_ILLEGAL;
			map.size--;
		}
	}

	 public class Entries : MapIterator , IEnumerable<Entry>, IEnumerator<Entry> {
		private readonly Entry entry = new Entry();

		public Entries (IntIntMap map) 
        : base(map)
        {
			
		}

		public void Dispose(){}

        /** Note the same entry instance is returned each time this method is called. */
        public Entry Current
        {
            get
            {
                if (!_hasNext) throw new NoSuchElementException();
                if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
                int[] keyTable = map.keyTable;
                if (nextIndex == INDEX_ZERO)
                {
                    entry.key = 0;
                    entry.value = map.zeroValue;
                }
                else
                {
                    entry.key = keyTable[nextIndex];
                    entry.value = map.valueTable[nextIndex];
                }

                currentIndex = nextIndex;
                findNextIndex();
                return entry;
            }
        }

        public bool MoveNext () {
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			return _hasNext;
		}

        object? IEnumerator.Current => Current;

        public IEnumerator<Entry> GetEnumerator () {
			return this;
		}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
     }

	 public class Values : MapIterator {
		public Values (IntIntMap map) 
        : base(map)
        {
			
		}

		public bool MoveNext () {
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			return _hasNext;
		}

        public int Current
        {
            get
            {
                if (!_hasNext) throw new NoSuchElementException();
                if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
                int value = nextIndex == INDEX_ZERO ? map.zeroValue : map.valueTable[nextIndex];
                currentIndex = nextIndex;
                findNextIndex();
                return value;
            }
        }

        public Values GetEnumerator () {
			return this;
		}

		/** Returns a new array containing the remaining values. */
		public IntArray toArray () {
			IntArray array = new IntArray(true, map.size);
			while (_hasNext)
				array.add(Current);
			return array;
		}

		/** Adds the remaining values to the specified array. */
		public IntArray toArray (IntArray array) {
			while (_hasNext)
				array.add(Current);
			return array;
		}
	}

	 public class Keys : MapIterator {
		public Keys (IntIntMap map) 
        : base(map)
        {
			
		}

		public int next () {
			if (!_hasNext) throw new NoSuchElementException();
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			int key = nextIndex == INDEX_ZERO ? 0 : map.keyTable[nextIndex];
			currentIndex = nextIndex;
			findNextIndex();
			return key;
		}

		/** Returns a new array containing the remaining keys. */
		public IntArray toArray () {
			IntArray array = new IntArray(true, map.size);
			while (_hasNext)
				array.add(next());
			return array;
		}

		/** Adds the remaining values to the specified array. */
		public IntArray toArray (IntArray array) {
			while (_hasNext)
				array.add(next());
			return array;
		}
	}
}
}
