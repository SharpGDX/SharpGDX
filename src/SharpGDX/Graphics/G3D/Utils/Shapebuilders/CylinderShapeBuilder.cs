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

/** Helper class with static methods to build cylinders shapes using {@link MeshPartBuilder}.
 * @author xoppa */
public class CylinderShapeBuilder : BaseShapeBuilder {
	/** Build a cylinder */
	public static void build (MeshPartBuilder builder, float width, float height, float depth, int divisions) {
		build(builder, width, height, depth, divisions, 0, 360);
	}

	/** Build a cylinder */
	public static void build (MeshPartBuilder builder, float width, float height, float depth, int divisions, float angleFrom,
		float angleTo) {
		build(builder, width, height, depth, divisions, angleFrom, angleTo, true);
	}

	/** Build a cylinder */
	public static void build (MeshPartBuilder builder, float width, float height, float depth, int divisions, float angleFrom,
		float angleTo, bool close) {
		// FIXME create better cylinder method (- axis on which to create the cylinder (matrix?))
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
		VertexInfo curr2 = vertTmp4.set(null, null, null, null);
		curr2.hasUV = curr2.hasPosition = curr2.hasNormal = true;
		short i1, i2, i3 = 0, i4 = 0;

		builder.ensureVertices(2 * (divisions + 1));
		builder.ensureRectangleIndices(divisions);
		for (int i = 0; i <= divisions; i++) {
			angle = ao + step * i;
			u = 1f - us * i;
			curr1.position.Set(MathUtils.cos(angle) * hw, 0f, MathUtils.sin(angle) * hd);
			curr1.normal.Set(curr1.position).nor();
			curr1.position.y = -hh;
			curr1.uv.Set(u, 1);
			curr2.position.Set(curr1.position);
			curr2.normal.Set(curr1.normal);
			curr2.position.y = hh;
			curr2.uv.Set(u, 0);
			i2 = builder.vertex(curr1);
			i1 = builder.vertex(curr2);
			if (i != 0) builder.rect(i3, i1, i2, i4); // FIXME don't duplicate lines and points
			i4 = i2;
			i3 = i1;
		}
		if (close) {
			EllipseShapeBuilder.build(builder, width, depth, 0, 0, divisions, 0, hh, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, angleFrom,
				angleTo);
			EllipseShapeBuilder.build(builder, width, depth, 0, 0, divisions, 0, -hh, 0, 0, -1, 0, -1, 0, 0, 0, 0, 1, 180f - angleTo,
				180f - angleFrom);
		}
	}
}
