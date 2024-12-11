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

/** Helper class with static methods to build capsule shapes using {@link MeshPartBuilder}.
 * @author xoppa */
public class CapsuleShapeBuilder : BaseShapeBuilder {
	public static void build (MeshPartBuilder builder, float radius, float height, int divisions) {
		if (height < 2f * radius) throw new GdxRuntimeException("Height must be at least twice the radius");
		float d = 2f * radius;
		CylinderShapeBuilder.build(builder, d, height - d, d, divisions, 0, 360, false);
		SphereShapeBuilder.build(builder, matTmp1.setToTranslation(0, .5f * (height - d), 0), d, d, d, divisions, divisions, 0, 360,
			0, 90);
		SphereShapeBuilder.build(builder, matTmp1.setToTranslation(0, -.5f * (height - d), 0), d, d, d, divisions, divisions, 0,
			360, 90, 180);
	}
}
