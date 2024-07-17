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

/** Ordered list of {@link MapLayer} instances owned by a {@link Map} */
public class MapLayers : IEnumerable<MapLayer> {
	private Array<MapLayer> layers = new Array<MapLayer>();

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/** @param index
	 * @return the MapLayer at the specified index */
	public MapLayer get (int index) {
		return layers.get(index);
	}

	/** @param name
	 * @return the first layer having the specified name, if one exists, otherwise null */
	public MapLayer get (String name) {
		for (int i = 0, n = layers.size; i < n; i++) {
			MapLayer layer = layers.get(i);
			if (name.Equals(layer.getName())) {
				return layer;
			}
		}
		return null;
	}

	/** Get the index of the layer having the specified name, or -1 if no such layer exists. */
	public int getIndex (String name) {
		return getIndex(get(name));
	}

	/** Get the index of the layer in the collection, or -1 if no such layer exists. */
	public int getIndex (MapLayer layer) {
		return layers.indexOf(layer, true);
	}

	/** @return number of layers in the collection */
	public int getCount () {
		return layers.size;
	}

	/** @param layer layer to be added to the set */
	public void add (MapLayer layer) {
		this.layers.add(layer);
	}

	/** @param index removes layer at index */
	public void remove (int index) {
		layers.removeIndex(index);
	}

	/** @param layer layer to be removed */
	public void remove (MapLayer layer) {
		layers.removeValue(layer, true);
	}

	/** @return the number of map layers **/
	public int size () {
		return layers.size;
	}

	/** @param type
	 * @return array with all the layers matching type */
	public Array<T> getByType<T> (Type type)
	where T: MapLayer{
		return getByType(type, new Array<T>());
	}

	/** @param type
	 * @param fill array to be filled with the matching layers
	 * @return array with all the layers matching type */
	public Array<T> getByType<T> (Type type, Array<T> fill)
		where T : MapLayer
	{
		fill.clear();
		for (int i = 0, n = layers.size; i < n; i++) {
			MapLayer layer = layers.get(i);
			if (ClassReflection.isInstance(type, layer)) {
				fill.add((T)layer);
			}
		}
		return fill;
	}

	/** @return iterator to set of layers */
	public IEnumerator<MapLayer> GetEnumerator () {
		return layers.GetEnumerator();
	}

}
