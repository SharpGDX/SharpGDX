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

/** A value which contains a single float variable.
 * @author Inferno */
public class NumericValue : ParticleValue {
	private float value;

	public float getValue () {
		return value;
	}

	public void setValue (float value) {
		this.value = value;
	}

	public void load (NumericValue value) {
		base.load(value);
		this.value = value.value;
	}

	public override void write (Json json) {
		base.write(json);
		json.writeValue("value", value);
	}

    public override void read (Json json, JsonValue jsonData) {
		base.read(json, jsonData);
		value = (float)json.readValue("value", typeof(float), jsonData);
	}

}
