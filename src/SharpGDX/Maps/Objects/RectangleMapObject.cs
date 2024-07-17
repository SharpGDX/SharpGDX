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

namespace SharpGDX.Maps.Objects;

/** @brief Represents a rectangle shaped map object */
public class RectangleMapObject : MapObject {

	private Rectangle rectangle;

	/** @return rectangle shape */
	public Rectangle getRectangle () {
		return rectangle;
	}

	/** Creates a rectangle object which lower left corner is at (0, 0) with width=1 and height=1 */
	public RectangleMapObject () 
	: this(0.0f, 0.0f, 1.0f, 1.0f)
	{
		
	}

	/** Creates a {@link Rectangle} object with the given X and Y coordinates along with a given width and height.
	 * 
	 * @param x X coordinate
	 * @param y Y coordinate
	 * @param width Width of the {@link Rectangle} to be created.
	 * @param height Height of the {@link Rectangle} to be created. */
	public RectangleMapObject (float x, float y, float width, float height) {
		rectangle = new Rectangle(x, y, width, height);
	}

}
