using System;
using SharpGDX.Assets;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;
using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;

namespace SharpGDX.Graphics.G3D.Particles.Values;

/** Encapsulate the formulas to spawn a particle on a cylinder shape.
 * @author Inferno */
public sealed class CylinderSpawnShapeValue : PrimitiveSpawnShapeValue {

	public CylinderSpawnShapeValue (CylinderSpawnShapeValue cylinderSpawnShapeValue) 
    : base(cylinderSpawnShapeValue)
    {
		
		load(cylinderSpawnShapeValue);
	}

	public CylinderSpawnShapeValue () {
	}

	public override void spawnAux (Vector3 vector, float percent) {
		// Generate the point on the surface of the sphere
		float width = spawnWidth + (spawnWidthDiff * spawnWidthValue.getScale(percent));
		float height = spawnHeight + (spawnHeightDiff * spawnHeightValue.getScale(percent));
		float depth = spawnDepth + (spawnDepthDiff * spawnDepthValue.getScale(percent));

		float radiusX, radiusZ;
		float hf = height / 2;
		float ty = MathUtils.random(height) - hf;

		// Where generate the point, on edges or inside ?
		if (edges) {
			radiusX = width / 2;
			radiusZ = depth / 2;
		} else {
			radiusX = MathUtils.random(width) / 2;
			radiusZ = MathUtils.random(depth) / 2;
		}

		float spawnTheta = 0;

		// Generate theta
		bool isRadiusXZero = radiusX == 0, isRadiusZZero = radiusZ == 0;
		if (!isRadiusXZero && !isRadiusZZero)
			spawnTheta = MathUtils.random(360f);
		else {
			if (isRadiusXZero)
				spawnTheta = MathUtils.random(1) == 0 ? -90 : 90;
			else if (isRadiusZZero) spawnTheta = MathUtils.random(1) == 0 ? 0 : 180;
		}

		vector.Set(radiusX * MathUtils.cosDeg(spawnTheta), ty, radiusZ * MathUtils.sinDeg(spawnTheta));
	}

    public override SpawnShapeValue copy () {
		return new CylinderSpawnShapeValue(this);
	}

}
