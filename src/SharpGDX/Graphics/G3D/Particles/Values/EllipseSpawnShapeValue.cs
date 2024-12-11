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

/** Encapsulate the formulas to spawn a particle on a ellipse shape.
 * @author Inferno */
public sealed class EllipseSpawnShapeValue : PrimitiveSpawnShapeValue {
	SpawnSide side = SpawnSide.both;

	public EllipseSpawnShapeValue (EllipseSpawnShapeValue value) 
    : base(value)
    {
		
		load(value);
	}

	public EllipseSpawnShapeValue () {
	}

	public override void spawnAux (Vector3 vector, float percent) {
		// Generate the point on the surface of the sphere
		float width = spawnWidth + spawnWidthDiff * spawnWidthValue.getScale(percent);
		float height = spawnHeight + spawnHeightDiff * spawnHeightValue.getScale(percent);
		float depth = spawnDepth + spawnDepthDiff * spawnDepthValue.getScale(percent);

		float radiusX, radiusY, radiusZ;
		// Where generate the point, on edges or inside ?
		float minT = 0, maxT = MathUtils.PI2;
		if (side == SpawnSide.top) {
			maxT = MathUtils.PI;
		} else if (side == SpawnSide.bottom) {
			maxT = -MathUtils.PI;
		}
		float t = MathUtils.random(minT, maxT);

		// Where generate the point, on edges or inside ?
		if (edges) {
			if (width == 0) {
				vector.Set(0, height / 2 * MathUtils.sin(t), depth / 2 * MathUtils.cos(t));
				return;
			}
			if (height == 0) {
				vector.Set(width / 2 * MathUtils.cos(t), 0, depth / 2 * MathUtils.sin(t));
				return;
			}
			if (depth == 0) {
				vector.Set(width / 2 * MathUtils.cos(t), height / 2 * MathUtils.sin(t), 0);
				return;
			}

			radiusX = width / 2;
			radiusY = height / 2;
			radiusZ = depth / 2;
		} else {
			radiusX = MathUtils.random(width / 2);
			radiusY = MathUtils.random(height / 2);
			radiusZ = MathUtils.random(depth / 2);
		}

		float z = MathUtils.random(-1, 1f);
		float r = (float)Math.Sqrt(1f - z * z);
		vector.Set(radiusX * r * MathUtils.cos(t), radiusY * r * MathUtils.sin(t), radiusZ * z);
	}

	public SpawnSide getSide () {
		return side;
	}

	public void setSide (SpawnSide side) {
		this.side = side;
	}

    public override void load (ParticleValue value) {
		base.load(value);
		EllipseSpawnShapeValue shape = (EllipseSpawnShapeValue)value;
		side = shape.side;
	}

    public override SpawnShapeValue copy () {
		return new EllipseSpawnShapeValue(this);
	}

    public override void write (Json json) {
		base.write(json);
		json.writeValue("side", side);
	}

    public override void read (Json json, JsonValue jsonData) {
		base.read(json, jsonData);
		side = (SpawnSide)json.readValue("side", typeof(SpawnSide), jsonData);
	}
}
