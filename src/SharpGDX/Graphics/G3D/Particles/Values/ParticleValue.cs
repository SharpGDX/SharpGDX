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

/** It's a class which represents a value bound to the particles. Generally used by a particle controller component to find the
 * current value of a particle property during the simulation.
 * @author Inferno */
public class ParticleValue : Json.Serializable {
	public bool active;

	public ParticleValue () {
	}

	public ParticleValue (ParticleValue value) {
		this.active = value.active;
	}

	public bool isActive () {
		return active;
	}

	public virtual void setActive (bool active) {
		this.active = active;
	}

	public virtual  void load (ParticleValue value) {
		active = value.active;
	}

    public virtual void write (Json json) {
		json.writeValue("active", active);
	}

    public virtual void read (Json json, JsonValue jsonData) {
		active = (bool)json.readValue("active", typeof(bool), jsonData);
	}
}
