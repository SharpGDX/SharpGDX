using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using static SharpGDX.Utils.Timer;
using static SharpGDX.Graphics.G3D.Utils.MeshPartBuilder;

namespace SharpGDX.Graphics.G3D.Utils.ShapeBuilders;

/** This class allows to reduce the static allocation needed for shape builders. It contains all the objects used internally by
 * shape builders.
 * @author realitix, xoppa */
public class BaseShapeBuilder {
	/* Color */
	protected static readonly Color tmpColor0 = new Color();
	protected static readonly Color tmpColor1 = new Color();
	protected static readonly Color tmpColor2 = new Color();
	protected static readonly Color tmpColor3 = new Color();
	protected static readonly Color tmpColor4 = new Color();

	/* Vector3 */
	protected static readonly Vector3 tmpV0 = new Vector3();
	protected static readonly Vector3 tmpV1 = new Vector3();
	protected static readonly Vector3 tmpV2 = new Vector3();
	protected static readonly Vector3 tmpV3 = new Vector3();
	protected static readonly Vector3 tmpV4 = new Vector3();
	protected static readonly Vector3 tmpV5 = new Vector3();
	protected static readonly Vector3 tmpV6 = new Vector3();
	protected static readonly Vector3 tmpV7 = new Vector3();

	/* VertexInfo */
	protected static readonly VertexInfo vertTmp0 = new VertexInfo();
	protected static readonly VertexInfo vertTmp1 = new VertexInfo();
	protected static readonly VertexInfo vertTmp2 = new VertexInfo();
	protected static readonly VertexInfo vertTmp3 = new VertexInfo();
	protected static readonly VertexInfo vertTmp4 = new VertexInfo();
	protected static readonly VertexInfo vertTmp5 = new VertexInfo();
	protected static readonly VertexInfo vertTmp6 = new VertexInfo();
	protected static readonly VertexInfo vertTmp7 = new VertexInfo();
	protected static readonly VertexInfo vertTmp8 = new VertexInfo();

	/* Matrix4 */
	protected static readonly Matrix4 matTmp1 = new Matrix4();

    private class VectorPool : FlushablePool<Vector3>
    {
        protected internal override Vector3 newObject()
        {
            return new Vector3();
        }
    }

	private readonly static FlushablePool<Vector3> vectorPool = new VectorPool();

    private class MatrixPool : FlushablePool<Matrix4>
    {
        protected internal override Matrix4 newObject()
        {
            return new Matrix4();
        }
    }

	private readonly static FlushablePool<Matrix4> matrices4Pool = new MatrixPool();

	/** Obtain a temporary {@link Vector3} object, must be free'd using {@link #freeAll()}. */
	protected static Vector3 obtainV3 () {
		return vectorPool.obtain();
	}

	/** Obtain a temporary {@link Matrix4} object, must be free'd using {@link #freeAll()}. */
	protected static Matrix4 obtainM4 () {
		Matrix4 result = matrices4Pool.obtain();
		return result;
	}

	/** Free all objects obtained using one of the `obtainXX` methods. */
	protected static void freeAll () {
		vectorPool.flush();
		matrices4Pool.flush();
	}
}
