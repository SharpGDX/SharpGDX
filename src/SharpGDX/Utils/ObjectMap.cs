using System.Collections;
using System.Numerics;
using System.Text;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Utils
{
	/** An unordered map where the keys and values are objects. Null keys are not allowed. No allocation is done except when growing
 * the table size.
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
	public class ObjectMap<K, V> : IEnumerable<ObjectMap<K, V>.Entry>
	{
		internal static readonly V dummy =default;

		public int size;

		K[] keyTable;
		V[] valueTable;

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

		[NonSerialized] protected Entries entries1, entries2;
		[NonSerialized] protected Values values1, values2;
		[NonSerialized] protected Keys keys1, keys2;

		/** Creates a new map with an initial capacity of 51 and a load factor of 0.8. */
		public ObjectMap()
			: this(51, 0.8f)
		{
		}

		/** Creates a new map with a load factor of 0.8.
		 * @param initialCapacity The backing array size is initialCapacity / loadFactor, increased to the next power of two. */
		public ObjectMap(int initialCapacity)
			: this(initialCapacity, 0.8f)
		{
		}

		/** Creates a new map with the specified initial capacity and load factor. This map will hold initialCapacity items before
		 * growing the backing table.
		 * @param initialCapacity The backing array size is initialCapacity / loadFactor, increased to the next power of two. */
		public ObjectMap(int initialCapacity, float loadFactor)
		{
			if (loadFactor <= 0f || loadFactor >= 1f)
				throw new IllegalArgumentException("loadFactor must be > 0 and < 1: " + loadFactor);
			this.loadFactor = loadFactor;

			int tableSize = ObjectSet<K>.tableSize(initialCapacity, loadFactor);
			threshold = (int)(tableSize * loadFactor);
			mask = tableSize - 1;
			shift = BitOperations.LeadingZeroCount((ulong)mask);

			keyTable = (K[])new K[tableSize];
			valueTable = (V[])new V[tableSize];
		}

		/** Creates a new map identical to the specified map. */
		public ObjectMap(ObjectMap<K, V> map)
			: this((int)(map.keyTable.Length * map.loadFactor), map.loadFactor)
		{
			Array.Copy(map.keyTable, 0, keyTable, 0, map.keyTable.Length);
			Array.Copy(map.valueTable, 0, valueTable, 0, map.valueTable.Length);
			size = map.size;
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
		protected int place(K item)
		{
			return (int)((ulong)item.GetHashCode() * 0x9E3779B97F4A7C15L >>> shift);
		}

		/** Returns the index of the key if already present, else -(index + 1) for the next empty index. This can be overridden in this
		 * pacakge to compare for equality differently than {@link Object#equals(Object)}. */
		int locateKey(K key)
		{
			if (key == null) throw new IllegalArgumentException("key cannot be null.");
			K[] keyTable = this.keyTable;
			for (int i = place(key);; i = i + 1 & mask)
			{
				K other = keyTable[i];
				if (other == null) return -(i + 1); // Empty space is available.
				if (other.Equals(key)) return i; // Same key was found.
			}
		}

		/** Returns the old value associated with the specified key, or null. */
		public virtual V? put(K key, V? value)
		{
			int i = locateKey(key);
			if (i >= 0)
			{
				// Existing key was found.
				V oldValue = valueTable[i];
				valueTable[i] = value;
				return oldValue;
			}

			i = -(i + 1); // Empty space was found.
			keyTable[i] = key;
			valueTable[i] = value;
			if (++size >= threshold) resize(keyTable.Length << 1);
			return default;
		}

		public void putAll(ObjectMap<K, V> map)
		{
			ensureCapacity(map.size);
			K[] keyTable = map.keyTable;
			V[] valueTable = map.valueTable;
			K key;
			for (int i = 0, n = keyTable.Length; i < n; i++)
			{
				key = keyTable[i];
				if (key != null) put(key, valueTable[i]);
			}
		}

		/** Skips checks for existing keys, doesn't increment size. */
		private void putResize(K key, V? value)
		{
			K[] keyTable = this.keyTable;
			for (int i = place(key);; i = (i + 1) & mask)
			{
				if (keyTable[i] == null)
				{
					keyTable[i] = key;
					valueTable[i] = value;
					return;
				}
			}
		}

		/** Returns the value for the specified key, or null if the key is not in the map. */
		public V? get<T>(T key)
			where T : K
		{
			int i = locateKey(key);
			return i < 0 ? default : valueTable[i];
		}

		/** Returns the value for the specified key, or the default value if the key is not in the map. */
		public V get(K key, V? defaultValue)
		{
			int i = locateKey(key);
			return i < 0 ? defaultValue : valueTable[i];
		}

		/** Returns the value for the removed key, or null if the key is not in the map. */
		public virtual V? remove(K key)
		{
			int i = locateKey(key);
			if (i < 0) return default;
			K[] keyTable = this.keyTable;
			V[] valueTable = this.valueTable;
			V oldValue = valueTable[i];
			int mask = this.mask, next = i + 1 & mask;
			while ((key = keyTable[next]) != null)
			{
				int placement = place(key);
				if ((next - placement & mask) > (i - placement & mask))
				{
					keyTable[i] = key;
					valueTable[i] = valueTable[next];
					i = next;
				}

				next = next + 1 & mask;
			}

			keyTable[i] = default;
			valueTable[i] = default;
			size--;
			return oldValue;
		}

		/** Returns true if the map has one or more items. */
		public bool notEmpty()
		{
			return size > 0;
		}

		/** Returns true if the map is empty. */
		public bool isEmpty()
		{
			return size == 0;
		}

		/** Reduces the size of the backing arrays to be the specified capacity / loadFactor, or less. If the capacity is already less,
		 * nothing is done. If the map contains more items than the specified capacity, the next highest power of two capacity is used
		 * instead. */
		public void shrink(int maximumCapacity)
		{
			if (maximumCapacity < 0)
				throw new IllegalArgumentException("maximumCapacity must be >= 0: " + maximumCapacity);
			int tableSize = ObjectSet<K>.tableSize(maximumCapacity, loadFactor);
			if (keyTable.Length > tableSize) resize(tableSize);
		}

		/** Clears the map and reduces the size of the backing arrays to be the specified capacity / loadFactor, if they are larger. */
		public virtual void clear(int maximumCapacity)
		{
			int tableSize = ObjectSet<K>.tableSize(maximumCapacity, loadFactor);
			if (keyTable.Length <= tableSize)
			{
				clear();
				return;
			}

			size = 0;
			resize(tableSize);
		}

		public virtual void clear()
		{
			if (size == 0) return;
			size = 0;
			Array.Fill(keyTable, default);
			Array.Fill(valueTable, default);
		}

		/** Returns true if the specified value is in the map. Note this traverses the entire map and compares every value, which may
		 * be an expensive operation.
		 * @param identity If true, uses == to compare the specified value with values in the map. If false, uses
		 *           {@link #equals(Object)}. */
		public bool containsValue(Object? value, bool identity)
		{
			V[] valueTable = this.valueTable;
			if (value == null)
			{
				K[] keyTable = this.keyTable;
				for (int i = valueTable.Length - 1; i >= 0; i--)
					if (keyTable[i] != null && valueTable[i] == null)
						return true;
			}
			else if (identity)
			{
				for (int i = valueTable.Length - 1; i >= 0; i--)
					if (ReferenceEquals(valueTable[i], value))
						return true;
			}
			else
			{
				for (int i = valueTable.Length - 1; i >= 0; i--)
					if (value.Equals(valueTable[i]))
						return true;
			}

			return false;
		}

		public bool containsKey(K key)
		{
			return locateKey(key) >= 0;
		}

		/** Returns the key for the specified value, or null if it is not in the map. Note this traverses the entire map and compares
		 * every value, which may be an expensive operation.
		 * @param identity If true, uses == to compare the specified value with values in the map. If false, uses
		 *           {@link #equals(Object)}. */
		public K? findKey(Object? value, bool identity)
		{
			V[] valueTable = this.valueTable;
			if (value == null)
			{
				K[] keyTable = this.keyTable;
				for (int i = valueTable.Length - 1; i >= 0; i--)
					if (keyTable[i] != null && valueTable[i] == null)
						return keyTable[i];
			}
			else if (identity)
			{
				for (int i = valueTable.Length - 1; i >= 0; i--)
					if (ReferenceEquals(valueTable[i], value))
						return keyTable[i];
			}
			else
			{
				for (int i = valueTable.Length - 1; i >= 0; i--)
					if (value.Equals(valueTable[i]))
						return keyTable[i];
			}

			return default;
		}

		/** Increases the size of the backing array to accommodate the specified number of additional items / loadFactor. Useful before
		 * adding many items to avoid multiple backing array resizes. */
		public void ensureCapacity(int additionalCapacity)
		{
			int tableSize = ObjectSet<K>.tableSize(size + additionalCapacity, loadFactor);
			if (keyTable.Length < tableSize) resize(tableSize);
		}

		void resize(int newSize)
		{
			int oldCapacity = keyTable.Length;
			threshold = (int)(newSize * loadFactor);
			mask = newSize - 1;
			shift = BitOperations.LeadingZeroCount((ulong)mask);

			K[] oldKeyTable = keyTable;
			V[] oldValueTable = valueTable;

			keyTable = (K[])new K[newSize];
			valueTable = (V[])new V[newSize];

			if (size > 0)
			{
				for (int i = 0; i < oldCapacity; i++)
				{
					K key = oldKeyTable[i];
					if (key != null) putResize(key, oldValueTable[i]);
				}
			}
		}

		public override int GetHashCode()
		{
			int h = size;
			K[] keyTable = this.keyTable;
			V[] valueTable = this.valueTable;
			for (int i = 0, n = keyTable.Length; i < n; i++)
			{
				K key = keyTable[i];
				if (key != null)
				{
					h += key.GetHashCode();
					V value = valueTable[i];
					if (value != null) h += value.GetHashCode();
				}
			}

			return h;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override bool Equals(Object? obj)
		{
			if (obj == this) return true;
			if (!(obj is ObjectMap<K, V>)) return false;
			ObjectMap<K, V> other = (ObjectMap<K, V>)obj;
			if (other.size != size) return false;
			K[] keyTable = this.keyTable;
			V[] valueTable = this.valueTable;
			for (int i = 0, n = keyTable.Length; i < n; i++)
			{
				K key = keyTable[i];
				if (key != null)
				{
					V value = valueTable[i];
					if (value == null)
					{
						if (other.get(key, dummy) != null) return false;
					}
					else
					{
						if (!value.Equals(other.get(key))) return false;
					}
				}
			}

			return true;
		}

		/** Uses == for comparison of each value. */
		public bool equalsIdentity(Object? obj)
		{
			if (obj == this) return true;
			if (!(obj is ObjectMap<K, V>)) return false;
			ObjectMap<K, V> other = (ObjectMap<K, V>)obj;
			if (other.size != size) return false;
			K[] keyTable = this.keyTable;
			V[] valueTable = this.valueTable;
			for (int i = 0, n = keyTable.Length; i < n; i++)
			{
				K key = keyTable[i];
				if (key != null && !ReferenceEquals(valueTable[i] , other.get(key, dummy))) return false;
			}

			return true;
		}

		public String toString(String separator)
		{
			return toString(separator, false);
		}

		public override String ToString()
		{
			return toString(", ", true);
		}

		protected virtual String toString(String separator, bool braces)
		{
			if (size == 0) return braces ? "{}" : "";
			StringBuilder buffer = new StringBuilder(32);
			if (braces) buffer.Append('{');
			K[] keyTable = this.keyTable;
			V[] valueTable = this.valueTable;
			int i = keyTable.Length;
			while (i-- > 0)
			{
				K key = keyTable[i];
				if (key == null) continue;
				buffer.Append(ReferenceEquals(key, this) ? "(this)" : key);
				buffer.Append('=');
				V value = valueTable[i];
				buffer.Append(ReferenceEquals(value, this) ? "(this)" : value);
				break;
			}

			while (i-- > 0)
			{
				K key = keyTable[i];
				if (key == null) continue;
				buffer.Append(separator);
				buffer.Append(ReferenceEquals(key, this) ? "(this)" : key);
				buffer.Append('=');
				V value = valueTable[i];
				buffer.Append(ReferenceEquals(value, this) ? "(this)" : value);
			}

			if (braces) buffer.Append('}');
			return buffer.ToString();
		}

		public IEnumerator<Entry> GetEnumerator()
		{
			return entries();
		}

		/** Returns an iterator for the entries in the map. Remove is supported.
		 * <p>
		 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
		 * Use the {@link Entries} constructor for nested or multithreaded iteration. */
		public virtual Entries entries()
		{
			if (Collections.allocateIterators) return new Entries(this);
			if (entries1 == null)
			{
				entries1 = new Entries(this);
				entries2 = new Entries(this);
			}

			if (!entries1.valid)
			{
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
		 * Use the {@link Values} constructor for nested or multithreaded iteration. */
		public virtual Values values()
		{
			if (Collections.allocateIterators) return new Values(this);
			if (values1 == null)
			{
				values1 = new Values(this);
				values2 = new Values(this);
			}

			if (!values1.valid)
			{
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
		 * Use the {@link Keys} constructor for nested or multithreaded iteration. */
		public virtual Keys keys()
		{
			if (Collections.allocateIterators) return new Keys(this);
			if (keys1 == null)
			{
				keys1 = new Keys(this);
				keys2 = new Keys(this);
			}

			if (!keys1.valid)
			{
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

		public class Entry
		{
			public K key;
			public V? value;

			public override String ToString()
			{
				return key + "=" + value;
			}
		}

		public abstract class MapIterator<K, V, I> : IEnumerable<I>, IEnumerator<I>
		{
			public bool _hasNext;

			internal readonly ObjectMap<K, V> map;
			internal int nextIndex, currentIndex;
			internal bool valid = true;

			public MapIterator(ObjectMap<K, V> map)
			{
				this.map = map;
				Reset();
			}

			public void Dispose()
			{
			}

			public abstract bool MoveNext();

			public void Reset()
			{
				currentIndex = -1;
				nextIndex = -1;
				findNextIndex();
			}

			public abstract I Current { get; }

			object IEnumerator.Current => Current;

			internal void findNextIndex()
			{
				K[] keyTable = map.keyTable;
				for (int n = keyTable.Length; ++nextIndex < n;)
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
				K[] keyTable = map.keyTable;
				V[] valueTable = map.valueTable;
				int mask = map.mask, next = i + 1 & mask;
				K key;
				while ((key = keyTable[next]) != null)
				{
					int placement = map.place(key);
					if ((next - placement & mask) > (i - placement & mask))
					{
						keyTable[i] = key;
						valueTable[i] = valueTable[next];
						i = next;
					}

					next = next + 1 & mask;
				}

				keyTable[i] = default;
				valueTable[i] = default;
				map.size--;
				if (i != currentIndex) --nextIndex;
				currentIndex = -1;
			}

			public abstract IEnumerator<I> GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public class Entries : MapIterator<K, V, Entry>
		{
			internal ObjectMap<K, V>.Entry entry = new();

			public Entries(ObjectMap<K, V> map)
				: base(map)
			{

			}

			/** Note the same entry instance is returned each time this method is called. */
			public override ObjectMap<K, V>.Entry Current
			{
				get
				{
					if (!_hasNext) throw new NoSuchElementException();
					if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
					K[] keyTable = map.keyTable;
					entry.key = keyTable[nextIndex];
					entry.value = map.valueTable[nextIndex];
					currentIndex = nextIndex;
					findNextIndex();
					return entry;
				}
			}

			public override bool MoveNext()
			{
				if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
				return _hasNext;
			}

			public override IEnumerator<Entry> GetEnumerator()
			{
				return this;
			}
		}

		public class Values : MapIterator<K, V, V>
		{
			public Values(ObjectMap<K, V> map)
				: base((ObjectMap<K, V>)map)
			{

			}

			public override bool MoveNext()
			{
				if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
				return _hasNext;
			}

			public override V? Current
			{
				get
				{
					if (!_hasNext) throw new NoSuchElementException();
					if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
					V value = map.valueTable[nextIndex];
					currentIndex = nextIndex;
					findNextIndex();
					return value;
				}
			}

			public override IEnumerator<V> GetEnumerator()
			{
				return this;
			}

			/** Returns a new array containing the remaining values. */
			public virtual Array<V> toArray()
			{
				return toArray(new Array<V>(true, map.size));
			}

			/** Adds the remaining values to the specified array. */
			public virtual Array<V> toArray(Array<V> array)
			{
				while (_hasNext)
					array.add(Current);
				return array;
			}
		}

		public class Keys : MapIterator<K, V, K>
		{
			public Keys(ObjectMap<K, V> map)
				: base((ObjectMap<K, V>)map)
			{
			}

			public override bool MoveNext()
			{
				if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
				return _hasNext;
			}

			public override K Current
			{
				get
				{
					if (!_hasNext) throw new NoSuchElementException();
					if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
					K key = map.keyTable[nextIndex];
					currentIndex = nextIndex;
					findNextIndex();
					return key;
				}
			}

			public override IEnumerator<K> GetEnumerator()
			{
				return this;
			}

			/** Returns a new array containing the remaining keys. */
			public virtual Array<K> toArray()
			{
				return toArray(new Array<K>(true, map.size));
			}

			/** Adds the remaining keys to the array. */
			public virtual Array<K> toArray(Array<K> array)
			{
				while (_hasNext)
					array.add(Current);
				return array;
			}
		}
	}
}