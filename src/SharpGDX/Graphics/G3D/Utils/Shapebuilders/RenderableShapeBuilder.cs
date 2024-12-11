using System;
using static SharpGDX.Graphics.VertexAttributes;
using SharpGDX.Graphics.GLUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Utils.ShapeBuilders;

/** RenderableShapeBuilder builds various properties of a renderable.
 * @author realitix */
public class RenderableShapeBuilder : BaseShapeBuilder {

	private  class RenderablePool : FlushablePool<Renderable> {
		public RenderablePool () 
        : base()
        {
			
		}

		protected internal override Renderable newObject () {
			return new Renderable();
		}

		public override Renderable obtain () {
			Renderable renderable = base.obtain();
			renderable.environment = null;
			renderable.material = null;
			renderable.meshPart.set("", null, 0, 0, 0);
			renderable.shader = null;
			renderable.userData = null;
			return renderable;
		}
	}

	private static short[] indices;
	private static float[] vertices;
	private readonly static RenderablePool renderablesPool = new RenderablePool();
	private readonly static Array<Renderable> renderables = new Array<Renderable>();
	private static readonly int FLOAT_BYTES = 4;

	/** Builds normal, tangent and binormal of a RenderableProvider with default colors (normal blue, tangent red, binormal green).
	 * @param builder
	 * @param renderableProvider
	 * @param vectorSize Size of the normal vector */
	public static void buildNormals (MeshPartBuilder builder, RenderableProvider renderableProvider, float vectorSize) {
		buildNormals(builder, renderableProvider, vectorSize, tmpColor0.Set(0, 0, 1, 1), tmpColor1.Set(1, 0, 0, 1),
			tmpColor2.Set(0, 1, 0, 1));
	}

	/** Builds normal, tangent and binormal of a RenderableProvider.
	 * @param builder
	 * @param renderableProvider
	 * @param vectorSize Size of the normal vector
	 * @param normalColor Normal vector's color
	 * @param tangentColor Tangent vector's color
	 * @param binormalColor Binormal vector's color */
	public static void buildNormals (MeshPartBuilder builder, RenderableProvider renderableProvider, float vectorSize,
		Color normalColor, Color tangentColor, Color binormalColor) {

		renderableProvider.getRenderables(renderables, renderablesPool);

		foreach (Renderable renderable in renderables) {
			buildNormals(builder, renderable, vectorSize, normalColor, tangentColor, binormalColor);
		}

		renderablesPool.flush();
		renderables.clear();
	}

	/** Builds normal, tangent and binormal of a Renderable.
	 * @param builder
	 * @param renderable
	 * @param vectorSize Size of the normal vector
	 * @param normalColor Normal vector's color
	 * @param tangentColor Tangent vector's color
	 * @param binormalColor Binormal vector's color */
	public static void buildNormals (MeshPartBuilder builder, Renderable renderable, float vectorSize, Color normalColor,
		Color tangentColor, Color binormalColor) {
		Mesh mesh = renderable.meshPart.mesh;

		// Position
		int positionOffset = -1;
		if (mesh.getVertexAttribute(Usage.Position) != null)
			positionOffset = mesh.getVertexAttribute(Usage.Position).offset / FLOAT_BYTES;

		// Normal
		int normalOffset = -1;
		if (mesh.getVertexAttribute(Usage.Normal) != null)
			normalOffset = mesh.getVertexAttribute(Usage.Normal).offset / FLOAT_BYTES;

		// Tangent
		int tangentOffset = -1;
		if (mesh.getVertexAttribute(Usage.Tangent) != null)
			tangentOffset = mesh.getVertexAttribute(Usage.Tangent).offset / FLOAT_BYTES;

		// Binormal
		int binormalOffset = -1;
		if (mesh.getVertexAttribute(Usage.BiNormal) != null)
			binormalOffset = mesh.getVertexAttribute(Usage.BiNormal).offset / FLOAT_BYTES;

		int attributesSize = mesh.getVertexSize() / FLOAT_BYTES;
		int verticesOffset = 0;
		int verticesQuantity = 0;

		if (mesh.getNumIndices() > 0) {
			// Get min vertice to max vertice in indices array
			ensureIndicesCapacity(mesh.getNumIndices());
			mesh.getIndices(renderable.meshPart.offset, renderable.meshPart.size, indices, 0);

			short minVertice = minVerticeInIndices();
			short maxVertice = maxVerticeInIndices();

			verticesOffset = minVertice;
			verticesQuantity = maxVertice - minVertice;
		} else {
			verticesOffset = renderable.meshPart.offset;
			verticesQuantity = renderable.meshPart.size;
		}

		ensureVerticesCapacity(verticesQuantity * attributesSize);
		mesh.getVertices(verticesOffset * attributesSize, verticesQuantity * attributesSize, vertices, 0);

		for (int i = verticesOffset; i < verticesQuantity; i++) {
			int id = i * attributesSize;

			// Vertex position
			tmpV0.Set(vertices[id + positionOffset], vertices[id + positionOffset + 1], vertices[id + positionOffset + 2]);

			// Vertex normal, tangent, binormal
			if (normalOffset != -1) {
				tmpV1.Set(vertices[id + normalOffset], vertices[id + normalOffset + 1], vertices[id + normalOffset + 2]);
				tmpV2.Set(tmpV0).add(tmpV1.scl(vectorSize));
			}

			if (tangentOffset != -1) {
				tmpV3.Set(vertices[id + tangentOffset], vertices[id + tangentOffset + 1], vertices[id + tangentOffset + 2]);
				tmpV4.Set(tmpV0).add(tmpV3.scl(vectorSize));
			}

			if (binormalOffset != -1) {
				tmpV5.Set(vertices[id + binormalOffset], vertices[id + binormalOffset + 1], vertices[id + binormalOffset + 2]);
				tmpV6.Set(tmpV0).add(tmpV5.scl(vectorSize));
			}

			// World transform
			tmpV0.mul(renderable.worldTransform);
			tmpV2.mul(renderable.worldTransform);
			tmpV4.mul(renderable.worldTransform);
			tmpV6.mul(renderable.worldTransform);

			// Draws normal, tangent, binormal
			if (normalOffset != -1) {
				builder.setColor(normalColor);
				builder.line(tmpV0, tmpV2);
			}

			if (tangentOffset != -1) {
				builder.setColor(tangentColor);
				builder.line(tmpV0, tmpV4);
			}

			if (binormalOffset != -1) {
				builder.setColor(binormalColor);
				builder.line(tmpV0, tmpV6);
			}
		}
	}

	private static void ensureVerticesCapacity (int capacity) {
		if (vertices == null || vertices.Length < capacity) vertices = new float[capacity];
	}

	private static void ensureIndicesCapacity (int capacity) {
		if (indices == null || indices.Length < capacity) indices = new short[capacity];
	}

	private static short minVerticeInIndices () {
		short min = (short)32767;
		for (int i = 0; i < indices.Length; i++)
			if (indices[i] < min) min = indices[i];
		return min;
	}

	private static short maxVerticeInIndices () {
		short max = (short)-32768;
		for (int i = 0; i < indices.Length; i++)
			if (indices[i] > max) max = indices[i];
		return max;
	}
}
