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

/** An Animation has an id and a list of {@link NodeAnimation} instances. Each NodeAnimation animates a single {@link Node} in the
 * {@link Model}. Every {@link NodeAnimation} is assumed to have the same amount of keyframes, at the same timestamps, as all
 * other node animations for faster keyframe searches.
 * 
 * @author badlogic */
public class Animation {
	/** the unique id of the animation **/
	public String id;
	/** the duration in seconds **/
	public float duration;
	/** the animation curves for individual nodes **/
	public Array<NodeAnimation> nodeAnimations = new Array<NodeAnimation>();
}
