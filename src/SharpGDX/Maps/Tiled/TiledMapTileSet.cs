using System.Collections;
using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;

namespace SharpGDX.Maps.Tiled;

/** @brief Set of {@link TiledMapTile} instances used to compose a TiledMapLayer */
public class TiledMapTileSet : IEnumerable<ITiledMapTile> {

	private String name;

	private IntMap<ITiledMapTile> tiles;

	private MapProperties properties;

	/** @return tileset's name */
	public String getName () {
		return name;
	}

	/** @param name new name for the tileset */
	public void setName (String name) {
		this.name = name;
	}

	/** @return tileset's properties set */
	public MapProperties getProperties () {
		return properties;
	}

	/** Creates empty tileset */
	public TiledMapTileSet () {
		tiles = new IntMap<ITiledMapTile>();
		properties = new MapProperties();
	}

	/** Gets the {@link TiledMapTile} that has the given id.
	 * 
	 * @param id the id of the {@link TiledMapTile} to retrieve.
	 * @return tile matching id, null if it doesn't exist */
	public ITiledMapTile getTile (int id) {
		return tiles.get(id);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/** @return iterator to tiles in this tileset */
	public IEnumerator<ITiledMapTile> GetEnumerator() {
		return tiles.values().GetEnumerator();
	}

	/** Adds or replaces tile with that id
	 * 
	 * @param id the id of the {@link TiledMapTile} to add or replace.
	 * @param tile the {@link TiledMapTile} to add or replace. */
	public void putTile (int id, ITiledMapTile tile) {
		tiles.put(id, tile);
	}

	/** @param id tile's id to be removed */
	public void removeTile (int id) {
		tiles.remove(id);
	}

	/** @return the size of this TiledMapTileSet. */
	public int size () {
		return tiles.size;
	}
}
