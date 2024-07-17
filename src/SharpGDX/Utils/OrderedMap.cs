using SharpGDX.Shims;
using System.Text;
using SharpGDX;

namespace SharpGDX.Utils
{
	/** An {@link ObjectMap} that also stores keys in an {@link Array} using the insertion order. Null keys are not allowed. No
 * allocation is done except when growing the table size.
 * <p>
 * Iteration over the {@link #entries()}, {@link #keys()}, and {@link #values()} is ordered and faster than an unordered map. Keys
 * can also be accessed and the order changed using {@link #orderedKeys()}. There is some additional overhead for put and remove.
 * When used for faster iteration versus ObjectMap and the order does not actually matter, copying during remove can be greatly
 * reduced by setting {@link Array#ordered} to false for {@link OrderedMap#orderedKeys()}.
 * <p>
 * This class performs fast contains (typically O(1), worst case O(n) but that is rare in practice). Remove is somewhat slower due
 * to {@link #orderedKeys()}. Add may be slightly slower, depending on hash collisions. Hashcodes are rehashed to reduce
 * collisions and the need to resize. Load factors greater than 0.91 greatly increase the chances to resize to the next higher POT
 * size.
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
public class OrderedMap<K, V> : ObjectMap<K, V> {
	readonly Array<K> _keys;

	/** Creates a new map with an initial capacity of 51 and a load factor of 0.8. */
	public OrderedMap () {
		_keys = new ();
	}

	/** Creates a new map with a load factor of 0.8.
	 * @param initialCapacity The backing array size is initialCapacity / loadFactor, increased to the next power of two. */
	public OrderedMap (int initialCapacity)
	:base(initialCapacity){
		
		_keys = new (initialCapacity);
	}

	/** Creates a new map with the specified initial capacity and load factor. This map will hold initialCapacity items before
	 * growing the backing table.
	 * @param initialCapacity The backing array size is initialCapacity / loadFactor, increased to the next power of two. */
	public OrderedMap (int initialCapacity, float loadFactor) 
	: base(initialCapacity, loadFactor)
	{
		
		_keys = new(initialCapacity);
	}

	/** Creates a new map containing the items in the specified map. */
	public OrderedMap (OrderedMap< K,  V> map) 
	: base(map)
	{
		
		_keys = new (map._keys);
	}

		public override V? put (K key, V value)
	{
		throw new NotImplementedException();
		//int i = locateKey(key);
		//if (i >= 0) { // Existing key was found.
		//	V oldValue = valueTable[i];
		//	valueTable[i] = value;
		//	return oldValue;
		//}
		//i = -(i + 1); // Empty space was found.
		//keyTable[i] = key;
		//valueTable[i] = value;
		//_keys.add(key);
		//if (++size >= threshold) resize(keyTable.Length << 1);
		//return default;
	}

	public void putAll<T> (OrderedMap<T,  V> map) 
	where T: K{
		ensureCapacity(map.size);
		var keys = map._keys.items;
		for (int i = 0, n = map._keys.size; i < n; i++) {
			K key = keys[i];
			put(key, map.get((T)key));
		}
	}

		public override V remove (K key) {
		_keys.removeValue(key, false);
		return base.remove(key);
	}

	public V removeIndex (int index) {
		return base.remove(_keys.removeIndex(index));
	}

	/** Changes the key {@code before} to {@code after} without changing its position in the order or its value. Returns true if
	 * {@code after} has been added to the OrderedMap and {@code before} has been removed; returns false if {@code after} is
	 * already present or {@code before} is not present. If you are iterating over an OrderedMap and have an index, you should
	 * prefer {@link #alterIndex(int, Object)}, which doesn't need to search for an index like this does and so can be faster.
	 * @param before a key that must be present for this to succeed
	 * @param after a key that must not be in this map for this to succeed
	 * @return true if {@code before} was removed and {@code after} was added, false otherwise */
	public bool alter (K before, K after) {
		if (containsKey(after)) return false;
		int index = _keys.indexOf(before, false);
		if (index == -1) return false;
		base.put(after, base.remove(before));
		_keys.set(index, after);
		return true;
	}

	/** Changes the key at the given {@code index} in the order to {@code after}, without changing the ordering of other entries or
	 * any values. If {@code after} is already present, this returns false; it will also return false if {@code index} is invalid
	 * for the size of this map. Otherwise, it returns true. Unlike {@link #alter(Object, Object)}, this operates in constant time.
	 * @param index the index in the order of the key to change; must be non-negative and less than {@link #size}
	 * @param after the key that will replace the contents at {@code index}; this key must not be present for this to succeed
	 * @return true if {@code after} successfully replaced the key at {@code index}, false otherwise */
	public bool alterIndex (int index, K after) {
		if (index < 0 || index >= size || containsKey(after)) return false;
		base.put(after, base.remove(_keys.get(index)));
		_keys.set(index, after);
		return true;
	}

		public override void clear (int maximumCapacity) {
		_keys.clear();
		base.clear(maximumCapacity);
	}

		public override void clear () {
		_keys.clear();
		base.clear();
	}

	public Array<K> orderedKeys () {
		return _keys;
	}

	public Entries iterator () {
		return entries();
	}

		/** Returns an iterator for the entries in the map. Remove is supported.
		 * <p>
		 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
		 * Use the {@link OrderedMapEntries} constructor for nested or multithreaded iteration. */
		public override Entries entries () {
		if (Collections.allocateIterators) return new OrderedMapEntries(this);
		if (entries1 == null) {
			entries1 = new OrderedMapEntries(this);
			entries2 = new OrderedMapEntries(this);
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
		 * Use the {@link OrderedMapValues} constructor for nested or multithreaded iteration. */
		public override Values values () {
		if (Collections.allocateIterators) return new (this);
		if (values1 == null) {
			values1 = new (this);
			values2 = new (this);
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
		 * Use the {@link OrderedMapKeys} constructor for nested or multithreaded iteration. */
		public override Keys keys () {
		if (Collections.allocateIterators) return new (this);
		if (keys1 == null) {
			keys1 = new (this);
			keys2 = new (this);
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

		protected override String toString (String separator, bool braces) {
		if (size == 0) return braces ? "{}" : "";
		StringBuilder buffer = new StringBuilder(32);
		if (braces) buffer.Append('{');
		Array<K> keys = this._keys;
		for (int i = 0, n = keys.size; i < n; i++) {
			K key = keys.get(i);
			if (i > 0) buffer.Append(separator);
			buffer.Append(ReferenceEquals(key , this) ? "(this)" : key);
			buffer.Append('=');
			V value = get(key);
			buffer.Append(ReferenceEquals(value , this) ? "(this)" : value);
		}
		if (braces) buffer.Append('}');
		return buffer.ToString();
	}

	 public class OrderedMapEntries : Entries {
		private Array<K> keys;

		public OrderedMapEntries (OrderedMap<K, V> map) 
		: base(map)
		{
			
			keys = map._keys;
		}

		public void reset () {
			currentIndex = -1;
			nextIndex = 0;
			_hasNext = map.size > 0;
		}

		public Entry next () {
			if (!_hasNext) throw new NoSuchElementException();
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			currentIndex = nextIndex;
			entry.key = keys.get(nextIndex);
			entry.value = map.get(entry.key);
			nextIndex++;
			_hasNext = nextIndex < map.size;
			return entry;
		}

			public override void remove () {
			if (currentIndex < 0) throw new IllegalStateException("next must be called before remove.");
			map.remove(entry.key);
			nextIndex--;
			currentIndex = -1;
		}
	}

	public class OrderedMapKeys : Keys {
		private Array<K> keys;

		public OrderedMapKeys (OrderedMap<K, V> map) 
		: base(map)
		{
			keys = map._keys;
		}

		public void reset () {
			currentIndex = -1;
			nextIndex = 0;
			_hasNext = map.size > 0;
		}

		public K next () {
			if (!_hasNext) throw new NoSuchElementException();
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			K key = keys.get(nextIndex);
			currentIndex = nextIndex;
			nextIndex++;
			_hasNext = nextIndex < map.size;
			return key;
		}

			public override void remove () {
			throw new NotImplementedException();
				//if (currentIndex < 0) throw new IllegalStateException("next must be called before remove.");
				//((OrderedMap)map).removeIndex(currentIndex);
				//nextIndex = currentIndex;
				//currentIndex = -1;
			}

			public override Array<K> toArray (Array<K> array) {
			array.addAll(keys, nextIndex, keys.size - nextIndex);
			nextIndex = keys.size;
			_hasNext = false;
			return array;
		}

			public override Array<K> toArray () {
			return toArray(new (true, keys.size - nextIndex));
		}
	}

	public class OrderedMapValues : Values {
		private Array<K> keys;

		public OrderedMapValues (OrderedMap<K, V> map) 
		: base(map)
			{
			
			keys = map._keys;
		}

		public void reset () {
			currentIndex = -1;
			nextIndex = 0;
			_hasNext = map.size > 0;
		}

		public V next () {
			throw new NotImplementedException();
			//	if (!_hasNext) throw new NoSuchElementException();
			//if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			//var value = map.get(keys.get(nextIndex));
			//currentIndex = nextIndex;
			//nextIndex++;
			//_hasNext = nextIndex < map.size;
			//return value;
		}

			public override void remove ()
		{
			throw new NotImplementedException();
			//if (currentIndex < 0) throw new IllegalStateException("next must be called before remove.");
			//((OrderedMap<K, V>)map).removeIndex(currentIndex);
			//nextIndex = currentIndex;
			//currentIndex = -1;
		}

			public override Array<V> toArray (Array<V> array) {
			throw new NotImplementedException();
			//	int n = this.keys.size;
			//array.ensureCapacity(n - nextIndex);
			//var keys = this.keys.items;
			//for (int i = nextIndex; i < n; i++)
			//	array.add(map.get(this.keys[i]));
			//currentIndex = n - 1;
			//nextIndex = n;
			//_hasNext = false;
			//return array;
		}

			public override Array<V> toArray () {
			return toArray(new (true, keys.size - nextIndex));
		}
	}
}
}
