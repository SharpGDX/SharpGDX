using System.Collections;
using SharpGDX.Scenes.Scene2D.UI;
using static SharpGDX.Graphics.G2D.IBatch;
using static SharpGDX.Maps.Tiled.ITiledMapTile;
using SharpGDX.Maps.Tiled;
using SharpGDX.Maps;
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

/** @brief Represents a changing {@link TiledMapTile}. */
public class AnimatedTiledMapTile : ITiledMapTile {

	private static long lastTiledMapRenderTime = 0;

	private int id;

	private BlendMode blendMode = BlendMode.ALPHA;

	private MapProperties properties;

	private MapObjects objects;

	private StaticTiledMapTile[] frameTiles;

	private int[] animationIntervals;
	private int loopDuration;
	private static readonly long initialTimeOffset = TimeUtils.millis();

	public int getId () {
		return id;
	}

	public void setId (int id) {
		this.id = id;
	}

	public BlendMode getBlendMode () {
		return blendMode;
	}

	public void setBlendMode (BlendMode blendMode) {
		this.blendMode = blendMode;
	}

	public int getCurrentFrameIndex () {
		int currentTime = (int)(lastTiledMapRenderTime % loopDuration);

		for (int i = 0; i < animationIntervals.Length; ++i) {
			int animationInterval = animationIntervals[i];
			if (currentTime <= animationInterval) return i;
			currentTime -= animationInterval;
		}

		throw new GdxRuntimeException(
			"Could not determine current animation frame in AnimatedTiledMapTile.  This should never happen.");
	}

	public ITiledMapTile getCurrentFrame () {
		return frameTiles[getCurrentFrameIndex()];
	}

	public TextureRegion getTextureRegion () {
		return getCurrentFrame().getTextureRegion();
	}

	public void setTextureRegion (TextureRegion textureRegion) {
		throw new GdxRuntimeException("Cannot set the texture region of AnimatedTiledMapTile.");
	}

	public float getOffsetX () {
		return getCurrentFrame().getOffsetX();
	}

	public void setOffsetX (float offsetX) {
		throw new GdxRuntimeException("Cannot set offset of AnimatedTiledMapTile.");
	}

	public float getOffsetY () {
		return getCurrentFrame().getOffsetY();
	}

	public void setOffsetY (float offsetY) {
		throw new GdxRuntimeException("Cannot set offset of AnimatedTiledMapTile.");
	}

	public int[] getAnimationIntervals () {
		return animationIntervals;
	}

	public void setAnimationIntervals (int[] intervals) {
		if (intervals.Length == animationIntervals.Length) {
			this.animationIntervals = intervals;

			loopDuration = 0;
			for (int i = 0; i < intervals.Length; i++) {
				loopDuration += intervals[i];
			}

		} else {
			throw new GdxRuntimeException("Cannot set " + intervals.Length + " frame intervals. The given int[] must have a size of "
				+ animationIntervals.Length + ".");
		}
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

	/** Function is called by BatchTiledMapRenderer render(), lastTiledMapRenderTime is used to keep all of the tiles in lock-step
	 * animation and avoids having to call TimeUtils.millis() in getTextureRegion() */
	public static void updateAnimationBaseTime () {
		lastTiledMapRenderTime = TimeUtils.millis() - initialTimeOffset;
	}

	/** Creates an animated tile with the given animation interval and frame tiles.
	 * 
	 * @param interval The interval between each individual frame tile.
	 * @param frameTiles An array of {@link StaticTiledMapTile}s that make up the animation. */
	public AnimatedTiledMapTile (float interval, Array<StaticTiledMapTile> frameTiles) {
		this.frameTiles = new StaticTiledMapTile[frameTiles.size];

		this.loopDuration = frameTiles.size * (int)(interval * 1000f);
		this.animationIntervals = new int[frameTiles.size];
		for (int i = 0; i < frameTiles.size; ++i) {
			this.frameTiles[i] = frameTiles.get(i);
			this.animationIntervals[i] = (int)(interval * 1000f);
		}
	}

	/** Creates an animated tile with the given animation intervals and frame tiles.
	 *
	 * @param intervals The intervals between each individual frame tile in milliseconds.
	 * @param frameTiles An array of {@link StaticTiledMapTile}s that make up the animation. */
	public AnimatedTiledMapTile (IntArray intervals, Array<StaticTiledMapTile> frameTiles) {
		this.frameTiles = new StaticTiledMapTile[frameTiles.size];

		this.animationIntervals = intervals.toArray();
		this.loopDuration = 0;

		for (int i = 0; i < intervals.size; ++i) {
			this.frameTiles[i] = frameTiles.get(i);
			this.loopDuration += intervals.get(i);
		}
	}

	public StaticTiledMapTile[] getFrameTiles () {
		return frameTiles;
	}
}
