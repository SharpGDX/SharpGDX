using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Models.Data;

/** Returned by a {@link ModelLoader}, contains meshes, materials, nodes and animations. OpenGL resources like textures or vertex
 * buffer objects are not stored. Instead, a ModelData instance needs to be converted to a Model first.
 * @author badlogic */
public class ModelData {
	public String id;
	public readonly short[] version = new short[2];
	public readonly Array<ModelMesh> meshes = new Array<ModelMesh>();
	public readonly Array<ModelMaterial> materials = new Array<ModelMaterial>();
	public readonly Array<ModelNode> nodes = new Array<ModelNode>();
	public readonly Array<ModelAnimation> animations = new Array<ModelAnimation>();

	public void addMesh (ModelMesh mesh) {
		foreach (ModelMesh other in meshes) {
			if (other.id.Equals(mesh.id)) {
				throw new GdxRuntimeException("Mesh with id '" + other.id + "' already in model");
			}
		}
		meshes.Add(mesh);
	}
}
