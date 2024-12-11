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

public class ModelMaterial {
	public enum MaterialType {
		Lambert, Phong
	}

	public String id;

	public MaterialType type;

	public Color ambient;
	public Color diffuse;
	public Color specular;
	public Color emissive;
	public Color reflection;

	public float shininess;
	public float opacity = 1.0f;

	public Array<ModelTexture> textures;
}
