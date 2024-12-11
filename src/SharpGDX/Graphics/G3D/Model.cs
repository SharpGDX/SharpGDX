using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Graphics.G3D.Attributess;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Utils;
using SharpGDX.Mathematics.Collision;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D;

/** A model represents a 3D assets. It stores a hierarchy of nodes. A node has a transform and optionally a graphical part in form
 * of a {@link MeshPart} and {@link Material}. Mesh parts reference subsets of vertices in one of the meshes of the model.
 * Animations can be applied to nodes, to modify their transform (translation, rotation, scale) over time.
 * </p>
 *
 * A model can be rendered by creating a {@link ModelInstance} from it. That instance has an additional transform to position the
 * model in the world, and allows modification of materials and nodes without destroying the original model. The original model is
 * the owner of any meshes and textures, all instances created from the model share these resources. Disposing the model will
 * automatically make all instances invalid!
 * </p>
 *
 * A model is created from {@link ModelData}, which in turn is loaded by a {@link ModelLoader}.
 *
 * @author badlogic, xoppa */
public class Model : IDisposable {
	/** the materials of the model, used by nodes that have a graphical representation FIXME not sure if superfluous, allows
	 * modification of materials without having to traverse the nodes **/
	public readonly Array<Material> materials = new ();
	/** root nodes of the model **/
	public readonly Array<Node> nodes = new ();
	/** animations of the model, modifying node transformations **/
	public readonly Array<Animation> animations = new ();
	/** the meshes of the model **/
	public readonly Array<Mesh> meshes = new ();
	/** parts of meshes, used by nodes that have a graphical representation FIXME not sure if superfluous, stored in Nodes as well,
	 * could be useful to create bullet meshes **/
	public readonly Array<MeshPart> meshParts = new ();
	/** Array of disposable resources like textures or meshes the Model is responsible for disposing **/
	protected readonly Array<IDisposable> disposables = new ();

	/** Constructs an empty model. Manual created models do not manage their resources by default. Use
	 * {@link #manageDisposable(Disposable)} to add resources to be managed by this model. */
	public Model () {
	}

	/** Constructs a new Model based on the {@link ModelData}. Texture files will be loaded from the internal file storage via an
	 * {@link FileTextureProvider}.
	 * @param modelData the {@link ModelData} got from e.g. {@link ModelLoader} */
	public Model (ModelData modelData) 
    : this(modelData, new TextureProvider.FileTextureProvider())
    {
		
	}

	/** Constructs a new Model based on the {@link ModelData}.
	 * @param modelData the {@link ModelData} got from e.g. {@link ModelLoader}
	 * @param textureProvider the {@link TextureProvider} to use for loading the textures */
	public Model (ModelData modelData, TextureProvider textureProvider) {
		load(modelData, textureProvider);
	}

	protected void load (ModelData modelData, TextureProvider textureProvider) {
		loadMeshes(modelData.meshes);
		loadMaterials(modelData.materials, textureProvider);
		loadNodes(modelData.nodes);
		loadAnimations(modelData.animations);
		calculateTransforms();
	}

	protected void loadAnimations (IEnumerable<ModelAnimation> modelAnimations) {
		foreach ( ModelAnimation anim in modelAnimations) {
			Animation animation = new Animation();
			animation.id = anim.id;
			foreach (ModelNodeAnimation nanim in anim.nodeAnimations) {
				 Node node = getNode(nanim.nodeId);
				if (node == null) continue;
				NodeAnimation nodeAnim = new NodeAnimation();
				nodeAnim.node = node;

				if (nanim.translation != null) {
					nodeAnim.translation = new Array<NodeKeyframe<Vector3>>();
					nodeAnim.translation.ensureCapacity(nanim.translation.size);
					foreach (ModelNodeKeyframe<Vector3> kf in nanim.translation) {
						if (kf.keytime > animation.duration) animation.duration = kf.keytime;
						nodeAnim.translation
							.Add(new NodeKeyframe<Vector3>(kf.keytime, new Vector3(kf.value == null ? node.translation : kf.value)));
					}
				}

				if (nanim.rotation != null) {
					nodeAnim.rotation = new Array<NodeKeyframe<Quaternion>>();
					nodeAnim.rotation.ensureCapacity(nanim.rotation.size);
					foreach (ModelNodeKeyframe<Quaternion> kf in nanim.rotation) {
						if (kf.keytime > animation.duration) animation.duration = kf.keytime;
						nodeAnim.rotation
							.Add(new NodeKeyframe<Quaternion>(kf.keytime, new Quaternion(kf.value == null ? node.rotation : kf.value)));
					}
				}

				if (nanim.scaling != null) {
					nodeAnim.scaling = new Array<NodeKeyframe<Vector3>>();
					nodeAnim.scaling.ensureCapacity(nanim.scaling.size);
					foreach (ModelNodeKeyframe<Vector3> kf in nanim.scaling) {
						if (kf.keytime > animation.duration) animation.duration = kf.keytime;
						nodeAnim.scaling
							.Add(new NodeKeyframe<Vector3>(kf.keytime, new Vector3(kf.value == null ? node.scale : kf.value)));
					}
				}

				if ((nodeAnim.translation != null && nodeAnim.translation.size > 0)
					|| (nodeAnim.rotation != null && nodeAnim.rotation.size > 0)
					|| (nodeAnim.scaling != null && nodeAnim.scaling.size > 0)) animation.nodeAnimations.Add(nodeAnim);
			}
			if (animation.nodeAnimations.size > 0) animations.Add(animation);
		}
	}

	private ObjectMap<NodePart, ArrayMap<String, Matrix4>> nodePartBones = new ObjectMap<NodePart, ArrayMap<String, Matrix4>>();

	protected void loadNodes (IEnumerable<ModelNode> modelNodes) {
		nodePartBones.clear();
		foreach (ModelNode node in modelNodes) {
			nodes.Add(loadNode(node));
		}
		foreach (var e in nodePartBones.entries()) {
			if (e.key.invBoneBindTransforms == null)
				e.key.invBoneBindTransforms = new ArrayMap<Node, Matrix4>(typeof(Node), typeof(Matrix4));
			e.key.invBoneBindTransforms.clear();
			foreach (var b in e.value.entries())
				e.key.invBoneBindTransforms.put(getNode(b.key), new Matrix4(b.value).inv());
		}
	}

	protected Node loadNode (ModelNode modelNode) {
		Node node = new Node();
		node.id = modelNode.id;

		if (modelNode.translation != null) node.translation.Set(modelNode.translation);
		if (modelNode.rotation != null) node.rotation.set(modelNode.rotation);
		if (modelNode.scale != null) node.scale.Set(modelNode.scale);
		// FIXME create temporary maps for faster lookup?
		if (modelNode.parts != null) {
			foreach (ModelNodePart modelNodePart in modelNode.parts) {
				MeshPart meshPart = null;
				Material meshMaterial = null;

				if (modelNodePart.meshPartId != null) {
					foreach (MeshPart part in meshParts) {
						if (modelNodePart.meshPartId.Equals(part.id)) {
							meshPart = part;
							break;
						}
					}
				}

				if (modelNodePart.materialId != null) {
					foreach (Material material in materials) {
						if (modelNodePart.materialId.Equals(material.id)) {
							meshMaterial = material;
							break;
						}
					}
				}

				if (meshPart == null || meshMaterial == null) throw new GdxRuntimeException("Invalid node: " + node.id);

				NodePart nodePart = new NodePart();
				nodePart.meshPart = meshPart;
				nodePart.material = meshMaterial;
				node.parts.Add(nodePart);
				if (modelNodePart.bones != null) nodePartBones.put(nodePart, modelNodePart.bones);
			}
		}

		if (modelNode.children != null) {
			foreach (ModelNode child in modelNode.children) {
				node.addChild(loadNode(child));
			}
		}

		return node;
	}

	protected void loadMeshes (IEnumerable<ModelMesh> meshes) {
		foreach (ModelMesh mesh in meshes) {
			convertMesh(mesh);
		}
	}

	protected void convertMesh (ModelMesh modelMesh) {
		int numIndices = 0;
		foreach (ModelMeshPart part in modelMesh.parts) {
			numIndices += part.indices.Length;
		}
		bool hasIndices = numIndices > 0;
		VertexAttributes attributes = new VertexAttributes(modelMesh.attributes);
		int numVertices = modelMesh.vertices.Length / (attributes.vertexSize / 4);

		Mesh mesh = new Mesh(true, numVertices, numIndices, attributes);
		meshes.Add(mesh);
		disposables.Add(mesh);

		BufferUtils.copy(modelMesh.vertices, mesh.getVerticesBuffer(true), modelMesh.vertices.Length, 0);
		int offset = 0;
		ShortBuffer indicesBuffer = mesh.getIndicesBuffer(true);
		((Buffer)indicesBuffer).clear();
		foreach (ModelMeshPart part in modelMesh.parts) {
			MeshPart meshPart = new MeshPart();
			meshPart.id = part.id;
			meshPart.primitiveType = part.primitiveType;
			meshPart.offset = offset;
			meshPart.size = hasIndices ? part.indices.Length : numVertices;
			meshPart.mesh = mesh;
			if (hasIndices) {
				indicesBuffer.put(part.indices);
			}
			offset += meshPart.size;
			meshParts.Add(meshPart);
		}
		((Buffer)indicesBuffer).position(0);
		foreach (MeshPart part in meshParts)
			part.update();
	}

	protected void loadMaterials (IEnumerable<ModelMaterial> modelMaterials, TextureProvider textureProvider) {
		foreach (ModelMaterial mtl in modelMaterials) {
			this.materials.Add(convertMaterial(mtl, textureProvider));
		}
	}

	protected Material convertMaterial (ModelMaterial mtl, TextureProvider textureProvider) {
		Material result = new Material();
		result.id = mtl.id;
		if (mtl.ambient != null) result.set(new ColorAttribute(ColorAttribute.Ambient, mtl.ambient));
		if (mtl.diffuse != null) result.set(new ColorAttribute(ColorAttribute.Diffuse, mtl.diffuse));
		if (mtl.specular != null) result.set(new ColorAttribute(ColorAttribute.Specular, mtl.specular));
		if (mtl.emissive != null) result.set(new ColorAttribute(ColorAttribute.Emissive, mtl.emissive));
		if (mtl.reflection != null) result.set(new ColorAttribute(ColorAttribute.Reflection, mtl.reflection));
		if (mtl.shininess > 0f) result.set(new FloatAttribute(FloatAttribute.Shininess, mtl.shininess));
		if (mtl.opacity != 1.0f) result.set(new BlendingAttribute(IGL20.GL_SRC_ALPHA, IGL20.GL_ONE_MINUS_SRC_ALPHA, mtl.opacity));

		ObjectMap<String, Texture> textures = new ObjectMap<String, Texture>();

		// FIXME uvScaling/uvTranslation totally ignored
		if (mtl.textures != null) {
			foreach (ModelTexture tex in mtl.textures) {
				Texture texture;
				if (textures.containsKey(tex.fileName)) {
					texture = textures.get(tex.fileName);
				} else {
					texture = textureProvider.load(tex.fileName);
					textures.put(tex.fileName, texture);
					disposables.Add(texture);
				}

				TextureDescriptor descriptor = new TextureDescriptor(texture);
				descriptor.minFilter = texture.getMinFilter();
				descriptor.magFilter = texture.getMagFilter();
				descriptor.uWrap = texture.getUWrap();
				descriptor.vWrap = texture.getVWrap();

				float offsetU = tex.uvTranslation == null ? 0f : tex.uvTranslation.x;
				float offsetV = tex.uvTranslation == null ? 0f : tex.uvTranslation.y;
				float scaleU = tex.uvScaling == null ? 1f : tex.uvScaling.x;
				float scaleV = tex.uvScaling == null ? 1f : tex.uvScaling.y;

				switch (tex.usage) {
				case ModelTexture.USAGE_DIFFUSE:
					result.set(new TextureAttribute(TextureAttribute.Diffuse, descriptor, offsetU, offsetV, scaleU, scaleV));
					break;
				case ModelTexture.USAGE_SPECULAR:
					result.set(new TextureAttribute(TextureAttribute.Specular, descriptor, offsetU, offsetV, scaleU, scaleV));
					break;
				case ModelTexture.USAGE_BUMP:
					result.set(new TextureAttribute(TextureAttribute.Bump, descriptor, offsetU, offsetV, scaleU, scaleV));
					break;
				case ModelTexture.USAGE_NORMAL:
					result.set(new TextureAttribute(TextureAttribute.Normal, descriptor, offsetU, offsetV, scaleU, scaleV));
					break;
				case ModelTexture.USAGE_AMBIENT:
					result.set(new TextureAttribute(TextureAttribute.Ambient, descriptor, offsetU, offsetV, scaleU, scaleV));
					break;
				case ModelTexture.USAGE_EMISSIVE:
					result.set(new TextureAttribute(TextureAttribute.Emissive, descriptor, offsetU, offsetV, scaleU, scaleV));
					break;
				case ModelTexture.USAGE_REFLECTION:
					result.set(new TextureAttribute(TextureAttribute.Reflection, descriptor, offsetU, offsetV, scaleU, scaleV));
					break;
				}
			}
		}

		return result;
	}

	/** Adds a {@link Disposable} to be managed and disposed by this Model. Can be used to keep track of manually loaded textures
	 * for {@link ModelInstance}.
	 * @param disposable the Disposable */
	public void manageDisposable (IDisposable disposable) {
		if (!disposables.contains(disposable, true)) disposables.Add(disposable);
	}

	/** @return the {@link Disposable} objects that will be disposed when the {@link #dispose()} method is called. */
	public IEnumerable<IDisposable> getManagedDisposables () {
		return disposables;
	}

	public void Dispose () {
		foreach (IDisposable disposable in disposables) {
			disposable.Dispose();
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
		return getAnimation(id, true);
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
				// TODO: Should this be current culture, invariant culture or ordinal? -RP
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
                // TODO: Should this be current culture, invariant culture or ordinal? -RP
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
