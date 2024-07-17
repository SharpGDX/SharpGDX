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
	/** A ScalingViewport that uses {@link Scaling#fit} so it keeps the aspect ratio by scaling the world up to fit the screen, adding
 * black bars (letterboxing) for the remaining space.
 * @author Daniel Holderbaum
 * @author Nathan Sweet */
public class FitViewport : ScalingViewport {
	/** Creates a new viewport using a new {@link OrthographicCamera}. */
	public FitViewport (float worldWidth, float worldHeight) 
	: base(Scaling.fit, worldWidth, worldHeight)
	{
		
	}

	public FitViewport (float worldWidth, float worldHeight, Camera camera) 
	: base(Scaling.fit, worldWidth, worldHeight, camera)
	{
		
	}
}
}
