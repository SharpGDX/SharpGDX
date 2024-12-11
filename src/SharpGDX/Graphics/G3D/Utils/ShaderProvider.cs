using System;
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
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Utils;

/** Returns {@link Shader} instances for a {@link Renderable} on request. Also responsible for disposing of any created
 * {@link Shader} instances on a call to {@link Disposable#dispose()}.
 * @author badlogic */
public interface ShaderProvider : IDisposable {
	/** Returns a {@link Shader} for the given {@link Renderable}. The RenderInstance may already contain a Shader, in which case
	 * the provider may decide to return that.
	 * @param renderable the Renderable
	 * @return the Shader to be used for the RenderInstance */
	Shader getShader (Renderable renderable);

}
