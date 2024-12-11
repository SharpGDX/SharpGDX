using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D;

/** Returns a list of {@link Renderable} instances to be rendered by a {@link ModelBatch}.
 * @author badlogic */
public interface RenderableProvider {
	/** Returns {@link Renderable} instances. Renderables are obtained from the provided {@link Pool} and added to the provided
	 * array. The Renderables obtained using {@link Pool#obtain()} will later be put back into the pool, do not store them
	 * internally. The resulting array can be rendered via a {@link ModelBatch}.
	 * @param renderables the output array
	 * @param pool the pool to obtain Renderables from */
	public void getRenderables (Array<Renderable> renderables, Pool<Renderable> pool);
}
