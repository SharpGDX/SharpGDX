using System;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Attributess;

public class DepthTestAttribute : Attribute {
	public readonly static String Alias = "depthStencil";
	public readonly static long Type = register(Alias);

	protected static long Mask = Type;

	public static bool @is ( long mask) {
		return (mask & Mask) != 0;
	}

	/** The depth test function, or 0 to disable depth test (default: GL10.GL_LEQUAL) */
	public int depthFunc;
	/** Mapping of near clipping plane to window coordinates (default: 0) */
	public float depthRangeNear;
	/** Mapping of far clipping plane to window coordinates (default: 1) */
	public float depthRangeFar;
	/** Whether to write to the depth buffer (default: true) */
	public bool depthMask;

	public DepthTestAttribute () 
    : this(IGL20.GL_LEQUAL)
    {
		
	}

	public DepthTestAttribute (bool depthMask) 
    : this(IGL20.GL_LEQUAL, depthMask)
    {
		
	}

	public DepthTestAttribute (int depthFunc) 
    : this(depthFunc, true)
    {
		
	}

	public DepthTestAttribute (int depthFunc, bool depthMask) 
    : this(depthFunc, 0, 1, depthMask)
    {
		
	}

	public DepthTestAttribute (int depthFunc, float depthRangeNear, float depthRangeFar) 
    : this(depthFunc, depthRangeNear, depthRangeFar, true)
    {
		
	}

	public DepthTestAttribute (int depthFunc, float depthRangeNear, float depthRangeFar, bool depthMask) 
    : this(Type, depthFunc, depthRangeNear, depthRangeFar, depthMask)
    {
		
	}

	public DepthTestAttribute (long type, int depthFunc, float depthRangeNear, float depthRangeFar, bool depthMask) 
    : base(type)
    {
		
		if (!@is(type)) throw new GdxRuntimeException("Invalid type specified");
		this.depthFunc = depthFunc;
		this.depthRangeNear = depthRangeNear;
		this.depthRangeFar = depthRangeFar;
		this.depthMask = depthMask;
	}

	public DepthTestAttribute (DepthTestAttribute rhs) 
    : this(rhs.type, rhs.depthFunc, rhs.depthRangeNear, rhs.depthRangeFar, rhs.depthMask)
    {
		
	}

	public override Attribute copy () {
		return new DepthTestAttribute(this);
	}

	public override int GetHashCode () {
		int result = base.GetHashCode();
		result = 971 * result + depthFunc;
		result = 971 * result + NumberUtils.floatToRawIntBits(depthRangeNear);
		result = 971 * result + NumberUtils.floatToRawIntBits(depthRangeFar);
		result = 971 * result + (depthMask ? 1 : 0);
		return result;
	}

	public override int CompareTo (Attribute o) {
		if (type != o.type) return (int)(type - o.type);
		DepthTestAttribute other = (DepthTestAttribute)o;
		if (depthFunc != other.depthFunc) return depthFunc - other.depthFunc;
		if (depthMask != other.depthMask) return depthMask ? -1 : 1;
		if (!MathUtils.isEqual(depthRangeNear, other.depthRangeNear)) return depthRangeNear < other.depthRangeNear ? -1 : 1;
		if (!MathUtils.isEqual(depthRangeFar, other.depthRangeFar)) return depthRangeFar < other.depthRangeFar ? -1 : 1;
		return 0;
	}
}
