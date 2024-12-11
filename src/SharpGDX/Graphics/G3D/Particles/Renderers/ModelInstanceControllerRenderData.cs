using static SharpGDX.Graphics.G3D.Particles.ParallelArray;
using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;

namespace SharpGDX.Graphics.G3D.Particles.Renderers;

/** Render data used by model instance particle batches
 * @author Inferno */
public class ModelInstanceControllerRenderData : ParticleControllerRenderData {
	public ObjectChannel<ModelInstance> modelInstanceChannel;
	public FloatChannel colorChannel, scaleChannel, rotationChannel;

}
