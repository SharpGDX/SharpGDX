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

/** The base class of all the {@link SpawnShapeValue} values which spawn the particles on a geometric primitive.
 * @author Inferno */
public abstract class PrimitiveSpawnShapeValue : SpawnShapeValue {
	protected static readonly Vector3 TMP_V1 = new Vector3();

	public enum SpawnSide {
		both, top, bottom
	}

	public ScaledNumericValue spawnWidthValue, spawnHeightValue, spawnDepthValue;
	protected float spawnWidth, spawnWidthDiff;
	protected float spawnHeight, spawnHeightDiff;
	protected float spawnDepth, spawnDepthDiff;
	protected bool edges = false;

	public PrimitiveSpawnShapeValue () {
		spawnWidthValue = new ScaledNumericValue();
		spawnHeightValue = new ScaledNumericValue();
		spawnDepthValue = new ScaledNumericValue();
	}

	public PrimitiveSpawnShapeValue (PrimitiveSpawnShapeValue value) 
    : base(value)
    {
		
		spawnWidthValue = new ScaledNumericValue();
		spawnHeightValue = new ScaledNumericValue();
		spawnDepthValue = new ScaledNumericValue();
	}

	public override void setActive (bool active) {
		base.setActive(active);
		spawnWidthValue.setActive(true);
		spawnHeightValue.setActive(true);
		spawnDepthValue.setActive(true);
	}

	public bool isEdges () {
		return edges;
	}

	public void setEdges (bool edges) {
		this.edges = edges;
	}

	public ScaledNumericValue getSpawnWidth () {
		return spawnWidthValue;
	}

	public ScaledNumericValue getSpawnHeight () {
		return spawnHeightValue;
	}

	public ScaledNumericValue getSpawnDepth () {
		return spawnDepthValue;
	}

	public void setDimensions (float width, float height, float depth) {
		spawnWidthValue.setHigh(width);
		spawnHeightValue.setHigh(height);
		spawnDepthValue.setHigh(depth);
	}

    public override void start () {
		spawnWidth = spawnWidthValue.newLowValue();
		spawnWidthDiff = spawnWidthValue.newHighValue();
		if (!spawnWidthValue.isRelative()) spawnWidthDiff -= spawnWidth;

		spawnHeight = spawnHeightValue.newLowValue();
		spawnHeightDiff = spawnHeightValue.newHighValue();
		if (!spawnHeightValue.isRelative()) spawnHeightDiff -= spawnHeight;

		spawnDepth = spawnDepthValue.newLowValue();
		spawnDepthDiff = spawnDepthValue.newHighValue();
		if (!spawnDepthValue.isRelative()) spawnDepthDiff -= spawnDepth;
	}

    public override void load (ParticleValue value) {
		base.load(value);
		PrimitiveSpawnShapeValue shape = (PrimitiveSpawnShapeValue)value;
		edges = shape.edges;
		spawnWidthValue.load(shape.spawnWidthValue);
		spawnHeightValue.load(shape.spawnHeightValue);
		spawnDepthValue.load(shape.spawnDepthValue);
	}

    public override void write (Json json) {
		base.write(json);
		json.writeValue("spawnWidthValue", spawnWidthValue);
		json.writeValue("spawnHeightValue", spawnHeightValue);
		json.writeValue("spawnDepthValue", spawnDepthValue);
		json.writeValue("edges", edges);
	}

    public override void read (Json json, JsonValue jsonData) {
		base.read(json, jsonData);
		spawnWidthValue = (ScaledNumericValue)json.readValue("spawnWidthValue", typeof(ScaledNumericValue), jsonData);
		spawnHeightValue = (ScaledNumericValue)json.readValue("spawnHeightValue", typeof(ScaledNumericValue), jsonData);
		spawnDepthValue = (ScaledNumericValue)json.readValue("spawnDepthValue", typeof(ScaledNumericValue), jsonData);
		edges = (bool)json.readValue("edges", typeof(bool), jsonData);
	}

}
