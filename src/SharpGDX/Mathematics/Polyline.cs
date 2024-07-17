using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics.Collision;

namespace SharpGDX.Mathematics
{
	public class Polyline : IShape2D {
	private float[] localVertices;
	private float[] worldVertices;
	private float x, y;
	private float originX, originY;
	private float rotation;
	private float scaleX = 1, scaleY = 1;
	private float length;
	private float scaledLength;
	private bool _calculateScaledLength = true;
	private bool _calculateLength = true;
	private bool _dirty = true;
	private Rectangle bounds;

	public Polyline () {
		this.localVertices = new float[0];
	}

	public Polyline (float[] vertices) {
		if (vertices.Length < 4) throw new IllegalArgumentException("polylines must contain at least 2 points.");
		this.localVertices = vertices;
	}

	/** Returns vertices without scaling or rotation and without being offset by the polyline position. */
	public float[] getVertices () {
		return localVertices;
	}

	/** Returns vertices scaled, rotated, and offset by the polygon position. */
	public float[] getTransformedVertices () {
		if (!_dirty) return this.worldVertices;
		_dirty = false;

		float[] localVertices = this.localVertices;
		if (this.worldVertices == null || this.worldVertices.Length < localVertices.Length) this.worldVertices = new float[localVertices.Length];

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

	/** Returns the euclidean length of the polyline without scaling */
	public float getLength () {
		if (!_calculateLength) return length;
		_calculateLength = false;

		length = 0;
		for (int i = 0, n = localVertices.Length - 2; i < n; i += 2) {
			float x = localVertices[i + 2] - localVertices[i];
			float y = localVertices[i + 1] - localVertices[i + 3];
			length += (float)Math.Sqrt(x * x + y * y);
		}

		return length;
	}

	/** Returns the euclidean length of the polyline */
	public float getScaledLength () {
		if (!_calculateScaledLength) return scaledLength;
		_calculateScaledLength = false;

		scaledLength = 0;
		for (int i = 0, n = localVertices.Length - 2; i < n; i += 2) {
			float x = localVertices[i + 2] * scaleX - localVertices[i] * scaleX;
			float y = localVertices[i + 1] * scaleY - localVertices[i + 3] * scaleY;
			scaledLength += (float)Math.Sqrt(x * x + y * y);
		}

		return scaledLength;
	}

	public float getX () {
		return x;
	}

	public float getY () {
		return y;
	}

	public float getOriginX () {
		return originX;
	}

	public float getOriginY () {
		return originY;
	}

	public float getRotation () {
		return rotation;
	}

	public float getScaleX () {
		return scaleX;
	}

	public float getScaleY () {
		return scaleY;
	}

	public void setOrigin (float originX, float originY) {
		this.originX = originX;
		this.originY = originY;
		_dirty = true;
	}

	public void setPosition (float x, float y) {
		this.x = x;
		this.y = y;
		_dirty = true;
	}

	public void setVertices (float[] vertices) {
		if (vertices.Length < 4) throw new IllegalArgumentException("polylines must contain at least 2 points.");
		this.localVertices = vertices;
		_dirty = true;
	}

	public void setRotation (float degrees) {
		this.rotation = degrees;
		_dirty = true;
	}

	public void rotate (float degrees) {
		rotation += degrees;
		_dirty = true;
	}

	public void setScale (float scaleX, float scaleY) {
		this.scaleX = scaleX;
		this.scaleY = scaleY;
		_dirty = true;
		_calculateScaledLength = true;
	}

	public void scale (float amount) {
		this.scaleX += amount;
		this.scaleY += amount;
		_dirty = true;
		_calculateScaledLength = true;
	}

	public void calculateLength () {
		_calculateLength = true;
	}

	public void calculateScaledLength () {
		_calculateScaledLength = true;
	}

	public void dirty () {
		_dirty = true;
	}

	public void translate (float x, float y) {
		this.x += x;
		this.y += y;
		_dirty = true;
	}

	/** Returns an axis-aligned bounding box of this polyline.
	 *
	 * Note the returned Rectangle is cached in this polyline, and will be reused if this Polyline is changed.
	 *
	 * @return this polyline's bounding box {@link Rectangle} */
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

	public bool contains (Vector2 point) {
		return false;
	}

	public bool contains (float x, float y) {
		return false;
	}
}
}
