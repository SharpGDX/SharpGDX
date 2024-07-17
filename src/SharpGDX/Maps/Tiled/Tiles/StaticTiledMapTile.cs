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

namespace SharpGDX.Maps.Tiled.Tiles;

/** @brief Represents a non changing {@link TiledMapTile} (can be cached) */
public class StaticTiledMapTile : ITiledMapTile {

	private int id;

	private ITiledMapTile.BlendMode blendMode = ITiledMapTile.BlendMode.ALPHA;

	private MapProperties properties;

	private MapObjects objects;

	private TextureRegion textureRegion;

	private float offsetX;

	private float offsetY;

	public int getId () {
		return id;
	}

	public void setId (int id) {
		this.id = id;
	}

	public ITiledMapTile.BlendMode getBlendMode () {
		return blendMode;
	}

	public void setBlendMode (ITiledMapTile.BlendMode blendMode) {
		this.blendMode = blendMode;
	}

	public MapProperties getProperties () {
		if (properties == null) {
			properties = new MapProperties();
		}
		return properties;
	}

	public MapObjects getObjects () {
		if (objects == null) {
			objects = new MapObjects();
		}
		return objects;
	}

	public TextureRegion getTextureRegion () {
		return textureRegion;
	}

	public void setTextureRegion (TextureRegion textureRegion) {
		this.textureRegion = textureRegion;
	}
	
	public float getOffsetX () {
		return offsetX;
	}

	public void setOffsetX (float offsetX) {
		this.offsetX = offsetX;
	}

	public float getOffsetY () {
		return offsetY;
	}
	
	public void setOffsetY (float offsetY) {
		this.offsetY = offsetY;
	}

	/** Creates a static tile with the given region
	 * 
	 * @param textureRegion the {@link TextureRegion} to use. */
	public StaticTiledMapTile (TextureRegion textureRegion) {
		this.textureRegion = textureRegion;
	}

	/** Copy constructor
	 * 
	 * @param copy the StaticTiledMapTile to copy. */
	public StaticTiledMapTile (StaticTiledMapTile copy) {
		if (copy.properties != null) {
			getProperties().putAll(copy.properties);
		}
		this.objects = copy.objects;
		this.textureRegion = copy.textureRegion;
		this.id = copy.id;
	}

}
