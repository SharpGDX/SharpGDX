using System;
using SharpGDX.Utils;
using SharpGDX.Mathematics.Collision;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Mathematics
{
	/** Encapsulates a 2D polygon defined by it's vertices relative to an origin point (default of 0, 0). */
public class Polygon : IShape2D {
	private float[] localVertices;
	private float[] worldVertices;
	private float x, y;
	private float originX, originY;
	private float rotation;
	private float scaleX = 1, scaleY = 1;
	private bool _dirty = true;
	private Rectangle bounds;

	/** Constructs a new polygon with no vertices. */
	public Polygon () {
		this.localVertices = new float[0];
	}

	/** Constructs a new polygon from a float array of parts of vertex points.
	 * 
	 * @param vertices an array where every even element represents the horizontal part of a point, and the following element
	 *           representing the vertical part
	 * 
	 * @throws IllegalArgumentException if less than 6 elements, representing 3 points, are provided */
	public Polygon (float[] vertices) {
		if (vertices.Length < 6) throw new IllegalArgumentException("polygons must contain at least 3 points.");
		this.localVertices = vertices;
	}

	/** Returns the polygon's local vertices without scaling or rotation and without being offset by the polygon position. */
	public float[] getVertices () {
		return localVertices;
	}

	/** Calculates and returns the vertices of the polygon after scaling, rotation, and positional translations have been applied,
	 * as they are position within the world.
	 * 
	 * @return vertices scaled, rotated, and offset by the polygon position. */
	public float[] getTransformedVertices () {
		if (!_dirty) return this.worldVertices;
		_dirty = false;

		 float[] localVertices = this.localVertices;
		if (this.worldVertices == null || this.worldVertices.Length != localVertices.Length) this.worldVertices = new float[localVertices.Length];

		 float[] worldVertices = this.worldVertices;
		 float positionX = x;
		 float positionY = y;
		 float originX = this.originX;
		 float originY = this.originY;
		 float scaleX = this.scaleX;
		 float scaleY = this.scaleY;
		 bool scale = scaleX != 1 || scaleY != 1;
		 float rotation = this.rotation;
		 float cos = MathUtils.cosDeg(rotation);
		 float sin = MathUtils.sinDeg(rotation);

		for (int i = 0, n = localVertices.Length; i < n; i += 2) {
			float x = localVertices[i] - originX;
			float y = localVertices[i + 1] - originY;

			// scale if needed
			if (scale) {
				x *= scaleX;
				y *= scaleY;
			}

			// rotate if needed
			if (rotation != 0) {
				float oldX = x;
				x = cos * x - sin * y;
				y = sin * oldX + cos * y;
			}

			worldVertices[i] = positionX + x + originX;
			worldVertices[i + 1] = positionY + y + originY;
		}
		return worldVertices;
	}

	/** Sets the origin point to which all of the polygon's local vertices are relative to. */
	public void setOrigin (float originX, float originY) {
		this.originX = originX;
		this.originY = originY;
		_dirty = true;
	}

	/** Sets the polygon's position within the world. */
	public void setPosition (float x, float y) {
		this.x = x;
		this.y = y;
		_dirty = true;
	}

	/** Sets the polygon's local vertices relative to the origin point, without any scaling, rotating or translations being
	 * applied.
	 * 
	 * @param vertices float array where every even element represents the x-coordinate of a vertex, and the proceeding element
	 *           representing the y-coordinate.
	 * @throws IllegalArgumentException if less than 6 elements, representing 3 points, are provided */
	public void setVertices (float[] vertices) {
		if (vertices.Length < 6) throw new IllegalArgumentException("polygons must contain at least 3 points.");
		localVertices = vertices;
		_dirty = true;
	}

	/** Set vertex position
	 * @param vertexNum min=0, max=vertices.length/2-1
	 * @throws IllegalArgumentException if vertex doesnt exist */
	public void setVertex (int vertexNum, float x, float y) {
		if (vertexNum < 0 || vertexNum > localVertices.Length / 2 - 1)
			throw new IllegalArgumentException("the vertex " + vertexNum + " doesn't exist");
		localVertices[2 * vertexNum] = x;
		localVertices[2 * vertexNum + 1] = y;
		_dirty = true;
	}

	/** Translates the polygon's position by the specified horizontal and vertical amounts. */
	public void translate (float x, float y) {
		this.x += x;
		this.y += y;
	_dirty = true;
	}

	/** Sets the polygon to be rotated by the supplied degrees. */
	public void setRotation (float degrees) {
		this.rotation = degrees;
		_dirty = true;
	}

	/** Applies additional rotation to the polygon by the supplied degrees. */
	public void rotate (float degrees) {
		rotation += degrees;
		_dirty = true;
	}

	/** Sets the amount of scaling to be applied to the polygon. */
	public void setScale (float scaleX, float scaleY) {
		this.scaleX = scaleX;
		this.scaleY = scaleY;
		_dirty = true;
	}

	/** Applies additional scaling to the polygon by the supplied amount. */
	public void scale (float amount) {
		this.scaleX += amount;
		this.scaleY += amount;
		_dirty = true;
	}

	/** Sets the polygon's world vertices to be recalculated when calling {@link #getTransformedVertices()
	 * getTransformedVertices}. */
	public void dirty () {
		_dirty = true;
	}

	/** Returns the area contained within the polygon. */
	public float area () {
		float[] vertices = getTransformedVertices();
		return GeometryUtils.polygonArea(vertices, 0, vertices.Length);
	}

	public int getVertexCount () {
		return this.localVertices.Length / 2;
	}

	/** @return Position(transformed) of vertex */
	public Vector2 getVertex (int vertexNum, Vector2 pos) {
		if (vertexNum < 0 || vertexNum > getVertexCount())
			throw new IllegalArgumentException("the vertex " + vertexNum + " doesn't exist");
		float[] vertices = this.getTransformedVertices();
		return pos.set(vertices[2 * vertexNum], vertices[2 * vertexNum + 1]);
	}

	public Vector2 getCentroid (Vector2 centroid) {
		float[] vertices = getTransformedVertices();
		return GeometryUtils.polygonCentroid(vertices, 0, vertices.Length, centroid);
	}

	/** Returns an axis-aligned bounding box of this polygon.
	 * 
	 * Note the returned Rectangle is cached in this polygon, and will be reused if this Polygon is changed.
	 * 
	 * @return this polygon's bounding box {@link Rectangle} */
	public Rectangle getBoundingRectangle () {
		float[] vertices = getTransformedVertices();

		float minX = vertices[0];
		float minY = vertices[1];
		float maxX = vertices[0];
		float maxY = vertices[1];

		 int numFloats = vertices.Length;
		for (int i = 2; i < numFloats; i += 2) {
			minX = minX > vertices[i] ? vertices[i] : minX;
			minY = minY > vertices[i + 1] ? vertices[i + 1] : minY;
			maxX = maxX < vertices[i] ? vertices[i] : maxX;
			maxY = maxY < vertices[i + 1] ? vertices[i + 1] : maxY;
		}

		if (bounds == null) bounds = new Rectangle();
		bounds.x = minX;
		bounds.y = minY;
		bounds.width = maxX - minX;
		bounds.height = maxY - minY;

		return bounds;
	}

	/** Returns whether an x, y pair is contained within the polygon. */
	public bool contains (float x, float y) {
		 float[] vertices = getTransformedVertices();
		 int numFloats = vertices.Length;
		int intersects = 0;

		for (int i = 0; i < numFloats; i += 2) {
			float x1 = vertices[i];
			float y1 = vertices[i + 1];
			float x2 = vertices[(i + 2) % numFloats];
			float y2 = vertices[(i + 3) % numFloats];
			if (((y1 <= y && y < y2) || (y2 <= y && y < y1)) && x < ((x2 - x1) / (y2 - y1) * (y - y1) + x1)) intersects++;
		}
		return (intersects & 1) == 1;
	}

	public bool contains (Vector2 point) {
		return contains(point.x, point.y);
	}

	/** Returns the x-coordinate of the polygon's position within the world. */
	public float getX () {
		return x;
	}

	/** Returns the y-coordinate of the polygon's position within the world. */
	public float getY () {
		return y;
	}

	/** Returns the x-coordinate of the polygon's origin point. */
	public float getOriginX () {
		return originX;
	}

	/** Returns the y-coordinate of the polygon's origin point. */
	public float getOriginY () {
		return originY;
	}

	/** Returns the total rotation applied to the polygon. */
	public float getRotation () {
		return rotation;
	}

	/** Returns the total horizontal scaling applied to the polygon. */
	public float getScaleX () {
		return scaleX;
	}

	/** Returns the total vertical scaling applied to the polygon. */
	public float getScaleY () {
		return scaleY;
	}
}
}
