using System;
using SharpGDX.Assets;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
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
using SharpGDX.Graphics.G3D.Particles.Batches;
using SharpGDX.Graphics.G3D.Particles.Emitters;
using SharpGDX.Graphics.G3D.Particles.Influencers;
using SharpGDX.Graphics.G3D.Particles.Renderers;

namespace SharpGDX.Graphics.G3D.Particles;

/** Base class of all the particle controllers. Encapsulate the generic structure of a controller and methods to update the
 * particles simulation.
 * @author Inferno */
public class ParticleController : Json.Serializable, ResourceData<ParticleEffect>.Configurable {

	/** the default time step used to update the simulation */
	protected static readonly float DEFAULT_TIME_STEP = 1f / 60;

	/** Name of the controller */
	public String name;

	/** Controls the emission of the particles */
	public Emitter emitter;

	/** Update the properties of the particles */
	public Array<Influencer> influencers;

	/** Controls the graphical representation of the particles */
	public ParticleControllerRenderer<ParticleControllerRenderData, ParticleBatch<ParticleControllerRenderData>> renderer;

	/** Particles components */
	public ParallelArray particles;
	public ParticleChannels particleChannels;

	/** Current transform of the controller DO NOT CHANGE MANUALLY */
	public Matrix4 transform;

	/** Transform flags */
	public Vector3 _scale;

	/** Not used by the simulation, it should represent the bounding box containing all the particles */
	protected BoundingBox boundingBox;

	/** Time step, DO NOT CHANGE MANUALLY */
	public float deltaTime, deltaTimeSqr;

	public ParticleController () {
		transform = new Matrix4();
		_scale = new Vector3(1, 1, 1);
		influencers = new Array<Influencer>(true, 3, typeof(Influencer));
		setTimeStep(DEFAULT_TIME_STEP);
	}

	public ParticleController (String name, Emitter emitter, ParticleControllerRenderer<ParticleControllerRenderData, ParticleBatch<ParticleControllerRenderData>> renderer,
		Influencer[] influencers) 
        : this()
        {
		
		this.name = name;
		this.emitter = emitter;
		this.renderer = renderer;
		this.particleChannels = new ParticleChannels();
		this.influencers = new Array<Influencer>(influencers);
	}

	/** Sets the delta used to step the simulation */
	private void setTimeStep (float timeStep) {
		deltaTime = timeStep;
		deltaTimeSqr = deltaTime * deltaTime;
	}

	/** Sets the current transformation to the given one.
	 * @param transform the new transform matrix */
	public void setTransform (Matrix4 transform) {
		this.transform.set(transform);
		transform.getScale(_scale);
	}

	/** Sets the current transformation. */
	public void setTransform (float x, float y, float z, float qx, float qy, float qz, float qw, float scale) {
		transform.set(x, y, z, qx, qy, qz, qw, scale, scale, scale);
		this._scale.Set(scale, scale, scale);
	}

	/** Post-multiplies the current transformation with a rotation matrix represented by the given quaternion. */
	public void rotate (Quaternion rotation) {
		this.transform.rotate(rotation);
	}

	/** Post-multiplies the current transformation with a rotation matrix by the given angle around the given axis.
	 * @param axis the rotation axis
	 * @param angle the rotation angle in degrees */
	public void rotate (Vector3 axis, float angle) {
		this.transform.rotate(axis, angle);
	}

	/** Postmultiplies the current transformation with a translation matrix represented by the given translation. */
	public void translate (Vector3 translation) {
		this.transform.translate(translation);
	}

	public void setTranslation (Vector3 translation) {
		this.transform.setTranslation(translation);
	}

	/** Postmultiplies the current transformation with a scale matrix represented by the given scale on x,y,z. */
	public void scale (float scaleX, float scaleY, float scaleZ) {
		this.transform.scale(scaleX, scaleY, scaleZ);
		this.transform.getScale(_scale);
	}

	/** Postmultiplies the current transformation with a scale matrix represented by the given scale vector. */
	public void scale (Vector3 scale) {
		this.scale(scale.x, scale.y, scale.z);
	}

	/** Postmultiplies the current transformation with the given matrix. */
	public void mul (Matrix4 transform) {
		this.transform.mul(transform);
		this.transform.getScale(_scale);
	}

	/** Set the given matrix to the current transformation matrix. */
	public void getTransform (Matrix4 transform) {
		transform.set(this.transform);
	}

	public bool isComplete () {
		return emitter.isComplete();
	}

	/** Initialize the controller. All the sub systems will be initialized and binded to the controller. Must be called before any
	 * other method. */
	public void init () {
		bind();
		if (particles != null) {
			end();
			particleChannels.resetIds();
		}
		allocateChannels(emitter.maxParticleCount);

		emitter.init();
		foreach (Influencer influencer in influencers)
			influencer.init();
		renderer.init();
	}

	protected void allocateChannels (int maxParticleCount) {
		particles = new ParallelArray(maxParticleCount);
		// Alloc additional channels
		emitter.allocateChannels();
		foreach (Influencer influencer in influencers)
			influencer.allocateChannels();
		renderer.allocateChannels();
	}

	/** Bind the sub systems to the controller Called once during the init phase. */
	protected void bind () {
		emitter.set(this);
		foreach (Influencer influencer in influencers)
			influencer.set(this);
		renderer.set(this);
	}

	/** Start the simulation. */
	public void start () {
		emitter.start();
		foreach (Influencer influencer in influencers)
			influencer.start();
	}

	/** Reset the simulation. */
	public void reset () {
		end();
		start();
	}

	/** End the simulation. */
	public void end () {
		foreach (Influencer influencer in influencers)
			influencer.end();
		emitter.end();
	}

	/** Generally called by the Emitter. This method will notify all the sub systems that a given amount of particles has been
	 * activated. */
	public void activateParticles (int startIndex, int count) {
		emitter.activateParticles(startIndex, count);
		foreach (Influencer influencer in influencers)
			influencer.activateParticles(startIndex, count);
	}

	/** Generally called by the Emitter. This method will notify all the sub systems that a given amount of particles has been
	 * killed. */
	public void killParticles (int startIndex, int count) {
		emitter.killParticles(startIndex, count);
		foreach (Influencer influencer in influencers)
			influencer.killParticles(startIndex, count);
	}

	/** Updates the particles data */
	public void update () {
		update(GDX.Graphics.GetDeltaTime());
	}

	/** Updates the particles data */
	public void update (float deltaTime) {
		setTimeStep(deltaTime);
		emitter.update();
		foreach (Influencer influencer in influencers)
			influencer.update();
	}

	/** Updates the renderer used by this controller, usually this means the particles will be draw inside a batch. */
	public void draw () {
		if (particles.size > 0) {
			renderer.update();
		}
	}

	/** @return a copy of this controller */
	public ParticleController copy () {
		Emitter emitter = (Emitter)this.emitter.copy();
		Influencer[] influencers = new Influencer[this.influencers.size];
		int i = 0;
		foreach (Influencer influencer in this.influencers) {
			influencers[i++] = (Influencer)influencer.copy();
		}
		return new ParticleController(new String(this.name), emitter, (ParticleControllerRenderer<ParticleControllerRenderData, ParticleBatch<ParticleControllerRenderData>>)renderer.copy(),
			influencers);
	}

	public void Dispose () {
		emitter.Dispose();
		foreach (Influencer influencer in influencers)
			influencer.Dispose();
	}

	/** @return a copy of this controller, should be used after the particle effect has been loaded. */
	public BoundingBox getBoundingBox () {
		if (boundingBox == null) boundingBox = new BoundingBox();
		calculateBoundingBox();
		return boundingBox;
	}

	/** Updates the bounding box using the position channel. */
	protected void calculateBoundingBox () {
		boundingBox.clr();
		FloatChannel positionChannel = particles.getChannel<FloatChannel>(ParticleChannels.Position);
		for (int pos = 0, c = positionChannel.strideSize * particles.size; pos < c; pos += positionChannel.strideSize) {
			boundingBox.ext(positionChannel.data[pos + ParticleChannels.XOffset],
				positionChannel.data[pos + ParticleChannels.YOffset], positionChannel.data[pos + ParticleChannels.ZOffset]);
		}
	}

	/** @return the index of the Influencer of the given type. */
	private int findIndex<K> (Type type)
	where K: Influencer{
		for (int i = 0; i < influencers.size; ++i) {
			Influencer influencer = influencers.Get(i);
			if (ClassReflection.isAssignableFrom(type, influencer.GetType())) {
				return i;
			}
		}
		return -1;
	}

	/** @return the influencer having the given type. */
	public K findInfluencer<K>(Type influencerClass)
        where K : Influencer
    {
		int index = findIndex<K>(influencerClass);
		return index > -1 ? (K)influencers.Get(index) : null;
	}

	/** Removes the Influencer of the given type. */
	public void removeInfluencer<K> (Type type)
        where K : Influencer
    {
		int index = findIndex<K>(type);
		if (index > -1) influencers.RemoveIndex(index);
	}

	/** Replaces the Influencer of the given type with the one passed as parameter. */
	public bool replaceInfluencer <K>(Type type, K newInfluencer)
	where K:Influencer{
		int index = findIndex<K>(type);
		if (index > -1) {
			influencers.insert(index, newInfluencer);
			influencers.RemoveIndex(index + 1);
			return true;
		}
		return false;
	}

	public void write (Json json) {
		json.writeValue("name", name);
		json.writeValue("emitter", emitter, typeof(Emitter));
		json.writeValue("influencers", influencers, typeof(Array), typeof(Influencer));
		json.writeValue("renderer", renderer, typeof(ParticleControllerRenderer<ParticleControllerRenderData, ParticleBatch<ParticleControllerRenderData>>));
	}

    public void read (Json json, JsonValue jsonMap) {
		name = (string)json.readValue("name", typeof(string), jsonMap);
		emitter = (Emitter)json.readValue("emitter", typeof(Emitter), jsonMap);
		influencers.addAll((Array<Influencer>)json.readValue("influencers", typeof(Array), typeof(Influencer), jsonMap));
		renderer = (ParticleControllerRenderer<ParticleControllerRenderData, ParticleBatch<ParticleControllerRenderData>>)json.readValue("renderer", typeof(ParticleControllerRenderer<ParticleControllerRenderData, ParticleBatch<ParticleControllerRenderData>>), jsonMap);
	}

    public void save (AssetManager manager, ResourceData<ParticleEffect> data) {
		emitter.save(manager, data);
		foreach (Influencer influencer in influencers)
			influencer.save(manager, data);
		renderer.save(manager, data);
	}

    public void load (AssetManager manager, ResourceData<ParticleEffect> data) {
		emitter.load(manager, data);
		foreach (Influencer influencer in influencers)
			influencer.load(manager, data);
		renderer.load(manager, data);
	}
}
