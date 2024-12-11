using System;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;
using SharpGDX.Graphics.G3D.Models;

namespace SharpGDX.Graphics.G3D.Utils;

/** Helper class to create {@link Model}s from code. To start building use the {@link #begin()} method, when finished building use
 * the {@link #end()} method. The end method returns the model just build. Building cannot be nested, only one model (per
 * ModelBuilder) can be build at the time. The same ModelBuilder can be used to build multiple models sequential. Use the
 * {@link #node()} method to start a new node. Use one of the #part(...) methods to add a part within a node. The
 * {@link #part(String, int, VertexAttributes, Material)} method will return a {@link MeshPartBuilder} which can be used to build
 * the node part.
 * @author Xoppa */
public class ModelBuilder {
	/** The model currently being build */
	private Model model;
	/** The node currently being build */
	private Node _node;
	/** The mesh builders created between begin and end */
	private Array<MeshBuilder> builders = new Array<MeshBuilder>();

	private Matrix4 tmpTransform = new Matrix4();

	private MeshBuilder getBuilder (VertexAttributes attributes) {
		foreach (MeshBuilder mb in builders)
			if (mb.getAttributes().Equals(attributes) && mb.lastIndex() < MeshBuilder.MAX_VERTICES / 2) return mb;
		MeshBuilder result = new MeshBuilder();
		result.begin(attributes);
		builders.Add(result);
		return result;
	}

	/** Begin building a new model */
	public void begin () {
		if (model != null) throw new GdxRuntimeException("Call end() first");
		_node = null;
		model = new Model();
		builders.clear();
	}

	/** End building the model.
	 * @return The newly created model. Call the {@link Model#dispose()} method when no longer used. */
	public Model end () {
		if (model == null) throw new GdxRuntimeException("Call begin() first");
		 Model result = model;
		endnode();
		model = null;

		foreach ( MeshBuilder mb in builders)
			mb.end();
		builders.clear();

		rebuildReferences(result);
		return result;
	}

	private void endnode () {
		if (_node != null) {
			_node = null;
		}
	}

	/** Adds the {@link Node} to the model and sets it active for building. Use any of the part(...) method to add a NodePart. */
	protected Node node (Node node) {
		if (model == null) throw new GdxRuntimeException("Call begin() first");

		endnode();

		model.nodes.Add(node);
		this._node = node;

		return node;
	}

	/** Add a node to the model. Use any of the part(...) method to add a NodePart.
	 * @return The node being created. */
	public Node node () {
		 Node node = new Node();
		this.node(node);
		node.id = "node" + model.nodes.size;
		return node;
	}

	/** Adds the nodes of the specified model to a new node of the model being build. After this method the given model can no
	 * longer be used. Do not call the {@link Model#dispose()} method on that model.
	 * @return The newly created node containing the nodes of the given model. */
	public Node node ( String id,  Model model) {
		 Node node = new Node();
		node.id = id;
		node.addChildren(model.nodes);
		this.node(node);
		foreach ( IDisposable disposable in model.getManagedDisposables())
			manage(disposable);
		return node;
	}

	/** Add the {@link Disposable} object to the model, causing it to be disposed when the model is disposed. */
	public void manage (IDisposable disposable) {
		if (model == null) throw new GdxRuntimeException("Call begin() first");
		model.manageDisposable(disposable);
	}

	/** Adds the specified MeshPart to the current Node. The Mesh will be managed by the model and disposed when the model is
	 * disposed. The resources the Material might contain are not managed, use {@link #manage(Disposable)} to add those to the
	 * model. */
	public void part ( MeshPart meshpart,  Material material) {
		if (_node == null) node();
        _node.parts.Add(new NodePart(meshpart, material));
	}

	/** Adds the specified mesh part to the current node. The Mesh will be managed by the model and disposed when the model is
	 * disposed. The resources the Material might contain are not managed, use {@link #manage(Disposable)} to add those to the
	 * model.
	 * @return The added MeshPart. */
	public MeshPart part ( String id,  Mesh mesh, int primitiveType, int offset, int size,  Material material) {
		 MeshPart meshPart = new MeshPart();
		meshPart.id = id;
		meshPart.primitiveType = primitiveType;
		meshPart.mesh = mesh;
		meshPart.offset = offset;
		meshPart.size = size;
		part(meshPart, material);
		return meshPart;
	}

	/** Adds the specified mesh part to the current node. The Mesh will be managed by the model and disposed when the model is
	 * disposed. The resources the Material might contain are not managed, use {@link #manage(Disposable)} to add those to the
	 * model.
	 * @return The added MeshPart. */
	public MeshPart part ( String id,  Mesh mesh, int primitiveType,  Material material) {
		return part(id, mesh, primitiveType, 0, mesh.getNumIndices(), material);
	}

	/** Creates a new MeshPart within the current Node and returns a {@link MeshPartBuilder} which can be used to build the shape
	 * of the part. If possible a previously used {@link MeshPartBuilder} will be reused, to reduce the number of mesh binds.
	 * Therefore you can only build one part at a time. The resources the Material might contain are not managed, use
	 * {@link #manage(Disposable)} to add those to the model.
	 * @return The {@link MeshPartBuilder} you can use to build the MeshPart. */
	public MeshPartBuilder part ( String id, int primitiveType,  VertexAttributes attributes,  Material material) {
		 MeshBuilder builder = getBuilder(attributes);
		part(builder.part(id, primitiveType), material);
		return builder;
	}

	/** Creates a new MeshPart within the current Node and returns a {@link MeshPartBuilder} which can be used to build the shape
	 * of the part. If possible a previously used {@link MeshPartBuilder} will be reused, to reduce the number of mesh binds.
	 * Therefore you can only build one part at a time. The resources the Material might contain are not managed, use
	 * {@link #manage(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported.
	 * @return The {@link MeshPartBuilder} you can use to build the MeshPart. */
	public MeshPartBuilder part ( String id, int primitiveType,  long attributes,  Material material) {
		return part(id, primitiveType, MeshBuilder.createAttributes(attributes), material);
	}

	/** Convenience method to create a model with a single node containing a box shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createBox (float width, float height, float depth,  Material material,  long attributes) {
		return createBox(width, height, depth, IGL20.GL_TRIANGLES, material, attributes);
	}

	/** Convenience method to create a model with a single node containing a box shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createBox (float width, float height, float depth, int primitiveType,  Material material,
		 long attributes) {
		begin();
		part("box", primitiveType, attributes, material).box(width, height, depth);
		return end();
	}

	/** Convenience method to create a model with a single node containing a rectangle shape. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createRect (float x00, float y00, float z00, float x10, float y10, float z10, float x11, float y11, float z11,
		float x01, float y01, float z01, float normalX, float normalY, float normalZ,  Material material,
		 long attributes) {
		return createRect(x00, y00, z00, x10, y10, z10, x11, y11, z11, x01, y01, z01, normalX, normalY, normalZ, IGL20.GL_TRIANGLES,
			material, attributes);
	}

	/** Convenience method to create a model with a single node containing a rectangle shape. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createRect (float x00, float y00, float z00, float x10, float y10, float z10, float x11, float y11, float z11,
		float x01, float y01, float z01, float normalX, float normalY, float normalZ, int primitiveType,  Material material,
		 long attributes) {
		begin();
		part("rect", primitiveType, attributes, material).rect(x00, y00, z00, x10, y10, z10, x11, y11, z11, x01, y01, z01, normalX,
			normalY, normalZ);
		return end();
	}

	/** Convenience method to create a model with a single node containing a cylinder shape. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCylinder (float width, float height, float depth, int divisions,  Material material,
		 long attributes) {
		return createCylinder(width, height, depth, divisions, IGL20.GL_TRIANGLES, material, attributes);
	}

	/** Convenience method to create a model with a single node containing a cylinder shape. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCylinder (float width, float height, float depth, int divisions, int primitiveType,  Material material,
		 long attributes) {
		return createCylinder(width, height, depth, divisions, primitiveType, material, attributes, 0, 360);
	}

	/** Convenience method to create a model with a single node containing a cylinder shape. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCylinder (float width, float height, float depth, int divisions,  Material material,
		 long attributes, float angleFrom, float angleTo) {
		return createCylinder(width, height, depth, divisions, IGL20.GL_TRIANGLES, material, attributes, angleFrom, angleTo);
	}

	/** Convenience method to create a model with a single node containing a cylinder shape. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCylinder (float width, float height, float depth, int divisions, int primitiveType,  Material material,
		 long attributes, float angleFrom, float angleTo) {
		begin();
		part("cylinder", primitiveType, attributes, material).cylinder(width, height, depth, divisions, angleFrom, angleTo);
		return end();
	}

	/** Convenience method to create a model with a single node containing a cone shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCone (float width, float height, float depth, int divisions,  Material material,
		 long attributes) {
		return createCone(width, height, depth, divisions, IGL20.GL_TRIANGLES, material, attributes);
	}

	/** Convenience method to create a model with a single node containing a cone shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCone (float width, float height, float depth, int divisions, int primitiveType,  Material material,
		 long attributes) {
		return createCone(width, height, depth, divisions, primitiveType, material, attributes, 0, 360);
	}

	/** Convenience method to create a model with a single node containing a cone shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCone (float width, float height, float depth, int divisions,  Material material,  long attributes,
		float angleFrom, float angleTo) {
		return createCone(width, height, depth, divisions, IGL20.GL_TRIANGLES, material, attributes, angleFrom, angleTo);
	}

	/** Convenience method to create a model with a single node containing a cone shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCone (float width, float height, float depth, int divisions, int primitiveType,  Material material,
		 long attributes, float angleFrom, float angleTo) {
		begin();
		part("cone", primitiveType, attributes, material).cone(width, height, depth, divisions, angleFrom, angleTo);
		return end();
	}

	/** Convenience method to create a model with a single node containing a sphere shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createSphere (float width, float height, float depth, int divisionsU, int divisionsV,  Material material,
		 long attributes) {
		return createSphere(width, height, depth, divisionsU, divisionsV, IGL20.GL_TRIANGLES, material, attributes);
	}

	/** Convenience method to create a model with a single node containing a sphere shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createSphere (float width, float height, float depth, int divisionsU, int divisionsV, int primitiveType,
		 Material material,  long attributes) {
		return createSphere(width, height, depth, divisionsU, divisionsV, primitiveType, material, attributes, 0, 360, 0, 180);
	}

	/** Convenience method to create a model with a single node containing a sphere shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createSphere (float width, float height, float depth, int divisionsU, int divisionsV,  Material material,
		 long attributes, float angleUFrom, float angleUTo, float angleVFrom, float angleVTo) {
		return createSphere(width, height, depth, divisionsU, divisionsV, IGL20.GL_TRIANGLES, material, attributes, angleUFrom,
			angleUTo, angleVFrom, angleVTo);
	}

	/** Convenience method to create a model with a single node containing a sphere shape. The resources the Material might contain
	 * are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createSphere (float width, float height, float depth, int divisionsU, int divisionsV, int primitiveType,
		 Material material,  long attributes, float angleUFrom, float angleUTo, float angleVFrom, float angleVTo) {
		begin();
		part("sphere", primitiveType, attributes, material).sphere(width, height, depth, divisionsU, divisionsV, angleUFrom,
			angleUTo, angleVFrom, angleVTo);
		return end();
	}

	/** Convenience method to create a model with a single node containing a capsule shape. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCapsule (float radius, float height, int divisions,  Material material,  long attributes) {
		return createCapsule(radius, height, divisions, IGL20.GL_TRIANGLES, material, attributes);
	}

	/** Convenience method to create a model with a single node containing a capsule shape. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param attributes bitwise mask of the {@link com.badlogic.gdx.graphics.VertexAttributes.Usage}, only Position, Color, Normal
	 *           and TextureCoordinates is supported. */
	public Model createCapsule (float radius, float height, int divisions, int primitiveType,  Material material,
		 long attributes) {
		begin();
		part("capsule", primitiveType, attributes, material).capsule(radius, height, divisions);
		return end();
	}

	/** Resets the references to {@link Material}s, {@link Mesh}es and {@link MeshPart}s within the model to the ones used within
	 * it's nodes. This will make the model responsible for disposing all referenced meshes. */
	public static void rebuildReferences ( Model model) {
		model.materials.clear();
		model.meshes.clear();
		model.meshParts.clear();
		foreach ( Node node in model.nodes)
			rebuildReferences(model, node);
	}

	private static void rebuildReferences ( Model model,  Node node) {
		foreach ( NodePart mpm in node.parts) {
			if (!model.materials.contains(mpm.material, true)) model.materials.Add(mpm.material);
			if (!model.meshParts.contains(mpm.meshPart, true)) {
				model.meshParts.Add(mpm.meshPart);
				if (!model.meshes.contains(mpm.meshPart.mesh, true)) model.meshes.Add(mpm.meshPart.mesh);
				model.manageDisposable(mpm.meshPart.mesh);
			}
		}
		foreach ( Node child in node.getChildren())
			rebuildReferences(model, child);
	}

	/** Convenience method to create a model with three orthonormal vectors shapes. The resources the Material might contain are
	 * not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param axisLength Length of each axis.
	 * @param capLength is the height of the cap in percentage, must be in (0,1)
	 * @param stemThickness is the percentage of stem diameter compared to cap diameter, must be in (0,1]
	 * @param divisions the amount of vertices used to generate the cap and stem ellipsoidal bases */
	public Model createXYZCoordinates (float axisLength, float capLength, float stemThickness, int divisions, int primitiveType,
		Material material, long attributes) {
		begin();
		MeshPartBuilder partBuilder;
		Node node = this.node();

		partBuilder = part("xyz", primitiveType, attributes, material);
		partBuilder.setColor(Color.Red);
		partBuilder.arrow(0, 0, 0, axisLength, 0, 0, capLength, stemThickness, divisions);
		partBuilder.setColor(Color.Green);
		partBuilder.arrow(0, 0, 0, 0, axisLength, 0, capLength, stemThickness, divisions);
		partBuilder.setColor(Color.Blue);
		partBuilder.arrow(0, 0, 0, 0, 0, axisLength, capLength, stemThickness, divisions);

		return end();
	}

	public Model createXYZCoordinates (float axisLength, Material material, long attributes) {
		return createXYZCoordinates(axisLength, 0.1f, 0.1f, 5, IGL20.GL_TRIANGLES, material, attributes);
	}

	/** Convenience method to create a model with an arrow. The resources the Material might contain are not managed, use
	 * {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param material
	 * @param capLength is the height of the cap in percentage, must be in (0,1)
	 * @param stemThickness is the percentage of stem diameter compared to cap diameter, must be in (0,1]
	 * @param divisions the amount of vertices used to generate the cap and stem ellipsoidal bases */
	public Model createArrow (float x1, float y1, float z1, float x2, float y2, float z2, float capLength, float stemThickness,
		int divisions, int primitiveType, Material material, long attributes) {
		begin();
		part("arrow", primitiveType, attributes, material).arrow(x1, y1, z1, x2, y2, z2, capLength, stemThickness, divisions);
		return end();
	}

	/** Convenience method to create a model with an arrow. The resources the Material might contain are not managed, use
	 * {@link Model#manageDisposable(Disposable)} to add those to the model. */
	public Model createArrow (Vector3 from, Vector3 to, Material material, long attributes) {
		return createArrow(from.x, from.y, from.z, to.x, to.y, to.z, 0.1f, 0.1f, 5, IGL20.GL_TRIANGLES, material, attributes);
	}

	/** Convenience method to create a model which represents a grid of lines on the XZ plane. The resources the Material might
	 * contain are not managed, use {@link Model#manageDisposable(Disposable)} to add those to the model.
	 * @param xDivisions row count along x axis.
	 * @param zDivisions row count along z axis.
	 * @param xSize Length of a single row on x.
	 * @param zSize Length of a single row on z. */
	public Model createLineGrid (int xDivisions, int zDivisions, float xSize, float zSize, Material material, long attributes) {
		begin();
		MeshPartBuilder partBuilder = part("lines", IGL20.GL_LINES, attributes, material);
		float xlength = xDivisions * xSize, zlength = zDivisions * zSize, hxlength = xlength / 2, hzlength = zlength / 2;
		float x1 = -hxlength, y1 = 0, z1 = hzlength, x2 = -hxlength, y2 = 0, z2 = -hzlength;
		for (int i = 0; i <= xDivisions; ++i) {
			partBuilder.line(x1, y1, z1, x2, y2, z2);
			x1 += xSize;
			x2 += xSize;
		}

		x1 = -hxlength;
		y1 = 0;
		z1 = -hzlength;
		x2 = hxlength;
		y2 = 0;
		z2 = -hzlength;
		for (int j = 0; j <= zDivisions; ++j) {
			partBuilder.line(x1, y1, z1, x2, y2, z2);
			z1 += zSize;
			z2 += zSize;
		}

		return end();
	}

}
