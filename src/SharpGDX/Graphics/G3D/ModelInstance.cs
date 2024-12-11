using System;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D;

/** An instance of a {@link Model}, allows to specify global transform and modify the materials, as it has a copy of the model's
 * materials. Multiple instances can be created from the same Model, all sharing the meshes and textures of the Model. The Model
 * owns the meshes and textures, to dispose of these, the Model has to be disposed. Therefor, the Model must outlive all its
 * ModelInstances
 * </p>
 * 
 * The ModelInstance creates a full copy of all materials, nodes and animations.
 * @author badlogic, xoppa */
public class ModelInstance : RenderableProvider {
	/** Whether, by default, {@link NodeKeyframe}'s are shared amongst {@link Model} and ModelInstance. Can be overridden per
	 * ModelInstance using the constructor argument. */
	public static bool defaultShareKeyframes = true;

	/** the materials of the model, used by nodes that have a graphical representation FIXME not sure if superfluous, allows
	 * modification of materials without having to traverse the nodes **/
	public readonly Array<Material> materials = new ();
	/** root nodes of the model **/
	public readonly Array<Node> nodes = new ();
	/** animations of the model, modifying node transformations **/
	public readonly Array<Animation> animations = new ();
	/** the {@link Model} this instances derives from **/
	public readonly Model model;
	/** the world transform **/
	public Matrix4 transform;
	/** user definable value, which is passed to the {@link Shader}. */
	public Object userData;

	/** Constructs a new ModelInstance with all nodes and materials of the given model.
	 * @param model The {@link Model} to create an instance of. */
	public ModelInstance ( Model model) 
    : this(model, (String[])null)
    {
	}

	/** @param model The source {@link Model}
	 * @param nodeId The ID of the root {@link Node} of the {@link Model} for the instance to contain
	 * @param mergeTransform True to apply the source node transform to the instance transform, resetting the node transform. */
	public ModelInstance ( Model model,  String nodeId, bool mergeTransform) 
    : this(model, null, nodeId, false, false, mergeTransform)
    {
	}

	/** @param model The source {@link Model}
	 * @param transform The {@link Matrix4} instance for this ModelInstance to reference or null to create a new matrix.
	 * @param nodeId The ID of the root {@link Node} of the {@link Model} for the instance to contain
	 * @param mergeTransform True to apply the source node transform to the instance transform, resetting the node transform. */
	public ModelInstance (Model model, Matrix4 transform, String nodeId, bool mergeTransform) 
    : this(model, transform, nodeId, false, false, mergeTransform)
    {
	}

	/** Recursively searches the mode for the specified node.
	 * @param model The source {@link Model}
	 * @param nodeId The ID of the {@link Node} within the {@link Model} for the instance to contain
	 * @param parentTransform True to apply the parent's node transform to the instance (only applicable if recursive is true).
	 * @param mergeTransform True to apply the source node transform to the instance transform, resetting the node transform. */
	public ModelInstance (Model model, String nodeId, bool parentTransform, bool mergeTransform) 
    : this(model, null, nodeId, true, parentTransform, mergeTransform)
    {
	}

	/** Recursively searches the mode for the specified node.
	 * @param model The source {@link Model}
	 * @param transform The {@link Matrix4} instance for this ModelInstance to reference or null to create a new matrix.
	 * @param nodeId The ID of the {@link Node} within the {@link Model} for the instance to contain
	 * @param parentTransform True to apply the parent's node transform to the instance (only applicable if recursive is true).
	 * @param mergeTransform True to apply the source node transform to the instance transform, resetting the node transform. */
	public ModelInstance (Model model, Matrix4 transform, String nodeId, bool parentTransform, bool mergeTransform) 
	: this(model, transform, nodeId, true, parentTransform, mergeTransform)
    {
	}

	/** @param model The source {@link Model}
	 * @param nodeId The ID of the {@link Node} within the {@link Model} for the instance to contain
	 * @param recursive True to recursively search the Model's node tree, false to only search for a root node
	 * @param parentTransform True to apply the parent's node transform to the instance (only applicable if recursive is true).
	 * @param mergeTransform True to apply the source node transform to the instance transform, resetting the node transform. */
	public ModelInstance (Model model, String nodeId, bool recursive, bool parentTransform, bool mergeTransform) 
    : this(model, null, nodeId, recursive, parentTransform, mergeTransform)
    {
	}

	/** @param model The source {@link Model}
	 * @param transform The {@link Matrix4} instance for this ModelInstance to reference or null to create a new matrix.
	 * @param nodeId The ID of the {@link Node} within the {@link Model} for the instance to contain
	 * @param recursive True to recursively search the Model's node tree, false to only search for a root node
	 * @param parentTransform True to apply the parent's node transform to the instance (only applicable if recursive is true).
	 * @param mergeTransform True to apply the source node transform to the instance transform, resetting the node transform. */
	public ModelInstance (Model model, Matrix4 transform, String nodeId, bool recursive, bool parentTransform, bool mergeTransform) 
    : this(model, transform, nodeId, recursive, parentTransform, mergeTransform, defaultShareKeyframes)
    {
	}

	/** @param model The source {@link Model}
	 * @param transform The {@link Matrix4} instance for this ModelInstance to reference or null to create a new matrix.
	 * @param nodeId The ID of the {@link Node} within the {@link Model} for the instance to contain
	 * @param recursive True to recursively search the Model's node tree, false to only search for a root node
	 * @param parentTransform True to apply the parent's node transform to the instance (only applicable if recursive is true).
	 * @param mergeTransform True to apply the source node transform to the instance transform, resetting the node transform. */
	public ModelInstance (Model model, Matrix4 transform, String nodeId, bool recursive, bool parentTransform, bool mergeTransform, bool shareKeyframes) {
		this.model = model;
		this.transform = transform == null ? new Matrix4() : transform;
		Node copy, node = model.getNode(nodeId, recursive);
		this.nodes.Add(copy = node.copy());
		if (mergeTransform) {
			this.transform.mul(parentTransform ? node.globalTransform : node.localTransform);
			copy.translation.Set(0, 0, 0);
			copy.rotation.idt();
			copy.scale.Set(1, 1, 1);
		} else if (parentTransform && copy.hasParent()) this.transform.mul(node.getParent().globalTransform);
		invalidate();
		copyAnimations(model.animations, shareKeyframes);
		calculateTransforms();
	}

	/** Constructs a new ModelInstance with only the specified nodes and materials of the given model. */
	public ModelInstance (Model model, String[] rootNodeIds) 
    : this(model, null, rootNodeIds)
    {
	}

	/** Constructs a new ModelInstance with only the specified nodes and materials of the given model. */
	public ModelInstance (Model model, Matrix4 transform, String[] rootNodeIds) {
		this.model = model;
		this.transform = transform == null ? new Matrix4() : transform;
		if (rootNodeIds == null)
			copyNodes(model.nodes);
		else
			copyNodes(model.nodes, rootNodeIds);
		copyAnimations(model.animations, defaultShareKeyframes);
		calculateTransforms();
	}

	/** Constructs a new ModelInstance with only the specified nodes and materials of the given model. */
	public ModelInstance (Model model, Array<String> rootNodeIds) 
    : this(model, null, rootNodeIds)
    {
	}

	/** Constructs a new ModelInstance with only the specified nodes and materials of the given model. */
	public ModelInstance (Model model, Matrix4 transform, Array<String> rootNodeIds) 
    : this(model, transform, rootNodeIds, defaultShareKeyframes)
    {
	}

	/** Constructs a new ModelInstance with only the specified nodes and materials of the given model. */
	public ModelInstance ( Model model,  Matrix4 transform,  Array<String> rootNodeIds, bool shareKeyframes) {
		this.model = model;
		this.transform = transform == null ? new Matrix4() : transform;
		copyNodes(model.nodes, rootNodeIds);
		copyAnimations(model.animations, shareKeyframes);
		calculateTransforms();
	}

	/** Constructs a new ModelInstance at the specified position. */
	public ModelInstance ( Model model, Vector3 position) 
    : this(model)
    {
		this.transform.setToTranslation(position);
	}

	/** Constructs a new ModelInstance at the specified position. */
	public ModelInstance ( Model model, float x, float y, float z) 
    : this(model)
    {
		this.transform.setToTranslation(x, y, z);
	}

	/** Constructs a new ModelInstance with the specified transform. */
	public ModelInstance ( Model model, Matrix4 transform) 
    : this(model, transform, (String[])null)
    {
		
	}

	/** Constructs a new ModelInstance which is an copy of the specified ModelInstance. */
	public ModelInstance (ModelInstance copyFrom) 
    : this(copyFrom, copyFrom.transform.cpy())
    {
		
	}

	/** Constructs a new ModelInstance which is an copy of the specified ModelInstance. */
	public ModelInstance (ModelInstance copyFrom,  Matrix4 transform) 
    : this(copyFrom, transform, defaultShareKeyframes)
    {
		
	}

	/** Constructs a new ModelInstance which is an copy of the specified ModelInstance. */
	public ModelInstance (ModelInstance copyFrom,  Matrix4 transform, bool shareKeyframes) {
		this.model = copyFrom.model;
		this.transform = transform == null ? new Matrix4() : transform;
		copyNodes(copyFrom.nodes);
		copyAnimations(copyFrom.animations, shareKeyframes);
		calculateTransforms();
	}

	/** @return A newly created ModelInstance which is a copy of this ModelInstance */
	public ModelInstance copy () {
		return new ModelInstance(this);
	}

	private void copyNodes (Array<Node> nodes) {
		for (int i = 0, n = nodes.size; i < n; ++i) {
			 Node node = nodes.Get(i);
			this.nodes.Add(node.copy());
		}
		invalidate();
	}

	private void copyNodes (Array<Node> nodes,  String[] nodeIds) {
		for (int i = 0, n = nodes.size; i < n; ++i) {
			 Node node = nodes.Get(i);
			foreach ( String nodeId in nodeIds) {
				if (nodeId.Equals(node.id)) {
					this.nodes.Add(node.copy());
					break;
				}
			}
		}
		invalidate();
	}

	private void copyNodes (Array<Node> nodes,  Array<String> nodeIds) {
		for (int i = 0, n = nodes.size; i < n; ++i) {
			 Node node = nodes.Get(i);
			foreach ( String nodeId in nodeIds) {
				if (nodeId.Equals(node.id)) {
					this.nodes.Add(node.copy());
					break;
				}
			}
		}
		invalidate();
	}

	/** Makes sure that each {@link NodePart} of the {@link Node} and its sub-nodes, doesn't reference a node outside this node
	 * tree and that all materials are listed in the {@link #materials} array. */
	private void invalidate (Node node) {
		for (int i = 0, n = node.parts.size; i < n; ++i) {
			NodePart part = node.parts.Get(i);
			ArrayMap<Node, Matrix4> bindPose = part.invBoneBindTransforms;
			if (bindPose != null) {
				for (int j = 0; j < bindPose.size; ++j) {
					bindPose._keys[j] = getNode(bindPose._keys[j].id);
				}
			}
			if (!materials.contains(part.material, true)) {
				 int midx = materials.indexOf(part.material, false);
				if (midx < 0)
					materials.Add(part.material = part.material.copy());
				else
					part.material = materials.Get(midx);
			}
		}
		for (int i = 0, n = node.getChildCount(); i < n; ++i) {
			invalidate(node.getChild(i));
		}
	}

	/** Makes sure that each {@link NodePart} of each {@link Node} doesn't reference a node outside this node tree and that all
	 * materials are listed in the {@link #materials} array. */
	private void invalidate () {
		for (int i = 0, n = nodes.size; i < n; ++i) {
			invalidate(nodes.Get(i));
		}
	}

	/** Copy source animations to this ModelInstance
	 * @param source Iterable collection of source animations {@link Animation} */
	public void copyAnimations ( IEnumerable<Animation> source) {
		foreach ( Animation anim in source) {
			copyAnimation(anim, defaultShareKeyframes);
		}
	}

	/** Copy source animations to this ModelInstance
	 * @param source Iterable collection of source animations {@link Animation}
	 * @param shareKeyframes Shallow copy of {@link NodeKeyframe}'s if it's true, otherwise make a deep copy. */
	public void copyAnimations (IEnumerable<Animation> source, bool shareKeyframes) {
		foreach ( Animation anim in source) {
			copyAnimation(anim, shareKeyframes);
		}
	}

	/** Copy the source animation to this ModelInstance
	 * @param sourceAnim The source animation {@link Animation} */
	public void copyAnimation (Animation sourceAnim) {
		copyAnimation(sourceAnim, defaultShareKeyframes);
	}

	/** Copy the source animation to this ModelInstance
	 * @param sourceAnim The source animation {@link Animation}
	 * @param shareKeyframes Shallow copy of {@link NodeKeyframe}'s if it's true, otherwise make a deep copy. */
	public void copyAnimation (Animation sourceAnim, bool shareKeyframes) {
		Animation animation = new Animation();
		animation.id = sourceAnim.id;
		animation.duration = sourceAnim.duration;
		foreach ( NodeAnimation nanim in sourceAnim.nodeAnimations) {
			 Node node = getNode(nanim.node.id);
			if (node == null) continue;
			NodeAnimation nodeAnim = new NodeAnimation();
			nodeAnim.node = node;
			if (shareKeyframes) {
				nodeAnim.translation = nanim.translation;
				nodeAnim.rotation = nanim.rotation;
				nodeAnim.scaling = nanim.scaling;
			} else {
				if (nanim.translation != null) {
					nodeAnim.translation = new Array<NodeKeyframe<Vector3>>();
					foreach ( NodeKeyframe<Vector3> kf in nanim.translation)
						nodeAnim.translation.Add(new NodeKeyframe<Vector3>(kf.keytime, kf.value));
				}
				if (nanim.rotation != null) {
					nodeAnim.rotation = new Array<NodeKeyframe<Quaternion>>();
					foreach ( NodeKeyframe<Quaternion> kf in nanim.rotation)
						nodeAnim.rotation.Add(new NodeKeyframe<Quaternion>(kf.keytime, kf.value));
				}
				if (nanim.scaling != null) {
					nodeAnim.scaling = new Array<NodeKeyframe<Vector3>>();
					foreach ( NodeKeyframe<Vector3> kf in nanim.scaling)
						nodeAnim.scaling.Add(new NodeKeyframe<Vector3>(kf.keytime, kf.value));
				}
			}
			if (nodeAnim.translation != null || nodeAnim.rotation != null || nodeAnim.scaling != null)
				animation.nodeAnimations.Add(nodeAnim);
		}
		if (animation.nodeAnimations.size > 0) animations.Add(animation);
	}

	/** Traverses the Node hierarchy and collects {@link Renderable} instances for every node with a graphical representation.
	 * Renderables are obtained from the provided pool. The resulting array can be rendered via a {@link ModelBatch}.
	 * 
	 * @param renderables the output array
	 * @param pool the pool to obtain Renderables from */
	public void getRenderables (Array<Renderable> renderables, Pool<Renderable> pool) {
		foreach (Node node in nodes) {
			getRenderables(node, renderables, pool);
		}
	}

	/** @return The renderable of the first node's first part. */
	public Renderable getRenderable ( Renderable @out) {
		return getRenderable(@out, nodes.Get(0));
	}

	/** @return The renderable of the node's first part. */
	public Renderable getRenderable ( Renderable @out,  Node node) {
		return getRenderable(@out, node, node.parts.Get(0));
	}

	public Renderable getRenderable ( Renderable @out,  Node node,  NodePart nodePart) {
		nodePart.setRenderable(@out);
		if (nodePart.bones == null && transform != null)
			@out.worldTransform.set(transform).mul(node.globalTransform);
		else if (transform != null)
			@out.worldTransform.set(transform);
		else
			@out.worldTransform.idt();
		@out.userData = userData;
		return @out;
	}

	protected void getRenderables (Node node, Array<Renderable> renderables, Pool<Renderable> pool) {
		if (node.parts.size > 0) {
			foreach (NodePart nodePart in node.parts) {
				if (nodePart.enabled) renderables.Add(getRenderable(pool.obtain(), node, nodePart));
			}
		}

		foreach (Node child in node.getChildren()) {
			getRenderables(child, renderables, pool);
		}
	}

	/** Calculates the local and world transform of all {@link Node} instances in this model, recursively. First each
	 * {@link Node#localTransform} transform is calculated based on the translation, rotation and scale of each Node. Then each
	 * {@link Node#calculateWorldTransform()} is calculated, based on the parent's world transform and the local transform of each
	 * Node. Finally, the animation bone matrices are updated accordingly.
	 * </p>
	 * 
	 * This method can be used to recalculate all transforms if any of the Node's local properties (translation, rotation, scale)
	 * was modified. */
	public void calculateTransforms () {
		int n = nodes.size;
		for (int i = 0; i < n; i++) {
			nodes.Get(i).calculateTransforms(true);
		}
		for (int i = 0; i < n; i++) {
			nodes.Get(i).calculateBoneTransforms(true);
		}
	}

	/** Calculate the bounding box of this model instance. This is a potential slow operation, it is advised to cache the result.
	 * @param out the {@link BoundingBox} that will be set with the bounds.
	 * @return the out parameter for chaining */
	public BoundingBox calculateBoundingBox ( BoundingBox @out) {
		@out.inf();
		return extendBoundingBox(@out);
	}

	/** Extends the bounding box with the bounds of this model instance. This is a potential slow operation, it is advised to cache
	 * the result.
	 * @param out the {@link BoundingBox} that will be extended with the bounds.
	 * @return the out parameter for chaining */
	public BoundingBox extendBoundingBox ( BoundingBox @out) {
		 int n = nodes.size;
		for (int i = 0; i < n; i++)
			nodes.Get(i).extendBoundingBox(@out);
		return @out;
	}

	/** @param id The ID of the animation to fetch (case sensitive).
	 * @return The {@link Animation} with the specified id, or null if not available. */
	public Animation getAnimation ( String id) {
		return getAnimation(id, false);
	}

	/** @param id The ID of the animation to fetch.
	 * @param ignoreCase whether to use case sensitivity when comparing the animation id.
	 * @return The {@link Animation} with the specified id, or null if not available. */
	public Animation getAnimation ( String id, bool ignoreCase) {
		 int n = animations.size;
		Animation animation;
		if (ignoreCase) {
            for (int i = 0; i < n; i++)
            {
				// TODO: Should this be invariant? -RP
                if ((animation = animations.Get(i)).id.Equals(id, StringComparison.InvariantCultureIgnoreCase)) return animation;
            }
		} else {
			for (int i = 0; i < n; i++)
				if ((animation = animations.Get(i)).id.Equals(id)) return animation;
		}
		return null;
	}

	/** @param id The ID of the material to fetch.
	 * @return The {@link Material} with the specified id, or null if not available. */
	public Material getMaterial ( String id) {
		return getMaterial(id, true);
	}

	/** @param id The ID of the material to fetch.
	 * @param ignoreCase whether to use case sensitivity when comparing the material id.
	 * @return The {@link Material} with the specified id, or null if not available. */
	public Material getMaterial ( String id, bool ignoreCase) {
		 int n = materials.size;
		Material material;
		if (ignoreCase) {
            for (int i = 0; i < n; i++)
            {
				// TODO: Should this be invariant? -RP
                if ((material = materials.Get(i)).id.Equals(id, StringComparison.InvariantCultureIgnoreCase)) return material;
            }
		} else {
			for (int i = 0; i < n; i++)
				if ((material = materials.Get(i)).id.Equals(id)) return material;
		}
		return null;
	}

	/** @param id The ID of the node to fetch.
	 * @return The {@link Node} with the specified id, or null if not found. */
	public Node getNode ( String id) {
		return getNode(id, true);
	}

	/** @param id The ID of the node to fetch.
	 * @param recursive false to fetch a root node only, true to search the entire node tree for the specified node.
	 * @return The {@link Node} with the specified id, or null if not found. */
	public Node getNode ( String id, bool recursive) {
		return getNode(id, recursive, false);
	}

	/** @param id The ID of the node to fetch.
	 * @param recursive false to fetch a root node only, true to search the entire node tree for the specified node.
	 * @param ignoreCase whether to use case sensitivity when comparing the node id.
	 * @return The {@link Node} with the specified id, or null if not found. */
	public Node getNode ( String id, bool recursive, bool ignoreCase) {
		return Node.getNode(nodes, id, recursive, ignoreCase);
	}
}
