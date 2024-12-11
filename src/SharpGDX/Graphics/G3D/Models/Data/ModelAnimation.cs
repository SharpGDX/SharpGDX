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

public class ModelAnimation {
	public String id;
	public Array<ModelNodeAnimation> nodeAnimations = new Array<ModelNodeAnimation>();
}
