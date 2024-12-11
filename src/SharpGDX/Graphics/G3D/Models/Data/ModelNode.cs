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

public class ModelNode {
	public String id;
	public Vector3 translation;
	public Quaternion rotation;
	public Vector3 scale;
	public String meshId;
	public ModelNodePart[] parts;
	public ModelNode[] children;
}
