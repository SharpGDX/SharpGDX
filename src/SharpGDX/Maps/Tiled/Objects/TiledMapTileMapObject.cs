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
using SharpGDX.Maps.Objects;

namespace SharpGDX.Maps.Tiled.Objects;

/** A {@link MapObject} with a {@link TiledMapTile}. Can be both {@link StaticTiledMapTile} or {@link AnimatedTiledMapTile}. For
 * compatibility reasons, this extends {@link TextureMapObject}. Use {@link TiledMapTile#getTextureRegion()} instead of
 * {@link #getTextureRegion()}.
 * @author Daniel Holderbaum */
public class TiledMapTileMapObject : TextureMapObject {

	private bool flipHorizontally;
	private bool flipVertically;

	private ITiledMapTile tile;

	public TiledMapTileMapObject (ITiledMapTile tile, bool flipHorizontally, bool flipVertically) {
		this.flipHorizontally = flipHorizontally;
		this.flipVertically = flipVertically;
		this.tile = tile;

		TextureRegion textureRegion = new TextureRegion(tile.getTextureRegion());
		textureRegion.flip(flipHorizontally, flipVertically);
		setTextureRegion(textureRegion);
	}

	public bool isFlipHorizontally () {
		return flipHorizontally;
	}

	public void setFlipHorizontally (bool flipHorizontally) {
		this.flipHorizontally = flipHorizontally;
	}

	public bool isFlipVertically () {
		return flipVertically;
	}

	public void setFlipVertically (bool flipVertically) {
		this.flipVertically = flipVertically;
	}

	public ITiledMapTile getTile () {
		return tile;
	}

	public void setTile (ITiledMapTile tile) {
		this.tile = tile;
	}

}
