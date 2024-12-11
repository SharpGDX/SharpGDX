using System;
using SharpGDX.Graphics.GLUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Utils;

/** A relatively lightweight class which can be used to render basic shapes which don't need a node structure and alike. Can be
 * used for batching both static and dynamic shapes which share the same {@link Material} and transformation {@link Matrix4}
 * within the world. Use {@link ModelBatch} to render the `ShapeCache`. Must be disposed when no longer needed to release native
 * resources.
 * <p>
 * How to use it :
 * </p>
 * 
 * <pre>
 * // Create cache
 * ShapeCache cache = new ShapeCache();
 * // Build the cache, for dynamic shapes, this would be in the render method.
 * MeshPartBuilder builder = cache.begin();
 * FrustumShapeBuilder.build(builder, camera);
 * BoxShapeBuilder.build(builder, box);
 * cache.end()
 * // Render
 * modelBatch.render(cache);
 * // After using it
 * cache.dispose();
 * </pre>
 * 
 * @author realitix */
public class ShapeCache : IDisposable, RenderableProvider {

	/** Builder used to update the mesh */
	private readonly MeshBuilder builder;

	/** Mesh being rendered */
	private readonly Mesh mesh;

	private bool building;
	private readonly String id = "id";
	private readonly Renderable renderable = new Renderable();

	/** Create a ShapeCache with default values */
	public ShapeCache () 
    : this(5000, 5000, new VertexAttributes(new VertexAttribute(VertexAttributes.Usage.Position, 3, "a_position"),
        new VertexAttribute(VertexAttributes.Usage.ColorPacked, 4, "a_color")), IGL20.GL_LINES)
    {
		
	}

	/** Create a ShapeCache with parameters
	 * @param maxVertices max vertices in mesh
	 * @param maxIndices max indices in mesh
	 * @param attributes vertex attributes
	 * @param primitiveType */
	public ShapeCache (int maxVertices, int maxIndices, VertexAttributes attributes, int primitiveType) {
		// Init mesh
		mesh = new Mesh(false, maxVertices, maxIndices, attributes);

		// Init builder
		builder = new MeshBuilder();

		// Init renderable
		renderable.meshPart.mesh = mesh;
		renderable.meshPart.primitiveType = primitiveType;
		renderable.material = new Material();
	}

	/** Initialize ShapeCache for mesh generation with GL_LINES primitive type */
	public MeshPartBuilder begin () {
		return begin(IGL20.GL_LINES);
	}

	/** Initialize ShapeCache for mesh generation
	 * @param primitiveType OpenGL primitive type */
	public MeshPartBuilder begin (int primitiveType) {
		if (building) throw new GdxRuntimeException("Call end() after calling begin()");
		building = true;

		builder.begin(mesh.getVertexAttributes());
		builder.part(id, primitiveType, renderable.meshPart);
		return builder;
	}

	/** Generate mesh and renderable */
	public void end () {
		if (!building) throw new GdxRuntimeException("Call begin() prior to calling end()");
		building = false;

		builder.end(mesh);
	}

	public void getRenderables (Array<Renderable> renderables, Pool<Renderable> pool) {
		renderables.Add(renderable);
	}

	/** Allows to customize the material.
	 * @return material */
	public Material getMaterial () {
		return renderable.material;
	}

	/** Allows to customize the world transform matrix.
	 * @return world transform */
	public Matrix4 getWorldTransform () {
		return renderable.worldTransform;
	}

	public void Dispose () {
		mesh.Dispose();
	}
}
