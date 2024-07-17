using System.Collections;
using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;

namespace SharpGDX.Maps;

/** @brief Set of string indexed values representing map elements' properties, allowing to retrieve, modify and add properties to
 *        the set. */
public class MapProperties {

	private ObjectMap<String, Object> properties;

	/** Creates an empty properties set */
	public MapProperties () {
		properties = new ObjectMap<String, Object>();
	}

	/** @param key property name
	 * @return true if and only if the property exists */
	public bool containsKey (String key) {
		return properties.containsKey(key);
	}

	/** @param key property name
	 * @return the value for that property if it exists, otherwise, null */
	public Object get (String key) {
		return properties.get(key);
	}

	/** Returns the object for the given key, casting it to clazz.
	 * @param key the key of the object
	 * @param clazz the class of the object
	 * @return the object or null if the object is not in the map
	 * @throws ClassCastException if the object with the given key is not of type clazz */
	public  T get<T>(String key, Type clazz) {
		return (T)get(key);
	}

	/** Returns the object for the given key, casting it to clazz.
	 * @param key the key of the object
	 * @param defaultValue the default value
	 * @param clazz the class of the object
	 * @return the object or the defaultValue if the object is not in the map
	 * @throws ClassCastException if the object with the given key is not of type clazz */
	public T get<T>(String key, T defaultValue, Type clazz) {
		Object obj = get(key);
		return obj == null ? defaultValue : (T)obj;
	}

	/** @param key property name
	 * @param value value to be inserted or modified (if it already existed) */
	public void put (String key, Object value) {
		properties.put(key, value);
	}

	/** @param properties set of properties to be added */
	public void putAll (MapProperties properties) {
		this.properties.putAll(properties.properties);
	}

	/** @param key property name to be removed */
	public void remove (String key) {
		properties.remove(key);
	}

	/** Removes all properties */
	public void clear () {
		properties.clear();
	}

	/** @return iterator for the property names */
	public IEnumerator<String> getKeys () {
		return properties.keys();
	}

	/** @return iterator to properties' values */
	public IEnumerator<Object> getValues () {
		return properties.values();
	}

}
