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

/** @brief Collection of MapObject instances */
public class MapObjects : IEnumerable<MapObject> {

	private Array<MapObject> objects;

	/** Creates an empty set of MapObject instances */
	public MapObjects () {
		objects = new Array<MapObject>();
	}

	/** @param index
	 * @return the MapObject at the specified index */
	public MapObject get (int index) {
		return objects.get(index);
	}

	/** @param name
	 * @return the first object having the specified name, if one exists, otherwise null */
	public MapObject get (String name) {
		for (int i = 0, n = objects.size; i < n; i++) {
			MapObject obj = objects.get(i);
			if (name.Equals(obj.getName())) {
				return obj;
			}
		}
		return null;
	}

	/** Get the index of the object having the specified name, or -1 if no such object exists. */
	public int getIndex (String name) {
		return getIndex(get(name));
	}

	/** Get the index of the object in the collection, or -1 if no such object exists. */
	public int getIndex (MapObject obj) {
		return objects.indexOf(obj, true);
	}

	/** @return number of objects in the collection */
	public int getCount () {
		return objects.size;
	}

	/** @param object instance to be added to the collection */
	public void add (MapObject obj) {
		this.objects.add(obj);
	}

	/** @param index removes MapObject instance at index */
	public void remove (int index) {
		objects.removeIndex(index);
	}

	/** @param object instance to be removed */
	public void remove (MapObject obj) {
		objects.removeValue(obj, true);
	}

	/** @param type class of the objects we want to retrieve
	 * @return array filled with all the objects in the collection matching type */
	public Array<T> getByType<T> (Type type)
	where T: MapObject{
		return getByType(type, new Array<T>());
	}

	/** @param type class of the objects we want to retrieve
	 * @param fill collection to put the returned objects in
	 * @return array filled with all the objects in the collection matching type */
	public Array<T> getByType<T>(Type type, Array<T> fill)
		where T : MapObject
	{
		fill.clear();
		for (int i = 0, n = objects.size; i < n; i++) {
			MapObject obj = objects.get(i);
			if (ClassReflection.isInstance(type, obj)) {
				fill.add((T)obj);
			}
		}
		return fill;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/** @return iterator for the objects within the collection */
	public IEnumerator<MapObject> GetEnumerator() {
		return objects.GetEnumerator();
	}

}
