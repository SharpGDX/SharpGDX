using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Viewports
{
	/** A ScalingViewport that uses {@link Scaling#fill} so it keeps the aspect ratio by scaling the world up to take the whole screen
 * (some of the world may be off screen).
 * @author Daniel Holderbaum
 * @author Nathan Sweet */
	public class FillViewport : ScalingViewport
	{
	/** Creates a new viewport using a new {@link OrthographicCamera}. */
	public FillViewport(float worldWidth, float worldHeight)
	: base(Scaling.fill, worldWidth, worldHeight)
		{
		
	}

	public FillViewport(float worldWidth, float worldHeight, Camera camera)
	: base(Scaling.fill, worldWidth, worldHeight, camera)
		{
		
	}
	}
}
