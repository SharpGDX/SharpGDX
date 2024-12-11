using System;
using static SharpGDX.Graphics.G3D.Utils.MeshPartBuilder;
using SharpGDX.Graphics.GLUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D.Utils.ShapeBuilders;

/** Helper class with static methods to build patch shapes using {@link MeshPartBuilder}.
 * @author xoppa */
public class PatchShapeBuilder : BaseShapeBuilder {
	/** Build a patch shape. Requires GL_POINTS, GL_LINES or GL_TRIANGLES primitive type. */
	public static void build (MeshPartBuilder builder, VertexInfo corner00, VertexInfo corner10, VertexInfo corner11,
		VertexInfo corner01, int divisionsU, int divisionsV) {
		if (divisionsU < 1 || divisionsV < 1) {
			throw new GdxRuntimeException("divisionsU and divisionV must be > 0, u,v: " + divisionsU + ", " + divisionsV);
		}
		builder.ensureVertices((divisionsV + 1) * (divisionsU + 1));
		builder.ensureRectangleIndices(divisionsV * divisionsU);
		for (int u = 0; u <= divisionsU; u++) {
			 float alphaU = (float)u / (float)divisionsU;
			vertTmp5.set(corner00).lerp(corner10, alphaU);
			vertTmp6.set(corner01).lerp(corner11, alphaU);
			for (int v = 0; v <= divisionsV; v++) {
				 short idx = builder.vertex(vertTmp7.set(vertTmp5).lerp(vertTmp6, (float)v / (float)divisionsV));
				if (u > 0 && v > 0) builder.rect((short)(idx - divisionsV - 2), (short)(idx - 1), idx, (short)(idx - divisionsV - 1));
			}
		}
	}

	/** Build a patch shape. Requires GL_POINTS, GL_LINES or GL_TRIANGLES primitive type. */
	public static void build (MeshPartBuilder builder, Vector3 corner00, Vector3 corner10, Vector3 corner11, Vector3 corner01,
		Vector3 normal, int divisionsU, int divisionsV) {
		build(builder, vertTmp1.set(corner00, normal, null, null).setUV(0f, 1f),
			vertTmp2.set(corner10, normal, null, null).setUV(1f, 1f), vertTmp3.set(corner11, normal, null, null).setUV(1f, 0f),
			vertTmp4.set(corner01, normal, null, null).setUV(0f, 0f), divisionsU, divisionsV);
	}

	/** Build a patch shape. Requires GL_POINTS, GL_LINES or GL_TRIANGLES primitive type. */
	public static void build (MeshPartBuilder builder, float x00, float y00, float z00, float x10, float y10, float z10, float x11,
		float y11, float z11, float x01, float y01, float z01, float normalX, float normalY, float normalZ, int divisionsU,
		int divisionsV) {
		build(builder, vertTmp1.set(null).setPos(x00, y00, z00).setNor(normalX, normalY, normalZ).setUV(0f, 1f),
			vertTmp2.set(null).setPos(x10, y10, z10).setNor(normalX, normalY, normalZ).setUV(1f, 1f),
			vertTmp3.set(null).setPos(x11, y11, z11).setNor(normalX, normalY, normalZ).setUV(1f, 0f),
			vertTmp4.set(null).setPos(x01, y01, z01).setNor(normalX, normalY, normalZ).setUV(0f, 0f), divisionsU, divisionsV);
	}
}
