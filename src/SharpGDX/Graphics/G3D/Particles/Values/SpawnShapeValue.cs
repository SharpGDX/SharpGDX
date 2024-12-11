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

/** Encapsulate the formulas to spawn a particle on a shape.
 * @author Inferno */
public abstract class SpawnShapeValue : ParticleValue , ResourceData<ParticleEffect>.Configurable, Json.Serializable {

	public RangedNumericValue xOffsetValue, yOffsetValue, zOffsetValue;

	public SpawnShapeValue () {
		xOffsetValue = new RangedNumericValue();
		yOffsetValue = new RangedNumericValue();
		zOffsetValue = new RangedNumericValue();
	}

	public SpawnShapeValue (SpawnShapeValue spawnShapeValue) 
    : this()
    {
		
	}

	public abstract void spawnAux (Vector3 vector, float percent);

	public Vector3 spawn (Vector3 vector, float percent) {
		spawnAux(vector, percent);
		if (xOffsetValue.active) vector.x += xOffsetValue.newLowValue();
		if (yOffsetValue.active) vector.y += yOffsetValue.newLowValue();
		if (zOffsetValue.active) vector.z += zOffsetValue.newLowValue();
		return vector;
	}

	public virtual void init () {
	}

	public virtual void start () {
	}

	public override void load (ParticleValue value) {
		base.load(value);
		SpawnShapeValue shape = (SpawnShapeValue)value;
		xOffsetValue.load(shape.xOffsetValue);
		yOffsetValue.load(shape.yOffsetValue);
		zOffsetValue.load(shape.zOffsetValue);
	}

	public abstract SpawnShapeValue copy ();

    public override void write (Json json) {
		base.write(json);
		json.writeValue("xOffsetValue", xOffsetValue);
		json.writeValue("yOffsetValue", yOffsetValue);
		json.writeValue("zOffsetValue", zOffsetValue);
	}

    public override void read (Json json, JsonValue jsonData) {
		base.read(json, jsonData);
		xOffsetValue = (RangedNumericValue)json.readValue("xOffsetValue", typeof(RangedNumericValue), jsonData);
		yOffsetValue = (RangedNumericValue)json.readValue("yOffsetValue", typeof(RangedNumericValue), jsonData);
		zOffsetValue = (RangedNumericValue)json.readValue("zOffsetValue", typeof(RangedNumericValue), jsonData);
	}

    public virtual void save (AssetManager manager, ResourceData<ParticleEffect> data) {
	}

    public virtual void load (AssetManager manager, ResourceData<ParticleEffect> data) {
	}

}
