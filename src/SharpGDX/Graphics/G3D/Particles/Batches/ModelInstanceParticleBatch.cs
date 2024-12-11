using SharpGDX.Assets;
using SharpGDX.Utils;
using SharpGDX.Graphics.G3D.Particles.Renderers;

namespace SharpGDX.Graphics.G3D.Particles.Batches;

/*** This class is used to render particles having a model instance channel.
 * @author Inferno */
public class ModelInstanceParticleBatch : ParticleBatch<ModelInstanceControllerRenderData> {
	Array<ModelInstanceControllerRenderData> controllersRenderData;
	int bufferedParticlesCount;

	public ModelInstanceParticleBatch () {
		controllersRenderData = new Array<ModelInstanceControllerRenderData>(false, 5);
	}

    public void getRenderables (Array<Renderable> renderables, Pool<Renderable> pool) {
		foreach (ModelInstanceControllerRenderData data in controllersRenderData) {
			for (int i = 0, count = data.controller.particles.size; i < count; ++i) {
				data.modelInstanceChannel.data[i].getRenderables(renderables, pool);
			}
		}
	}

	public int getBufferedCount () {
		return bufferedParticlesCount;
	}

    public void begin () {
		controllersRenderData.clear();
		bufferedParticlesCount = 0;
	}

    public void end () {
	}

    public void draw (ModelInstanceControllerRenderData data) {
		controllersRenderData.Add(data);
		bufferedParticlesCount += data.controller.particles.size;
	}

    public virtual void save (AssetManager manager, ResourceData<ParticleEffect> assetDependencyData) {
	}

    public virtual void load (AssetManager manager, ResourceData<ParticleEffect> assetDependencyData) {
	}
}
