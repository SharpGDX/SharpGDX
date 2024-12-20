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

public class BlendingAttribute : Attribute {
	public readonly static String Alias = "blended";
	public readonly static long Type = register(Alias);

	public static bool @is (long mask) {
		return (mask & Type) == mask;
	}

	/** Whether this material should be considered blended (default: true). This is used for sorting (back to front instead of
	 * front to back). */
	public bool blended;
	/** Specifies how the (incoming) red, green, blue, and alpha source blending factors are computed (default: GL_SRC_ALPHA) */
	public int sourceFunction;
	/** Specifies how the (existing) red, green, blue, and alpha destination blending factors are computed (default:
	 * GL_ONE_MINUS_SRC_ALPHA) */
	public int destFunction;
	/** The opacity used as source alpha value, ranging from 0 (fully transparent) to 1 (fully opaque), (default: 1). */
	public float opacity = 1.0f;

	public BlendingAttribute () 
    : this(null)
    {
		
	}

	public BlendingAttribute ( bool blended,  int sourceFunc,  int destFunc,  float opacity) 
    : base(Type)
    {
		
		this.blended = blended;
		this.sourceFunction = sourceFunc;
		this.destFunction = destFunc;
		this.opacity = opacity;
	}

	public BlendingAttribute ( int sourceFunc,  int destFunc,  float opacity) 
    : this(true, sourceFunc, destFunc, opacity)
    {
		
	}

	public BlendingAttribute ( int sourceFunc,  int destFunc) 
    : this(sourceFunc, destFunc, 1.0f)
    {
		
	}

	public BlendingAttribute ( bool blended,  float opacity) 
    : this(blended, IGL20.GL_SRC_ALPHA, IGL20.GL_ONE_MINUS_SRC_ALPHA, opacity)
    {
		
	}

	public BlendingAttribute ( float opacity) 
    : this(true, opacity)
    {
		
	}

	public BlendingAttribute ( BlendingAttribute? copyFrom) 
    : this(copyFrom == null || copyFrom.blended, copyFrom == null ? IGL20.GL_SRC_ALPHA : copyFrom.sourceFunction,
        copyFrom == null ? IGL20.GL_ONE_MINUS_SRC_ALPHA : copyFrom.destFunction, copyFrom == null ? 1.0f : copyFrom.opacity)
    {
		
	}

	public override BlendingAttribute copy () {
		return new BlendingAttribute(this);
	}

	public override int GetHashCode () {
		int result = base.GetHashCode();
		result = 947 * result + (blended ? 1 : 0);
		result = 947 * result + sourceFunction;
		result = 947 * result + destFunction;
		result = 947 * result + NumberUtils.floatToRawIntBits(opacity);
		return result;
	}

	public override int CompareTo (Attribute? o) {
		if (type != o.type) return (int)(type - o.type);
		BlendingAttribute other = (BlendingAttribute)o;
		if (blended != other.blended) return blended ? 1 : -1;
		if (sourceFunction != other.sourceFunction) return sourceFunction - other.sourceFunction;
		if (destFunction != other.destFunction) return destFunction - other.destFunction;
		return (MathUtils.isEqual(opacity, other.opacity)) ? 0 : (opacity < other.opacity ? 1 : -1);
	}
}
