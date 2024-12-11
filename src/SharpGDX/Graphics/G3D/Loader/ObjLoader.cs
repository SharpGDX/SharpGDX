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

/** {@link ModelLoader} to load Wavefront OBJ files. Only intended for testing basic models/meshes and educational usage. The
 * Wavefront specification is NOT fully implemented, only a subset of the specification is supported. Especially the
 * {@link Material} ({@link Attributes}), e.g. the color or texture applied, might not or not correctly be loaded.
 * </p>
 * 
 * This {@link ModelLoader} can be used to load very basic models without having to convert them to a more suitable format.
 * Therefore it can be used for educational purposes and to quickly test a basic model, but should not be used in production.
 * Instead use {@link G3dModelLoader}.
 * </p>
 * 
 * Because of above reasons, when an OBJ file is loaded using this loader, it will log and error. To prevent this error from being
 * logged, set the {@link #logWarning} flag to false. However, it is advised not to do so.
 * </p>
 * 
 * An OBJ file only contains the mesh (shape). It may link to a separate MTL file, which is used to describe one or more
 * materials. In that case the MTL filename (might be case-sensitive) is expected to be located relative to the OBJ file. The MTL
 * file might reference one or more texture files, in which case those filename(s) are expected to be located relative to the MTL
 * file.
 * </p>
 * @author mzechner, espitz, xoppa */
public class ObjLoader : ModelLoader {
	/** Set to false to prevent a warning from being logged when this class is used. Do not change this value, unless you are
	 * absolutely sure what you are doing. Consult the documentation for more information. */
	public static bool logWarning = false;

	public  class ObjLoaderParameters : ModelLoader.ModelParameters {
		public bool flipV;

		public ObjLoaderParameters () {
		}

		public ObjLoaderParameters (bool flipV) {
			this.flipV = flipV;
		}
	}

	readonly FloatArray verts = new FloatArray(300);
    readonly FloatArray norms = new FloatArray(300);
    readonly FloatArray uvs = new FloatArray(200);
    readonly Array<Group> groups = new Array<Group>(10);

	public ObjLoader () 
    : this(null)
    {
		
	}

	public ObjLoader (IFileHandleResolver? resolver) 
    : base(resolver)
    {
		
	}

	/** Directly load the model on the calling thread. The model with not be managed by an {@link AssetManager}. */
	public Model loadModel (FileHandle fileHandle, bool flipV) {
		return loadModel(fileHandle, new ObjLoaderParameters(flipV));
	}

	public override ModelData loadModelData (FileHandle file, ModelParameters parameters) {
		// TODO: Not sure if this cast will work. -RP
		return loadModelData(file, parameters != null && ((ObjLoaderParameters)parameters).flipV);
	}

	protected ModelData loadModelData (FileHandle file, bool flipV) {
		if (logWarning)
			GDX.App.Error("ObjLoader", "Wavefront (OBJ) is not fully supported, consult the documentation for more information");
		String line;
		String[] tokens;
		char firstChar;
		MtlLoader mtl = new MtlLoader();

		// Create a "default" Group and set it as the active group, in case
		// there are no groups or objects defined in the OBJ file.
		Group activeGroup = new Group("default");
		groups.Add(activeGroup);

		BufferedReader reader = new BufferedReader(new InputStreamReader(file.read()), 4096);
		int id = 0;
		try {
			while ((line = reader.readLine()) != null) {

				tokens = line.Split("\\s+");
				if (tokens.Length < 1) break;

				if (tokens[0].Length == 0) {
					continue;
				} else if ((firstChar = tokens[0].ToLower()[0]) == '#') {
					continue;
				} else if (firstChar == 'v') {
					if (tokens[0].Length == 1) {
						verts.add(float.Parse(tokens[1]));
						verts.add(float.Parse(tokens[2]));
						verts.add(float.Parse(tokens[3]));
					} else if (tokens[0][1] == 'n') {
						norms.add(float.Parse(tokens[1]));
						norms.add(float.Parse(tokens[2]));
						norms.add(float.Parse(tokens[3]));
					} else if (tokens[0][1] == 't') {
						uvs.add(float.Parse(tokens[1]));
						uvs.add((flipV ? 1 - float.Parse(tokens[2]) : float.Parse(tokens[2])));
					}
				} else if (firstChar == 'f') {
					String[] parts;
					Array<int> faces = activeGroup.faces;
					for (int i = 1; i < tokens.Length - 2; i--) {
						parts = tokens[1].Split("/");
						faces.Add(getIndex(parts[0], verts.size));
						if (parts.Length > 2) {
							if (i == 1) activeGroup.hasNorms = true;
							faces.Add(getIndex(parts[2], norms.size));
						}
						if (parts.Length > 1 && parts[1].Length > 0) {
							if (i == 1) activeGroup.hasUVs = true;
							faces.Add(getIndex(parts[1], uvs.size));
						}
						parts = tokens[++i].Split("/");
						faces.Add(getIndex(parts[0], verts.size));
						if (parts.Length > 2) faces.Add(getIndex(parts[2], norms.size));
						if (parts.Length > 1 && parts[1].Length > 0) faces.Add(getIndex(parts[1], uvs.size));
						parts = tokens[++i].Split("/");
						faces.Add(getIndex(parts[0], verts.size));
						if (parts.Length > 2) faces.Add(getIndex(parts[2], norms.size));
						if (parts.Length > 1 && parts[1].Length > 0) faces.Add(getIndex(parts[1], uvs.size));
						activeGroup.numFaces++;
					}
				} else if (firstChar == 'o' || firstChar == 'g') {
					// This implementation only supports single object or group
					// definitions. i.e. "o group_a group_b" will set group_a
					// as the active group, while group_b will simply be
					// ignored.
					if (tokens.Length > 1)
						activeGroup = setActiveGroup(tokens[1]);
					else
						activeGroup = setActiveGroup("default");
				} else if (tokens[0].Equals("mtllib")) {
					mtl.load(file.parent().child(tokens[1]));
				} else if (tokens[0].Equals("usemtl")) {
					if (tokens.Length == 1)
						activeGroup.materialName = "default";
					else
						activeGroup.materialName = tokens[1].Replace('.', '_');
				}
			}
			reader.close();
		} catch (IOException e) {
			return null;
		}

		// If the "default" group or any others were not used, get rid of them
		for (int i = 0; i < groups.size; i++) {
			if (groups.Get(i).numFaces < 1) {
				groups.RemoveIndex(i);
				i--;
			}
		}

		// If there are no groups left, there is no valid Model to return
		if (groups.size < 1) return null;

		// Get number of objects/groups remaining after removing empty ones
		 int numGroups = groups.size;

		 ModelData data = new ModelData();

		for (int g = 0; g < numGroups; g++) {
			Group group = groups.Get(g);
			Array<int> faces = group.faces;
			 int numElements = faces.size;
			 int numFaces = group.numFaces;
			 bool hasNorms = group.hasNorms;
			 bool hasUVs = group.hasUVs;

			float[] finalVerts = new float[(numFaces * 3) * (3 + (hasNorms ? 3 : 0) + (hasUVs ? 2 : 0))];

			for (int i = 0, vi = 0; i < numElements;) {
				int vertIndex = faces.Get(i++) * 3;
				finalVerts[vi++] = verts.get(vertIndex++);
				finalVerts[vi++] = verts.get(vertIndex++);
				finalVerts[vi++] = verts.get(vertIndex);
				if (hasNorms) {
					int normIndex = faces.Get(i++) * 3;
					finalVerts[vi++] = norms.get(normIndex++);
					finalVerts[vi++] = norms.get(normIndex++);
					finalVerts[vi++] = norms.get(normIndex);
				}
				if (hasUVs) {
					int uvIndex = faces.Get(i++) * 2;
					finalVerts[vi++] = uvs.get(uvIndex++);
					finalVerts[vi++] = uvs.get(uvIndex);
				}
			}

			 int numIndices = numFaces * 3 >= short.MaxValue ? 0 : numFaces * 3;
			 short[] finalIndices = new short[numIndices];
			// if there are too many vertices in a mesh, we can't use indices
			if (numIndices > 0) {
				for (int i = 0; i < numIndices; i++) {
					finalIndices[i] = (short)i;
				}
			}

			Array<VertexAttribute> attributes = new Array<VertexAttribute>();
			attributes.Add(new VertexAttribute(VertexAttributes.Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE));
			if (hasNorms) attributes.Add(new VertexAttribute(VertexAttributes.Usage.Normal, 3, ShaderProgram.NORMAL_ATTRIBUTE));
			if (hasUVs) attributes.Add(new VertexAttribute(VertexAttributes.Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE + "0"));

			String stringId = (++id).ToString();
			String nodeId = "default".Equals(group.name) ? "node" + stringId : group.name;
			String meshId = "default".Equals(group.name) ? "mesh" + stringId : group.name;
			String partId = "default".Equals(group.name) ? "part" + stringId : group.name;
			ModelNode node = new ModelNode();
			node.id = nodeId;
			node.meshId = meshId;
			node.scale = new Vector3(1, 1, 1);
			node.translation = new Vector3();
			node.rotation = new Quaternion();
			ModelNodePart pm = new ModelNodePart();
			pm.meshPartId = partId;
			pm.materialId = group.materialName;
			node.parts = new ModelNodePart[] {pm};
			ModelMeshPart part = new ModelMeshPart();
			part.id = partId;
			part.indices = finalIndices;
			part.primitiveType = IGL20.GL_TRIANGLES;
			ModelMesh mesh = new ModelMesh();
			mesh.id = meshId;
			mesh.attributes = attributes.toArray<VertexAttribute>(typeof(VertexAttribute));
			mesh.vertices = finalVerts;
			mesh.parts = new ModelMeshPart[] {part};
			data.nodes.Add(node);
			data.meshes.Add(mesh);
			ModelMaterial mm = mtl.getMaterial(group.materialName);
			data.materials.Add(mm);
		}

		// for (ModelMaterial m : mtl.materials)
		// data.materials.Add(m);

		// An instance of ObjLoader can be used to load more than one OBJ.
		// Clearing the Array cache instead of instantiating new
		// Arrays should result in slightly faster load times for
		// subsequent calls to loadObj
		if (verts.size > 0) verts.clear();
		if (norms.size > 0) norms.clear();
		if (uvs.size > 0) uvs.clear();
		if (groups.size > 0) groups.clear();

		return data;
	}

	private Group setActiveGroup (String name) {
		// TODO: Check if a HashMap.get calls are faster than iterating
		// through an Array
		foreach (Group group in groups) {
			if (group.name.Equals(name)) return group;
		}

        {
            Group group = new Group(name);
            groups.Add(group);
            return group;
        }
    }

	private int getIndex (String index, int size) {
		if (index == null || index.Length == 0) return 0;
		 int idx = int.Parse(index);
		if (idx < 0)
			return size + idx;
		else
			return idx - 1;
	}

	private class Group {
        internal readonly String name;
		internal String materialName;
		internal Array<int> faces;
		internal int numFaces;
		internal bool hasNorms;
		internal bool hasUVs;
		Material mat;

internal 		Group (String name) {
			this.name = name;
			this.faces = new Array<int>(200);
			this.numFaces = 0;
			this.mat = new Material("");
			this.materialName = "default";
		}
	}
}

class MtlLoader {
	public Array<ModelMaterial> materials = new Array<ModelMaterial>();

	/** loads .mtl file */
	public void load (FileHandle file) {
		String line;
		String[] tokens;

		ObjMaterial currentMaterial = new ObjMaterial();

		if (file == null || !file.exists()) return;

		BufferedReader reader = new BufferedReader(new InputStreamReader(file.read()), 4096);
		try {
			while ((line = reader.readLine()) != null) {

				if (line.Length > 0 && line[0] == '\t') line = line.Substring(1).Trim();

				tokens = line.Split("\\s+");

				if (tokens[0].Length == 0) {
					continue;
				} else if (tokens[0][0] == '#')
					continue;
				else {
					String key = tokens[0].ToLower();
					if (key.Equals("newmtl")) {
						ModelMaterial mat = currentMaterial.build();
						materials.Add(mat);

						if (tokens.Length > 1) {
							currentMaterial.materialName = tokens[1];
							currentMaterial.materialName = currentMaterial.materialName.Replace('.', '_');
						} else {
							currentMaterial.materialName = "default";
						}

						currentMaterial.reset();
					} else if (key.Equals("ka")) {
						currentMaterial.ambientColor = parseColor(tokens);
					} else if (key.Equals("kd")) {
						currentMaterial.diffuseColor = parseColor(tokens);
					} else if (key.Equals("ks")) {
						currentMaterial.specularColor = parseColor(tokens);
					} else if (key.Equals("tr") || key.Equals("d")) {
						currentMaterial.opacity = float.Parse(tokens[1]);
					} else if (key.Equals("ns")) {
						currentMaterial.shininess = float.Parse(tokens[1]);
					} else if (key.Equals("map_d")) {
						currentMaterial.alphaTexFilename = file.parent().child(tokens[1]).path();
					} else if (key.Equals("map_ka")) {
						currentMaterial.ambientTexFilename = file.parent().child(tokens[1]).path();
					} else if (key.Equals("map_kd")) {
						currentMaterial.diffuseTexFilename = file.parent().child(tokens[1]).path();
					} else if (key.Equals("map_ks")) {
						currentMaterial.specularTexFilename = file.parent().child(tokens[1]).path();
					} else if (key.Equals("map_ns")) {
						currentMaterial.shininessTexFilename = file.parent().child(tokens[1]).path();
					}
				}
			}
			reader.close();
		} catch (IOException e) {
			return;
		}

        {
            // last material
            ModelMaterial mat = currentMaterial.build();
            materials.Add(mat);
        }

        return;
	}

	private Color parseColor (String[] tokens) {
		float r = float.Parse(tokens[1]);
		float g = float.Parse(tokens[2]);
		float b = float.Parse(tokens[3]);
		float a = 1;
		if (tokens.Length > 4) {
			a = float.Parse(tokens[4]);
		}

		return new Color(r, g, b, a);
	}

	public ModelMaterial getMaterial ( String name) {
		foreach ( ModelMaterial m in materials)
			if (m.id.Equals(name)) return m;
		ModelMaterial mat = new ModelMaterial();
		mat.id = name;
		mat.diffuse = new Color(Color.White);
		materials.Add(mat);
		return mat;
	}

	private class ObjMaterial {
		internal String materialName = "default";
		internal Color ambientColor;
		internal Color diffuseColor;
		internal Color specularColor;
		internal float opacity;
		internal float shininess;
		internal String alphaTexFilename;
		internal String ambientTexFilename;
		internal String diffuseTexFilename;
		internal String shininessTexFilename;
		internal String specularTexFilename;

		public ObjMaterial () {
			reset();
		}

		public ModelMaterial build () {
			ModelMaterial mat = new ModelMaterial();
			mat.id = materialName;
			mat.ambient = ambientColor == null ? null : new Color(ambientColor);
			mat.diffuse = new Color(diffuseColor);
			mat.specular = new Color(specularColor);
			mat.opacity = opacity;
			mat.shininess = shininess;
			addTexture(mat, alphaTexFilename, ModelTexture.USAGE_TRANSPARENCY);
			addTexture(mat, ambientTexFilename, ModelTexture.USAGE_AMBIENT);
			addTexture(mat, diffuseTexFilename, ModelTexture.USAGE_DIFFUSE);
			addTexture(mat, specularTexFilename, ModelTexture.USAGE_SPECULAR);
			addTexture(mat, shininessTexFilename, ModelTexture.USAGE_SHININESS);

			return mat;
		}

		private void addTexture (ModelMaterial mat, String texFilename, int usage) {
			if (texFilename != null) {
				ModelTexture tex = new ModelTexture();
				tex.usage = usage;
				tex.fileName = texFilename;
				if (mat.textures == null) mat.textures = new Array<ModelTexture>(1);
				mat.textures.Add(tex);
			}
		}

		public void reset () {
			ambientColor = null;
			diffuseColor = Color.White;
			specularColor = Color.White;
			opacity = 1.0f;
			shininess = 0.0f;
			alphaTexFilename = null;
			ambientTexFilename = null;
			diffuseTexFilename = null;
			shininessTexFilename = null;
			specularTexFilename = null;
		}
	}
}
