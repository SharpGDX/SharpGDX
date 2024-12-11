using SharpGDX.Utils;
using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
using SharpGDX.Graphics.G3D.Particles.Batches;

namespace SharpGDX.Graphics.G3D.Particles.Renderers;

/** A {@link ParticleControllerRenderer} which will render the {@link ParticleController} of each particle.
 * @author Inferno */
// TODO: @SuppressWarnings("rawtypes")
public class ParticleControllerControllerRenderer : ParticleControllerRenderer<ParticleControllerRenderData, ParticleBatch<ParticleControllerRenderData>> {
	ObjectChannel<ParticleController> controllerChannel;

	public override void init () {
		controllerChannel = controller.particles.getChannel<ObjectChannel<ParticleController>>(ParticleChannels.ParticleController);
		if (controllerChannel == null) throw new GdxRuntimeException(
			"ParticleController channel not found, specify an influencer which will allocate it please.");
	}

	public override void update () {
		for (int i = 0, c = controller.particles.size; i < c; ++i) {
			controllerChannel.data[i].draw();
		}
	}

	public override ParticleControllerComponent copy () {
		return new ParticleControllerControllerRenderer();
	}

	public override bool isCompatible (ParticleBatch<ParticleControllerRenderData> batch) {
		return false;
	}

}
