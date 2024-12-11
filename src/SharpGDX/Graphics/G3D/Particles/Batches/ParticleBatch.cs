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
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.G3D.Utils;
using SharpGDX.Graphics.G3D.Particles.Renderers;

namespace SharpGDX.Graphics.G3D.Particles.Batches;

/** Common interface to all the batches that render particles.
 * @author Inferno */
public interface ParticleBatch<T> : RenderableProvider, ResourceData<ParticleEffect>.Configurable 
where T: ParticleControllerRenderData
{

	/** Must be called once before any drawing operation */
	public void begin ();

	public void draw (T controller);

	/** Must be called after all the drawing operations */
	public void end ();

	public void save (AssetManager manager, ResourceData<ParticleEffect> assetDependencyData);

	public void load (AssetManager manager, ResourceData<ParticleEffect> assetDependencyData);
}
