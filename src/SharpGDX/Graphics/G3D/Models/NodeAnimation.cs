using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Models;

/** A NodeAnimation defines keyframes for a {@link Node} in a {@link Model}. The keyframes are given as a translation vector, a
 * rotation quaternion and a scale vector. Keyframes are interpolated linearly for now. Keytimes are given in seconds.
 * @author badlogic, Xoppa */
public class NodeAnimation {
	/** the Node affected by this animation **/
	public Node node;
	/** the translation keyframes if any (might be null), sorted by time ascending **/
	public Array<NodeKeyframe<Vector3>> translation = null;
	/** the rotation keyframes if any (might be null), sorted by time ascending **/
	public Array<NodeKeyframe<Quaternion>> rotation = null;
	/** the scaling keyframes if any (might be null), sorted by time ascending **/
	public Array<NodeKeyframe<Vector3>> scaling = null;
}
