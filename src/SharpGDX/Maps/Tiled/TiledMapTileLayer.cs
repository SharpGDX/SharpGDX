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

/** @brief Layer for a TiledMap */
public class TiledMapTileLayer : MapLayer {

	private int width;
	private int height;

	private int tileWidth;
	private int tileHeight;

	private Cell[][] cells;

	/** @return layer's width in tiles */
	public int getWidth () {
		return width;
	}

	/** @return layer's height in tiles */
	public int getHeight () {
		return height;
	}

	/** @return tiles' width in pixels */
	public int getTileWidth () {
		return tileWidth;
	}

	/** @return tiles' height in pixels */
	public int getTileHeight () {
		return tileHeight;
	}

	/** Creates TiledMap layer
	 * 
	 * @param width layer width in tiles
	 * @param height layer height in tiles
	 * @param tileWidth tile width in pixels
	 * @param tileHeight tile height in pixels */
	public TiledMapTileLayer (int width, int height, int tileWidth, int tileHeight)
	{
		throw new NotImplementedException();
		//this.width = width;
		//this.height = height;
		//this.tileWidth = tileWidth;
		//this.tileHeight = tileHeight;
		//this.cells = new Cell[width][height];
	}

	/** @param x X coordinate
	 * @param y Y coordinate
	 * @return {@link Cell} at (x, y) */
	public Cell getCell (int x, int y) {
		if (x < 0 || x >= width) return null;
		if (y < 0 || y >= height) return null;
		return cells[x][y];
	}

	/** Sets the {@link Cell} at the given coordinates.
	 * 
	 * @param x X coordinate
	 * @param y Y coordinate
	 * @param cell the {@link Cell} to set at the given coordinates. */
	public void setCell (int x, int y, Cell cell) {
		if (x < 0 || x >= width) return;
		if (y < 0 || y >= height) return;
		cells[x][y] = cell;
	}

	/** @brief represents a cell in a TiledLayer: TiledMapTile, flip and rotation properties. */
	public  class Cell {

		private ITiledMapTile tile;

		private bool flipHorizontally;

		private bool flipVertically;

		private int rotation;

		/** @return The tile currently assigned to this cell. */
		public ITiledMapTile getTile () {
			return tile;
		}

		/** Sets the tile to be used for this cell.
		 * 
		 * @param tile the {@link TiledMapTile} to use for this cell.
		 * @return this, for method chaining */
		public Cell setTile (ITiledMapTile tile) {
			this.tile = tile;
			return this;
		}

		/** @return Whether the tile should be flipped horizontally. */
		public bool getFlipHorizontally () {
			return flipHorizontally;
		}

		/** Sets whether to flip the tile horizontally.
		 * 
		 * @param flipHorizontally whether or not to flip the tile horizontally.
		 * @return this, for method chaining */
		public Cell setFlipHorizontally (bool flipHorizontally) {
			this.flipHorizontally = flipHorizontally;
			return this;
		}

		/** @return Whether the tile should be flipped vertically. */
		public bool getFlipVertically () {
			return flipVertically;
		}

		/** Sets whether to flip the tile vertically.
		 * 
		 * @param flipVertically whether or not this tile should be flipped vertically.
		 * @return this, for method chaining */
		public Cell setFlipVertically (bool flipVertically) {
			this.flipVertically = flipVertically;
			return this;
		}

		/** @return The rotation of this cell, in 90 degree increments. */
		public int getRotation () {
			return rotation;
		}

		/** Sets the rotation of this cell, in 90 degree increments.
		 * 
		 * @param rotation the rotation in 90 degree increments (see ints below).
		 * @return this, for method chaining */
		public Cell setRotation (int rotation) {
			this.rotation = rotation;
			return this;
		}

		public const int ROTATE_0 = 0;
		public const int ROTATE_90 = 1;
		public const int ROTATE_180 = 2;
		public const int ROTATE_270 = 3;
	}
}
