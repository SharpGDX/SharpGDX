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

/** @brief Represents a tiled map, adds the concept of tiles and tilesets.
 * 
 * @see Map */
public class TiledMap : Map {
	private TiledMapTileSets tilesets;
	private Array<IDisposable> ownedResources;

	/** @return collection of tilesets for this map. */
	public TiledMapTileSets getTileSets () {
		return tilesets;
	}

	/** Creates an empty TiledMap. */
	public TiledMap () {
		tilesets = new TiledMapTileSets();
	}

	/** Used by loaders to set resources when loading the map directly, without {@link AssetManager}. To be disposed in
	 * {@link #dispose()}.
	 * @param resources */
	public void setOwnedResources (Array<IDisposable> resources) {
		this.ownedResources = resources;
	}

	public override void Dispose () {
		if (ownedResources != null) {
			foreach (IDisposable resource in ownedResources) {
				resource.Dispose();
			}
		}
	}
}
