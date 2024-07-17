using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using System;
using SharpGDX.Mathematics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Viewports
{
	/** A viewport that scales the world using {@link Scaling}.
 * <p>
 * {@link Scaling#fit} keeps the aspect ratio by scaling the world up to fit the screen, adding black bars (letterboxing) for the
 * remaining space.
 * <p>
 * {@link Scaling#fill} keeps the aspect ratio by scaling the world up to take the whole screen (some of the world may be off
 * screen).
 * <p>
 * {@link Scaling#stretch} does not keep the aspect ratio, the world is scaled to take the whole screen.
 * <p>
 * {@link Scaling#none} keeps the aspect ratio by using a fixed size world (the world may not fill the screen or some of the world
 * may be off screen).
 * @author Daniel Holderbaum
 * @author Nathan Sweet */
public class ScalingViewport : Viewport {
	private Scaling scaling;

	/** Creates a new viewport using a new {@link OrthographicCamera}. */
	public ScalingViewport (Scaling scaling, float worldWidth, float worldHeight) 
	: this(scaling, worldWidth, worldHeight, new OrthographicCamera())
	{
		
	}

	public ScalingViewport (Scaling scaling, float worldWidth, float worldHeight, Camera camera) {
		this.scaling = scaling;
		setWorldSize(worldWidth, worldHeight);
		setCamera(camera);
	}

	public override void update (int screenWidth, int screenHeight, bool centerCamera) {
		Vector2 scaled = scaling.apply(getWorldWidth(), getWorldHeight(), screenWidth, screenHeight);
		int viewportWidth = (int)Math.Round(scaled.x);
		int viewportHeight = (int)Math.Round(scaled.y);

		// Center.
		setScreenBounds((screenWidth - viewportWidth) / 2, (screenHeight - viewportHeight) / 2, viewportWidth, viewportHeight);

		apply(centerCamera);
	}

	public Scaling getScaling () {
		return scaling;
	}

	public void setScaling (Scaling scaling) {
		this.scaling = scaling;
	}
}
}
