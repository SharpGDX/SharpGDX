using System;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;


namespace SharpGDX.Graphics.G3D.Loader;

public class G3dModelLoader : ModelLoader {
	public static readonly short VERSION_HI = 0;
	public static readonly short VERSION_LO = 1;
	protected readonly BaseJsonReader reader;

	public G3dModelLoader (BaseJsonReader reader) 
    : this(reader, null)
    {
		
	}

	public G3dModelLoader (BaseJsonReader reader, IFileHandleResolver resolver) 
    : base(resolver)
    {
		
		this.reader = reader;
	}

	public override ModelData loadModelData (FileHandle fileHandle, ModelLoader.ModelParameters parameters) {
		return parseModel(fileHandle);
	}

	public ModelData parseModel (FileHandle handle) {
		JsonValue json = reader.parse(handle);
		ModelData model = new ModelData();
		JsonValue version = json.require("version");
		model.version[0] = version.getShort(0);
		model.version[1] = version.getShort(1);
		if (model.version[0] != VERSION_HI || model.version[1] != VERSION_LO)
			throw new GdxRuntimeException("Model version not supported");

		model.id = json.getString("id", "");
		parseMeshes(model, json);
		parseMaterials(model, json, handle.parent().path());
		parseNodes(model, json);
		parseAnimations(model, json);
		return model;
	}

	protected void parseMeshes (ModelData model, JsonValue json) {
		JsonValue meshes = json.get("meshes");
		if (meshes != null) {

			model.meshes.ensureCapacity(meshes.Size);
			for (JsonValue mesh = meshes._child; mesh != null; mesh = mesh._next) {
				ModelMesh jsonMesh = new ModelMesh();

				String id = mesh.getString("id", "");
				jsonMesh.id = id;

				JsonValue attributes = mesh.require("attributes");
				jsonMesh.attributes = parseAttributes(attributes);
				jsonMesh.vertices = mesh.require("vertices").asFloatArray();

				JsonValue meshParts = mesh.require("parts");
				Array<ModelMeshPart> parts = new Array<ModelMeshPart>();
				for (JsonValue meshPart = meshParts._child; meshPart != null; meshPart = meshPart._next) {
					ModelMeshPart jsonPart = new ModelMeshPart();
					String partId = meshPart.getString("id", null);
					if (partId == null) {
						throw new GdxRuntimeException("Not id given for mesh part");
					}
					foreach (ModelMeshPart other in parts) {
						if (other.id.Equals(partId)) {
							throw new GdxRuntimeException("Mesh part with id '" + partId + "' already in defined");
						}
					}
					jsonPart.id = partId;

					String type = meshPart.getString("type", null);
					if (type == null) {
						throw new GdxRuntimeException("No primitive type given for mesh part '" + partId + "'");
					}
					jsonPart.primitiveType = parseType(type);

					jsonPart.indices = meshPart.require("indices").asShortArray();
					parts.Add(jsonPart);
				}
				jsonMesh.parts = parts.toArray<ModelMeshPart>(typeof(ModelMeshPart));
				model.meshes.Add(jsonMesh);
			}
		}
	}

	protected int parseType (String type) {
		if (type.Equals("TRIANGLES")) {
			return IGL20.GL_TRIANGLES;
		} else if (type.Equals("LINES")) {
			return IGL20.GL_LINES;
		} else if (type.Equals("POINTS")) {
			return IGL20.GL_POINTS;
		} else if (type.Equals("TRIANGLE_STRIP")) {
			return IGL20.GL_TRIANGLE_STRIP;
		} else if (type.Equals("LINE_STRIP")) {
			return IGL20.GL_LINE_STRIP;
		} else {
			throw new GdxRuntimeException(
				"Unknown primitive type '" + type + "', should be one of triangle, trianglestrip, line, linestrip or point");
		}
	}

	protected VertexAttribute[] parseAttributes (JsonValue attributes) {
		Array<VertexAttribute> vertexAttributes = new Array<VertexAttribute>();
		int unit = 0;
		int blendWeightCount = 0;
		for (JsonValue value = attributes._child; value != null; value = value._next) {
			String attribute = value.asString();
			String attr = (String)attribute;
			if (attr.Equals("POSITION")) {
				vertexAttributes.Add(VertexAttribute.Position());
			} else if (attr.Equals("NORMAL")) {
				vertexAttributes.Add(VertexAttribute.Normal());
			} else if (attr.Equals("COLOR")) {
				vertexAttributes.Add(VertexAttribute.ColorUnpacked());
			} else if (attr.Equals("COLORPACKED")) {
				vertexAttributes.Add(VertexAttribute.ColorPacked());
			} else if (attr.Equals("TANGENT")) {
				vertexAttributes.Add(VertexAttribute.Tangent());
			} else if (attr.Equals("BINORMAL")) {
				vertexAttributes.Add(VertexAttribute.Binormal());
			} else if (attr.StartsWith("TEXCOORD")) {
				vertexAttributes.Add(VertexAttribute.TexCoords(unit++));
			} else if (attr.StartsWith("BLENDWEIGHT")) {
				vertexAttributes.Add(VertexAttribute.BoneWeight(blendWeightCount++));
			} else {
				throw new GdxRuntimeException(
					"Unknown vertex attribute '" + attr + "', should be one of position, normal, uv, tangent or binormal");
			}
		}
		return vertexAttributes.toArray<VertexAttribute>(typeof(VertexAttribute));
	}

	protected void parseMaterials (ModelData model, JsonValue json, String materialDir) {
		JsonValue materials = json.get("materials");
		if (materials == null) {
			// we should probably create some default material in this case
		} else {
			model.materials.ensureCapacity(materials.Size);
			for (JsonValue material = materials._child; material != null; material = material._next) {
				ModelMaterial jsonMaterial = new ModelMaterial();

				String id = material.getString("id", null);
				if (id == null) throw new GdxRuntimeException("Material needs an id.");

				jsonMaterial.id = id;

				// Read material colors
				 JsonValue diffuse = material.get("diffuse");
				if (diffuse != null) jsonMaterial.diffuse = parseColor(diffuse);
				 JsonValue ambient = material.get("ambient");
				if (ambient != null) jsonMaterial.ambient = parseColor(ambient);
				 JsonValue emissive = material.get("emissive");
				if (emissive != null) jsonMaterial.emissive = parseColor(emissive);
				 JsonValue specular = material.get("specular");
				if (specular != null) jsonMaterial.specular = parseColor(specular);
				 JsonValue reflection = material.get("reflection");
				if (reflection != null) jsonMaterial.reflection = parseColor(reflection);
				// Read shininess
				jsonMaterial.shininess = material.getFloat("shininess", 0.0f);
				// Read opacity
				jsonMaterial.opacity = material.getFloat("opacity", 1.0f);

				// Read textures
				JsonValue textures = material.get("textures");
				if (textures != null) {
					for (JsonValue texture = textures._child; texture != null; texture = texture._next) {
						ModelTexture jsonTexture = new ModelTexture();

						String textureId = texture.getString("id", null);
						if (textureId == null) throw new GdxRuntimeException("Texture has no id.");
						jsonTexture.id = textureId;

						String fileName = texture.getString("filename", null);
						if (fileName == null) throw new GdxRuntimeException("Texture needs filename.");
						jsonTexture.fileName = materialDir + (materialDir.Length == 0 || materialDir.EndsWith("/") ? "" : "/")
							+ fileName;

						jsonTexture.uvTranslation = readVector2(texture.get("uvTranslation"), 0f, 0f);
						jsonTexture.uvScaling = readVector2(texture.get("uvScaling"), 1f, 1f);

						String textureType = texture.getString("type", null);
						if (textureType == null) throw new GdxRuntimeException("Texture needs type.");

						jsonTexture.usage = parseTextureUsage(textureType);

						if (jsonMaterial.textures == null) jsonMaterial.textures = new Array<ModelTexture>();
						jsonMaterial.textures.Add(jsonTexture);
					}
				}

				model.materials.Add(jsonMaterial);
			}
		}
	}

	protected int parseTextureUsage (String value) {
		// TODO: See if 'invariant' is correct. -RP
		if (value.Equals("AMBIENT", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_AMBIENT;
		else if (value.Equals("BUMP", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_BUMP;
		else if (value.Equals("DIFFUSE", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_DIFFUSE;
		else if (value.Equals("EMISSIVE", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_EMISSIVE;
		else if (value.Equals("NONE", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_NONE;
		else if (value.Equals("NORMAL", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_NORMAL;
		else if (value.Equals("REFLECTION", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_REFLECTION;
		else if (value.Equals("SHININESS", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_SHININESS;
		else if (value.Equals("SPECULAR", StringComparison.InvariantCultureIgnoreCase))
			return ModelTexture.USAGE_SPECULAR;
		else if (value.Equals("TRANSPARENCY", StringComparison.InvariantCultureIgnoreCase)) return ModelTexture.USAGE_TRANSPARENCY;
		return ModelTexture.USAGE_UNKNOWN;
	}

	protected Color parseColor (JsonValue colorArray) {
		if (colorArray.Size >= 3)
			return new Color(colorArray.getFloat(0), colorArray.getFloat(1), colorArray.getFloat(2), 1.0f);
		else
			throw new GdxRuntimeException("Expected Color values <> than three.");
	}

	protected Vector2 readVector2 (JsonValue vectorArray, float x, float y) {
		if (vectorArray == null)
			return new Vector2(x, y);
		else if (vectorArray.Size == 2)
			return new Vector2(vectorArray.getFloat(0), vectorArray.getFloat(1));
		else
			throw new GdxRuntimeException("Expected Vector2 values <> than two.");
	}

	protected Array<ModelNode> parseNodes (ModelData model, JsonValue json) {
		JsonValue nodes = json.get("nodes");
		if (nodes != null) {
			model.nodes.ensureCapacity(nodes.Size);
			for (JsonValue node = nodes._child; node != null; node = node._next) {
				model.nodes.Add(parseNodesRecursively(node));
			}
		}

		return model.nodes;
	}

	protected readonly Quaternion tempQ = new Quaternion();

	protected ModelNode parseNodesRecursively (JsonValue json) {
		ModelNode jsonNode = new ModelNode();

		String id = json.getString("id", null);
		if (id == null) throw new GdxRuntimeException("Node id missing.");
		jsonNode.id = id;

		JsonValue translation = json.get("translation");
		if (translation != null && translation.Size != 3) throw new GdxRuntimeException("Node translation incomplete");
		jsonNode.translation = translation == null ? null
			: new Vector3(translation.getFloat(0), translation.getFloat(1), translation.getFloat(2));

		JsonValue rotation = json.get("rotation");
		if (rotation != null && rotation.Size != 4) throw new GdxRuntimeException("Node rotation incomplete");
		jsonNode.rotation = rotation == null ? null
			: new Quaternion(rotation.getFloat(0), rotation.getFloat(1), rotation.getFloat(2), rotation.getFloat(3));

		JsonValue scale = json.get("scale");
		if (scale != null && scale.Size != 3) throw new GdxRuntimeException("Node scale incomplete");
		jsonNode.scale = scale == null ? null : new Vector3(scale.getFloat(0), scale.getFloat(1), scale.getFloat(2));

		String meshId = json.getString("mesh", null);
		if (meshId != null) jsonNode.meshId = meshId;

		JsonValue materials = json.get("parts");
		if (materials != null) {
			jsonNode.parts = new ModelNodePart[materials.Size];
			int i = 0;
			for (JsonValue material = materials._child; material != null; material = material._next, i++) {
				ModelNodePart nodePart = new ModelNodePart();

				String meshPartId = material.getString("meshpartid", null);
				String materialId = material.getString("materialid", null);
				if (meshPartId == null || materialId == null) {
					throw new GdxRuntimeException("Node " + id + " part is missing meshPartId or materialId");
				}
				nodePart.materialId = materialId;
				nodePart.meshPartId = meshPartId;

				JsonValue bones = material.get("bones");
				if (bones != null) {
					nodePart.bones = new ArrayMap<String, Matrix4>(true, bones.Size, typeof(String), typeof(Matrix4));
					int j = 0;
					for (JsonValue bone = bones._child; bone != null; bone = bone._next, j++) {
						String nodeId = bone.getString("node", null);
						if (nodeId == null) throw new GdxRuntimeException("Bone node ID missing");

						Matrix4 transform = new Matrix4();

						JsonValue val = bone.get("translation");
						if (val != null && val.Size >= 3) transform.translate(val.getFloat(0), val.getFloat(1), val.getFloat(2));

						val = bone.get("rotation");
						if (val != null && val.Size >= 4)
							transform.rotate(tempQ.set(val.getFloat(0), val.getFloat(1), val.getFloat(2), val.getFloat(3)));

						val = bone.get("scale");
						if (val != null && val.Size >= 3) transform.scale(val.getFloat(0), val.getFloat(1), val.getFloat(2));

						nodePart.bones.put(nodeId, transform);
					}
				}

				jsonNode.parts[i] = nodePart;
			}
		}

		JsonValue children = json.get("children");
		if (children != null) {
			jsonNode.children = new ModelNode[children.Size];

			int i = 0;
			for (JsonValue child = children._child; child != null; child = child._next, i++) {
				jsonNode.children[i] = parseNodesRecursively(child);
			}
		}

		return jsonNode;
	}

	protected void parseAnimations (ModelData model, JsonValue json) {
		JsonValue animations = json.get("animations");
		if (animations == null) return;

		model.animations.ensureCapacity(animations.Size);

		for (JsonValue anim = animations._child; anim != null; anim = anim._next) {
			JsonValue nodes = anim.get("bones");
			if (nodes == null) continue;
			ModelAnimation animation = new ModelAnimation();
			model.animations.Add(animation);
			animation.nodeAnimations.ensureCapacity(nodes.Size);
			animation.id = anim.getString("id");
			for (JsonValue node = nodes._child; node != null; node = node._next) {
				ModelNodeAnimation nodeAnim = new ModelNodeAnimation();
				animation.nodeAnimations.Add(nodeAnim);
				nodeAnim.nodeId = node.getString("boneId");

				// For backwards compatibility (version 0.1):
				JsonValue keyframes = node.get("keyframes");
				if (keyframes != null && keyframes.isArray()) {
					for (JsonValue keyframe = keyframes._child; keyframe != null; keyframe = keyframe._next) {
						 float keytime = keyframe.getFloat("keytime", 0f) / 1000.0f;
						JsonValue translation = keyframe.get("translation");
						if (translation != null && translation.Size == 3) {
							if (nodeAnim.translation == null) nodeAnim.translation = new Array<ModelNodeKeyframe<Vector3>>();
							ModelNodeKeyframe<Vector3> tkf = new ModelNodeKeyframe<Vector3>();
							tkf.keytime = keytime;
							tkf.value = new Vector3(translation.getFloat(0), translation.getFloat(1), translation.getFloat(2));
							nodeAnim.translation.Add(tkf);
						}
						JsonValue rotation = keyframe.get("rotation");
						if (rotation != null && rotation.Size == 4) {
							if (nodeAnim.rotation == null) nodeAnim.rotation = new Array<ModelNodeKeyframe<Quaternion>>();
							ModelNodeKeyframe<Quaternion> rkf = new ModelNodeKeyframe<Quaternion>();
							rkf.keytime = keytime;
							rkf.value = new Quaternion(rotation.getFloat(0), rotation.getFloat(1), rotation.getFloat(2),
								rotation.getFloat(3));
							nodeAnim.rotation.Add(rkf);
						}
						JsonValue scale = keyframe.get("scale");
						if (scale != null && scale.Size == 3) {
							if (nodeAnim.scaling == null) nodeAnim.scaling = new Array<ModelNodeKeyframe<Vector3>>();
							ModelNodeKeyframe<Vector3> skf = new ();
							skf.keytime = keytime;
							skf.value = new Vector3(scale.getFloat(0), scale.getFloat(1), scale.getFloat(2));
							nodeAnim.scaling.Add(skf);
						}
					}
				} else { // Version 0.2:
					JsonValue translationKF = node.get("translation");
					if (translationKF != null && translationKF.isArray()) {
						nodeAnim.translation = new Array<ModelNodeKeyframe<Vector3>>();
						nodeAnim.translation.ensureCapacity(translationKF.Size);
						for (JsonValue keyframe = translationKF._child; keyframe != null; keyframe = keyframe._next) {
							ModelNodeKeyframe<Vector3> kf = new ModelNodeKeyframe<Vector3>();
							nodeAnim.translation.Add(kf);
							kf.keytime = keyframe.getFloat("keytime", 0f) / 1000.0f;
							JsonValue translation = keyframe.get("value");
							if (translation != null && translation.Size >= 3)
								kf.value = new Vector3(translation.getFloat(0), translation.getFloat(1), translation.getFloat(2));
						}
					}

					JsonValue rotationKF = node.get("rotation");
					if (rotationKF != null && rotationKF.isArray()) {
						nodeAnim.rotation = new Array<ModelNodeKeyframe<Quaternion>>();
						nodeAnim.rotation.ensureCapacity(rotationKF.Size);
						for (JsonValue keyframe = rotationKF._child; keyframe != null; keyframe = keyframe._next) {
							ModelNodeKeyframe<Quaternion> kf = new ModelNodeKeyframe<Quaternion>();
							nodeAnim.rotation.Add(kf);
							kf.keytime = keyframe.getFloat("keytime", 0f) / 1000.0f;
							JsonValue rotation = keyframe.get("value");
							if (rotation != null && rotation.Size >= 4) kf.value = new Quaternion(rotation.getFloat(0),
								rotation.getFloat(1), rotation.getFloat(2), rotation.getFloat(3));
						}
					}

					JsonValue scalingKF = node.get("scaling");
					if (scalingKF != null && scalingKF.isArray()) {
						nodeAnim.scaling = new Array<ModelNodeKeyframe<Vector3>>();
						nodeAnim.scaling.ensureCapacity(scalingKF.Size);
						for (JsonValue keyframe = scalingKF._child; keyframe != null; keyframe = keyframe._next) {
							ModelNodeKeyframe<Vector3> kf = new ModelNodeKeyframe<Vector3>();
							nodeAnim.scaling.Add(kf);
							kf.keytime = keyframe.getFloat("keytime", 0f) / 1000.0f;
							JsonValue scaling = keyframe.get("value");
							if (scaling != null && scaling.Size >= 3)
								kf.value = new Vector3(scaling.getFloat(0), scaling.getFloat(1), scaling.getFloat(2));
						}
					}
				}
			}
		}
	}
}
