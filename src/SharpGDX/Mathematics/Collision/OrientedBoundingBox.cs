using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Mathematics.Collision
{
	[Serializable]
	public class OrientedBoundingBox  {
	private static readonly long serialVersionUID = 3864065514676250557L;

	private static readonly Vector3[] tempAxes = new Vector3[15];
	private static readonly Vector3[] tempVertices = new Vector3[8];
	private readonly static Vector3[] tmpVectors = new Vector3[9];
	static OrientedBoundingBox() {
		for (int i = 0; i < tmpVectors.Length; i++) {
			tmpVectors[i] = new Vector3();
		}
		for (int i = 0; i < tempVertices.Length; i++) {
			tempVertices[i] = new Vector3();
		}
	}

	/** Bounds used as size. */
	private readonly BoundingBox bounds = new BoundingBox();
	/** Transform matrix. */
	public readonly Matrix4 transform = new Matrix4();
	private readonly Matrix4 inverseTransform = new Matrix4();

	private readonly Vector3[] axes = new Vector3[3];
	private readonly Vector3[] vertices = new Vector3[8];

	/** Constructs a new oriented bounding box with the minimum and maximum vector set to zeros. */
	public OrientedBoundingBox () {
		bounds.clr();
		init();
	}

	/** Constructs a new oriented bounding box from the given bounding box.
	 *
	 * @param bounds The bounding box to copy */
	public OrientedBoundingBox (BoundingBox bounds) {
		this.bounds.set(bounds.min, bounds.max);
		init();
	}

	/** Constructs a new oriented bounding box from the given bounding box and transform.
	 *
	 * @param bounds The bounding box to copy
	 * @param transform The transformation matrix to copy */
	public OrientedBoundingBox (BoundingBox bounds, Matrix4 transform) {
		this.bounds.set(bounds.min, bounds.max);
		this.transform.set(transform);
		init();
	}

	private void init () {
		for (int i = 0; i < axes.Length; i++) {
			axes[i] = new Vector3();
		}
		for (int i = 0; i < vertices.Length; i++) {
			vertices[i] = new Vector3();
		}
		update();
	}

	public Vector3[] getVertices () {
		return vertices;
	}

	/** Get the current bounds. Call {@link #update()} if you manually change this bounding box. */
	public BoundingBox getBounds () {
		return bounds;
	}

	/** Sets the base bounds of the oriented bounding box as the bounds given, the transform is applied to the vertices.
	 *
	 * @param bounds The bounding box to copy */
	public void setBounds (BoundingBox bounds) {
		this.bounds.set(bounds);
		bounds.getCorner000(vertices[0b000]).mul(transform);
		bounds.getCorner001(vertices[0b001]).mul(transform);
		bounds.getCorner010(vertices[0b010]).mul(transform);
		bounds.getCorner011(vertices[0b011]).mul(transform);
		bounds.getCorner100(vertices[0b100]).mul(transform);
		bounds.getCorner101(vertices[0b101]).mul(transform);
		bounds.getCorner110(vertices[0b110]).mul(transform);
		bounds.getCorner111(vertices[0b111]).mul(transform);
	}

	/** Get the current transformation matrix. Call {@link #update()} if you manually change this matrix. */
	public Matrix4 getTransform () {
		return transform;
	}

	public void setTransform (Matrix4 transform) {
		this.transform.set(transform);
		update();
	}

	public OrientedBoundingBox set (BoundingBox bounds, Matrix4 transform) {
		setBounds(bounds);
		setTransform(transform);
		return this;
	}

	public Vector3 getCorner000 ( Vector3 @out) {
		return @out.set(vertices[0b000]);
	}

	public Vector3 getCorner001 ( Vector3 @out) {
		return @out.set(vertices[0b001]);
	}

	public Vector3 getCorner010 ( Vector3 @out) {
		return @out.set(vertices[0b010]);
	}

	public Vector3 getCorner011 ( Vector3 @out) {
		return @out.set(vertices[0b011]);
	}

	public Vector3 getCorner100 ( Vector3 @out) {
		return @out.set(vertices[0b100]);
	}

	public Vector3 getCorner101 ( Vector3 @out) {
		return @out.set(vertices[0b101]);
	}

	public Vector3 getCorner110 ( Vector3 @out) {
		return @out.set(vertices[0b110]);
	}

	public Vector3 getCorner111 ( Vector3 @out) {
		return @out.set(vertices[0b111]);
	}

	/** Returns whether the given vector is contained in this oriented bounding box.
	 * @param v The vector
	 * @return Whether the vector is contained or not. */
	public bool contains (Vector3 v) {
		return contains(v, inverseTransform);
	}

	private bool contains (Vector3 v, Matrix4 invTransform) {
		Vector3 localV = tmpVectors[0].set(v).mul(invTransform);
		return bounds.contains(localV);
	}

	/** Returns whether the given bounding box is contained in this oriented bounding box.
	 * @param b The bounding box
	 * @return Whether the given bounding box is contained */
	public bool contains (BoundingBox b) {
		Vector3 tmpVector = tmpVectors[0];
		return contains(b.getCorner000(tmpVector), inverseTransform) && contains(b.getCorner001(tmpVector), inverseTransform)
			&& contains(b.getCorner010(tmpVector), inverseTransform) && contains(b.getCorner011(tmpVector), inverseTransform)
			&& contains(b.getCorner100(tmpVector), inverseTransform) && contains(b.getCorner101(tmpVector), inverseTransform)
			&& contains(b.getCorner110(tmpVector), inverseTransform) && contains(b.getCorner111(tmpVector), inverseTransform);
	}

	/** Returns whether the given oriented bounding box is contained in this oriented bounding box.
	 * @param obb The oriented bounding box
	 * @return Whether the given oriented bounding box is contained */
	public bool contains (OrientedBoundingBox obb) {
		return contains(obb.getCorner000(tmpVectors[0]), inverseTransform)
			&& contains(obb.getCorner001(tmpVectors[0]), inverseTransform)
			&& contains(obb.getCorner010(tmpVectors[0]), inverseTransform)
			&& contains(obb.getCorner011(tmpVectors[0]), inverseTransform)
			&& contains(obb.getCorner100(tmpVectors[0]), inverseTransform)
			&& contains(obb.getCorner101(tmpVectors[0]), inverseTransform)
			&& contains(obb.getCorner110(tmpVectors[0]), inverseTransform)
			&& contains(obb.getCorner111(tmpVectors[0]), inverseTransform);
	}

	/** Returns whether the given bounding box is intersecting this oriented bounding box (at least one point in).
	 * @param b The bounding box
	 * @return Whether the given bounding box is intersected */
	public bool intersects (BoundingBox b) {
		Vector3[] aAxes = axes;

		tempAxes[0] = aAxes[0];
		tempAxes[1] = aAxes[1];
		tempAxes[2] = aAxes[2];
		tempAxes[3] = Vector3.X;
		tempAxes[4] = Vector3.Y;
		tempAxes[5] = Vector3.Z;
		tempAxes[6] = tmpVectors[0].set(aAxes[0]).crs(Vector3.X);
		tempAxes[7] = tmpVectors[1].set(aAxes[0]).crs(Vector3.Y);
		tempAxes[8] = tmpVectors[2].set(aAxes[0]).crs(Vector3.Z);
		tempAxes[9] = tmpVectors[3].set(aAxes[1]).crs(Vector3.X);
		tempAxes[10] = tmpVectors[4].set(aAxes[1]).crs(Vector3.Y);
		tempAxes[11] = tmpVectors[5].set(aAxes[1]).crs(Vector3.Z);
		tempAxes[12] = tmpVectors[6].set(aAxes[2]).crs(Vector3.X);
		tempAxes[13] = tmpVectors[7].set(aAxes[2]).crs(Vector3.Y);
		tempAxes[14] = tmpVectors[8].set(aAxes[2]).crs(Vector3.Z);

		Vector3[] aVertices = getVertices();
		Vector3[] bVertices = getVertices(b);

		return Intersector.hasOverlap(tempAxes, aVertices, bVertices);
	}

	/** Returns whether the given oriented bounding box is intersecting this oriented bounding box (at least one point in).
	 * @param obb The oriented bounding box
	 * @return Whether the given bounding box is intersected */
	public bool intersects (OrientedBoundingBox obb) {
		Vector3[] aAxes = axes;
		Vector3[] bAxes = obb.axes;

		tempAxes[0] = aAxes[0];
		tempAxes[1] = aAxes[1];
		tempAxes[2] = aAxes[2];
		tempAxes[3] = bAxes[0];
		tempAxes[4] = bAxes[1];
		tempAxes[5] = bAxes[2];
		tempAxes[6] = tmpVectors[0].set(aAxes[0]).crs(bAxes[0]);
		tempAxes[7] = tmpVectors[1].set(aAxes[0]).crs(bAxes[1]);
		tempAxes[8] = tmpVectors[2].set(aAxes[0]).crs(bAxes[2]);
		tempAxes[9] = tmpVectors[3].set(aAxes[1]).crs(bAxes[0]);
		tempAxes[10] = tmpVectors[4].set(aAxes[1]).crs(bAxes[1]);
		tempAxes[11] = tmpVectors[5].set(aAxes[1]).crs(bAxes[2]);
		tempAxes[12] = tmpVectors[6].set(aAxes[2]).crs(bAxes[0]);
		tempAxes[13] = tmpVectors[7].set(aAxes[2]).crs(bAxes[1]);
		tempAxes[14] = tmpVectors[8].set(aAxes[2]).crs(bAxes[2]);

		return Intersector.hasOverlap(tempAxes, vertices, obb.vertices);
	}

	private Vector3[] getVertices (BoundingBox b) {
		b.getCorner000(tempVertices[0b000]);
		b.getCorner001(tempVertices[0b001]);
		b.getCorner010(tempVertices[0b010]);
		b.getCorner011(tempVertices[0b011]);
		b.getCorner100(tempVertices[0b100]);
		b.getCorner101(tempVertices[0b101]);
		b.getCorner110(tempVertices[0b110]);
		b.getCorner111(tempVertices[0b111]);
		return tempVertices;
	}

	public void mul (Matrix4 transform) {
		this.transform.mul(transform);
		update();
	}

	private void update () {
		// Update vertices
		bounds.getCorner000(vertices[0b000]).mul(transform);
		bounds.getCorner001(vertices[0b001]).mul(transform);
		bounds.getCorner010(vertices[0b010]).mul(transform);
		bounds.getCorner011(vertices[0b011]).mul(transform);
		bounds.getCorner100(vertices[0b100]).mul(transform);
		bounds.getCorner101(vertices[0b101]).mul(transform);
		bounds.getCorner110(vertices[0b110]).mul(transform);
		bounds.getCorner111(vertices[0b111]).mul(transform);

		axes[0].set(transform.val[Matrix4.M00], transform.val[Matrix4.M10], transform.val[Matrix4.M20]).nor();
		axes[1].set(transform.val[Matrix4.M01], transform.val[Matrix4.M11], transform.val[Matrix4.M21]).nor();
		axes[2].set(transform.val[Matrix4.M02], transform.val[Matrix4.M12], transform.val[Matrix4.M22]).nor();

		inverseTransform.set(transform).inv();
	}
}
}
