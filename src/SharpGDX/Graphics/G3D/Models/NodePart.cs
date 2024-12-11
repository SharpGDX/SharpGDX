using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Models;

/** A combination of {@link MeshPart} and {@link Material}, used to represent a {@link Node}'s graphical properties. A NodePart is
 * the smallest visible part of a {@link Model}, each NodePart implies a render call.
 * @author badlogic, Xoppa */
public class NodePart {
	/** The MeshPart (shape) to render. Must not be null. */
	public MeshPart meshPart;
	/** The Material used to render the {@link #meshPart}. Must not be null. */
	public Material material;
	/** Mapping to each bone (node) and the inverse transform of the bind pose. Will be used to fill the {@link #bones} array. May
	 * be null. */
	public ArrayMap<Node, Matrix4> invBoneBindTransforms;
	/** The current transformation (relative to the bind pose) of each bone, may be null. When the part is skinned, this will be
	 * updated by a call to {@link ModelInstance#calculateTransforms()}. Do not set or change this value manually. */
	public Matrix4[] bones;
	/** true by default. If set to false, this part will not participate in rendering and bounding box calculation. */
	public bool enabled = true;

	/** Construct a new NodePart with null values. At least the {@link #meshPart} and {@link #material} member must be set before
	 * the newly created part can be used. */
	public NodePart () {
	}

	/** Construct a new NodePart referencing the provided {@link MeshPart} and {@link Material}.
	 * @param meshPart The MeshPart to reference.
	 * @param material The Material to reference. */
	public NodePart (MeshPart meshPart, Material material) {
		this.meshPart = meshPart;
		this.material = material;
	}

	// FIXME add copy constructor and override #equals.

	/** Convenience method to set the material, mesh, meshPartOffset, meshPartSize, primitiveType and bones members of the
	 * specified Renderable. The other member of the provided {@link Renderable} remain untouched. Note that the material, mesh and
	 * bones members are referenced, not copied. Any changes made to those objects will be reflected in both the NodePart and
	 * Renderable object.
	 * @param out The Renderable of which to set the members to the values of this NodePart. */
	public Renderable setRenderable (Renderable @out) {
		@out.material = material;
		@out.meshPart.set(meshPart);
		@out.bones = bones;
		return @out;
	}

	public NodePart copy () {
		return new NodePart().set(this);
	}

	protected NodePart set (NodePart other) {
		meshPart = new MeshPart(other.meshPart);
		material = other.material;
		enabled = other.enabled;
		if (other.invBoneBindTransforms == null) {
			invBoneBindTransforms = null;
			bones = null;
		} else {
			if (invBoneBindTransforms == null)
				invBoneBindTransforms = new ArrayMap<Node, Matrix4>(true, other.invBoneBindTransforms.size, typeof(Node),
					typeof(Matrix4));
			else
				invBoneBindTransforms.clear();
			invBoneBindTransforms.putAll(other.invBoneBindTransforms);

			if (bones == null || bones.Length != invBoneBindTransforms.size) bones = new Matrix4[invBoneBindTransforms.size];

			for (int i = 0; i < bones.Length; i++) {
				if (bones[i] == null) bones[i] = new Matrix4();
			}
		}
		return this;
	}
}
