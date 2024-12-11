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

/** Helper class with static methods to build cone shapes using {@link MeshPartBuilder}.
 * @author xoppa */
public class ConeShapeBuilder : BaseShapeBuilder {
	public static void build (MeshPartBuilder builder, float width, float height, float depth, int divisions) {
		build(builder, width, height, depth, divisions, 0, 360);
	}

	public static void build (MeshPartBuilder builder, float width, float height, float depth, int divisions, float angleFrom,
		float angleTo) {
		build(builder, width, height, depth, divisions, angleFrom, angleTo, true);
	}

	public static void build (MeshPartBuilder builder, float width, float height, float depth, int divisions, float angleFrom,
		float angleTo, bool close) {
		// FIXME create better cylinder method (- axis on which to create the cone (matrix?))
		builder.ensureVertices(divisions + 2);
		builder.ensureTriangleIndices(divisions);

		 float hw = width * 0.5f;
		 float hh = height * 0.5f;
		 float hd = depth * 0.5f;
		 float ao = MathUtils.degreesToRadians * angleFrom;
		 float step = (MathUtils.degreesToRadians * (angleTo - angleFrom)) / divisions;
		 float us = 1f / divisions;
		float u = 0f;
		float angle = 0f;
		VertexInfo curr1 = vertTmp3.set(null, null, null, null);
		curr1.hasUV = curr1.hasPosition = curr1.hasNormal = true;
		VertexInfo curr2 = vertTmp4.set(null, null, null, null).setPos(0, hh, 0).setNor(0, 1, 0).setUV(0.5f, 0);
		 short @base = builder.vertex(curr2);
		short i1, i2 = 0;
		for (int i = 0; i <= divisions; i++) {
			angle = ao + step * i;
			u = 1f - us * i;
			curr1.position.Set(MathUtils.cos(angle) * hw, 0f, MathUtils.sin(angle) * hd);
			curr1.normal.Set(curr1.position).nor();
			curr1.position.y = -hh;
			curr1.uv.Set(u, 1);
			i1 = builder.vertex(curr1);
			if (i != 0) builder.triangle(@base, i1, i2); // FIXME don't duplicate lines and points
			i2 = i1;
		}
		if (close) EllipseShapeBuilder.build(builder, width, depth, 0, 0, divisions, 0, -hh, 0, 0, -1, 0, -1, 0, 0, 0, 0, 1,
			180f - angleTo, 180f - angleFrom);
	}
}
