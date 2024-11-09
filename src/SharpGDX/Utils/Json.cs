using SharpGDX.Utils.Reflect;
using System.Collections;
using SharpGDX.Shims;
using SharpGDX.Files;
using SharpGDX.Scenes.Scene2D.UI;

namespace SharpGDX.Utils;

/** Reads/writes Java objects to/from JSON, automatically. See the wiki for usage:
 * https://libgdx.com/wiki/utils/reading-and-writing-json
 * @author Nathan Sweet */
public class Json {
	static private readonly bool debug = false;

	private JsonWriter _writer;
	private String typeName = "class";
	private bool usePrototypes = true;
	private JsonWriter.OutputType outputType;
	private bool quoteLongValues;
	private bool ignoreUnknownFields;
	private bool ignoreDeprecated;
	private bool readDeprecated;
	private bool enumNames = true;
	private bool _sortFields;
	private Serializer defaultSerializer;
	private readonly ObjectMap<Type, OrderedMap<String, FieldMetadata>> typeToFields = new ();
	private readonly ObjectMap<String, Type> tagToClass = new ();
	private readonly ObjectMap<Type, String> classToTag = new ();
	private readonly ObjectMap<Type, Serializer> classToSerializer = new ();
	private readonly ObjectMap<Type, Object[]> classToDefaultValues = new ();
	private readonly Object[] equals1 = {null}, equals2 = {null};

	public Json () {
		outputType = JsonWriter.OutputType.minimal;
	}

	public Json (JsonWriter.OutputType outputType) {
		this.outputType = outputType;
	}

	/** When true, fields in the JSON that are not found on the class will not throw a {@link SerializationException}. Default is
	 * false. */
	public void setIgnoreUnknownFields (bool ignoreUnknownFields) {
		this.ignoreUnknownFields = ignoreUnknownFields;
	}

	public bool getIgnoreUnknownFields () {
		return ignoreUnknownFields;
	}

	/** When true, fields with the {@link Deprecated} annotation will not be read or written. Default is false.
	 * @see #setReadDeprecated(bool)
	 * @see #setDeprecated(Class, String, bool) */
	public void setIgnoreDeprecated (bool ignoreDeprecated) {
		this.ignoreDeprecated = ignoreDeprecated;
	}

	/** When true, fields with the {@link Deprecated} annotation will be read (but not written) when
	 * {@link #setIgnoreDeprecated(bool)} is true. Default is false.
	 * @see #setDeprecated(Class, String, bool) */
	public void setReadDeprecated (bool readDeprecated) {
		this.readDeprecated = readDeprecated;
	}

	/** Default is {@link OutputType#minimal}.
	 * @see JsonWriter#setOutputType(OutputType) */
	public void setOutputType (JsonWriter.OutputType outputType) {
		this.outputType = outputType;
	}

	/** Default is false.
	 * @see JsonWriter#setQuoteLongValues(bool) */
	public void setQuoteLongValues (bool quoteLongValues) {
		this.quoteLongValues = quoteLongValues;
	}

	/** When true, {@link Enum#name()} is used to write enum values. When false, {@link Enum#toString()} is used which may not be
	 * unique. Default is true. */
	public void setEnumNames (bool enumNames) {
		this.enumNames = enumNames;
	}

	/** Sets a tag to use instead of the fully qualifier class name. This can make the JSON easier to read. */
	public void addClassTag (String tag, Type type) {
		tagToClass.put(tag, type);
		classToTag.put(type, tag);
	}

	/** Returns the class for the specified tag, or null. */
	public Type? getClass (String tag) {
		return tagToClass.get(tag);
	}

	/** Returns the tag for the specified class, or null. */
	public string? getTag (Type type) {
		return classToTag.get(type);
	}

	/** Sets the name of the JSON field to store the Java class name or class tag when required to avoid ambiguity during
	 * deserialization. Set to null to never output this information, but be warned that deserialization may fail. Default is
	 * "class". */
	public void setTypeName (string? typeName) {
		this.typeName = typeName;
	}

	/** Sets the serializer to use when the type being deserialized is not known (null). */
	public void setDefaultSerializer (Serializer? defaultSerializer) {
		this.defaultSerializer = defaultSerializer;
	}

	/** Registers a serializer to use for the specified type instead of the default behavior of serializing all of an objects
	 * fields. */
	public void setSerializer (Type type, Serializer serializer) {
		classToSerializer.put(type, serializer);
	}

	public Serializer getSerializer (Type type) {
		return classToSerializer.get(type);
	}

	/** When true, field values that are identical to a newly constructed instance are not written. Default is true. */
	public void setUsePrototypes (bool usePrototypes) {
		this.usePrototypes = usePrototypes;
	}

    /** Sets the type of elements in a collection. When the element type is known, the class for each element in the collection
	 * does not need to be written unless different from the element type. */
	public void setElementType (Type type, String fieldName, Type elementType) {
		FieldMetadata metadata = getFields(type).get(fieldName);
		if (metadata == null) throw new SerializationException("Field not found: " + fieldName + " (" + type.Name + ")");
		metadata.elementType = elementType;
	}

	/** The specified field will be treated as if it has or does not have the {@link Deprecated} annotation.
	 * @see #setIgnoreDeprecated(bool)
	 * @see #setReadDeprecated(bool) */
	public void setDeprecated (Type type, String fieldName, bool deprecated) {
		FieldMetadata metadata = getFields(type).get(fieldName);
		if (metadata == null) throw new SerializationException("Field not found: " + fieldName + " (" + type.Name + ")");
		metadata.deprecated = deprecated;
	}

	/** When true, fields are sorted alphabetically when written, otherwise the source code order is used. Default is false.
	 * @see #sortFields(Class, Array) */
	public void setSortFields (bool sortFields) {
		this._sortFields = sortFields;
	}

	/** Called to sort the fields for a class. Default implementation sorts alphabetically if {@link #setSortFields(bool)} is
	 * true. */
	protected void sortFields (Type type, Array<String> fieldNames) {
		if (_sortFields) fieldNames.sort();
	}

	private OrderedMap<String, FieldMetadata> getFields (Type type) {
		OrderedMap<String, FieldMetadata> fields = typeToFields.get(type);
		if (fields != null) return fields;

		Array<Type> classHierarchy = new ();
		Type nextClass = type;
		while (nextClass != typeof(object)) {
			classHierarchy.add(nextClass);
			nextClass = nextClass.BaseType;
		}

        System.Collections.Generic.List<Field> allFields = new ();
        for (int i = classHierarchy.size - 1; i >= 0; i--)
        {
			allFields.AddRange(ClassReflection.getDeclaredFields(classHierarchy.get(i)));
        }

		OrderedMap<String, FieldMetadata> nameToField = new (allFields.Count());
		for (int i = 0, n = allFields.Count(); i < n; i++) {
			Field field = allFields[i];

			if (field.isTransient()) continue;
			if (field.isStatic()) continue;
			if (field.isSynthetic()) continue;

			if (!field.isAccessible()) {
				try {
					field.setAccessible(true);
				} catch (RuntimeException ex) {
					continue;
				}
			}

			nameToField.put(field.getName(), new FieldMetadata(field));
		}
		sortFields(type, nameToField._keys);
		typeToFields.put(type, nameToField);
		return nameToField;
	}

	public String toJson ( Object? obj) {
		return toJson(obj, obj == null ? null : obj.GetType(), (Type)null);
	}

	public String toJson (Object? obj, Type? knownType) {
		return toJson(obj, knownType, (Type)null);
	}

	/** @param knownType May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown. */
	public String toJson (Object? obj, Type? knownType, Type? elementType) {
		Shims.StringWriter buffer = new ();
		toJson(obj, knownType, elementType, buffer);
		return buffer.ToString();
	}

	public void toJson (Object? obj, FileHandle file) {
		toJson(obj, obj == null ? null : obj.GetType(), null, file);
	}

	/** @param knownType May be null if the type is unknown. */
	public void toJson (object? obj, Type? knownType, FileHandle file) {
		toJson(obj, knownType, null, file);
	}

	/** @param knownType May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown. */
	public void toJson (object? obj, Type? knownType, Type? elementType, FileHandle file) {
		Writer writer = null;
		try {
			writer = file.writer(false, "UTF-8");
			toJson(obj, knownType, elementType, writer);
		} catch (Exception ex) {
			throw new SerializationException("Error writing file: " + file, ex);
		} finally {
			StreamUtils.closeQuietly(writer);
		}
	}

	public void toJson (object? obj, Writer writer) {
		toJson(obj, obj == null ? null : obj.GetType(), null, writer);
	}

	/** @param knownType May be null if the type is unknown. */
	public void toJson (object? obj, Type? knownType, Writer writer) {
		toJson(obj, knownType, null, writer);
	}

	/** @param knownType May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown. */
	public void toJson (object? obj, Type? knownType, Type? elementType, Writer writer) {
		setWriter(writer);
		try {
			writeValue(obj, knownType, elementType);
		} finally {
			StreamUtils.closeQuietly(this._writer);
			this._writer = null;
		}
	}

	/** Sets the writer where JSON output will be written. This is only necessary when not using the toJson methods. */
	public void setWriter (Writer writer) {
		if (!(writer is JsonWriter)) writer = new JsonWriter(writer);
		this._writer = (JsonWriter)writer;
		this._writer.setOutputType(outputType);
		this._writer.setQuoteLongValues(quoteLongValues);
	}

	public JsonWriter getWriter () {
		return _writer;
	}

	/** Writes all fields of the specified object to the current JSON object. */
	public void writeFields (Object obj) {
		Type type = obj.GetType();

		Object[] defaultValues = getDefaultValues(type);

		OrderedMap<String, FieldMetadata> fields = getFields(type);
		int defaultIndex = 0;
		Array<String> fieldNames = fields.orderedKeys();
		for (int i = 0, n = fieldNames.size; i < n; i++) {
			FieldMetadata metadata = fields.get(fieldNames.get(i));
			if (ignoreDeprecated && metadata.deprecated) continue;
			Field field = metadata.field;
			try {
				Object value = field.get(obj);
				if (defaultValues != null) {
					Object defaultValue = defaultValues[defaultIndex++];
					if (value == null && defaultValue == null) continue;
					if (value != null && defaultValue != null) {
						if (value.Equals(defaultValue)) continue;
						if (value.GetType().IsArray && defaultValue.GetType().IsArray) {
							equals1[0] = value;
							equals2[0] = defaultValue;
                            throw new NotImplementedException();
							// TODO: if (Arrays.deepEquals(equals1, equals2)) continue;
						}
					}
				}

				if (debug) Console.WriteLine("Writing field: " + field.getName() + " (" + type.Name + ")");
				_writer.name(field.getName());
				writeValue(value, field.getType(), metadata.elementType);
			} catch (ReflectionException ex) {
				throw new SerializationException("Error accessing field: " + field.getName() + " (" + type.Name + ")", ex);
			} catch (SerializationException ex) {
				ex.addTrace(field + " (" + type.Name + ")");
				throw ex;
			} catch (Exception runtimeEx) {
				SerializationException ex = new SerializationException(runtimeEx);
				ex.addTrace(field + " (" + type.Name + ")");
				throw ex;
			}
		}
	}

	private Object[]? getDefaultValues (Type type) {
		if (!usePrototypes) return null;
		if (classToDefaultValues.containsKey(type)) return classToDefaultValues.get(type);
		Object obj;
		try {
			obj = newInstance(type);
		} catch (Exception ex) {
			classToDefaultValues.put(type, null);
			return null;
		}

		OrderedMap<String, FieldMetadata> fields = getFields(type);
		Object[] values = new Object[fields.size];
		classToDefaultValues.put(type, values);

		int defaultIndex = 0;
		Array<String> fieldNames = fields.orderedKeys();
		for (int i = 0, n = fieldNames.size; i < n; i++) {
			FieldMetadata metadata = fields.get(fieldNames.get(i));
			if (ignoreDeprecated && metadata.deprecated) continue;
			Field field = metadata.field;
			try {
				values[defaultIndex++] = field.get(obj);
			} catch (ReflectionException ex) {
				throw new SerializationException("Error accessing field: " + field.getName() + " (" + type.Name + ")", ex);
			} catch (SerializationException ex) {
				ex.addTrace(field + " (" + type.Name + ")");
				throw ex;
			} catch (RuntimeException runtimeEx) {
				SerializationException ex = new SerializationException(runtimeEx);
				ex.addTrace(field + " (" + type.Name + ")");
				throw ex;
			}
		}
		return values;
	}

	/** @see #writeField(Object, String, String, Class) */
	public void writeField (Object obj, String name) {
		writeField(obj, name, name, null);
	}

	/** @param elementType May be null if the type is unknown.
	 * @see #writeField(Object, String, String, Class) */
	public void writeField (Object obj, String name, Type? elementType) {
		writeField(obj, name, name, elementType);
	}

	/** @see #writeField(Object, String, String, Class) */
	public void writeField (Object obj, String fieldName, String jsonName) {
		writeField(obj, fieldName, jsonName, null);
	}

	/** Writes the specified field to the current JSON object.
	 * @param elementType May be null if the type is unknown. */
	public void writeField (Object obj, String fieldName, String jsonName, Type? elementType) {
		Type type = obj.GetType();
		FieldMetadata metadata = getFields(type).get(fieldName);
		if (metadata == null) throw new SerializationException("Field not found: " + fieldName + " (" + type.Name + ")");
		Field field = metadata.field;
		if (elementType == null) elementType = metadata.elementType;
		try {
			if (debug) Console.WriteLine("Writing field: " + field.getName() + " (" + type.Name + ")");
			_writer.name(jsonName);
			writeValue(field.get(obj), field.getType(), elementType);
		} catch (ReflectionException ex) {
			throw new SerializationException("Error accessing field: " + field.getName() + " (" + type.Name + ")", ex);
		} catch (SerializationException ex) {
			ex.addTrace(field + " (" + type.Name + ")");
			throw ex;
		} catch (Exception runtimeEx) {
			SerializationException ex = new SerializationException(runtimeEx);
			ex.addTrace(field + " (" + type.Name + ")");
			throw ex;
		}
	}

	/** Writes the value as a field on the current JSON object, without writing the actual class.
	 * @param value May be null.
	 * @see #writeValue(String, Object, Class, Class) */
	public void writeValue (String name, Object? value) {
		try {
			_writer.name(name);
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
		if (value == null)
			writeValue(value, null, null);
		else
			writeValue(value, value.GetType(), null);
	}

	/** Writes the value as a field on the current JSON object, writing the class of the object if it differs from the specified
	 * known type.
	 * @param value May be null.
	 * @param knownType May be null if the type is unknown.
	 * @see #writeValue(String, Object, Class, Class) */
	public void writeValue (String name,  Object? value, Type? knownType) {
		try {
			_writer.name(name);
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
		writeValue(value, knownType, null);
	}

	/** Writes the value as a field on the current JSON object, writing the class of the object if it differs from the specified
	 * known type. The specified element type is used as the default type for collections.
	 * @param value May be null.
	 * @param knownType May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown. */
	public void writeValue (String name, Object? value, Type? knownType, Type? elementType) {
		try {
			_writer.name(name);
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
		writeValue(value, knownType, elementType);
	}

	/** Writes the value, without writing the class of the object.
	 * @param value May be null. */
	public void writeValue ( Object? value) {
		if (value == null)
			writeValue(value, null, null);
		else
			writeValue(value, value.GetType(), null);
	}

	/** Writes the value, writing the class of the object if it differs from the specified known type.
	 * @param value May be null.
	 * @param knownType May be null if the type is unknown. */
	public void writeValue (Object? value, Type? knownType) {
		writeValue(value, knownType, null);
	}

	/** Writes the value, writing the class of the object if it differs from the specified known type. The specified element type
	 * is used as the default type for collections.
	 * @param value May be null.
	 * @param knownType May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown. */
	public void writeValue (Object? value, Type? knownType, Type? elementType) {
        throw new NotImplementedException();
//try {
//			if (value == null) {
//				writer.value(null);
//				return;
//			}

//			if ((knownType != null && knownType.isPrimitive()) || knownType == String.class || knownType == Integer.class
//				|| knownType == Boolean.class || knownType == Float.class || knownType == Long.class || knownType == Double.class
//				|| knownType == Short.class || knownType == Byte.class || knownType == Character.class) {
//				writer.value(value);
//				return;
//			}

//			Class actualType = value.getClass();

//			if (actualType.isPrimitive() || actualType == String.class || actualType == Integer.class || actualType == Boolean.class
//				|| actualType == Float.class || actualType == Long.class || actualType == Double.class || actualType == Short.class
//				|| actualType == Byte.class || actualType == Character.class) {
//				writeObjectStart(actualType, null);
//				writeValue("value", value);
//				writeObjectEnd();
//				return;
//			}

//			if (value instanceof Serializable) {
//				writeObjectStart(actualType, knownType);
//				((Serializable)value).write(this);
//				writeObjectEnd();
//				return;
//			}

//			Serializer serializer = classToSerializer.get(actualType);
//			if (serializer != null) {
//				serializer.write(this, value, knownType);
//				return;
//			}

//			// JSON array special cases.
//			if (value instanceof Array) {
//				if (knownType != null && actualType != knownType && actualType != Array.class)
//					throw new SerializationException("Serialization of an Array other than the known type is not supported.\n"
//						+ "Known type: " + knownType + "\nActual type: " + actualType);
//				writeArrayStart();
//				Array array = (Array)value;
//				for (int i = 0, n = array.size; i < n; i++)
//					writeValue(array.get(i), elementType, null);
//				writeArrayEnd();
//				return;
//			}
//			if (value instanceof Queue) {
//				if (knownType != null && actualType != knownType && actualType != Queue.class)
//					throw new SerializationException("Serialization of a Queue other than the known type is not supported.\n"
//						+ "Known type: " + knownType + "\nActual type: " + actualType);
//				writeArrayStart();
//				Queue queue = (Queue)value;
//				for (int i = 0, n = queue.size; i < n; i++)
//					writeValue(queue.get(i), elementType, null);
//				writeArrayEnd();
//				return;
//			}
//			if (value instanceof Collection) {
//				if (typeName != null && actualType != ArrayList.class && (knownType == null || knownType != actualType)) {
//					writeObjectStart(actualType, knownType);
//					writeArrayStart("items");
//					for (Object item : (Collection)value)
//						writeValue(item, elementType, null);
//					writeArrayEnd();
//					writeObjectEnd();
//				} else {
//					writeArrayStart();
//					for (Object item : (Collection)value)
//						writeValue(item, elementType, null);
//					writeArrayEnd();
//				}
//				return;
//			}
//			if (actualType.isArray()) {
//				if (elementType == null) elementType = actualType.getComponentType();
//				int length = ArrayReflection.getLength(value);
//				writeArrayStart();
//				for (int i = 0; i < length; i++)
//					writeValue(ArrayReflection.get(value, i), elementType, null);
//				writeArrayEnd();
//				return;
//			}

//			// JSON object special cases.
//			if (value instanceof ObjectMap) {
//				if (knownType == null) knownType = ObjectMap.class;
//				writeObjectStart(actualType, knownType);
//				for (Entry entry : ((ObjectMap<?, ?>)value).entries()) {
//					writer.name(convertToString(entry.key));
//					writeValue(entry.value, elementType, null);
//				}
//				writeObjectEnd();
//				return;
//			}
//			if (value instanceof ObjectIntMap) {
//				if (knownType == null) knownType = ObjectIntMap.class;
//				writeObjectStart(actualType, knownType);
//				for (ObjectIntMap.Entry entry : ((ObjectIntMap<?>)value).entries()) {
//					writer.name(convertToString(entry.key));
//					writeValue(entry.value, Integer.class);
//				}
//				writeObjectEnd();
//				return;
//			}
//			if (value instanceof ObjectFloatMap) {
//				if (knownType == null) knownType = ObjectFloatMap.class;
//				writeObjectStart(actualType, knownType);
//				for (ObjectFloatMap.Entry entry : ((ObjectFloatMap<?>)value).entries()) {
//					writer.name(convertToString(entry.key));
//					writeValue(entry.value, Float.class);
//				}
//				writeObjectEnd();
//				return;
//			}
//			if (value instanceof ObjectSet) {
//				if (knownType == null) knownType = ObjectSet.class;
//				writeObjectStart(actualType, knownType);
//				writer.name("values");
//				writeArrayStart();
//				for (Object entry : (ObjectSet)value)
//					writeValue(entry, elementType, null);
//				writeArrayEnd();
//				writeObjectEnd();
//				return;
//			}
//			if (value instanceof IntMap) {
//				if (knownType == null) knownType = IntMap.class;
//				writeObjectStart(actualType, knownType);
//				for (IntMap.Entry entry : ((IntMap<?>)value).entries()) {
//					writer.name(String.valueOf(entry.key));
//					writeValue(entry.value, elementType, null);
//				}
//				writeObjectEnd();
//				return;
//			}
//			if (value instanceof LongMap) {
//				if (knownType == null) knownType = LongMap.class;
//				writeObjectStart(actualType, knownType);
//				for (LongMap.Entry entry : ((LongMap<?>)value).entries()) {
//					writer.name(String.valueOf(entry.key));
//					writeValue(entry.value, elementType, null);
//				}
//				writeObjectEnd();
//				return;
//			}
//			if (value instanceof IntSet) {
//				if (knownType == null) knownType = IntSet.class;
//				writeObjectStart(actualType, knownType);
//				writer.name("values");
//				writeArrayStart();
//				for (IntSetIterator iter = ((IntSet)value).iterator(); iter.hasNext;)
//					writeValue(iter.next(), Integer.class, null);
//				writeArrayEnd();
//				writeObjectEnd();
//				return;
//			}
//			if (value instanceof ArrayMap) {
//				if (knownType == null) knownType = ArrayMap.class;
//				writeObjectStart(actualType, knownType);
//				ArrayMap map = (ArrayMap)value;
//				for (int i = 0, n = map.size; i < n; i++) {
//					writer.name(convertToString(map.keys[i]));
//					writeValue(map.values[i], elementType, null);
//				}
//				writeObjectEnd();
//				return;
//			}
//			if (value instanceof Map) {
//				if (knownType == null) knownType = HashMap.class;
//				writeObjectStart(actualType, knownType);
//				for (Map.Entry entry : ((Map<?, ?>)value).entrySet()) {
//					writer.name(convertToString(entry.getKey()));
//					writeValue(entry.getValue(), elementType, null);
//				}
//				writeObjectEnd();
//				return;
//			}

//			// Enum special case.
//			if (ClassReflection.isAssignableFrom(Enum.class, actualType)) {
//				if (actualType.getEnumConstants() == null) // Get the enum type when an enum value is an inner class (enum A {b{}}).
//					actualType = actualType.getSuperclass();
//				if (typeName != null && (knownType == null || knownType != actualType)) {
//					writeObjectStart(actualType, null);
//					writer.name("value");
//					writer.value(convertToString((Enum)value));
//					writeObjectEnd();
//				} else {
//					writer.value(convertToString((Enum)value));
//				}
//				return;
//			}

//			writeObjectStart(actualType, knownType);
//			writeFields(value);
//			writeObjectEnd();
//		} catch (IOException ex) {
//			throw new SerializationException(ex);
//		}
	}

	public void writeObjectStart (String name) {
		try {
			_writer.name(name);
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
		writeObjectStart();
	}

	/** @param knownType May be null if the type is unknown. */
	public void writeObjectStart (String name, Type actualType, Type? knownType) {
		try {
			_writer.name(name);
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
		writeObjectStart(actualType, knownType);
	}

	public void writeObjectStart () {
		try {
			_writer.@object();
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
	}

	/** Starts writing an object, writing the actualType to a field if needed.
	 * @param knownType May be null if the type is unknown. */
	public void writeObjectStart (Type actualType, Type? knownType) {
		try {
			_writer.@object();
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
		if (knownType == null || knownType != actualType) writeType(actualType);
	}

	public void writeObjectEnd () {
		try {
			_writer.pop();
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
	}

	public void writeArrayStart (String name) {
		try {
			_writer.name(name);
			_writer.array();
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
	}

	public void writeArrayStart () {
		try {
			_writer.array();
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
	}

	public void writeArrayEnd () {
		try {
			_writer.pop();
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
	}

	public void writeType (Type type) {
		if (typeName == null) return;
		String className = getTag(type);
		if (className == null) className = type.Name;
		try {
			_writer.set(typeName, className);
		} catch (IOException ex) {
			throw new SerializationException(ex);
		}
		if (debug) Console.WriteLine("Writing type: " + type.Name);
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, Reader reader) {
		return readValue(type, null, new JsonReader().parse(reader));
	}

	/** @param type May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, Type elementType, Reader reader) {
		return readValue(type, elementType, new JsonReader().parse(reader));
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, InputStream input) {
		return readValue(type, null, new JsonReader().parse(input));
	}

	/** @param type May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, Type elementType, InputStream input) {
		return readValue(type, elementType, new JsonReader().parse(input));
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, FileHandle file) {
		try {
			return readValue(type, null, new JsonReader().parse(file));
		} catch (Exception ex)
        {
			throw new SerializationException("Error reading file: " + file, ex);
		}
	}

	/** @param type May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, Type elementType, FileHandle file) {
		try {
			return readValue(type, elementType, new JsonReader().parse(file));
		} catch (Exception ex) {
			throw new SerializationException("Error reading file: " + file, ex);
		}
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, char[] data, int offset, int length) {
		return readValue(type, null, new JsonReader().parse(data, offset, length));
	}

	/** @param type May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, Type elementType, char[] data, int offset, int length) {
		return readValue(type, elementType, new JsonReader().parse(data, offset, length));
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, String json) {
		return readValue(type, null, new JsonReader().parse(json));
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? fromJson (Type type, Type elementType, String json) {
		return readValue(type, elementType, new JsonReader().parse(json));
	}

	public void readField (Object obj, String name, JsonValue jsonData) {
		readField(obj, name, name, null, jsonData);
	}

	public void readField (Object obj, String name, Type? elementType, JsonValue jsonData) {
		readField(obj, name, name, elementType, jsonData);
	}

	public void readField (Object obj, String fieldName, String jsonName, JsonValue jsonData) {
		readField(obj, fieldName, jsonName, null, jsonData);
	}

	/** @param elementType May be null if the type is unknown. */
	public void readField (Object obj, String fieldName, String jsonName, Type? elementType, JsonValue jsonMap) {
		var type = obj.GetType();
		FieldMetadata metadata = getFields(type).get(fieldName);
		if (metadata == null) throw new SerializationException("Field not found: " + fieldName + " (" + type.Name + ")");
		Field field = metadata.field;
		if (elementType == null) elementType = metadata.elementType;
		readField(obj, field, jsonName, elementType, jsonMap);
	}

	/** @param object May be null if the field is static.
	 * @param elementType May be null if the type is unknown. */
	public void readField (Object? obj, Field? field, String jsonName, Type? elementType, JsonValue jsonMap) {
        throw new NotImplementedException();
//JsonValue jsonValue = jsonMap.get(jsonName);
//		if (jsonValue == null) return;
//		try {
//			field.set(object, readValue(field.getType(), elementType, jsonValue));
//		} catch (ReflectionException ex) {
//			throw new SerializationException(
//				"Error accessing field: " + field.getName() + " (" + field.getDeclaringClass().getName() + ")", ex);
//		} catch (SerializationException ex) {
//			ex.addTrace(field.getName() + " (" + field.getDeclaringClass().getName() + ")");
//			throw ex;
//		} catch (RuntimeException runtimeEx) {
//			SerializationException ex = new SerializationException(runtimeEx);
//			ex.addTrace(jsonValue.trace());
//			ex.addTrace(field.getName() + " (" + field.getDeclaringClass().getName() + ")");
//			throw ex;
//		}
	}

	public virtual void readFields (Object obj, JsonValue jsonMap)
    {
		Type type = obj.GetType();
		OrderedMap<String, FieldMetadata> fields = getFields(type);
		for (JsonValue child = jsonMap._child; child != null; child = child._next)
		{
            FieldMetadata metadata = fields.get(child.name().Replace(" ", "_"));
			if (metadata == null)
			{
				if (child._name.Equals(typeName)) continue;
				if (ignoreUnknownFields || ignoreUnknownField(type, child._name))
				{
					if (debug) Console.WriteLine("Ignoring unknown field: " + child._name + " (" + type.Name + ")");
					continue;
				}
				else
				{
					SerializationException ex = new SerializationException(
						"Field not found: " + child.name + " (" + type.Name + ")");
					ex.addTrace(child.trace());
					throw ex;
				}
			}
			else
			{
				if (ignoreDeprecated && !readDeprecated && metadata.deprecated) continue;
			}
			Field field = metadata.field;
			try
			{
				field.set(obj, readValue(field.getType(), metadata.elementType, child));
			}
			catch (ReflectionException ex)
			{
				throw new SerializationException("Error accessing field: " + field.getName() + " (" + type.Name + ")", ex);
			}
			catch (SerializationException ex)
			{
				ex.addTrace(field.getName() + " (" + type.Name + ")");
				throw ex;
			}
			catch (RuntimeException runtimeEx)
			{
				SerializationException ex = new SerializationException(runtimeEx);
				ex.addTrace(child.trace());
				ex.addTrace(field.getName() + " (" + type.Name + ")");
				throw ex;
			}
		}
	}

	/** Called for each unknown field name encountered by {@link #readFields(Object, JsonValue)} when {@link #ignoreUnknownFields}
	 * is false to determine whether the unknown field name should be ignored.
	 * @param type The object type being read.
	 * @param fieldName A field name encountered in the JSON for which there is no matching class field.
	 * @return true if the field name should be ignored and an exception won't be thrown by
	 *         {@link #readFields(Object, JsonValue)}. */
	protected virtual bool ignoreUnknownField (Type type, String fieldName) {
		return false;
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? readValue (string name, Type? type, JsonValue jsonMap) {
		return readValue(type, null, jsonMap.get(name));
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? readValue (String name, Type? type, object defaultValue, JsonValue jsonMap) {
		JsonValue jsonValue = jsonMap.get(name);
		if (jsonValue == null) return defaultValue;
		return readValue(type, null, jsonValue);
	}

	/** @param type May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown.
	 * @return May be null. */
	public object? readValue (String name, Type? type, Type? elementType, JsonValue jsonMap) {
		return readValue(type, elementType, jsonMap.get(name));
	}

	/** @param type May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown.
	 * @return May be null. */
	public object? readValue (String name, Type? type, Type? elementType, object defaultValue, JsonValue jsonMap) {
		JsonValue jsonValue = jsonMap.get(name);
		return readValue(type, elementType, defaultValue, jsonValue);
	}

	/** @param type May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown.
	 * @return May be null. */
	public object? readValue (Type? type, Type? elementType, object defaultValue, JsonValue jsonData) {
		if (jsonData == null) return defaultValue;
		return readValue(type, elementType, jsonData);
	}

	/** @param type May be null if the type is unknown.
	 * @return May be null. */
	public object? readValue (Type? type, JsonValue jsonData) {
		return readValue(type, null, jsonData);
	}

	/** @param type May be null if the type is unknown.
	 * @param elementType May be null if the type is unknown.
	 * @return May be null. */
	public virtual object? readValue (Type? type, Type? elementType, JsonValue jsonData) {
        
		if (jsonData == null) return null;

		if (jsonData.isObject()) {
			String className = typeName == null ? null : jsonData.getString(typeName, null);
			if (className != null) {
				type = getClass(className);
				if (type == null) {
					try {
						type = ClassReflection.forName(className);
					} catch (ReflectionException ex) {
						throw new SerializationException(ex);
					}
				}
			}

			if (type == null) {
				// TODO: These two returns originally cast the data to T, should we ensure it is of 'type'? -RP
				if (defaultSerializer != null) return defaultSerializer.read(this, jsonData, type);
				return jsonData;
			}

			if (typeName != null && ClassReflection.isAssignableFrom(typeof(ICollection), type)) {
				// JSON object wrapper to specify type.
				jsonData = jsonData.get("items");
				if (jsonData == null) throw new SerializationException(
					"Unable to convert object to collection: " + jsonData + " (" + type.Name + ")");
			} else {
				Serializer serializer = classToSerializer.get(type);

				// TODO: This originally cast to T, should we verify type and throw exception? -RP
				if (serializer != null) return serializer.read(this, jsonData, type);

				if (type == typeof(string) || type == typeof(int) || type == typeof(bool) || type == typeof(float)
					|| type == typeof(long) || type == typeof(double) || type == typeof(short) || type == typeof(byte)
					|| type == typeof(char) || ClassReflection.isAssignableFrom(typeof(Enum), type)) {
					return readValue("value", type, jsonData);
				}

				Object obj = newInstance(type);

				if (obj is Serializable) {
					((Serializable)obj).read(this, jsonData);

                    // TODO: This originally cast to T, should we verify type and throw exception? -RP
return obj;
				}

				// JSON object special cases.
				// TODO: if (obj is ObjectMap) {
				//	ObjectMap result = (ObjectMap)obj;
				//	for (JsonValue child = jsonData.child; child != null; child = child.next)
				//		result.put(child.name, readValue(elementType, null, child));
				//	return (T)result;
				//}
				//if (obj is ObjectIntMap) {
				//	ObjectIntMap result = (ObjectIntMap)obj;
				//	for (JsonValue child = jsonData.child; child != null; child = child.next)
				//		result.put(child.name, readValue(Integer.class, null, child));
				//	return (T)result;
				//}
				//if (obj is ObjectFloatMap) {
				//	ObjectFloatMap result = (ObjectFloatMap)obj;
				//	for (JsonValue child = jsonData.child; child != null; child = child.next)
				//		result.put(child.name, readValue(Float.class, null, child));
				//	return (T)result;
				//}
				//if (obj is ObjectSet) {
				//	ObjectSet result = (ObjectSet)obj;
				//	for (JsonValue child = jsonData.getChild("values"); child != null; child = child.next)
				//		result.add(readValue(elementType, null, child));
				//	return (T)result;
				//}
				//if (obj is IntMap) {
				//	IntMap result = (IntMap)obj;
				//	for (JsonValue child = jsonData.child; child != null; child = child.next)
				//		result.put(Integer.parseInt(child.name), readValue(elementType, null, child));
				//	return (T)result;
				//}
				//if (obj is LongMap) {
				//	LongMap result = (LongMap)obj;
				//	for (JsonValue child = jsonData.child; child != null; child = child.next)
				//		result.put(Long.parseLong(child.name), readValue(elementType, null, child));
				//	return (T)result;
				//}
				//if (obj is IntSet) {
				//	IntSet result = (IntSet)obj;
				//	for (JsonValue child = jsonData.getChild("values"); child != null; child = child.next)
				//		result.add(child.asInt());
				//	return (T)result;
				//}
				//if (obj is ArrayMap) {
				//	ArrayMap result = (ArrayMap)obj;
				//	for (JsonValue child = jsonData.child; child != null; child = child.next)
				//		result.put(child.name, readValue(elementType, null, child));
				//	return (T)result;
				//}
				//if (object is Map) {
				//	Map result = (Map)object;
				//	for (JsonValue child = jsonData.child; child != null; child = child.next) {
				//		if (child.name.equals(typeName)) continue;
				//		result.put(child.name, readValue(elementType, null, child));
				//	}
				//	return (T)result;
				//}

				readFields(obj, jsonData);
                // TODO: This originally cast to T, should we verify type and throw exception? -RP
return obj;
			}
		}

		if (type != null) {
			Serializer serializer = classToSerializer.get(type);
            // TODO: This originally cast to T, should we verify type and throw exception? -RP
if (serializer != null) return serializer.read(this, jsonData, type);

			if (ClassReflection.isAssignableFrom(typeof(Serializable), type)) {
				// A Serializable may be read as an array, string, etc, even though it will be written as an object.
				Object obj = newInstance(type);
				((Serializable)obj).read(this, jsonData);
                // TODO: This originally cast to T, should we verify type and throw exception? -RP
				return obj;
			}
		}

		if (jsonData.isArray())
        {
            throw new NotImplementedException();
			// JSON array special cases.
			//if (type == null || type == typeof(object)) type = (Class<T>)Array.class;
			//if (ClassReflection.isAssignableFrom(Array.class, type)) {
			//	Array result = type == Array.class ? new Array() : (Array)newInstance(type);
			//	for (JsonValue child = jsonData.child; child != null; child = child.next)
			//		result.add(readValue(elementType, null, child));
			//	return (T)result;
			//}
			//if (ClassReflection.isAssignableFrom(Queue.class, type)) {
			//	Queue result = type == Queue.class ? new Queue() : (Queue)newInstance(type);
			//	for (JsonValue child = jsonData.child; child != null; child = child.next)
			//		result.addLast(readValue(elementType, null, child));
			//	return (T)result;
			//}
			//if (ClassReflection.isAssignableFrom(Collection.class, type)) {
			//	Collection result = type.isInterface() ? new ArrayList() : (Collection)newInstance(type);
			//	for (JsonValue child = jsonData.child; child != null; child = child.next)
			//		result.add(readValue(elementType, null, child));
			//	return (T)result;
			//}
			//if (type.isArray()) {
			//	Class componentType = type.getComponentType();
			//	if (elementType == null) elementType = componentType;
			//	Object result = ArrayReflection.newInstance(componentType, jsonData.size);
			//	int i = 0;
			//	for (JsonValue child = jsonData.child; child != null; child = child.next)
			//		ArrayReflection.set(result, i++, readValue(elementType, null, child));
			//	return (T)result;
			//}
			//throw new SerializationException("Unable to convert value to required type: " + jsonData + " (" + type.getName() + ")");
		}

		if (jsonData.isNumber()) {
			try {
                // TODO: This originally cast to T, should we verify type and throw exception? -RP
if (type == null || type == typeof(float) || type == typeof(float)) return (float)jsonData.asFloat();
				if (type == typeof(int) || type == typeof(int)) return (int)jsonData.asInt();
				if (type == typeof(long) || type == typeof(long)) return (long)jsonData.asLong();
				if (type ==typeof(double) || type == typeof(double)) return (Double)jsonData.asDouble();
				if (type == typeof(string)) return jsonData.asString();
				if (type == typeof(short) || type == typeof(short)) return (short)jsonData.asShort();
				if (type == typeof(byte) || type == typeof(byte)) return (Byte)jsonData.asByte();
			} catch (FormatException ignored) {
			}
			jsonData = new JsonValue(jsonData.asString());
		}

		if (jsonData.isBoolean()) {
			try {
                // TODO: This originally cast to T, should we verify type and throw exception? -RP
if (type == null || type == typeof(bool) || type == typeof(bool)) return (Boolean)jsonData.asBoolean();
			} catch (FormatException ignored) {
				// TODO: Will this catch correctly? -RP
			}
			jsonData = new JsonValue(jsonData.asString());
		}

		if (jsonData.isString()) {
			String str = jsonData.asString();
            // TODO: This originally cast to T, should we verify type and throw exception? -RP
if (type == null || type == typeof(string)) return str;
			try {
                // TODO: This originally cast to T, should we verify type and throw exception? -RP
if (type == typeof(int) || type == typeof(int)) return Convert.ToInt32(str);
				if (type == typeof(float) || type == typeof(float)) return Convert.ToSingle(str);
				if (type == typeof(long) || type == typeof(long)) return Convert.ToInt64(str);
				if (type == typeof(double) || type == typeof(double)) return Convert.ToDouble(str);
				if (type == typeof(short) || type == typeof(short)) return Convert.ToInt16(str);
				if (type == typeof(byte) || type ==  typeof(byte)) return Convert.ToByte(str);
			} catch (FormatException ignored) {
			}

            // TODO: This originally cast to T, should we verify type and throw exception? -RP
            if (type == typeof(bool) || type == typeof(bool)) return Convert.ToBoolean(str);
            // TODO: This originally cast to T, should we verify type and throw exception? -RP
if (type == typeof(char) || type == typeof(char)) return (char)str[0];
			if (ClassReflection.isAssignableFrom(typeof(Enum), type)) {
				Enum[] constants = (Enum[])Enum.GetValues(type);
				for (int i = 0, n = constants.Length; i < n; i++) {
					Enum e = constants[i];
                    // TODO: This originally cast to T, should we verify type and throw exception? -RP
if (str.Equals(convertToString(e))) return e;
				}
			}

			// TODO: if (type == CharSequence.class) return (T)string;
			throw new SerializationException("Unable to convert value to required type: " + jsonData + " (" + type.Name + ")");
		}

		return null;
	}

	/** Each field on the <code>to</code> object is set to the value for the field with the same name on the <code>from</code>
	 * object. The <code>to</code> object must have at least all the fields of the <code>from</code> object with the same name and
	 * type. */
	public void copyFields (Object from, Object to) {
		OrderedMap<String, FieldMetadata> toFields = getFields(to.GetType());
		foreach (var entry in getFields(from.GetType())) {
			FieldMetadata toField = toFields.get(entry.key);
			Field fromField = entry.value.field;
			if (toField == null) throw new SerializationException("To object is missing field: " + entry.key);
			try {
				toField.field.set(to, fromField.get(from));
			} catch (ReflectionException ex) {
				throw new SerializationException("Error copying field: " + fromField.getName(), ex);
			}
		}
	}

	private String convertToString (Enum e) {
		// TODO: Verify that this is correct. -RP
		return enumNames ? e.ToString("G") : e.ToString();
	}

	private String convertToString (Object obj) {
		if (obj is Enum) return convertToString((Enum)obj);
		if (obj is Type) return ((Type)obj).Name;
		return obj.ToString();
	}

	protected Object newInstance (Type type)
    {
		try
		{
			return ClassReflection.newInstance(type);
		}
		catch (Exception ex)
		{
			try
			{
				// Try a private constructor.
				Constructor constructor = ClassReflection.getDeclaredConstructor(type);
				constructor.setAccessible(true);
				return constructor.newInstance();
			}
			// TODO: What would this equate to? -RP
			//catch (SecurityException ignored)
			//{
			//}
			catch (ReflectionException ignored)
			{
				if (ClassReflection.isAssignableFrom(typeof(Enum), type))
				{
					if (type.GetEnumValues() == null) type = type.BaseType;
					return type.GetEnumValues().GetValue(0);
				}
				if (type.IsArray)
					throw new SerializationException("Encountered JSON object when expected array of type: " + type.Name, ex);
				else if (ClassReflection.isMemberClass(type) && !ClassReflection.isStaticClass(type))
					throw new SerializationException("Class cannot be created (non-static member class): " + type.Name, ex);
				else
					throw new SerializationException("Class cannot be created (missing no-arg constructor): " + type.Name, ex);
			}
			catch (Exception privateConstructorException)
			{
				ex = privateConstructorException;
			}
			throw new SerializationException("Error constructing instance of class: " + type.Name, ex);
		}
	}

	public String prettyPrint (Object? obj) {
		return prettyPrint(obj, 0);
	}

	public String prettyPrint (String json) {
		return prettyPrint(json, 0);
	}

	public String prettyPrint (Object? obj, int singleLineColumns) {
		return prettyPrint(toJson(obj), singleLineColumns);
	}

	public String prettyPrint (String json, int singleLineColumns) {
		return new JsonReader().parse(json).prettyPrint(outputType, singleLineColumns);
	}

	public String prettyPrint (Object? obj, JsonValue.PrettyPrintSettings settings) {
		return prettyPrint(toJson(obj), settings);
	}

	public String prettyPrint (String json, JsonValue.PrettyPrintSettings settings) {
		return new JsonReader().parse(json).prettyPrint(settings);
	}

	private class FieldMetadata {
		internal readonly Field field;
		internal Type elementType;
		internal bool deprecated;

		public FieldMetadata (Field field)
        {
			this.field = field;
			// TODO: Implement
			//int index = (ClassReflection.isAssignableFrom(ObjectMap.class, field.getType())
			//	|| ClassReflection.isAssignableFrom(Map.class, field.getType())) ? 1 : 0;
            int index = 0;
            this.elementType = field.getFieldType();// TODO: .getElementType(index);
			deprecated = field.isAnnotationPresent(typeof(ObsoleteAttribute));
		}
	}

	 public interface Serializer {
		public void write (Json json, object obj, Type knownType);

		public object read (Json json, JsonValue jsonData, Type type);
	}

	 abstract public class ReadOnlySerializer : Serializer {
		public void write (Json json, object obj, Type knownType) {
		}

		abstract public object read (Json json, JsonValue jsonData, Type type);
	}

	public interface Serializable {
		public void write (Json json);

		public void read (Json json, JsonValue jsonData);
	}
}
