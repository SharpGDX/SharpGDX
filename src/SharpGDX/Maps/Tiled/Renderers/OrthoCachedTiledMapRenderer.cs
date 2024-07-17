using System.Collections;
using static SharpGDX.Maps.Tiled.TiledMapTileLayer;
using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using static SharpGDX.Graphics.G2D.IBatch;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;

namespace SharpGDX.Maps.Tiled.Renderers;

/** Renders ortho tiles by caching geometry on the GPU. How much is cached is controlled by {@link #setOverCache(float)}. When the
 * view reaches the edge of the cached tiles, the cache is rebuilt at the new view position.
 * <p>
 * This class may have poor performance when tiles are often changed dynamically, since the cache must be rebuilt after each
 * change.
 * @author Justin Shapcott
 * @author Nathan Sweet */
public class OrthoCachedTiledMapRenderer : ITiledMapRenderer, Disposable {
	static private readonly float tolerance = 0.00001f;
	static protected readonly int NUM_VERTICES = 20;

	protected readonly TiledMap map;
	protected readonly SpriteCache spriteCache;

	protected readonly float[] vertices = new float[20];
	protected bool blending;

	protected float unitScale;
	protected readonly Rectangle viewBounds = new Rectangle();
	protected readonly Rectangle cacheBounds = new Rectangle();

	protected float overCache = 0.50f;
	protected float maxTileWidth, maxTileHeight;
	protected bool cached;
	protected int count;
	protected bool canCacheMoreN, canCacheMoreE, canCacheMoreW, canCacheMoreS;

	/** Creates a renderer with a unit scale of 1 and cache size of 2000. */
	public OrthoCachedTiledMapRenderer (TiledMap map) 
	: this(map, 1, 2000)
	{
		
	}

	/** Creates a renderer with a cache size of 2000. */
	public OrthoCachedTiledMapRenderer (TiledMap map, float unitScale) 
	: this(map, unitScale, 2000)
	{
		
	}

	/** @param cacheSize The maximum number of tiles that can be cached. */
	public OrthoCachedTiledMapRenderer (TiledMap map, float unitScale, int cacheSize) {
		this.map = map;
		this.unitScale = unitScale;
		spriteCache = new SpriteCache(cacheSize, true);
	}

	public void setView (OrthographicCamera camera) {
		spriteCache.setProjectionMatrix(camera.combined);
		float width = camera.viewportWidth * camera.zoom + maxTileWidth * 2 * unitScale;
		float height = camera.viewportHeight * camera.zoom + maxTileHeight * 2 * unitScale;
		viewBounds.set(camera.position.x - width / 2, camera.position.y - height / 2, width, height);

		if ((canCacheMoreW && viewBounds.x < cacheBounds.x - tolerance) || //
			(canCacheMoreS && viewBounds.y < cacheBounds.y - tolerance) || //
			(canCacheMoreE && viewBounds.x + viewBounds.width > cacheBounds.x + cacheBounds.width + tolerance) || //
			(canCacheMoreN && viewBounds.y + viewBounds.height > cacheBounds.y + cacheBounds.height + tolerance) //
		) cached = false;
	}

	public void setView (Matrix4 projection, float x, float y, float width, float height) {
		spriteCache.setProjectionMatrix(projection);
		x -= maxTileWidth * unitScale;
		y -= maxTileHeight * unitScale;
		width += maxTileWidth * 2 * unitScale;
		height += maxTileHeight * 2 * unitScale;
		viewBounds.set(x, y, width, height);

		if ((canCacheMoreW && viewBounds.x < cacheBounds.x - tolerance) || //
			(canCacheMoreS && viewBounds.y < cacheBounds.y - tolerance) || //
			(canCacheMoreE && viewBounds.x + viewBounds.width > cacheBounds.x + cacheBounds.width + tolerance) || //
			(canCacheMoreN && viewBounds.y + viewBounds.height > cacheBounds.y + cacheBounds.height + tolerance) //
		) cached = false;
	}

	public void render () {
		if (!cached) {
			cached = true;
			count = 0;
			spriteCache.clear();

			 float extraWidth = viewBounds.width * overCache;
			 float extraHeight = viewBounds.height * overCache;
			cacheBounds.x = viewBounds.x - extraWidth;
			cacheBounds.y = viewBounds.y - extraHeight;
			cacheBounds.width = viewBounds.width + extraWidth * 2;
			cacheBounds.height = viewBounds.height + extraHeight * 2;

			foreach (MapLayer layer in map.getLayers()) {
				spriteCache.beginCache();
				if (layer is TiledMapTileLayer) {
					renderTileLayer((TiledMapTileLayer)layer);
				} else if (layer is TiledMapImageLayer) {
					renderImageLayer((TiledMapImageLayer)layer);
				}
				spriteCache.endCache();
			}
		}

		if (blending) {
			Gdx.gl.glEnable(GL20.GL_BLEND);
			Gdx.gl.glBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
		}
		spriteCache.begin();
		MapLayers mapLayers = map.getLayers();
		for (int i = 0, j = mapLayers.getCount(); i < j; i++) {
			MapLayer layer = mapLayers.get(i);
			if (layer.isVisible()) {
				spriteCache.draw(i);
				renderObjects(layer);
			}
		}
		spriteCache.end();
		if (blending) Gdx.gl.glDisable(GL20.GL_BLEND);
	}

	public void render (int[] layers) {
		if (!cached) {
			cached = true;
			count = 0;
			spriteCache.clear();

			 float extraWidth = viewBounds.width * overCache;
			 float extraHeight = viewBounds.height * overCache;
			cacheBounds.x = viewBounds.x - extraWidth;
			cacheBounds.y = viewBounds.y - extraHeight;
			cacheBounds.width = viewBounds.width + extraWidth * 2;
			cacheBounds.height = viewBounds.height + extraHeight * 2;

			foreach (MapLayer layer in map.getLayers()) {
				spriteCache.beginCache();
				if (layer is TiledMapTileLayer) {
					renderTileLayer((TiledMapTileLayer)layer);
				} else if (layer is TiledMapImageLayer) {
					renderImageLayer((TiledMapImageLayer)layer);
				}
				spriteCache.endCache();
			}
		}

		if (blending) {
			Gdx.gl.glEnable(GL20.GL_BLEND);
			Gdx.gl.glBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
		}
		spriteCache.begin();
		MapLayers mapLayers = map.getLayers();
		foreach (int i in layers) {
			MapLayer layer = mapLayers.get(i);
			if (layer.isVisible()) {
				spriteCache.draw(i);
				renderObjects(layer);
			}
		}
		spriteCache.end();
		if (blending) Gdx.gl.glDisable(GL20.GL_BLEND);
	}

	public void renderObjects (MapLayer layer) {
		foreach (MapObject obj in layer.getObjects()) {
			renderObject(obj);
		}
	}

	public void renderObject (MapObject obj) {
	}

	public void renderTileLayer (TiledMapTileLayer layer) {
		 float color = Color.toFloatBits(1, 1, 1, layer.getOpacity());

		 int layerWidth = layer.getWidth();
		 int layerHeight = layer.getHeight();

		 float layerTileWidth = layer.getTileWidth() * unitScale;
		 float layerTileHeight = layer.getTileHeight() * unitScale;

		 float layerOffsetX = layer.getRenderOffsetX() * unitScale - viewBounds.x * (layer.getParallaxX() - 1);
		// offset in tiled is y down, so we flip it
		 float layerOffsetY = -layer.getRenderOffsetY() * unitScale - viewBounds.y * (layer.getParallaxY() - 1);

		 int col1 = Math.Max(0, (int)((cacheBounds.x - layerOffsetX) / layerTileWidth));
		 int col2 = Math.Min(layerWidth,
			(int)((cacheBounds.x + cacheBounds.width + layerTileWidth - layerOffsetX) / layerTileWidth));

		 int row1 = Math.Max(0, (int)((cacheBounds.y - layerOffsetY) / layerTileHeight));
		 int row2 = Math.Min(layerHeight,
			(int)((cacheBounds.y + cacheBounds.height + layerTileHeight - layerOffsetY) / layerTileHeight));

		canCacheMoreN = row2 < layerHeight;
		canCacheMoreE = col2 < layerWidth;
		canCacheMoreW = col1 > 0;
		canCacheMoreS = row1 > 0;

		float[] vertices = this.vertices;
		for (int row = row2; row >= row1; row--) {
			for (int col = col1; col < col2; col++) {
				 TiledMapTileLayer.Cell cell = layer.getCell(col, row);
				if (cell == null) continue;

				 ITiledMapTile tile = cell.getTile();
				if (tile == null) continue;

				count++;
				 bool flipX = cell.getFlipHorizontally();
				 bool flipY = cell.getFlipVertically();
				 int rotations = cell.getRotation();

				 TextureRegion region = tile.getTextureRegion();
				 Texture texture = region.getTexture();

				 float x1 = col * layerTileWidth + tile.getOffsetX() * unitScale + layerOffsetX;
				 float y1 = row * layerTileHeight + tile.getOffsetY() * unitScale + layerOffsetY;
				 float x2 = x1 + region.getRegionWidth() * unitScale;
				 float y2 = y1 + region.getRegionHeight() * unitScale;

				 float adjustX = 0.5f / texture.getWidth();
				 float adjustY = 0.5f / texture.getHeight();
				 float u1 = region.getU() + adjustX;
				 float v1 = region.getV2() - adjustY;
				 float u2 = region.getU2() - adjustX;
				 float v2 = region.getV() + adjustY;

				vertices[X1] = x1;
				vertices[Y1] = y1;
				vertices[C1] = color;
				vertices[U1] = u1;
				vertices[V1] = v1;

				vertices[X2] = x1;
				vertices[Y2] = y2;
				vertices[C2] = color;
				vertices[U2] = u1;
				vertices[V2] = v2;

				vertices[X3] = x2;
				vertices[Y3] = y2;
				vertices[C3] = color;
				vertices[U3] = u2;
				vertices[V3] = v2;

				vertices[X4] = x2;
				vertices[Y4] = y1;
				vertices[C4] = color;
				vertices[U4] = u2;
				vertices[V4] = v1;

				if (flipX) {
					float temp = vertices[U1];
					vertices[U1] = vertices[U3];
					vertices[U3] = temp;
					temp = vertices[U2];
					vertices[U2] = vertices[U4];
					vertices[U4] = temp;
				}
				if (flipY) {
					float temp = vertices[V1];
					vertices[V1] = vertices[V3];
					vertices[V3] = temp;
					temp = vertices[V2];
					vertices[V2] = vertices[V4];
					vertices[V4] = temp;
				}
				if (rotations != 0) {
					switch (rotations) {
					case Cell.ROTATE_90: {
						float tempV = vertices[V1];
						vertices[V1] = vertices[V2];
						vertices[V2] = vertices[V3];
						vertices[V3] = vertices[V4];
						vertices[V4] = tempV;

						float tempU = vertices[U1];
						vertices[U1] = vertices[U2];
						vertices[U2] = vertices[U3];
						vertices[U3] = vertices[U4];
						vertices[U4] = tempU;
						break;
					}
					case Cell.ROTATE_180: {
						float tempU = vertices[U1];
						vertices[U1] = vertices[U3];
						vertices[U3] = tempU;
						tempU = vertices[U2];
						vertices[U2] = vertices[U4];
						vertices[U4] = tempU;
						float tempV = vertices[V1];
						vertices[V1] = vertices[V3];
						vertices[V3] = tempV;
						tempV = vertices[V2];
						vertices[V2] = vertices[V4];
						vertices[V4] = tempV;
						break;
					}
					case Cell.ROTATE_270: {
						float tempV = vertices[V1];
						vertices[V1] = vertices[V4];
						vertices[V4] = vertices[V3];
						vertices[V3] = vertices[V2];
						vertices[V2] = tempV;

						float tempU = vertices[U1];
						vertices[U1] = vertices[U4];
						vertices[U4] = vertices[U3];
						vertices[U3] = vertices[U2];
						vertices[U2] = tempU;
						break;
					}
					}
				}
				spriteCache.add(texture, vertices, 0, NUM_VERTICES);
			}
		}
	}

	
	public void renderImageLayer (TiledMapImageLayer layer) {
		 float color = Color.toFloatBits(1.0f, 1.0f, 1.0f, layer.getOpacity());
		 float[] vertices = this.vertices;

		TextureRegion region = layer.getTextureRegion();

		if (region == null) {
			return;
		}

		 float x = layer.getX();
		 float y = layer.getY();
		 float x1 = x * unitScale - viewBounds.x * (layer.getParallaxX() - 1);
		 float y1 = y * unitScale - viewBounds.y * (layer.getParallaxY() - 1);
		 float x2 = x1 + region.getRegionWidth() * unitScale;
		 float y2 = y1 + region.getRegionHeight() * unitScale;

		 float u1 = region.getU();
		 float v1 = region.getV2();
		 float u2 = region.getU2();
		 float v2 = region.getV();

		vertices[X1] = x1;
		vertices[Y1] = y1;
		vertices[C1] = color;
		vertices[U1] = u1;
		vertices[V1] = v1;

		vertices[X2] = x1;
		vertices[Y2] = y2;
		vertices[C2] = color;
		vertices[U2] = u1;
		vertices[V2] = v2;

		vertices[X3] = x2;
		vertices[Y3] = y2;
		vertices[C3] = color;
		vertices[U3] = u2;
		vertices[V3] = v2;

		vertices[X4] = x2;
		vertices[Y4] = y1;
		vertices[C4] = color;
		vertices[U4] = u2;
		vertices[V4] = v1;

		spriteCache.add(region.getTexture(), vertices, 0, NUM_VERTICES);
	}

	/** Causes the cache to be rebuilt the next time it is rendered. */
	public void invalidateCache () {
		cached = false;
	}

	/** Returns true if tiles are currently cached. */
	public bool isCached () {
		return cached;
	}

	/** Sets the percentage of the view that is cached in each direction. Default is 0.5.
	 * <p>
	 * Eg, 0.75 will cache 75% of the width of the view to the left and right of the view, and 75% of the height of the view above
	 * and below the view. */
	public void setOverCache (float overCache) {
		this.overCache = overCache;
	}

	/** Expands the view size in each direction, ensuring that tiles of this size or smaller are never culled from the visible
	 * portion of the view. Default is 0,0.
	 * <p>
	 * The amount of tiles cached is computed using <code>(view size + max tile size) * overCache</code>, meaning the max tile size
	 * increases the amount cached and possibly {@link #setOverCache(float)} can be reduced.
	 * <p>
	 * If the view size and {@link #setOverCache(float)} are configured so the size of the cached tiles is always larger than the
	 * largest tile size, this setting is not needed. */
	public void setMaxTileSize (float maxPixelWidth, float maxPixelHeight) {
		this.maxTileWidth = maxPixelWidth;
		this.maxTileHeight = maxPixelHeight;
	}

	public void setBlending (bool blending) {
		this.blending = blending;
	}

	public SpriteCache getSpriteCache () {
		return spriteCache;
	}

	public void dispose () {
		spriteCache.dispose();
	}
}
