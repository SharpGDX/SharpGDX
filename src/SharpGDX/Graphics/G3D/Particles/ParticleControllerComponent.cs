using System;
using SharpGDX.Assets;
using SharpGDX.Utils.Reflect;
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

namespace SharpGDX.Graphics.G3D.Particles;

/** It's the base class of every {@link ParticleController} component. A component duty is to participate in one or some events
 * during the simulation. (i.e it can handle the particles emission or modify particle properties, etc.).
 * @author inferno */
public abstract class ParticleControllerComponent : IDisposable, Json.Serializable, ResourceData<ParticleEffect>.Configurable {
	protected static readonly Vector3 TMP_V1 = new Vector3(), TMP_V2 = new Vector3(), TMP_V3 = new Vector3(), TMP_V4 = new Vector3(),
		TMP_V5 = new Vector3(), TMP_V6 = new Vector3();
	protected static readonly Quaternion TMP_Q = new Quaternion(), TMP_Q2 = new Quaternion();
	protected static readonly Matrix3 TMP_M3 = new Matrix3();
	protected static readonly Matrix4 TMP_M4 = new Matrix4();
	protected ParticleController controller;

    /** Called to initialize new emitted particles. */
    public virtual void activateParticles (int startIndex, int count) {
	}

    /** Called to notify which particles have been killed. */
    public virtual void killParticles (int startIndex, int count) {
	}

	/** Called to execute the component behavior. */
	public virtual void update () {
	}

    /** Called once during intialization */
    public virtual void init () {
	}

    /** Called at the start of the simulation. */
    public virtual void start()
    {
    }

    /** Called at the end of the simulation. */
	public virtual void end () {
	}

	public virtual void Dispose () {
	}

	public abstract ParticleControllerComponent copy ();

	/** Called during initialization to allocate additional particles channels */
	public virtual void allocateChannels () {
	}

	public virtual void set (ParticleController particleController) {
		controller = particleController;
	}

	public virtual void save (AssetManager manager, ResourceData<ParticleEffect> data) {
	}

	public virtual void load (AssetManager manager, ResourceData<ParticleEffect> data) {
	}

	public virtual void write (Json json) {
	}

	public virtual void read (Json json, JsonValue jsonData) {
	}

}
