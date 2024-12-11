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

/** A NodeKeyframe specifies the a value (e.g. the translation, rotation or scale) of a frame within a {@link NodeAnimation}.
 * @author badlogic, Xoppa */
public class NodeKeyframe<T> {
	/** the timestamp of this keyframe **/
	public float keytime;
	/** the value of this keyframe at the specified timestamp **/
	public readonly T value;

	public NodeKeyframe (float t, T v) {
		keytime = t;
		value = v;
	}
}
