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

public class ModelNodeAnimation {
	/** the id of the node animated by this animation FIXME should be nodeId **/
	public String nodeId;
	/** the keyframes, defining the translation of a node for a specific timestamp **/
	public Array<ModelNodeKeyframe<Vector3>> translation;
	/** the keyframes, defining the rotation of a node for a specific timestamp **/
	public Array<ModelNodeKeyframe<Quaternion>> rotation;
	/** the keyframes, defining the scaling of a node for a specific timestamp **/
	public Array<ModelNodeKeyframe<Vector3>> scaling;
}
