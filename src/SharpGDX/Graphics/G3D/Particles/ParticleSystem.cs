using System;
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
using SharpGDX.Graphics.G3D.Particles.Renderers;

namespace SharpGDX.Graphics.G3D.Particles;

/** Singleton class which manages the particle effects. It's a utility class to ease particle batches management and particle
 * effects update.
 * @author inferno */
public sealed class ParticleSystem : RenderableProvider {
	private static ParticleSystem? instance;

	/** @deprecated Please directly use the constructor */
	[Obsolete]
	public static ParticleSystem get () {
		if (instance == null) instance = new ParticleSystem();
		return instance;
	}

	private Array<ParticleBatch<ParticleControllerRenderData>> batches;
	private Array<ParticleEffect> effects;

	public ParticleSystem () {
		batches = new Array<ParticleBatch<ParticleControllerRenderData>>();
		effects = new Array<ParticleEffect>();
	}

	public void add (ParticleBatch<ParticleControllerRenderData> batch) {
		batches.Add(batch);
	}

	public void add (ParticleEffect effect) {
		effects.Add(effect);
	}

	public void remove (ParticleEffect effect) {
		effects.RemoveValue(effect, true);
	}

	/** Removes all the effects added to the system */
	public void removeAll () {
		effects.clear();
	}

	/** Updates the simulation of all effects */
	public void update () {
		foreach (ParticleEffect effect in effects) {
			effect.update();
		}
	}

	public void updateAndDraw () {
		foreach (ParticleEffect effect in effects) {
			effect.update();
			effect.draw();
		}
	}

	public void update (float deltaTime) {
		foreach (ParticleEffect effect in effects) {
			effect.update(deltaTime);
		}
	}

	public void updateAndDraw (float deltaTime) {
		foreach (ParticleEffect effect in effects) {
			effect.update(deltaTime);
			effect.draw();
		}
	}

	/** Must be called one time per frame before any particle effect drawing operation will occur. */
	public void begin () {
		foreach (var batch in batches)
			batch.begin();
	}

	/** Draws all the particle effects. Call {@link #begin()} before this method and {@link #end()} after. */
	public void draw () {
		foreach (ParticleEffect effect in effects) {
			effect.draw();
		}
	}

	/** Must be called one time per frame at the end of all drawing operations. */
	public void end () {
		foreach (var batch in batches)
			batch.end();
	}

	public void getRenderables (Array<Renderable> renderables, Pool<Renderable> pool) {
		foreach (var batch in batches)
			batch.getRenderables(renderables, pool);
	}

	public Array<ParticleBatch<ParticleControllerRenderData>> getBatches () {
		return batches;
	}
}
