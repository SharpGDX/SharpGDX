using System;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Assets;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Graphics.G3D.Particles.Renderers;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Particles.Batches;

/** Base class of all the batches requiring to buffer {@link ParticleControllerRenderData}
 * @author Inferno */
public abstract class BufferedParticleBatch<T> : ParticleBatch<T>
where T: ParticleControllerRenderData{
    protected Array<T> renderData;
	protected int bufferedParticlesCount, currentCapacity = 0;
	protected ParticleSorter sorter;
	protected Camera camera;

	protected BufferedParticleBatch (Type type) {
		this.sorter = new ParticleSorter.Distance();
		renderData = new Array<T>(false, 10, type);
	}

	public virtual void begin () {
		renderData.clear();
		bufferedParticlesCount = 0;
	}

	public void draw (T data) {
		if (data.controller.particles.size > 0) {
			renderData.Add(data);
			bufferedParticlesCount += data.controller.particles.size;
		}
	}

	/** */
	public void end () {
		if (bufferedParticlesCount > 0) {
			ensureCapacity(bufferedParticlesCount);
			flush(sorter.sort<T>(renderData));
		}
	}
    
    /** Ensure the batch can contain the passed in amount of particles */
	public void ensureCapacity (int capacity) {
		if (currentCapacity >= capacity) return;
		sorter.ensureCapacity(capacity);
		allocParticlesData(capacity);
		currentCapacity = capacity;
	}

	public void resetCapacity () {
		currentCapacity = bufferedParticlesCount = 0;
	}

	protected abstract void allocParticlesData (int capacity);

	public void setCamera (Camera camera) {
		this.camera = camera;
		sorter.setCamera(camera);
	}

	public ParticleSorter getSorter () {
		return sorter;
	}

	public void setSorter (ParticleSorter sorter) {
		this.sorter = sorter;
		sorter.setCamera(camera);
		sorter.ensureCapacity(currentCapacity);
	}

	/** Sends the data to the gpu. This method must use the calculated offsets to build the particles meshes. The offsets represent
	 * the position at which a particle should be placed into the vertex array.
	 * @param offsets the calculated offsets */
	protected abstract void flush (int[] offsets);

	public int getBufferedCount () {
		return bufferedParticlesCount;
	}

    public abstract void getRenderables(Array<Renderable> renderables, Pool<Renderable> pool);

    public abstract void load(AssetManager manager, ResourceData<ParticleEffect> resources);

    public abstract void save(AssetManager manager, ResourceData<ParticleEffect> assetDependencyData);
}
