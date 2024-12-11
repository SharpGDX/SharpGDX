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
using System.Numerics;

namespace SharpGDX.Graphics.G3D.Utils.ShapeBuilders;

/** Helper class with static methods to build sphere shapes using {@link MeshPartBuilder}.
 * @author xoppa */
public class SphereShapeBuilder : BaseShapeBuilder {
	private readonly static ShortArray tmpIndices = new ShortArray();
	private readonly static Matrix3 normalTransform = new Matrix3();

	public static void build (MeshPartBuilder builder, float width, float height, float depth, int divisionsU, int divisionsV) {
		build(builder, width, height, depth, divisionsU, divisionsV, 0, 360, 0, 180);
	}

	/** @deprecated use {@link MeshPartBuilder#setVertexTransform(Matrix4)} instead of using the method signature taking a
	 *             matrix. */
	[Obsolete]
	public static void build (MeshPartBuilder builder,  Matrix4 transform, float width, float height, float depth,
		int divisionsU, int divisionsV) {
		build(builder, transform, width, height, depth, divisionsU, divisionsV, 0, 360, 0, 180);
	}

	public static void build (MeshPartBuilder builder, float width, float height, float depth, int divisionsU, int divisionsV,
		float angleUFrom, float angleUTo, float angleVFrom, float angleVTo) {
		build(builder, matTmp1.idt(), width, height, depth, divisionsU, divisionsV, angleUFrom, angleUTo, angleVFrom, angleVTo);
	}

    /** @deprecated use {@link MeshPartBuilder#setVertexTransform(Matrix4)} instead of using the method signature taking a
	 *             matrix. */
    [Obsolete]
public static void build (MeshPartBuilder builder,  Matrix4 transform, float width, float height, float depth,
		int divisionsU, int divisionsV, float angleUFrom, float angleUTo, float angleVFrom, float angleVTo) {
		 bool closedVFrom = MathUtils.isEqual(angleVFrom, 0f);
		 bool closedVTo = MathUtils.isEqual(angleVTo, 180f);
		 float hw = width * 0.5f;
		 float hh = height * 0.5f;
		 float hd = depth * 0.5f;
		 float auo = MathUtils.degreesToRadians * angleUFrom;
		 float stepU = (MathUtils.degreesToRadians * (angleUTo - angleUFrom)) / divisionsU;
		 float avo = MathUtils.degreesToRadians * angleVFrom;
		 float stepV = (MathUtils.degreesToRadians * (angleVTo - angleVFrom)) / divisionsV;
		 float us = 1f / divisionsU;
		 float vs = 1f / divisionsV;
		float u = 0f;
		float v = 0f;
		float angleU = 0f;
		float angleV = 0f;
		MeshPartBuilder.VertexInfo curr1 = vertTmp3.set(null, null, null, null);
		curr1.hasUV = curr1.hasPosition = curr1.hasNormal = true;

		normalTransform.set(transform);

		 int s = divisionsU + 3;
		tmpIndices.clear();
		tmpIndices.ensureCapacity(divisionsU * 2);
		tmpIndices.size = s;
		int tempOffset = 0;

		builder.ensureVertices((divisionsV + 1) * (divisionsU + 1));
		builder.ensureRectangleIndices(divisionsU);
		for (int iv = 0; iv <= divisionsV; iv++) {
			angleV = avo + stepV * iv;
			v = vs * iv;
			 float t = MathUtils.sin(angleV);
			 float h = MathUtils.cos(angleV) * hh;
			for (int iu = 0; iu <= divisionsU; iu++) {
				angleU = auo + stepU * iu;
				if (iv == 0 && closedVFrom || iv == divisionsV && closedVTo) {
					u = 1f - us * (iu - .5f);
				} else {
					u = 1f - us * iu;
				}
				curr1.position.Set(MathUtils.cos(angleU) * hw * t, h, MathUtils.sin(angleU) * hd * t);
				curr1.normal.Set(curr1.position).mul(normalTransform).nor();
				curr1.position.mul(transform);
				curr1.uv.Set(u, v);
				tmpIndices.set(tempOffset, builder.vertex(curr1));
				 int o = tempOffset + s;
				if ((iv > 0) && (iu > 0)) { // FIXME don't duplicate lines and points
					if (iv == 1 && closedVFrom) {
						builder.triangle(tmpIndices.get(tempOffset), tmpIndices.get((o - 1) % s),
							tmpIndices.get((o - (divisionsU + 1)) % s));
					} else if (iv == divisionsV && closedVTo) {
						builder.triangle(tmpIndices.get(tempOffset), tmpIndices.get((o - (divisionsU + 2)) % s),
							tmpIndices.get((o - (divisionsU + 1)) % s));
					} else {
						builder.rect(tmpIndices.get(tempOffset), tmpIndices.get((o - 1) % s),
							tmpIndices.get((o - (divisionsU + 2)) % s), tmpIndices.get((o - (divisionsU + 1)) % s));
					}
				}
				tempOffset = (tempOffset + 1) % tmpIndices.size;
			}
		}
	}
}
