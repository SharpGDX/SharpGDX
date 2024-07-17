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

/** @brief Represents {@link Circle} shaped map objects */
public class CircleMapObject : MapObject {

	private Circle circle;

	/** @return circle shape */
	public Circle getCircle () {
		return circle;
	}

	/** Creates a circle map object at (0,0) with r=1.0 */
	public CircleMapObject () 
	: this(0.0f, 0.0f, 1.0f)
	{
		
	}

	/** Creates a circle map object
	 * 
	 * @param x X coordinate
	 * @param y Y coordinate
	 * @param radius Radius of the circle object. */
	public CircleMapObject (float x, float y, float radius) {
		circle = new Circle(x, y, radius);
	}
}
