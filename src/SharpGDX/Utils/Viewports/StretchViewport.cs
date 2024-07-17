using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;

namespace SharpGDX.Utils.Viewports;

/**
 * A ScalingViewport that uses {@link Scaling#stretch} so it does not keep the aspect ratio, the world is scaled to take the
 * whole screen.
 * @author Daniel Holderbaum
 * @author Nathan Sweet
 */
public class StretchViewport : ScalingViewport
{
	/**
	 * Creates a new viewport using a new {@link OrthographicCamera}.
	 */
	public StretchViewport(float worldWidth, float worldHeight)
		: base(Scaling.stretch, worldWidth, worldHeight)
	{
	}

	public StretchViewport(float worldWidth, float worldHeight, Camera camera)
		: base(Scaling.stretch, worldWidth, worldHeight, camera)
	{
	}
}