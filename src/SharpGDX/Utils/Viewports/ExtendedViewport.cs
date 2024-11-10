using SharpGDX.Graphics;
using SharpGDX.Mathematics;

namespace SharpGDX.Utils.Viewports
{
	/** A viewport that keeps the world aspect ratio by both scaling and extending the world. By default, the world is first scaled to
 * fit within the viewport using {@link Scaling#fit}, then the shorter dimension is lengthened to fill the viewport. Other
 * scaling, such as {@link Scaling#contain}, may lengthen the world in both directions. A maximum size can be specified to limit
 * how much the world is extended and black bars (letterboxing) are used for any remaining space.
 * @author Nathan Sweet */
public class ExtendViewport : Viewport {
	private float _minWorldWidth, _minWorldHeight;
	private float _maxWorldWidth, _maxWorldHeight;
	private Scaling _scaling = Scaling.fit;

        /// <summary>
        /// Creates a new viewport using a new <see cref="OrthographicCamera"/> with no maximum world size.
        /// </summary>
        /// <param name="minWorldWidth"></param>
        /// <param name="minWorldHeight"></param>
        public ExtendViewport (float minWorldWidth, float minWorldHeight) 
	: this(minWorldWidth, minWorldHeight, 0, 0, new OrthographicCamera())
	{
		
	}

        /// <summary>
        /// Creates a new viewport with no maximum world size.
        /// </summary>
        /// <param name="minWorldWidth"></param>
        /// <param name="minWorldHeight"></param>
        /// <param name="camera"></param>
        public ExtendViewport (float minWorldWidth, float minWorldHeight, Camera camera) 
	: this(minWorldWidth, minWorldHeight, 0, 0, camera)
	{
		
	}

        /// <summary>
        /// Creates a new viewport using a new <see cref="OrthographicCamera"/> and a maximum world size.
        /// </summary>
        /// <remarks>
        ///<see cref="ExtendViewport(float, float, float, float, Camera)"/>
        /// </remarks>
        /// <param name="minWorldWidth"></param>
        /// <param name="minWorldHeight"></param>
        /// <param name="maxWorldWidth"></param>
        /// <param name="maxWorldHeight"></param>
        public ExtendViewport (float minWorldWidth, float minWorldHeight, float maxWorldWidth, float maxWorldHeight) 
	: this(minWorldWidth, minWorldHeight, maxWorldWidth, maxWorldHeight, new OrthographicCamera())
	{
		
	}

        /// <summary>
        /// Creates a new viewport with a maximum world size.
        /// </summary>
        /// <param name="minWorldWidth"></param>
        /// <param name="minWorldHeight"></param>
        /// <param name="maxWorldWidth">User 0 for no maximum width.</param>
        /// <param name="maxWorldHeight">User 0 for no maximum height.</param>
        /// <param name="camera"></param>
        public ExtendViewport (float minWorldWidth, float minWorldHeight, float maxWorldWidth, float maxWorldHeight, Camera camera) {
		this._minWorldWidth = minWorldWidth;
		this._minWorldHeight = minWorldHeight;
		this._maxWorldWidth = maxWorldWidth;
		this._maxWorldHeight = maxWorldHeight;
		SetCamera(camera);
	}

		public override void Update (int screenWidth, int screenHeight, bool centerCamera) {
		// Fit min size to the screen.
		float worldWidth = _minWorldWidth;
		float worldHeight = _minWorldHeight;
		Vector2 scaled = _scaling.apply(worldWidth, worldHeight, screenWidth, screenHeight);

		// Extend, possibly in both directions depending on the scaling.
		int viewportWidth = (int)Math.Round(scaled.x);
		int viewportHeight = (int)Math.Round(scaled.y);
		if (viewportWidth < screenWidth) {
			float toViewportSpace = viewportHeight / worldHeight;
			float toWorldSpace = worldHeight / viewportHeight;
			float lengthen = (screenWidth - viewportWidth) * toWorldSpace;
			if (_maxWorldWidth > 0) lengthen = Math.Min(lengthen, _maxWorldWidth - _minWorldWidth);
			worldWidth += lengthen;
			viewportWidth += (int)Math.Round(lengthen * toViewportSpace);
		}
		if (viewportHeight < screenHeight) {
			float toViewportSpace = viewportWidth / worldWidth;
			float toWorldSpace = worldWidth / viewportWidth;
			float lengthen = (screenHeight - viewportHeight) * toWorldSpace;
			if (_maxWorldHeight > 0) lengthen = Math.Min(lengthen, _maxWorldHeight - _minWorldHeight);
			worldHeight += lengthen;
			viewportHeight += (int)Math.Round(lengthen * toViewportSpace);
		}

		SetWorldSize(worldWidth, worldHeight);

		// Center.
		SetScreenBounds((screenWidth - viewportWidth) / 2, (screenHeight - viewportHeight) / 2, viewportWidth, viewportHeight);

		Apply(centerCamera);
	}

	public float GetMinWorldWidth () {
		return _minWorldWidth;
	}

	public void SetMinWorldWidth (float minWorldWidth) {
		this._minWorldWidth = minWorldWidth;
	}

	public float GetMinWorldHeight () {
		return _minWorldHeight;
	}

	public void SetMinWorldHeight (float minWorldHeight) {
		this._minWorldHeight = minWorldHeight;
	}

	public float GetMaxWorldWidth () {
		return _maxWorldWidth;
	}

	public void SetMaxWorldWidth (float maxWorldWidth) {
		this._maxWorldWidth = maxWorldWidth;
	}

	public float GetMaxWorldHeight () {
		return _maxWorldHeight;
	}

	public void SetMaxWorldHeight (float maxWorldHeight) {
		this._maxWorldHeight = maxWorldHeight;
	}

	public void SetScaling (Scaling scaling) {
		this._scaling = scaling;
	}
}
}
