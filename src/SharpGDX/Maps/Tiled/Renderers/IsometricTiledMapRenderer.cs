using System.Collections;
using static SharpGDX.Maps.Tiled.TiledMapTileLayer;
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

public class IsometricTiledMapRenderer : BatchTiledMapRenderer {

	private Matrix4 isoTransform;
	private Matrix4 invIsotransform;
	private Vector3 screenPos = new Vector3();

	private Vector2 topRight = new Vector2();
	private Vector2 bottomLeft = new Vector2();
	private Vector2 topLeft = new Vector2();
	private Vector2 bottomRight = new Vector2();

	public IsometricTiledMapRenderer (TiledMap map) 
	: base(map)
	{
		
		init();
	}

	public IsometricTiledMapRenderer (TiledMap map, IBatch batch) 
	: base(map, batch)
	{
		
		init();
	}

	public IsometricTiledMapRenderer (TiledMap map, float unitScale) 
	: base(map, unitScale)
	{
		
		init();
	}

	public IsometricTiledMapRenderer (TiledMap map, float unitScale, IBatch batch) 
	: base(map, unitScale, batch)
	{
		
		init();
	}

	private void init () {
		// create the isometric transform
		isoTransform = new Matrix4();
		isoTransform.idt();

		// isoTransform.translate(0, 32, 0);
		isoTransform.scale((float)(Math.Sqrt(2.0) / 2.0), (float)(Math.Sqrt(2.0) / 4.0), 1.0f);
		isoTransform.rotate(0.0f, 0.0f, 1.0f, -45);

		// ... and the inverse matrix
		invIsotransform = new Matrix4(isoTransform);
		invIsotransform.inv();
	}

	private Vector3 translateScreenToIso (Vector2 vec) {
		screenPos.set(vec.x, vec.y, 0);
		screenPos.mul(invIsotransform);

		return screenPos;
	}

	public override void renderTileLayer (TiledMapTileLayer layer) {
		 Color batchColor = batch.getColor();
		 float color = Color.toFloatBits(batchColor.r, batchColor.g, batchColor.b, batchColor.a * layer.getOpacity());

		float tileWidth = layer.getTileWidth() * unitScale;
		float tileHeight = layer.getTileHeight() * unitScale;

		 float layerOffsetX = layer.getRenderOffsetX() * unitScale - viewBounds.x * (layer.getParallaxX() - 1);
		// offset in tiled is y down, so we flip it
		 float layerOffsetY = -layer.getRenderOffsetY() * unitScale - viewBounds.y * (layer.getParallaxY() - 1);

		float halfTileWidth = tileWidth * 0.5f;
		float halfTileHeight = tileHeight * 0.5f;

		// setting up the screen points
		// COL1
		topRight.set(viewBounds.x + viewBounds.width - layerOffsetX, viewBounds.y - layerOffsetY);
		// COL2
		bottomLeft.set(viewBounds.x - layerOffsetX, viewBounds.y + viewBounds.height - layerOffsetY);
		// ROW1
		topLeft.set(viewBounds.x - layerOffsetX, viewBounds.y - layerOffsetY);
		// ROW2
		bottomRight.set(viewBounds.x + viewBounds.width - layerOffsetX, viewBounds.y + viewBounds.height - layerOffsetY);

		// transforming screen coordinates to iso coordinates
		int row1 = (int)(translateScreenToIso(topLeft).y / tileWidth) - 2;
		int row2 = (int)(translateScreenToIso(bottomRight).y / tileWidth) + 2;

		int col1 = (int)(translateScreenToIso(bottomLeft).x / tileWidth) - 2;
		int col2 = (int)(translateScreenToIso(topRight).x / tileWidth) + 2;

		for (int row = row2; row >= row1; row--) {
			for (int col = col1; col <= col2; col++) {
				float x = (col * halfTileWidth) + (row * halfTileWidth);
				float y = (row * halfTileHeight) - (col * halfTileHeight);

				 TiledMapTileLayer.Cell cell = layer.getCell(col, row);
				if (cell == null) continue;
				 ITiledMapTile tile = cell.getTile();

				if (tile != null) {
					 bool flipX = cell.getFlipHorizontally();
					 bool flipY = cell.getFlipVertically();
					 int rotations = cell.getRotation();

					TextureRegion region = tile.getTextureRegion();

					float x1 = x + tile.getOffsetX() * unitScale + layerOffsetX;
					float y1 = y + tile.getOffsetY() * unitScale + layerOffsetY;
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
					batch.draw(region.getTexture(), vertices, 0, NUM_VERTICES);
				}
			}
		}
	}
}
