using System;
using OutputType = SharpGDX.Utils.JsonWriter.OutputType;
using System.Collections;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** Container for a JSON object, array, string, double, long, boolean, or null.
 * <p>
 * JsonValue children are a linked list. Iteration of arrays or objects is easily done using an iterator or the {@link #next()}
 * field, both shown below. This is much more efficient than accessing children by index when there are many children.<br>
 *
 * <pre>
 * JsonValue map = ...;
 * // Allocates an iterator:
 * for (JsonValue entry : map)
 * 	System.out.println(entry.name + " = " + entry.asString());
 * // No allocation:
 * for (JsonValue entry = map.child; entry != null; entry = entry.next)
 * 	System.out.println(entry.name + " = " + entry.asString());
 * </pre>
 *
 * @author Nathan Sweet */
	public class JsonValue : IEnumerable<JsonValue>
	{
		private ValueType _type;

		/** May be null. */
		private String? stringValue;

		private double doubleValue;
		private long longValue;

		public String? _name;

		/** May be null. */
		public JsonValue? _child, _parent;

		/** May be null. When changing this field the parent {@link #size()} may need to be changed. */
		public JsonValue? _next, _prev;

		public int _size;

		public JsonValue(ValueType type)
		{
			this._type = type;
		}

		/** @param value May be null. */
		public JsonValue(String? value)
		{
			set(value);
		}

		public JsonValue(double value)
		{
			set(value, null);
		}

		public JsonValue(long value)
		{
			set(value, null);
		}

		public JsonValue(double value, String stringValue)
		{
			set(value, stringValue);
		}

		public JsonValue(long value, String stringValue)
		{
			set(value, stringValue);
		}

		public JsonValue(bool value)
		{
			set(value);
		}

		/** Returns the child at the specified index. This requires walking the linked list to the specified entry, see
		 * {@link JsonValue} for how to iterate efficiently.
		 * @return May be null. */
		public JsonValue? get(int index)
		{
			JsonValue? current = _child;
			while (current != null && index > 0)
			{
				index--;
				current = current._next;
			}

			return current;
		}

		/** Returns the child with the specified name.
		 * @return May be null. */
		public JsonValue? get(String name)
		{
			JsonValue? current = _child;
			// TODO: Not sure what type of '__IgnoreCase' the original call, equalsIgnoreCase(), is equal to.
			while (current != null && (current._name == null ||
			                           !current._name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
				current = current._next;
			return current;
		}

		/** Returns true if a child with the specified name exists. */
		public bool has(String name)
		{
			return get(name) != null;
		}

		/** Returns an iterator for the child with the specified name, or an empty iterator if no child is found. */
		public JsonIterator iterator(String name)
		{
			throw new NotImplementedException();
			// TODO: JsonValue current = get(name);
			//	if (current == null) {
			//	JsonIterator iter = new JsonIterator();
			//	iter.entry = null;
			//	return iter;
			//}
			//return current.iterator();
		}

		/** Returns the child at the specified index. This requires walking the linked list to the specified entry, see
		 * {@link JsonValue} for how to iterate efficiently.
		 * @throws IllegalArgumentException if the child was not found. */
		public JsonValue require(int index)
		{
			JsonValue? current = get(index);
			if (current == null) throw new IllegalArgumentException("Child not found with index: " + index);
			return current;
		}

		/** Returns the child with the specified name.
		 * @throws IllegalArgumentException if the child was not found. */
		public JsonValue require(String name)
		{
			JsonValue? current = get(name);
			if (current == null) throw new IllegalArgumentException("Child not found with name: " + name);
			return current;
		}

		/** Removes the child with the specified index. This requires walking the linked list to the specified entry, see
		 * {@link JsonValue} for how to iterate efficiently.
		 * @return May be null. */
		public JsonValue? remove(int index)
		{
			JsonValue? child = get(index);
			if (child == null) return null;
			if (child._prev == null)
			{
				this._child = child._next;
				if (this._child != null) this._child._prev = null;
			}
			else
			{
				child._prev._next = child._next;
				if (child._next != null) child._next._prev = child._prev;
			}

			_size--;
			return child;
		}

		/** Removes the child with the specified name.
		 * @return May be null. */
		public JsonValue? remove(String name)
		{
			JsonValue? child = get(name);
			if (child == null) return null;
			if (child._prev == null)
			{
				this._child = child._next;
				if (this._child != null) this._child._prev = null;
			}
			else
			{
				child._prev._next = child._next;
				if (child._next != null) child._next._prev = child._prev;
			}

			_size--;
			return child;
		}

		/** Removes this value from its parent. */
		public void remove()
		{
			if (_parent == null) throw new IllegalStateException();
			if (_prev == null)
			{
				_parent._child = _next;
				if (_parent._child != null) _parent._child._prev = null;
			}
			else
			{
				_prev._next = _next;
				if (_next != null) _next._prev = _prev;
			}

			_parent._size--;
		}

		/** Returns true if there are one or more children in the array or object. */
		public bool notEmpty()
		{
			return _size > 0;
		}

		/** Returns true if there are not children in the array or object. */
		public bool isEmpty()
		{
			return _size == 0;
		}

		/** Returns this value as a string.
		 * @return May be null if this value is null.
		 * @throws IllegalStateException if this an array or object. */
		public String? asString()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					return stringValue;
				case ValueType.doubleValue:
					return stringValue != null ? stringValue : (doubleValue).ToString();
				case ValueType.longValue:
					return stringValue != null ? stringValue : (longValue).ToString();
				case ValueType.booleanValue:
					return longValue != 0 ? "true" : "false";
				case ValueType.nullValue:
					return null;
			}

			throw new IllegalStateException("Value cannot be converted to string: " + _type);
		}

		/** Returns this value as a float.
		 * @throws IllegalStateException if this an array or object. */
		public float asFloat()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					return float.Parse(stringValue);
				case ValueType.doubleValue:
					return (float)doubleValue;
				case ValueType.longValue:
					return longValue;
				case ValueType.booleanValue:
					return longValue != 0 ? 1 : 0;
			}

			throw new IllegalStateException("Value cannot be converted to float: " + _type);
		}

		/** Returns this value as a double.
		 * @throws IllegalStateException if this an array or object. */
		public double asDouble()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					return double.Parse(stringValue);
				case ValueType.doubleValue:
					return doubleValue;
				case ValueType.longValue:
					return longValue;
				case ValueType.booleanValue:
					return longValue != 0 ? 1 : 0;
			}

			throw new IllegalStateException("Value cannot be converted to double: " + _type);
		}

		/** Returns this value as a long.
		 * @throws IllegalStateException if this an array or object. */
		public long asLong()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					return long.Parse(stringValue);
				case ValueType.doubleValue:
					return (long)doubleValue;
				case ValueType.longValue:
					return longValue;
				case ValueType.booleanValue:
					return longValue != 0 ? 1 : 0;
			}

			throw new IllegalStateException("Value cannot be converted to long: " + _type);
		}

		/** Returns this value as an int.
		 * @throws IllegalStateException if this an array or object. */
		public int asInt()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					return int.Parse(stringValue);
				case ValueType.doubleValue:
					return (int)doubleValue;
				case ValueType.longValue:
					return (int)longValue;
				case ValueType.booleanValue:
					return longValue != 0 ? 1 : 0;
			}

			throw new IllegalStateException("Value cannot be converted to int: " + _type);
		}

		/** Returns this value as a boolean.
		 * @throws IllegalStateException if this an array or object. */
		public bool asBoolean()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					// TODO: Not sure what IgnoreCaseType the original call was, equalsIgnoreCase. -RP
					return stringValue.Equals("true", StringComparison.CurrentCultureIgnoreCase);
				case ValueType.doubleValue:
					return doubleValue != 0;
				case ValueType.longValue:
					return longValue != 0;
				case ValueType.booleanValue:
					return longValue != 0;
			}

			throw new IllegalStateException("Value cannot be converted to boolean: " + _type);
		}

		/** Returns this value as a byte.
		 * @throws IllegalStateException if this an array or object. */
		public byte asByte()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					return byte.Parse(stringValue);
				case ValueType.doubleValue:
					return (byte)doubleValue;
				case ValueType.longValue:
					return (byte)longValue;
				case ValueType.booleanValue:
					return longValue != 0 ? (byte)1 : (byte)0;
			}

			throw new IllegalStateException("Value cannot be converted to byte: " + _type);
		}

		/** Returns this value as a short.
		 * @throws IllegalStateException if this an array or object. */
		public short asShort()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					return short.Parse(stringValue);
				case ValueType.doubleValue:
					return (short)doubleValue;
				case ValueType.longValue:
					return (short)longValue;
				case ValueType.booleanValue:
					return longValue != 0 ? (short)1 : (short)0;
			}

			throw new IllegalStateException("Value cannot be converted to short: " + _type);
		}

		/** Returns this value as a char.
		 * @throws IllegalStateException if this an array or object. */
		public char asChar()
		{
			switch (_type)
			{
				case ValueType.stringValue:
					return stringValue.Length == 0 ? (char)0 : stringValue[0];
				case ValueType.doubleValue:
					return (char)doubleValue;
				case ValueType.longValue:
					return (char)longValue;
				case ValueType.booleanValue:
					return longValue != 0 ? (char)1 : (char)0;
			}

			throw new IllegalStateException("Value cannot be converted to char: " + _type);
		}

		/** Returns the children of this value as a newly allocated String array.
		 * @throws IllegalStateException if this is not an array. */
		public String[] asStringArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			String[] array = new String[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				String v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = value.stringValue;
						break;
					case ValueType.doubleValue:
						v = stringValue != null ? stringValue : (value.doubleValue).ToString();
						break;
					case ValueType.longValue:
						v = stringValue != null ? stringValue : (value.longValue).ToString();
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0 ? "true" : "false";
						break;
					case ValueType.nullValue:
						v = null;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to string: " + value._type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns the children of this value as a newly allocated float array.
		 * @throws IllegalStateException if this is not an array. */
		public float[] asFloatArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			float[] array = new float[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				float v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = float.Parse(value.stringValue);
						break;
					case ValueType.doubleValue:
						v = (float)value.doubleValue;
						break;
					case ValueType.longValue:
						v = value.longValue;
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0 ? 1 : 0;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to float: " + value._type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns the children of this value as a newly allocated double array.
		 * @throws IllegalStateException if this is not an array. */
		public double[] asDoubleArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			double[] array = new double[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				double v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = double.Parse(value.stringValue);
						break;
					case ValueType.doubleValue:
						v = value.doubleValue;
						break;
					case ValueType.longValue:
						v = value.longValue;
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0 ? 1 : 0;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to double: " + value._type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns the children of this value as a newly allocated long array.
		 * @throws IllegalStateException if this is not an array. */
		public long[] asLongArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			long[] array = new long[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				long v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = long.Parse(value.stringValue);
						break;
					case ValueType.doubleValue:
						v = (long)value.doubleValue;
						break;
					case ValueType.longValue:
						v = value.longValue;
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0 ? 1 : 0;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to long: " + value._type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns the children of this value as a newly allocated int array.
		 * @throws IllegalStateException if this is not an array. */
		public int[] asIntArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			int[] array = new int[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				int v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = int.Parse(value.stringValue);
						break;
					case ValueType.doubleValue:
						v = (int)value.doubleValue;
						break;
					case ValueType.longValue:
						v = (int)value.longValue;
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0 ? 1 : 0;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to int: " + value._type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns the children of this value as a newly allocated boolean array.
		 * @throws IllegalStateException if this is not an array. */
		public bool[] asBooleanArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			bool[] array = new bool[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				bool v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = bool.Parse(value.stringValue);
						break;
					case ValueType.doubleValue:
						v = value.doubleValue == 0;
						break;
					case ValueType.longValue:
						v = value.longValue == 0;
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to boolean: " + value._type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns the children of this value as a newly allocated byte array.
		 * @throws IllegalStateException if this is not an array. */
		public byte[] asByteArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			byte[] array = new byte[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				byte v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = byte.Parse(value.stringValue);
						break;
					case ValueType.doubleValue:
						v = (byte)value.doubleValue;
						break;
					case ValueType.longValue:
						v = (byte)value.longValue;
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0 ? (byte)1 : (byte)0;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to byte: " + value._type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns the children of this value as a newly allocated short array.
		 * @throws IllegalStateException if this is not an array. */
		public short[] asShortArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			short[] array = new short[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				short v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = short.Parse(value.stringValue);
						break;
					case ValueType.doubleValue:
						v = (short)value.doubleValue;
						break;
					case ValueType.longValue:
						v = (short)value.longValue;
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0 ? (short)1 : (short)0;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to short: " + value._type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns the children of this value as a newly allocated char array.
		 * @throws IllegalStateException if this is not an array. */
		public char[] asCharArray()
		{
			if (_type != ValueType.array) throw new IllegalStateException("Value is not an array: " + _type);
			char[] array = new char[_size];
			int i = 0;
			for (JsonValue? value = _child; value != null; value = value._next, i++)
			{
				char v;
				switch (value._type)
				{
					case ValueType.stringValue:
						v = value.stringValue.Length == 0 ? (char)0 : value.stringValue[0];
						break;
					case ValueType.doubleValue:
						v = (char)value.doubleValue;
						break;
					case ValueType.longValue:
						v = (char)value.longValue;
						break;
					case ValueType.booleanValue:
						v = value.longValue != 0 ? (char)1 : (char)0;
						break;
					default:
						throw new IllegalStateException("Value cannot be converted to char: " + value.type);
				}

				array[i] = v;
			}

			return array;
		}

		/** Returns true if a child with the specified name exists and has a child. */
		public bool hasChild(String name)
		{
			return getChild(name) != null;
		}

		/** Finds the child with the specified name and returns its first child.
		 * @return May be null. */
		public JsonValue? getChild(String name)
		{
			JsonValue? child = get(name);
			return child == null ? null : child._child;
		}

		/** Finds the child with the specified name and returns it as a string. Returns defaultValue if not found.
		 * @param defaultValue May be null. */
		public String getString(String name, String? defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asString();
		}

		/** Finds the child with the specified name and returns it as a float. Returns defaultValue if not found. */
		public float getFloat(String name, float defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asFloat();
		}

		/** Finds the child with the specified name and returns it as a double. Returns defaultValue if not found. */
		public double getDouble(String name, double defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asDouble();
		}

		/** Finds the child with the specified name and returns it as a long. Returns defaultValue if not found. */
		public long getLong(String name, long defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asLong();
		}

		/** Finds the child with the specified name and returns it as an int. Returns defaultValue if not found. */
		public int getInt(String name, int defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asInt();
		}

		/** Finds the child with the specified name and returns it as a boolean. Returns defaultValue if not found. */
		public bool getBoolean(String name, bool defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asBoolean();
		}

		/** Finds the child with the specified name and returns it as a byte. Returns defaultValue if not found. */
		public byte getByte(String name, byte defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asByte();
		}

		/** Finds the child with the specified name and returns it as a short. Returns defaultValue if not found. */
		public short getShort(String name, short defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asShort();
		}

		/** Finds the child with the specified name and returns it as a char. Returns defaultValue if not found. */
		public char getChar(String name, char defaultValue)
		{
			JsonValue? child = get(name);
			return (child == null || !child.isValue() || child.isNull()) ? defaultValue : child.asChar();
		}

		/** Finds the child with the specified name and returns it as a string.
		 * @throws IllegalArgumentException if the child was not found. */
		public String getString(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asString();
		}

		/** Finds the child with the specified name and returns it as a float.
		 * @throws IllegalArgumentException if the child was not found. */
		public float getFloat(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asFloat();
		}

		/** Finds the child with the specified name and returns it as a double.
		 * @throws IllegalArgumentException if the child was not found. */
		public double getDouble(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asDouble();
		}

		/** Finds the child with the specified name and returns it as a long.
		 * @throws IllegalArgumentException if the child was not found. */
		public long getLong(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asLong();
		}

		/** Finds the child with the specified name and returns it as an int.
		 * @throws IllegalArgumentException if the child was not found. */
		public int getInt(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asInt();
		}

		/** Finds the child with the specified name and returns it as a boolean.
		 * @throws IllegalArgumentException if the child was not found. */
		public bool getBoolean(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asBoolean();
		}

		/** Finds the child with the specified name and returns it as a byte.
		 * @throws IllegalArgumentException if the child was not found. */
		public byte getByte(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asByte();
		}

		/** Finds the child with the specified name and returns it as a short.
		 * @throws IllegalArgumentException if the child was not found. */
		public short getShort(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asShort();
		}

		/** Finds the child with the specified name and returns it as a char.
		 * @throws IllegalArgumentException if the child was not found. */
		public char getChar(String name)
		{
			JsonValue? child = get(name);
			if (child == null) throw new IllegalArgumentException("Named value not found: " + name);
			return child.asChar();
		}

		/** Finds the child with the specified index and returns it as a string.
		 * @throws IllegalArgumentException if the child was not found. */
		public String getString(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asString();
		}

		/** Finds the child with the specified index and returns it as a float.
		 * @throws IllegalArgumentException if the child was not found. */
		public float getFloat(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asFloat();
		}

		/** Finds the child with the specified index and returns it as a double.
		 * @throws IllegalArgumentException if the child was not found. */
		public double getDouble(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asDouble();
		}

		/** Finds the child with the specified index and returns it as a long.
		 * @throws IllegalArgumentException if the child was not found. */
		public long getLong(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asLong();
		}

		/** Finds the child with the specified index and returns it as an int.
		 * @throws IllegalArgumentException if the child was not found. */
		public int getInt(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asInt();
		}

		/** Finds the child with the specified index and returns it as a boolean.
		 * @throws IllegalArgumentException if the child was not found. */
		public bool getBoolean(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asBoolean();
		}

		/** Finds the child with the specified index and returns it as a byte.
		 * @throws IllegalArgumentException if the child was not found. */
		public byte getByte(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asByte();
		}

		/** Finds the child with the specified index and returns it as a short.
		 * @throws IllegalArgumentException if the child was not found. */
		public short getShort(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asShort();
		}

		/** Finds the child with the specified index and returns it as a char.
		 * @throws IllegalArgumentException if the child was not found. */
		public char getChar(int index)
		{
			JsonValue? child = get(index);
			if (child == null) throw new IllegalArgumentException("Indexed value not found: " + name);
			return child.asChar();
		}

		public ValueType type()
		{
			return _type;
		}

		public void setType(ValueType? type)
		{
			if (type == null) throw new IllegalArgumentException("type cannot be null.");
			this._type = type.Value;
		}

		public bool isArray()
		{
			return _type == ValueType.array;
		}

		public bool isObject()
		{
			return _type == ValueType.@object;
		}

		public bool isString()
		{
			return _type == ValueType.stringValue;
		}

		/** Returns true if this is a double or long value. */
		public bool isNumber()
		{
			return _type == ValueType.doubleValue || _type == ValueType.longValue;
		}

		public bool isDouble()
		{
			return _type == ValueType.doubleValue;
		}

		public bool isLong()
		{
			return _type == ValueType.longValue;
		}

		public bool isBoolean()
		{
			return _type == ValueType.booleanValue;
		}

		public bool isNull()
		{
			return _type == ValueType.nullValue;
		}

		/** Returns true if this is not an array or object. */
		public bool isValue()
		{
			switch (_type)
			{
				case ValueType.stringValue:
				case ValueType.doubleValue:
				case ValueType.longValue:
				case ValueType.booleanValue:
				case ValueType.nullValue:
					return true;
			}

			return false;
		}

		/** Returns the name for this object value.
		 * @return May be null. */
		public String? name()
		{
			return _name;
		}

		/** @param name May be null. */
		public void setName(String? name)
		{
			this._name = name;
		}

		/** Returns the parent for this value.
		 * @return May be null. */
		public JsonValue? parent()
		{
			return _parent;
		}

		/** Returns the first child for this object or array.
		 * @return May be null. */
		public JsonValue? child()
		{
			return _child;
		}

		/** Sets the name of the specified value and adds it after the last child. */
		public void addChild(String name, JsonValue value)
		{
			if (name == null) throw new IllegalArgumentException("name cannot be null.");
			value._name = name;
			addChild(value);
		}

		/** Adds the specified value after the last child.
		 * @throws IllegalStateException if this is an object and the specified child's name is null. */
		public void addChild(JsonValue value)
		{
			if (_type == ValueType.@object && value._name == null)
				throw new IllegalStateException("An object child requires a name: " + value);
			value._parent = this;
			value._next = null;
			_size++;
			JsonValue current = _child;
			if (current == null)
			{
				value._prev = null;
				_child = value;
			}
			else
			{
				while (true)
				{
					if (current._next == null)
					{
						current._next = value;
						value._prev = current;
						return;
					}

					current = current._next;
				}
			}
		}

		/** Returns the next sibling of this value.
		 * @return May be null. */
		public JsonValue? next()
		{
			return _next;
		}

		/** Sets the next sibling of this value. Does not change the parent {@link #size()}.
		 * @param next May be null. */
		public void setNext(JsonValue? next)
		{
			this._next = next;
		}

		/** Returns the previous sibling of this value.
		 * @return May be null. */
		public JsonValue? prev()
		{
			return _prev;
		}

		/** Sets the next sibling of this value. Does not change the parent {@link #size()}.
		 * @param prev May be null. */
		public void setPrev(JsonValue? prev)
		{
			this._prev = prev;
		}

		/** @param value May be null. */
		public void set(String? value)
		{
			stringValue = value;
			_type = value == null ? ValueType.nullValue : ValueType.stringValue;
		}

		/** @param stringValue May be null if the string representation is the string value of the double (eg, no leading zeros). */
		public void set(double value, String? stringValue)
		{
			doubleValue = value;
			longValue = (long)value;
			this.stringValue = stringValue;
			_type = ValueType.doubleValue;
		}

		/** @param stringValue May be null if the string representation is the string value of the long (eg, no leading zeros). */
		public void set(long value, String? stringValue)
		{
			longValue = value;
			doubleValue = value;
			this.stringValue = stringValue;
			_type = ValueType.longValue;
		}

		public void set(bool value)
		{
			longValue = value ? 1 : 0;
			_type = ValueType.booleanValue;
		}

		public String toJson(OutputType outputType)
		{
			if (isValue()) return asString();
			StringBuilder buffer = new StringBuilder(512);
			json(this, buffer, outputType);
			return buffer.ToString();
		}

		private void json(JsonValue @object, StringBuilder buffer, OutputType outputType)
		{
			throw new NotImplementedException();
			// TODO: if (@object.isObject()) {
			//	if (@object.child == null)
			//		buffer.Append("{}");
			//	else {
			//		int start = buffer.Length;
			//		while (true) {
			//			buffer.Append('{');
			//			int i = 0;
			//			for (JsonValue child = @object._child; child != null; child = child._next) {
			//				buffer.Append(outputType.quoteName(child._name));
			//				buffer.Append(':');
			//				json(child, buffer, outputType);
			//				if (child._next != null) buffer.Append(',');
			//			}
			//			break;
			//		}
			//		buffer.Append('}');
			//	}
			//} else if (@object.isArray()) {
			//	if (@object._child == null)
			//		buffer.Append("[]");
			//	else {
			//		int start = buffer.Length;
			//		while (true) {
			//			buffer.Append('[');
			//			for (JsonValue child = @object._child; child != null; child = child._next) {
			//				json(child, buffer, outputType);
			//				if (child.next != null) buffer.Append(',');
			//			}
			//			break;
			//		}
			//		buffer.Append(']');
			//	}
			//} else if (@object.isString()) {
			//	buffer.Append(outputType.quoteValue(@object.asString()));
			//} else if (@object.isDouble()) {
			//	double doubleValue = @object.asDouble();
			//	long longValue = @object.asLong();
			//	buffer.Append(doubleValue == longValue ? longValue : doubleValue);
			//} else if (@object.isLong()) {
			//	buffer.Append(@object.asLong());
			//} else if (@object.isBoolean()) {
			//	buffer.Append(@object.asBoolean());
			//} else if (@object.isNull()) {
			//	buffer.Append("null");
			//} else
			//	throw new SerializationException("Unknown object type: " + @object);
		}

		/** Iterates the children of this array or object. */
		public JsonIterator iterator()
		{
			return new JsonIterator();
		}

		public IEnumerator<JsonValue> GetEnumerator()
		{
			return iterator();
		}

		public override String ToString()
		{
			if (isValue()) return _name == null ? asString() : _name + ": " + asString();
			return (_name == null ? "" : _name + ": ") + prettyPrint(OutputType.minimal, 0);
		}

		/** Returns a human readable string representing the path from the root of the JSON object graph to this value. */
		public String trace()
		{
			if (_parent == null)
			{
				if (_type == ValueType.array) return "[]";
				if (_type == ValueType.@object) return "{}";
				return "";
			}

			String trace;
			if (_parent._type == ValueType.array)
			{
				trace = "[]";
				int i = 0;
				for (JsonValue? child = _parent._child; child != null; child = child._next, i++)
				{
					if (child == this)
					{
						trace = "[" + i + "]";
						break;
					}
				}
			}
			else if (_name.IndexOf('.') != -1)
				trace = ".\"" + _name.Replace("\"", "\\\"") + "\"";
			else
				trace = '.' + _name;

			return _parent.trace() + trace;
		}

		public String prettyPrint(OutputType outputType, int singleLineColumns)
		{
			PrettyPrintSettings settings = new PrettyPrintSettings();
			settings.outputType = outputType;
			settings.singleLineColumns = singleLineColumns;
			return prettyPrint(settings);
		}

		public String prettyPrint(PrettyPrintSettings settings)
		{
			StringBuilder buffer = new StringBuilder(512);
			prettyPrint(this, buffer, 0, settings);
			return buffer.ToString();
		}

		private void prettyPrint(JsonValue obj, StringBuilder buffer, int indent, PrettyPrintSettings settings)
		{
			throw new NotImplementedException();
			// TODO: OutputType outputType = settings.outputType;
			//	if (obj.isObject()) {
			//	if (obj.child == null)
			//		buffer.Append("{}");
			//	else {
			//		bool newLines = !isFlat(obj);
			//		int start = buffer.Length;
			//		outer:
			//		while (true) {
			//			buffer.Append(newLines ? "{\n" : "{ ");
			//			int i = 0;
			//			for (JsonValue child = obj._child; child != null; child = child._next) {
			//				if (newLines) JsonValue.indent(indent, buffer);
			//				buffer.Append(outputType.quoteName(child.name));
			//				buffer.Append(": ");
			//				prettyPrint(child, buffer, indent + 1, settings);
			//				if ((!newLines || outputType != OutputType.minimal) && child._next != null) buffer.Append(',');
			//				buffer.Append(newLines ? '\n' : ' ');
			//				if (!newLines && buffer.Length - start > settings.singleLineColumns) {
			//					buffer.Length = (start);
			//					newLines = true;
			//					continue outer;
			//				}
			//			}
			//			break;
			//		}
			//		if (newLines) JsonValue.indent(indent - 1, buffer);
			//		buffer.Append('}');
			//	}
			//} else if (obj.isArray()) {
			//	if (obj.child == null)
			//		buffer.Append("[]");
			//	else {
			//		bool newLines = !isFlat(obj);
			//		bool wrap = settings.wrapNumericArrays || !isNumeric(obj);
			//		int start = buffer.Length;
			//		outer:
			//		while (true) {
			//			buffer.Append(newLines ? "[\n" : "[ ");
			//			for (JsonValue child = obj._child; child != null; child = child._next) {
			//				if (newLines) JsonValue.indent(indent, buffer);
			//				prettyPrint(child, buffer, indent + 1, settings);
			//				if ((!newLines || outputType != OutputType.minimal) && child.next != null) buffer.Append(',');
			//				buffer.Append(newLines ? '\n' : ' ');
			//				if (wrap && !newLines && buffer.Length - start > settings.singleLineColumns) {
			//					buffer.Length=(start);
			//					newLines = true;
			//					continue outer;
			//				}
			//			}
			//			break;
			//		}
			//		if (newLines) JsonValue.indent(indent - 1, buffer);
			//		buffer.Append(']');
			//	}
			//} else if (obj.isString()) {
			//	buffer.Append(outputType.quoteValue(obj.asString()));
			//} else if (obj.isDouble()) {
			//	double doubleValue = obj.asDouble();
			//	long longValue = obj.asLong();
			//	buffer.Append(doubleValue == longValue ? longValue : doubleValue);
			//} else if (obj.isLong()) {
			//	buffer.Append(obj.asLong());
			//} else if (obj.isBoolean()) {
			//	buffer.Append(obj.asBoolean());
			//} else if (obj.isNull()) {
			//	buffer.Append("null");
			//} else
			//	throw new SerializationException("Unknown object type: " + obj);
		}

		/** More efficient than {@link #prettyPrint(PrettyPrintSettings)} but {@link PrettyPrintSettings#singleLineColumns} and
		 * {@link PrettyPrintSettings#wrapNumericArrays} are not supported. */
		public void prettyPrint(OutputType outputType, Writer writer) // TODO: throws IOException
		{
			PrettyPrintSettings settings = new PrettyPrintSettings();
			settings.outputType = outputType;
			prettyPrint(this, writer, 0, settings);
		}

		private void
			prettyPrint(JsonValue obj, Writer writer, int indent,
				PrettyPrintSettings settings) // TODO: throws IOException 
		{
			throw new NotImplementedException();
			//OutputType outputType = settings.outputType;
			//if (obj.isObject()) {
			//	if (obj.child == null)
			//		writer.append("{}");
			//	else {
			//		bool newLines = !isFlat(obj) || obj._size > 6;
			//		writer.append(newLines ? "{\n" : "{ ");
			//		int i = 0;
			//		for (JsonValue child = obj._child; child != null; child = child._next) {
			//			if (newLines) JsonValue.indent(indent, writer);
			//			writer.append(outputType.quoteName(child._name));
			//			writer.append(": ");
			//			prettyPrint(child, writer, indent + 1, settings);
			//			if ((!newLines || outputType != OutputType.minimal) && child.next != null) writer.append(',');
			//			writer.append(newLines ? '\n' : ' ');
			//		}
			//		if (newLines) JsonValue.indent(indent - 1, writer);
			//		writer.append('}');
			//	}
			//} else if (obj.isArray()) {
			//	if (obj.child == null)
			//		writer.append("[]");
			//	else {
			//		bool newLines = !isFlat(obj);
			//		writer.append(newLines ? "[\n" : "[ ");
			//		int i = 0;
			//		for (JsonValue child = obj._child; child != null; child = child._next) {
			//			if (newLines) JsonValue.indent(indent, writer);
			//			prettyPrint(child, writer, indent + 1, settings);
			//			if ((!newLines || outputType != OutputType.minimal) && child._next != null) writer.append(',');
			//			writer.append(newLines ? '\n' : ' ');
			//		}
			//		if (newLines) JsonValue.indent(indent - 1, writer);
			//		writer.append(']');
			//	}
			//} else if (obj.isString()) {
			//	writer.append(outputType.quoteValue(obj.asString()));
			//} else if (obj.isDouble()) {
			//	double doubleValue = obj.asDouble();
			//	long longValue = obj.asLong();
			//	writer.append((doubleValue == longValue ? longValue : doubleValue).ToString());
			//} else if (obj.isLong()) {
			//	writer.append((obj.asLong()).ToString());
			//} else if (obj.isBoolean()) {
			//	writer.append((obj.asBoolean()).ToString());
			//} else if (obj.isNull()) {
			//	writer.append("null");
			//} else
			//	throw new SerializationException("Unknown object type: " + obj);
		}

		static private bool isFlat(JsonValue obj)
		{
			for (JsonValue? child = obj._child; child != null; child = child._next)
				if (child.isObject() || child.isArray())
					return false;
			return true;
		}

		static private bool isNumeric(JsonValue obj)
		{
			for (JsonValue? child = obj._child; child != null; child = child._next)
				if (!child.isNumber())
					return false;
			return true;
		}

		static private void indent(int count, StringBuilder buffer)
		{
			for (int i = 0; i < count; i++)
				buffer.Append('\t');
		}

		static private void indent(int count, Writer buffer) // TODO: throws IOException 
		{
			for (int i = 0; i < count; i++)
				buffer.append('\t');
		}

		public class JsonIterator : IEnumerator<JsonValue>, IEnumerable<JsonValue>
		{
			// TODO: JsonValue entry = _child;
			JsonValue current;

			public bool MoveNext()
			{
				throw new NotImplementedException();
				// TODO: return entry != null;
			}

			object IEnumerator.Current => Current;

			public JsonValue Current
			{
				get
				{
					throw new NotImplementedException();
					// TODO: current = entry;
					//	if (current == null) throw new NoSuchElementException();
					//entry = current._next;
					//return current;
				}
			}

			public void Reset()
			{
			}

			public void Dispose()
			{
			}

			public void remove()
			{
				throw new NotImplementedException();
				// TODO: if (current._prev == null) {
				//	_child = current._next;
				//	if (_child != null) _child._prev = null;
				//} else {
				//	current._prev._next = current._next;
				//	if (current._next != null) current._next._prev = current._prev;
				//}
				//_size--;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public IEnumerator<JsonValue> GetEnumerator()
			{
				return this;
			}
		}

		public enum ValueType
		{
			@object,
			array,
			stringValue,
			doubleValue,
			longValue,
			booleanValue,
			nullValue
		}

		public class PrettyPrintSettings
		{
			public OutputType outputType;

			/** If an object on a single line fits this many columns, it won't wrap. */
			public int singleLineColumns;

			/** Arrays of floats won't wrap. */
			public bool wrapNumericArrays;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}