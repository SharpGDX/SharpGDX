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

/** Encapsulate the formulas to spawn a particle on a rectangle shape.
 * @author Inferno */
public sealed class RectangleSpawnShapeValue : PrimitiveSpawnShapeValue {
	public RectangleSpawnShapeValue (RectangleSpawnShapeValue value) 
    : base(value)
    {
		
		load(value);
	}

	public RectangleSpawnShapeValue () {
	}

	public override void spawnAux (Vector3 vector, float percent) {
		float width = spawnWidth + (spawnWidthDiff * spawnWidthValue.getScale(percent));
		float height = spawnHeight + (spawnHeightDiff * spawnHeightValue.getScale(percent));
		float depth = spawnDepth + (spawnDepthDiff * spawnDepthValue.getScale(percent));
		// Where generate the point, on edges or inside ?
		if (edges) {
			int a = MathUtils.random(-1, 1);
			float tx = 0, ty = 0, tz = 0;
			if (a == -1) {
				tx = MathUtils.random(1) == 0 ? -width / 2 : width / 2;
				if (tx == 0) {
					ty = MathUtils.random(1) == 0 ? -height / 2 : height / 2;
					tz = MathUtils.random(1) == 0 ? -depth / 2 : depth / 2;
				} else {
					ty = MathUtils.random(height) - height / 2;
					tz = MathUtils.random(depth) - depth / 2;
				}
			} else if (a == 0) {
				// Z
				tz = MathUtils.random(1) == 0 ? -depth / 2 : depth / 2;
				if (tz == 0) {
					ty = MathUtils.random(1) == 0 ? -height / 2 : height / 2;
					tx = MathUtils.random(1) == 0 ? -width / 2 : width / 2;
				} else {
					ty = MathUtils.random(height) - height / 2;
					tx = MathUtils.random(width) - width / 2;
				}
			} else {
				// Y
				ty = MathUtils.random(1) == 0 ? -height / 2 : height / 2;
				if (ty == 0) {
					tx = MathUtils.random(1) == 0 ? -width / 2 : width / 2;
					tz = MathUtils.random(1) == 0 ? -depth / 2 : depth / 2;
				} else {
					tx = MathUtils.random(width) - width / 2;
					tz = MathUtils.random(depth) - depth / 2;
				}
			}
			vector.x = tx;
			vector.y = ty;
			vector.z = tz;
		} else {
			vector.x = MathUtils.random(width) - width / 2;
			vector.y = MathUtils.random(height) - height / 2;
			vector.z = MathUtils.random(depth) - depth / 2;
		}
	}

    public override SpawnShapeValue copy () {
		return new RectangleSpawnShapeValue(this);
	}
}
