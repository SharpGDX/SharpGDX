using System.Collections;
using SharpGDX.Utils.Reflect;
using static SharpGDX.Graphics.G2D.IBatch;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;

namespace SharpGDX.Maps.Tiled.Renderers;

public class HexagonalTiledMapRenderer : BatchTiledMapRenderer {

	/** true for X-Axis, false for Y-Axis */
	private bool staggerAxisX = true;
	/** true for even StaggerIndex, false for odd */
	private bool staggerIndexEven = false;
	/** the parameter defining the shape of the hexagon from tiled. more specifically it represents the length of the sides that
	 * are parallel to the stagger axis. e.g. with respect to the stagger axis a value of 0 results in a rhombus shape, while a
	 * value equal to the tile length/height represents a square shape and a value of 0.5 represents a regular hexagon if tile
	 * length equals tile height */
	private float hexSideLength = 0f;

	public HexagonalTiledMapRenderer (TiledMap map) 
	: base(map)
	{
		
		init(map);
	}

	public HexagonalTiledMapRenderer (TiledMap map, float unitScale) 
	: base(map, unitScale)
	{
		
		init(map);
	}

	public HexagonalTiledMapRenderer (TiledMap map, IBatch batch) 
	: base(map, batch)
	{
		
		init(map);
	}

	public HexagonalTiledMapRenderer (TiledMap map, float unitScale, IBatch batch) 
	: base(map, unitScale, batch)
	{
		
		init(map);
	}

	private void init (TiledMap map)
	{
		throw new NotImplementedException();
		//String axis = map.getProperties().get("staggeraxis", typeof(String));
		//if (axis != null) {
		//	if (axis.Equals("x")) {
		//		staggerAxisX = true;
		//	} else {
		//		staggerAxisX = false;
		//	}
		//}

		//String index = map.getProperties().get("staggerindex", typeof(String));
		//if (index != null) {
		//	if (index.Equals("even")) {
		//		staggerIndexEven = true;
		//	} else {
		//		staggerIndexEven = false;
		//	}
		//}

		//// due to y-axis being different we need to change stagger index in even map height situations as else it would render
		//// differently.
		//if (!staggerAxisX && map.getProperties().get("height", typeof(int) ) % 2 == 0) staggerIndexEven = !staggerIndexEven;

		//int length = map.getProperties().get("hexsidelength", typeof(int));
		//if (length != null) {
		//	hexSideLength = length.intValue();
		//} else {
		//	if (staggerAxisX) {
		//		length = map.getProperties().get("tilewidth", typeof(int));
		//		if (length != null) {
		//			hexSideLength = 0.5f * length.intValue();
		//		} else {
		//			TiledMapTileLayer tmtl = (TiledMapTileLayer)map.getLayers().get(0);
		//			hexSideLength = 0.5f * tmtl.getTileWidth();
		//		}
		//	} else {
		//		length = map.getProperties().get("tileheight", typeof(int));
		//		if (length != null) {
		//			hexSideLength = 0.5f * length.intValue();
		//		} else {
		//			TiledMapTileLayer tmtl = (TiledMapTileLayer)map.getLayers().get(0);
		//			hexSideLength = 0.5f * tmtl.getTileHeight();
		//		}
		//	}
		//}
	}

	public override void renderTileLayer (TiledMapTileLayer layer) {
		 Color batchColor = batch.getColor();
		 float color = Color.toFloatBits(batchColor.r, batchColor.g, batchColor.b, batchColor.a * layer.getOpacity());

		 int layerWidth = layer.getWidth();
		 int layerHeight = layer.getHeight();

		 float layerTileWidth = layer.getTileWidth() * unitScale;
		 float layerTileHeight = layer.getTileHeight() * unitScale;

		 float layerOffsetX = layer.getRenderOffsetX() * unitScale - viewBounds.x * (layer.getParallaxX() - 1);
		// offset in tiled is y down, so we flip it
		 float layerOffsetY = -layer.getRenderOffsetY() * unitScale - viewBounds.y * (layer.getParallaxY() - 1);

		 float layerHexLength = hexSideLength * unitScale;

		if (staggerAxisX) {
			 float tileWidthLowerCorner = (layerTileWidth - layerHexLength) / 2;
			 float tileWidthUpperCorner = (layerTileWidth + layerHexLength) / 2;
			 float layerTileHeight50 = layerTileHeight * 0.50f;

			 int row1 = Math.Max(0, (int)((viewBounds.y - layerTileHeight50 - layerOffsetX) / layerTileHeight));
			 int row2 = Math.Min(layerHeight,
				(int)((viewBounds.y + viewBounds.height + layerTileHeight - layerOffsetX) / layerTileHeight));

			 int col1 = Math.Max(0, (int)(((viewBounds.x - tileWidthLowerCorner - layerOffsetY) / tileWidthUpperCorner)));
			 int col2 = Math.Min(layerWidth,
				(int)((viewBounds.x + viewBounds.width + tileWidthUpperCorner - layerOffsetY) / tileWidthUpperCorner));

			// depending on the stagger index either draw all even before the odd or vice versa
			 int colA = (staggerIndexEven == (col1 % 2 == 0)) ? col1 + 1 : col1;
			 int colB = (staggerIndexEven == (col1 % 2 == 0)) ? col1 : col1 + 1;

			for (int row = row2 - 1; row >= row1; row--) {
				for (int col = colA; col < col2; col += 2) {
					renderCell(layer.getCell(col, row), tileWidthUpperCorner * col + layerOffsetX,
						layerTileHeight50 + (layerTileHeight * row) + layerOffsetY, color);
				}
				for (int col = colB; col < col2; col += 2) {
					renderCell(layer.getCell(col, row), tileWidthUpperCorner * col + layerOffsetX,
						layerTileHeight * row + layerOffsetY, color);
				}
			}
		} else {
			 float tileHeightLowerCorner = (layerTileHeight - layerHexLength) / 2;
			 float tileHeightUpperCorner = (layerTileHeight + layerHexLength) / 2;
			 float layerTileWidth50 = layerTileWidth * 0.50f;

			 int row1 = Math.Max(0, (int)(((viewBounds.y - tileHeightLowerCorner - layerOffsetX) / tileHeightUpperCorner)));
			 int row2 = Math.Min(layerHeight,
				(int)((viewBounds.y + viewBounds.height + tileHeightUpperCorner - layerOffsetX) / tileHeightUpperCorner));

			 int col1 = Math.Max(0, (int)(((viewBounds.x - layerTileWidth50 - layerOffsetY) / layerTileWidth)));
			 int col2 = Math.Min(layerWidth,
				(int)((viewBounds.x + viewBounds.width + layerTileWidth - layerOffsetY) / layerTileWidth));

			float shiftX = 0;
			for (int row = row2 - 1; row >= row1; row--) {
				// depending on the stagger index either shift for even or uneven indexes
				if ((row % 2 == 0) == staggerIndexEven)
					shiftX = layerTileWidth50;
				else
					shiftX = 0;
				for (int col = col1; col < col2; col++) {
					renderCell(layer.getCell(col, row), layerTileWidth * col + shiftX + layerOffsetX,
						tileHeightUpperCorner * row + layerOffsetY, color);
				}
			}
		}
	}

	/** render a single cell */
	private void renderCell ( TiledMapTileLayer.Cell cell,  float x,  float y,  float color) {
		if (cell != null) {
			 ITiledMapTile tile = cell.getTile();
			if (tile != null) {
				if (tile is Tiles.AnimatedTiledMapTile) return;

				 bool flipX = cell.getFlipHorizontally();
				 bool flipY = cell.getFlipVertically();
				 int rotations = cell.getRotation();

				TextureRegion region = tile.getTextureRegion();

				float x1 = x + tile.getOffsetX() * unitScale;
				float y1 = y + tile.getOffsetY() * unitScale;
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
				if (rotations == 2) {
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
				}
				batch.draw(region.getTexture(), vertices, 0, NUM_VERTICES);
			}
		}
	}
}
