using System;
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

/** Helper class with static methods to build arrow shapes using {@link MeshPartBuilder}.
 * @author xoppa */
public class ArrowShapeBuilder : BaseShapeBuilder {
	/** Build an arrow
	 * @param x1 source x
	 * @param y1 source y
	 * @param z1 source z
	 * @param x2 destination x
	 * @param y2 destination y
	 * @param z2 destination z
	 * @param capLength is the height of the cap in percentage, must be in (0,1)
	 * @param stemThickness is the percentage of stem diameter compared to cap diameter, must be in (0,1]
	 * @param divisions the amount of vertices used to generate the cap and stem ellipsoidal bases */
	public static void build (MeshPartBuilder builder, float x1, float y1, float z1, float x2, float y2, float z2, float capLength,
		float stemThickness, int divisions) {
		Vector3 begin = obtainV3().Set(x1, y1, z1), end = obtainV3().Set(x2, y2, z2);
		float length = begin.dst(end);
		float coneHeight = length * capLength;
		float coneDiameter = 2 * (float)(coneHeight * Math.Sqrt(1f / 3));
		float stemLength = length - coneHeight;
		float stemDiameter = coneDiameter * stemThickness;

		Vector3 up = obtainV3().Set(end).sub(begin).nor();
		Vector3 forward = obtainV3().Set(up).crs(Vector3.Z);
		if (forward.isZero()) forward.Set(Vector3.X);
		forward.crs(up).nor();
		Vector3 left = obtainV3().Set(up).crs(forward).nor();
		Vector3 direction = obtainV3().Set(end).sub(begin).nor();

		// Matrices
		Matrix4 userTransform = builder.getVertexTransform(obtainM4());
		Matrix4 transform = obtainM4();
		float[] val = transform.val;
		val[Matrix4.M00] = left.x;
		val[Matrix4.M01] = up.x;
		val[Matrix4.M02] = forward.x;
		val[Matrix4.M10] = left.y;
		val[Matrix4.M11] = up.y;
		val[Matrix4.M12] = forward.y;
		val[Matrix4.M20] = left.z;
		val[Matrix4.M21] = up.z;
		val[Matrix4.M22] = forward.z;
		Matrix4 temp = obtainM4();

		// Stem
		transform.setTranslation(obtainV3().Set(direction).scl(stemLength / 2).add(x1, y1, z1));
		builder.setVertexTransform(temp.set(transform).mul(userTransform));
		CylinderShapeBuilder.build(builder, stemDiameter, stemLength, stemDiameter, divisions);

		// Cap
		transform.setTranslation(obtainV3().Set(direction).scl(stemLength).add(x1, y1, z1));
		builder.setVertexTransform(temp.set(transform).mul(userTransform));
		ConeShapeBuilder.build(builder, coneDiameter, coneHeight, coneDiameter, divisions);

		builder.setVertexTransform(userTransform);
		freeAll();
	}
}
