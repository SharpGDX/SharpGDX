using System.Collections;
using SharpGDX.Maps.Tiled.Tiles;
using SharpGDX.Maps;
using SharpGDX.Scenes.Scene2D.UI;
using static SharpGDX.Graphics.G2D.IBatch;
using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;

namespace SharpGDX.Maps.Tiled.Renderers;

public abstract class BatchTiledMapRenderer : ITiledMapRenderer, Disposable {
	static protected readonly int NUM_VERTICES = 20;

	protected TiledMap map;

	protected float unitScale;

	protected IBatch batch;

	protected Rectangle viewBounds;
	protected Rectangle imageBounds = new Rectangle();

	protected bool ownsBatch;

	protected float[] vertices = new float[NUM_VERTICES];

	public TiledMap getMap () {
		return map;
	}

	public void setMap (TiledMap map) {
		this.map = map;
	}

	public float getUnitScale () {
		return unitScale;
	}

	public IBatch getBatch () {
		return batch;
	}

	public Rectangle getViewBounds () {
		return viewBounds;
	}

	public BatchTiledMapRenderer (TiledMap map) 
	: this(map, 1.0f)
	{
		
	}

	public BatchTiledMapRenderer (TiledMap map, float unitScale) {
		this.map = map;
		this.unitScale = unitScale;
		this.viewBounds = new Rectangle();
		this.batch = new SpriteBatch();
		this.ownsBatch = true;
	}

	public BatchTiledMapRenderer (TiledMap map, IBatch batch) 
	: this(map, 1.0f, batch)
	{
		
	}

	public BatchTiledMapRenderer (TiledMap map, float unitScale, IBatch batch) {
		this.map = map;
		this.unitScale = unitScale;
		this.viewBounds = new Rectangle();
		this.batch = batch;
		this.ownsBatch = false;
	}

	public void setView (OrthographicCamera camera) {
		batch.setProjectionMatrix(camera.combined);
		float width = camera.viewportWidth * camera.zoom;
		float height = camera.viewportHeight * camera.zoom;
		float w = width * Math.Abs(camera.up.y) + height * Math.Abs(camera.up.x);
		float h = height * Math.Abs(camera.up.y) + width * Math.Abs(camera.up.x);
		viewBounds.set(camera.position.x - w / 2, camera.position.y - h / 2, w, h);
	}

	public void setView (Matrix4 projection, float x, float y, float width, float height) {
		batch.setProjectionMatrix(projection);
		viewBounds.set(x, y, width, height);
	}

	public void render () {
		beginRender();
		foreach (MapLayer layer in map.getLayers()) {
			renderMapLayer(layer);
		}
		endRender();
	}

	public void render (int[] layers) {
		beginRender();
		foreach (int layerIdx in layers) {
			MapLayer layer = map.getLayers().get(layerIdx);
			renderMapLayer(layer);
		}
		endRender();
	}

	protected void renderMapLayer (MapLayer layer) {
		if (!layer.isVisible()) return;
		if (layer is MapGroupLayer) {
			MapLayers childLayers = ((MapGroupLayer)layer).getLayers();
			for (int i = 0; i < childLayers.size(); i++) {
				MapLayer childLayer = childLayers.get(i);
				if (!childLayer.isVisible()) continue;
				renderMapLayer(childLayer);
			}
		} else {
			if (layer is TiledMapTileLayer) {
				renderTileLayer((TiledMapTileLayer)layer);
			} else if (layer is TiledMapImageLayer) {
				renderImageLayer((TiledMapImageLayer)layer);
			} else {
				renderObjects(layer);
			}
		}
	}

	public void renderObjects (MapLayer layer) {
		foreach (MapObject obj in layer.getObjects()) {
			renderObject(obj);
		}
	}

	public void renderObject (MapObject obj) {

	}

	public abstract void renderTileLayer(TiledMapTileLayer layer);

	public void renderImageLayer (TiledMapImageLayer layer) {
		 Color batchColor = batch.getColor();
		 float color = Color.toFloatBits(batchColor.r, batchColor.g, batchColor.b, batchColor.a * layer.getOpacity());

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

		imageBounds.set(x1, y1, x2 - x1, y2 - y1);

		if (viewBounds.contains(imageBounds) || viewBounds.overlaps(imageBounds)) {
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

			batch.draw(region.getTexture(), vertices, 0, NUM_VERTICES);
		}
	}

	/** Called before the rendering of all layers starts. */
	protected void beginRender () {
		AnimatedTiledMapTile.updateAnimationBaseTime();
		batch.begin();
	}

	/** Called after the rendering of all layers ended. */
	protected void endRender () {
		batch.end();
	}

	public void dispose () {
		if (ownsBatch) {
			batch.dispose();
		}
	}

}
