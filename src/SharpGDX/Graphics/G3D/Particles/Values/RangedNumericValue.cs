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

/** A value which has a defined minimum and maximum bounds.
 * @author Inferno */
public class RangedNumericValue : ParticleValue {
	private float lowMin, lowMax;

	public float newLowValue () {
		return lowMin + (lowMax - lowMin) * MathUtils.random();
	}

	public void setLow (float value) {
		lowMin = value;
		lowMax = value;
	}

	public void setLow (float min, float max) {
		lowMin = min;
		lowMax = max;
	}

	public float getLowMin () {
		return lowMin;
	}

	public void setLowMin (float lowMin) {
		this.lowMin = lowMin;
	}

	public float getLowMax () {
		return lowMax;
	}

	public void setLowMax (float lowMax) {
		this.lowMax = lowMax;
	}

	public void load (RangedNumericValue value) {
		base.load(value);
		lowMax = value.lowMax;
		lowMin = value.lowMin;
	}

    public override void write (Json json) {
		base.write(json);
		json.writeValue("lowMin", lowMin);
		json.writeValue("lowMax", lowMax);
	}

    public override void read (Json json, JsonValue jsonData) {
		base.read(json, jsonData);
		lowMin = (float)json.readValue("lowMin", typeof(float), jsonData);
		lowMax = (float)json.readValue("lowMax", typeof(float), jsonData);
	}

}
