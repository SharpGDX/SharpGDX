using System;
using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Mathematics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Viewports
{
	/** A viewport that keeps the world aspect ratio by both scaling and extending the world. By default, the world is first scaled to
 * fit within the viewport using {@link Scaling#fit}, then the shorter dimension is lengthened to fill the viewport. Other
 * scaling, such as {@link Scaling#contain}, may lengthen the world in both directions. A maximum size can be specified to limit
 * how much the world is extended and black bars (letterboxing) are used for any remaining space.
 * @author Nathan Sweet */
public class ExtendViewport : Viewport {
	private float minWorldWidth, minWorldHeight;
	private float maxWorldWidth, maxWorldHeight;
	private Scaling scaling = Scaling.fit;

	/** Creates a new viewport using a new {@link OrthographicCamera} with no maximum world size. */

	public ExtendViewport (float minWorldWidth, float minWorldHeight) 
	: this(minWorldWidth, minWorldHeight, 0, 0, new OrthographicCamera())
	{
		
	}

	/** Creates a new viewport with no maximum world size. */
	public ExtendViewport (float minWorldWidth, float minWorldHeight, Camera camera) 
	: this(minWorldWidth, minWorldHeight, 0, 0, camera)
	{
		
	}

	/** Creates a new viewport using a new {@link OrthographicCamera} and a maximum world size.
	 * @see ExtendViewport#ExtendViewport(float, float, float, float, Camera) */
	public ExtendViewport (float minWorldWidth, float minWorldHeight, float maxWorldWidth, float maxWorldHeight) 
	: this(minWorldWidth, minWorldHeight, maxWorldWidth, maxWorldHeight, new OrthographicCamera())
	{
		
	}

	/** Creates a new viewport with a maximum world size.
	 * @param maxWorldWidth User 0 for no maximum width.
	 * @param maxWorldHeight User 0 for no maximum height. */
	public ExtendViewport (float minWorldWidth, float minWorldHeight, float maxWorldWidth, float maxWorldHeight, Camera camera) {
		this.minWorldWidth = minWorldWidth;
		this.minWorldHeight = minWorldHeight;
		this.maxWorldWidth = maxWorldWidth;
		this.maxWorldHeight = maxWorldHeight;
		setCamera(camera);
	}

		public override void update (int screenWidth, int screenHeight, bool centerCamera) {
		// Fit min size to the screen.
		float worldWidth = minWorldWidth;
		float worldHeight = minWorldHeight;
		Vector2 scaled = scaling.apply(worldWidth, worldHeight, screenWidth, screenHeight);

		// Extend, possibly in both directions depending on the scaling.
		int viewportWidth = (int)Math.Round(scaled.x);
		int viewportHeight = (int)Math.Round(scaled.y);
		if (viewportWidth < screenWidth) {
			float toViewportSpace = viewportHeight / worldHeight;
			float toWorldSpace = worldHeight / viewportHeight;
			float lengthen = (screenWidth - viewportWidth) * toWorldSpace;
			if (maxWorldWidth > 0) lengthen = Math.Min(lengthen, maxWorldWidth - minWorldWidth);
			worldWidth += lengthen;
			viewportWidth += (int)Math.Round(lengthen * toViewportSpace);
		}
		if (viewportHeight < screenHeight) {
			float toViewportSpace = viewportWidth / worldWidth;
			float toWorldSpace = worldWidth / viewportWidth;
			float lengthen = (screenHeight - viewportHeight) * toWorldSpace;
			if (maxWorldHeight > 0) lengthen = Math.Min(lengthen, maxWorldHeight - minWorldHeight);
			worldHeight += lengthen;
			viewportHeight += (int)Math.Round(lengthen * toViewportSpace);
		}

		setWorldSize(worldWidth, worldHeight);

		// Center.
		setScreenBounds((screenWidth - viewportWidth) / 2, (screenHeight - viewportHeight) / 2, viewportWidth, viewportHeight);

		apply(centerCamera);
	}

	public float getMinWorldWidth () {
		return minWorldWidth;
	}

	public void setMinWorldWidth (float minWorldWidth) {
		this.minWorldWidth = minWorldWidth;
	}

	public float getMinWorldHeight () {
		return minWorldHeight;
	}

	public void setMinWorldHeight (float minWorldHeight) {
		this.minWorldHeight = minWorldHeight;
	}

	public float getMaxWorldWidth () {
		return maxWorldWidth;
	}

	public void setMaxWorldWidth (float maxWorldWidth) {
		this.maxWorldWidth = maxWorldWidth;
	}

	public float getMaxWorldHeight () {
		return maxWorldHeight;
	}

	public void setMaxWorldHeight (float maxWorldHeight) {
		this.maxWorldHeight = maxWorldHeight;
	}

	public void setScaling (Scaling scaling) {
		this.scaling = scaling;
	}
}
}
