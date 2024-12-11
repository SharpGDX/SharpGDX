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

/** Encapsulate the formulas to spawn a particle on a point shape.
 * @author Inferno */
public sealed class PointSpawnShapeValue : PrimitiveSpawnShapeValue {

	public PointSpawnShapeValue (PointSpawnShapeValue value) 
    : base(value)
    {
		
		load(value);
	}

	public PointSpawnShapeValue () {
	}

	public override void spawnAux (Vector3 vector, float percent) {
		vector.x = spawnWidth + (spawnWidthDiff * spawnWidthValue.getScale(percent));
		vector.y = spawnHeight + (spawnHeightDiff * spawnHeightValue.getScale(percent));
		vector.z = spawnDepth + (spawnDepthDiff * spawnDepthValue.getScale(percent));
	}

    public override SpawnShapeValue copy () {
		return new PointSpawnShapeValue(this);
	}
}
