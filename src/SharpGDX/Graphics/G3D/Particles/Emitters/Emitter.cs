using System;
using SharpGDX.Assets;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
using SharpGDX.Graphics.G3D.Attributess;
using SharpGDX.Graphics.G3D.Shaders;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.G3D.Utils;
using static SharpGDX.Graphics.VertexAttributes;

namespace SharpGDX.Graphics.G3D.Particles.Emitters;

/** An {@link Emitter} is a {@link ParticleControllerComponent} which will handle the particles emission. It must update the
 * {@link Emitter#percent} to reflect the current percentage of the current emission cycle. It should consider
 * {@link Emitter#minParticleCount} and {@link Emitter#maxParticleCount} to rule particle emission. It should notify the particle
 * controller when particles are activated, killed, or when an emission cycle begins.
 * @author Inferno */
public abstract class Emitter : ParticleControllerComponent , Json.Serializable {
	/** The min/max quantity of particles */
	public int minParticleCount, maxParticleCount = 4;

	/** Current state of the emission, should be currentTime/ duration Must be updated on each update */
	public float percent;

	public Emitter (Emitter regularEmitter) {
		set(regularEmitter);
	}

	public Emitter () {
	}

	public virtual void init () {
		controller.particles.size = 0;
	}

	public override void end () {
		controller.particles.size = 0;
	}

	public virtual bool isComplete () {
		return percent >= 1.0f;
	}

	public int getMinParticleCount () {
		return minParticleCount;
	}

	public void setMinParticleCount (int minParticleCount) {
		this.minParticleCount = minParticleCount;
	}

	public int getMaxParticleCount () {
		return maxParticleCount;
	}

	public void setMaxParticleCount (int maxParticleCount) {
		this.maxParticleCount = maxParticleCount;
	}

	public void setParticleCount (int aMin, int aMax) {
		setMinParticleCount(aMin);
		setMaxParticleCount(aMax);
	}

	public void set (Emitter emitter) {
		minParticleCount = emitter.minParticleCount;
		maxParticleCount = emitter.maxParticleCount;
	}

	public virtual void write (Json json) {
		json.writeValue("minParticleCount", minParticleCount);
		json.writeValue("maxParticleCount", maxParticleCount);
	}

	public virtual void read (Json json, JsonValue jsonData) {
		minParticleCount = (int)json.readValue("minParticleCount", typeof(int), jsonData);
		maxParticleCount = (int)json.readValue("maxParticleCount", typeof(int), jsonData);
	}

}
