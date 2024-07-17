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

/** @brief Generalises the concept of tile in a TiledMap */
public interface ITiledMapTile {

	public enum BlendMode {
		NONE, ALPHA
	}

	public int getId ();

	public void setId (int id);

	/** @return the {@link BlendMode} to use for rendering the tile */
	public BlendMode getBlendMode ();

	/** Sets the {@link BlendMode} to use for rendering the tile
	 * 
	 * @param blendMode the blend mode to use for rendering the tile */
	public void setBlendMode (BlendMode blendMode);

	/** @return texture region used to render the tile */
	public TextureRegion getTextureRegion ();

	/** Sets the texture region used to render the tile */
	public void setTextureRegion (TextureRegion textureRegion);

	/** @return the amount to offset the x position when rendering the tile */
	public float getOffsetX ();

	/** Set the amount to offset the x position when rendering the tile */
	public void setOffsetX (float offsetX);

	/** @return the amount to offset the y position when rendering the tile */
	public float getOffsetY ();

	/** Set the amount to offset the y position when rendering the tile */
	public void setOffsetY (float offsetY);

	/** @return tile's properties set */
	public MapProperties getProperties ();

	/** @return collection of objects contained in the tile */
	public MapObjects getObjects ();

}
