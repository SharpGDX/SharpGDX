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

public class ModelMesh {
	public String id;
	public VertexAttribute[] attributes;
	public float[] vertices;
	public ModelMeshPart[] parts;
}
