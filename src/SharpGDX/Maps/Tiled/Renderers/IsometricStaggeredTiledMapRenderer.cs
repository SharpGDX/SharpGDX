using System.Collections;
using static SharpGDX.Graphics.G2D.IBatch;
using static SharpGDX.Maps.Tiled.TiledMapTileLayer;
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

public class IsometricStaggeredTiledMapRenderer : BatchTiledMapRenderer {

	public IsometricStaggeredTiledMapRenderer (TiledMap map) 
	: base(map)
	{
		
	}

	public IsometricStaggeredTiledMapRenderer (TiledMap map, IBatch batch) 
	: base(map, batch)
	{
		
	}

	public IsometricStaggeredTiledMapRenderer (TiledMap map, float unitScale) 
	: base(map, unitScale)
	{
		
	}

	public IsometricStaggeredTiledMapRenderer (TiledMap map, float unitScale, IBatch batch) 
	: base(map, unitScale, batch)
	{
		
	}

	public override void renderTileLayer (TiledMapTileLayer layer) {
		 Color batchColor = batch.getColor();
		 float color = Color.toFloatBits(batchColor.r, batchColor.g, batchColor.b, batchColor.a * layer.getOpacity());

		 int layerWidth = layer.getWidth();
		 int layerHeight = layer.getHeight();

		 float layerOffsetX = layer.getRenderOffsetX() * unitScale - viewBounds.x * (layer.getParallaxX() - 1);
		// offset in tiled is y down, so we flip it
		 float layerOffsetY = -layer.getRenderOffsetY() * unitScale - viewBounds.y * (layer.getParallaxY() - 1);

		 float layerTileWidth = layer.getTileWidth() * unitScale;
		 float layerTileHeight = layer.getTileHeight() * unitScale;

		 float layerTileWidth50 = layerTileWidth * 0.50f;
		 float layerTileHeight50 = layerTileHeight * 0.50f;

		 int minX = Math.Max(0, (int)(((viewBounds.x - layerTileWidth50 - layerOffsetX) / layerTileWidth)));
		 int maxX = Math.Min(layerWidth,
			(int)((viewBounds.x + viewBounds.width + layerTileWidth + layerTileWidth50 - layerOffsetX) / layerTileWidth));

		 int minY = Math.Max(0, (int)(((viewBounds.y - layerTileHeight - layerOffsetY) / layerTileHeight)));
		 int maxY = Math.Min(layerHeight,
			(int)((viewBounds.y + viewBounds.height + layerTileHeight - layerOffsetY) / layerTileHeight50));

		for (int y = maxY - 1; y >= minY; y--) {
			float offsetX = (y % 2 == 1) ? layerTileWidth50 : 0;
			for (int x = maxX - 1; x >= minX; x--) {
				 TiledMapTileLayer.Cell cell = layer.getCell(x, y);
				if (cell == null) continue;
				 ITiledMapTile tile = cell.getTile();

				if (tile != null) {
					 bool flipX = cell.getFlipHorizontally();
					 bool flipY = cell.getFlipVertically();
					 int rotations = cell.getRotation();
					TextureRegion region = tile.getTextureRegion();

					float x1 = x * layerTileWidth - offsetX + tile.getOffsetX() * unitScale + layerOffsetX;
					float y1 = y * layerTileHeight50 + tile.getOffsetY() * unitScale + layerOffsetY;
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
