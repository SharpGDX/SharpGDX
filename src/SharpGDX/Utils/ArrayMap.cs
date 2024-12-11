using System;
using System.Collections;
using SharpGDX.Mathematics;
using SharpGDX.Shims;
using SharpGDX.Utils.Reflect;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils;

/** An ordered or unordered map of objects. This implementation uses arrays to store the keys and values, which means
 * {@link #getKey(Object, boolean) gets} do a comparison for each key in the map. This is slower than a typical hash map
 * implementation, but may be acceptable for small maps and has the benefits that keys and values can be accessed by index, which
 * makes iteration fast. Like {@link Array}, if ordered is false, this class avoids a memory copy when removing elements (the last
 * element is moved to the removed element's position).
 * @author Nathan Sweet */
public class ArrayMap<K, V> : IEnumerable<ObjectMap<K, V>.Entry> {
	public K[] _keys;
	public V[] _values;
	public int size;
	public bool ordered;

	[NonSerialized]
	private Entries entries1, entries2;
    [NonSerialized]
    private Values<V> values1, values2;
    [NonSerialized]
    private Keys<K> keys1, keys2;

	/** Creates an ordered map with a capacity of 16. */
	public ArrayMap () 
    : this(true, 16)
    {
		
	}

	/** Creates an ordered map with the specified capacity. */
	public ArrayMap (int capacity) 
    : this(true, capacity)
    {
		
	}

	/** @param ordered If false, methods that remove elements may change the order of other elements in the arrays, which avoids a
	 *           memory copy.
	 * @param capacity Any elements added beyond this will cause the backing arrays to be grown. */
	public ArrayMap (bool ordered, int capacity) {
		this.ordered = ordered;
		_keys = (K[])new K[capacity];
        _values = (V[])new V[capacity];
	}

	/** Creates a new map with {@link #keys} and {@link #values} of the specified type.
	 * @param ordered If false, methods that remove elements may change the order of other elements in the arrays, which avoids a
	 *           memory copy.
	 * @param capacity Any elements added beyond this will cause the backing arrays to be grown. */
	public ArrayMap (bool ordered, int capacity, Type keyArrayType, Type valueArrayType) {
		this.ordered = ordered;
        _keys = (K[])ArrayReflection.newInstance(keyArrayType, capacity);
        _values = (V[])ArrayReflection.newInstance(valueArrayType, capacity);
	}

	/** Creates an ordered map with {@link #keys} and {@link #values} of the specified type and a capacity of 16. */
	public ArrayMap (Type keyArrayType, Type valueArrayType) 
    : this(false, 16, keyArrayType, valueArrayType)
    {
		
	}

	/** Creates a new map containing the elements in the specified map. The new map will have the same type of backing arrays and
	 * will be ordered if the specified map is ordered. The capacity is set to the number of elements, so any subsequent elements
	 * added will cause the backing arrays to be grown. */
	public ArrayMap (ArrayMap<K,V> array) 
    : this(array.ordered, array.size, array._keys.GetType().GetElementType(), array._values.GetType().GetElementType())
    {
		// TODO: Verify that GetType().GetElementType() is correct.
		size = array.size;
		Array.Copy(array._keys, 0, _keys, 0, size);
		Array.Copy(array._values, 0, _values, 0, size);
	}

	public int put (K key, V value) {
		int index = indexOfKey(key);
		if (index == -1) {
			if (size == _keys.Length) resize(Math.Max(8, (int)(size * 1.75f)));
			index = size++;
		}
        _keys[index] = key;
        _values[index] = value;
		return index;
	}

	public int put (K key, V value, int index) {
		int existingIndex = indexOfKey(key);
		if (existingIndex != -1)
			removeIndex(existingIndex);
		else if (size == _keys.Length) //
			resize(Math.Max(8, (int)(size * 1.75f)));
		Array.Copy(_keys, index, _keys, index + 1, size - index);
		Array.Copy(_values, index, _values, index + 1, size - index);
        _keys[index] = key;
        _values[index] = value;
		size++;
		return index;
	}

	public void putAll (ArrayMap< K, V> map) {
		putAll(map, 0, map.size);
	}

	public void putAll (ArrayMap< K,  V> map, int offset, int length) {
		if (offset + length > map.size)
			throw new IllegalArgumentException("offset + length must be <= size: " + offset + " + " + length + " <= " + map.size);
		int sizeNeeded = size + length - offset;
		if (sizeNeeded >= _keys.Length) resize(Math.Max(8, (int)(sizeNeeded * 1.75f)));
		Array.Copy(map._keys, offset, _keys, size, length);
		Array.Copy(map._values, offset, _values, size, length);
		size += length;
	}

	/** Returns the value (which may be null) for the specified key, or null if the key is not in the map. Note this does a
	 * .equals() comparison of each key in reverse order until the specified key is found. */
	public V? get (K key) {
		return get(key, default);
	}

	/** Returns the value (which may be null) for the specified key, or the default value if the key is not in the map. Note this
	 * does a .equals() comparison of each key in reverse order until the specified key is found. */
	public V? get (K key,  V? defaultValue) {
		var keys = this._keys;
		int i = size - 1;
		if (key == null) {
			for (; i >= 0; i--)
				if (ReferenceEquals(keys[i] , key)) return _values[i];
		} else {
			for (; i >= 0; i--)
				if (key.Equals(keys[i])) return _values[i];
		}
		return defaultValue;
	}

	/** Returns the key for the specified value. Note this does a comparison of each value in reverse order until the specified
	 * value is found.
	 * @param identity If true, == comparison will be used. If false, .equals() comparison will be used. */
	public K? getKey (V value, bool identity) {
		var values = this._values;
		int i = size - 1;
		if (identity || value == null) {
			for (; i >= 0; i--)
				if (ReferenceEquals(values[i] , value)) return _keys[i];
		} else {
			for (; i >= 0; i--)
				if (value.Equals(values[i])) return _keys[i];
		}
		return default;
	}

	public K getKeyAt (int index) {
		if (index >= size)
        {
            // TODO: was String.valueOf(index). This should be ok? -RP
            throw new IndexOutOfBoundsException((index).ToString());
        }
        return _keys[index];
	}

	public V getValueAt (int index) {
		if (index >= size)
        {
            // TODO: was String.valueOf(index). This should be ok? -RP
            throw new IndexOutOfBoundsException((index).ToString());
        }
        return _values[index];
	}

	public K firstKey () {
		if (size == 0) throw new IllegalStateException("Map is empty.");
		return _keys[0];
	}

	public V firstValue () {
		if (size == 0) throw new IllegalStateException("Map is empty.");
		return _values[0];
	}

	public void setKey (int index, K key) {
		if (index >= size)
        {
            // TODO: was String.valueOf(index). This should be ok? -RP
            throw new IndexOutOfBoundsException((index).ToString());
        }
        _keys[index] = key;
	}

	public void setValue (int index, V value) {
		if (index >= size)
        {
            // TODO: was String.valueOf(index). This should be ok? -RP
            throw new IndexOutOfBoundsException((index).ToString());
        }
        _values[index] = value;
	}

	public void insert (int index, K key, V value) {
        if (index > size)
        {
            // TODO: was String.valueOf(index). This should be ok? -RP
            throw new IndexOutOfBoundsException((index).ToString());
        }
		if (size == _keys.Length) resize(Math.Max(8, (int)(size * 1.75f)));
		if (ordered) {
			Array.Copy(_keys, index, _keys, index + 1, size - index);
			Array.Copy(_values, index, _values, index + 1, size - index);
		} else {
            _keys[size] = _keys[index];
            _values[size] = _values[index];
		}
		size++;
        _keys[index] = key;
        _values[index] = value;
	}

	public bool containsKey (K key) {
		K[] keys = this._keys;
		int i = size - 1;
		if (key == null) {
			while (i >= 0)
				if (ReferenceEquals(keys[i--] , key)) return true;
		} else {
			while (i >= 0)
				if (key.Equals(keys[i--])) return true;
		}
		return false;
	}

	/** @param identity If true, == comparison will be used. If false, .equals() comparison will be used. */
	public bool containsValue (V value, bool identity) {
		V[] values = this._values;
		int i = size - 1;
		if (identity || value == null) {
			while (i >= 0)
				if (ReferenceEquals(values[i--] , value)) return true;
		} else {
			while (i >= 0)
				if (value.Equals(values[i--])) return true;
		}
		return false;
	}

	public int indexOfKey (K key) {
		var keys = this._keys;
		if (key == null) {
			for (int i = 0, n = size; i < n; i++)
				if (ReferenceEquals(keys[i] , key)) return i;
		} else {
			for (int i = 0, n = size; i < n; i++)
				if (key.Equals(keys[i])) return i;
		}
		return -1;
	}

	public int indexOfValue (V value, bool identity) {
		var values = this._values;
		if (identity || value == null) {
			for (int i = 0, n = size; i < n; i++)
				if (ReferenceEquals(values[i] , value)) return i;
		} else {
			for (int i = 0, n = size; i < n; i++)
				if (value.Equals(values[i])) return i;
		}
		return -1;
	}

	public V? removeKey (K key) {
		var keys = this._keys;
		if (key == null) {
			for (int i = 0, n = size; i < n; i++) {
				if (ReferenceEquals(keys[i] , key)) {
					V value = _values[i];
					removeIndex(i);
					return value;
				}
			}
		} else {
			for (int i = 0, n = size; i < n; i++) {
				if (key.Equals(keys[i])) {
					V value = _values[i];
					removeIndex(i);
					return value;
				}
			}
		}
		return default;
	}

	public bool removeValue (V value, bool identity) {
		var values = this._values;
		if (identity || value == null) {
			for (int i = 0, n = size; i < n; i++) {
				if (object.ReferenceEquals(values[i] , value)) {
					removeIndex(i);
					return true;
				}
			}
		} else {
			for (int i = 0, n = size; i < n; i++) {
				if (value.Equals(values[i])) {
					removeIndex(i);
					return true;
				}
			}
		}
		return false;
	}

	/** Removes and returns the key/values pair at the specified index. */
	public void removeIndex (int index) {
        if (index >= size)
        {
            // TODO: was String.valueOf(index). This should be ok? -RP
            throw new IndexOutOfBoundsException((index).ToString());
        }
		var keys = this._keys;
		size--;
		if (ordered) {
			Array.Copy(keys, index + 1, keys, index, size - index);
			Array.Copy(_values, index + 1, _values, index, size - index);
		} else {
			keys[index] = keys[size];
            _values[index] = _values[size];
		}
        _keys[size] = default;
        _values[size] = default;
	}

	/** Returns true if the map has one or more items. */
	public bool notEmpty () {
		return size > 0;
	}

	/** Returns true if the map is empty. */
	public bool isEmpty () {
		return size == 0;
	}

	/** Returns the last key. */
	public K peekKey () {
		return _keys[size - 1];
	}

	/** Returns the last value. */
	public V peekValue () {
		return _values[size - 1];
	}

	/** Clears the map and reduces the size of the backing arrays to be the specified capacity if they are larger. */
	public void clear (int maximumCapacity) {
		if (_keys.Length <= maximumCapacity) {
			clear();
			return;
		}
		size = 0;
		resize(maximumCapacity);
	}

	public void clear () {
		Array.Fill(_keys, default, 0, size);
		Array.Fill(_values, default, 0, size);
		size = 0;
	}

	/** Reduces the size of the backing arrays to the size of the actual number of entries. This is useful to release memory when
	 * many items have been removed, or if it is known that more entries will not be added. */
	public void shrink () {
		if (_keys.Length == size) return;
		resize(size);
	}

	/** Increases the size of the backing arrays to accommodate the specified number of additional entries. Useful before adding
	 * many entries to avoid multiple backing array resizes. */
	public void ensureCapacity (int additionalCapacity) {
		if (additionalCapacity < 0) throw new IllegalArgumentException("additionalCapacity must be >= 0: " + additionalCapacity);
		int sizeNeeded = size + additionalCapacity;
		if (sizeNeeded > _keys.Length) resize(Math.Max(Math.Max(8, sizeNeeded), (int)(size * 1.75f)));
	}

	protected void resize (int newSize) {
        // TODO: Verify that GetType().GetElementType() is correct.
        K[] newKeys = (K[])ArrayReflection.newInstance(_keys.GetType().GetElementType(), newSize);
		Array.Copy(_keys, 0, newKeys, 0, Math.Min(size, newKeys.Length));
		this._keys = newKeys;

        // TODO: Verify that GetType().GetElementType() is correct.
        V[] newValues = (V[])ArrayReflection.newInstance(_values.GetType().GetElementType(), newSize);
		Array.Copy(_values, 0, newValues, 0, Math.Min(size, newValues.Length));
		this._values = newValues;
	}

	public void reverse () {
		for (int i = 0, lastIndex = size - 1, n = size / 2; i < n; i++) {
			int ii = lastIndex - i;
			K tempKey = _keys[i];
            _keys[i] = _keys[ii];
            _keys[ii] = tempKey;

			V tempValue = _values[i];
            _values[i] = _values[ii];
            _values[ii] = tempValue;
		}
	}

	public void shuffle () {
		for (int i = size - 1; i >= 0; i--) {
			int ii = MathUtils.random(i);
			K tempKey = _keys[i];
            _keys[i] = _keys[ii];
            _keys[ii] = tempKey;

			V tempValue = _values[i];
            _values[i] = _values[ii];
            _values[ii] = tempValue;
		}
	}

	/** Reduces the size of the arrays to the specified size. If the arrays are already smaller than the specified size, no action
	 * is taken. */
	public void truncate (int newSize) {
		if (size <= newSize) return;
		for (int i = newSize; i < size; i++) {
            _keys[i] = default;
            _values[i] = default;
		}
		size = newSize;
	}

	public override int GetHashCode () {
		K[] keys = this._keys;
		V[] values = this._values;
		int h = 0;
		for (int i = 0, n = size; i < n; i++) {
			K key = keys[i];
			V value = values[i];
			if (key != null) h += key.GetHashCode() * 31;
			if (value != null) h += value.GetHashCode();
		}
		return h;
	}

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override bool Equals (Object? obj) {
		if (obj == this) return true;
		if (!(obj is ArrayMap<K,V>)) return false;
		ArrayMap<K,V> other = (ArrayMap<K,V>)obj;
		if (other.size != size) return false;
		K[] keys = this._keys;
		V[] values = this._values;
		for (int i = 0, n = size; i < n; i++) {
			K key = keys[i];
			V value = values[i];
			if (value == null) {
				if (other.get(key, ObjectMap<K,V>.dummy) != null) return false;
			} else {
				if (!value.Equals(other.get(key))) return false;
			}
		}
		return true;
	}

	/** Uses == for comparison of each value. */
	public bool equalsIdentity (Object obj) {
		if (obj == this) return true;
		if (!(obj is ArrayMap<K,V>)) return false;
		ArrayMap<K, V> other = (ArrayMap<K, V>)obj;
		if (other.size != size) return false;
		K[] keys = this._keys;
		V[] values = this._values;
        for (int i = 0, n = size; i < n; i++)
        {
			// TODO: Should this be a reference comparison? -RP
            if (!ReferenceEquals(values[i], other.get(keys[i], ObjectMap<K, V>.dummy)))
            {
                return false;
            }
        }
		return true;
	}

	public String toString () {
		if (size == 0) return "{}";
		K[] keys = this._keys;
		V[] values = this._values;
		StringBuilder buffer = new StringBuilder(32);
		buffer.Append('{');
		buffer.Append(keys[0]);
		buffer.Append('=');
		buffer.Append(values[0]);
		for (int i = 1; i < size; i++) {
			buffer.Append(", ");
			buffer.Append(keys[i]);
			buffer.Append('=');
			buffer.Append(values[i]);
		}
		buffer.Append('}');
		return buffer.ToString();
	}

	public IEnumerator<ObjectMap<K, V>.Entry> GetEnumerator () {
		return entries();
	}

	/** Returns an iterator for the entries in the map. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link Entries} constructor for nested or multithreaded iteration.
	 * @see Collections#allocateIterators */
	public Entries entries () {
		if (Collections.allocateIterators) return new Entries(this);
		if (entries1 == null) {
			entries1 = new Entries(this);
			entries2 = new Entries(this);
		}
		if (!entries1.valid) {
			entries1.index = 0;
			entries1.valid = true;
			entries2.valid = false;
			return entries1;
		}
		entries2.index = 0;
		entries2.valid = true;
		entries1.valid = false;
		return entries2;
	}

	/** Returns an iterator for the values in the map. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link Entries} constructor for nested or multithreaded iteration.
	 * @see Collections#allocateIterators */
	public Values<V> values () {
		if (Collections.allocateIterators) return new Values<V>(this);
		if (values1 == null) {
			values1 = new Values<V>(this);
			values2 = new Values<V>(this);
		}
		if (!values1.valid) {
			values1.index = 0;
			values1.valid = true;
			values2.valid = false;
			return values1;
		}
		values2.index = 0;
		values2.valid = true;
		values1.valid = false;
		return values2;
	}

	/** Returns an iterator for the keys in the map. Remove is supported.
	 * <p>
	 * If {@link Collections#allocateIterators} is false, the same iterator instance is returned each time this method is called.
	 * Use the {@link Entries} constructor for nested or multithreaded iteration.
	 * @see Collections#allocateIterators */
	public Keys<K> keys () {
		if (Collections.allocateIterators) return new Keys<K>(this);
		if (keys1 == null) {
			keys1 = new Keys<K>(this);
			keys2 = new Keys<K>(this);
		}
		if (!keys1.valid) {
			keys1.index = 0;
			keys1.valid = true;
			keys2.valid = false;
			return keys1;
		}
		keys2.index = 0;
		keys2.valid = true;
		keys1.valid = false;
		return keys2;
	}

	public class Entries : IEnumerable<ObjectMap<K, V>.Entry>, IEnumerator<ObjectMap<K, V>.Entry> {
		private readonly ArrayMap<K, V> map;
        ObjectMap<K, V>.Entry entry = new ObjectMap<K, V>.Entry();
		internal int index;
		internal bool valid = true;

		public Entries (ArrayMap<K, V> map) {
			this.map = map;
		}

		public bool MoveNext () {
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			return index < map.size;
		}

		public IEnumerator<ObjectMap<K, V>.Entry> GetEnumerator () {
			return this;
		}

        /** Note the same entry instance is returned each time this method is called. */
        public ObjectMap<K, V>.Entry Current
        {
            get
            {
                if (index >= map.size)
                {
                    // TODO: was String.valueOf(index). This should be ok? -RP
                    throw new NoSuchElementException((index).ToString());
                }

                if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
                entry.key = map._keys[index];
                entry.value = map._values[index++];
                return entry;
            }
        }

		public void Dispose(){}

        public void Remove () {
			index--;
			map.removeIndex(index);
		}

		public void Reset () {
			index = 0;
		}

        object? IEnumerator.Current => Current;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

	public class Values<V> : IEnumerable<V>, IEnumerator<V> {
		private readonly ArrayMap<K, V> map;
		internal int index;
		internal bool valid = true;

		public Values (ArrayMap<K, V> map) {
			this.map = map;
		}

		public bool MoveNext () {
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			return index < map.size;
		}

		public IEnumerator<V> GetEnumerator() {
			return this;
		}

        public V Current
        {
            get
            {
                if (index >= map.size)
                {
                    // TODO: was String.valueOf(index). This should be ok? -RP
                    throw new NoSuchElementException((index).ToString());
                }

                if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
                return map._values[index++];
            }
        }

        public void Dispose(){}

		public void Remove () {
			index--;
			map.removeIndex(index);
		}

		public void Reset () {
			index = 0;
		}

        object? IEnumerator.Current => Current;

        public Array<V> toArray () {
			return new Array<V>(true, map._values, index, map.size - index);
		}

		public Array<V> toArray (Array<V> array) {
			array.addAll(map._values, index, map.size - index);
			return array;
		}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

	public class Keys<K> : IEnumerable<K>, IEnumerator<K> {
		private readonly ArrayMap<K, V> map;
		internal int index;
		internal bool valid = true;

		public Keys (ArrayMap<K, V> map) {
			this.map = map;
		}

		public bool MoveNext () {
			if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
			return index < map.size;
		}

		public IEnumerator<K> GetEnumerator() {
			return this;
		}

		public void Dispose(){}

		public K Current {
            get
            {
				// TODO: was String.valueOf(index). This should be ok? -RP
                if (index >= map.size) throw new NoSuchElementException((index).ToString());
                if (!valid) throw new GdxRuntimeException("#iterator() cannot be used nested.");
                return map._keys[index++];
            }
        }

		public void Remove () {
			index--;
			map.removeIndex(index);
		}

		public void Reset () {
			index = 0;
		}

        object? IEnumerator.Current => Current;

        public Array<K> toArray () {
			return new Array<K>(true, map._keys, index, map.size - index);
		}

		public Array<K> toArray (Array<K> array) {
			array.addAll(map._keys, index, map.size - index);
			return array;
		}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}