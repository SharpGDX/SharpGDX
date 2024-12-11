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

/** Encapsulate the formulas to spawn a particle on a line shape.
 * @author Inferno */
public sealed class LineSpawnShapeValue : PrimitiveSpawnShapeValue {

	public LineSpawnShapeValue (LineSpawnShapeValue value) 
    : base(value)
    {
		
		load(value);
	}

	public LineSpawnShapeValue () {
	}

	public override void spawnAux (Vector3 vector, float percent) {
		float width = spawnWidth + (spawnWidthDiff * spawnWidthValue.getScale(percent));
		float height = spawnHeight + (spawnHeightDiff * spawnHeightValue.getScale(percent));
		float depth = spawnDepth + (spawnDepthDiff * spawnDepthValue.getScale(percent));

		float a = MathUtils.random();
		vector.x = a * width;
		vector.y = a * height;
		vector.z = a * depth;
	}

    public override SpawnShapeValue copy () {
		return new LineSpawnShapeValue(this);
	}
}
