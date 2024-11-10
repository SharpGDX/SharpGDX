using SharpGDX.Graphics;

namespace SharpGDX.Utils.Viewports;

/// <summary>
///     A viewport that scales the world using <see cref="Scaling" />.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="Scaling.fit" /> keeps the aspect ratio by scaling the world up to fit the screen, adding black bars
///         (letterboxing) for the remaining space.
///     </para>
///     <para>
///         <see cref="Scaling.fill" /> keeps the aspect ratio by scaling the world up to take the whole screen (some of
///         the world may be offscreen).
///     </para>
///     <para>
///         <see cref="Scaling.stretch" /> does not keep the aspect ratio, the world is scaled to take the whole screen.
///     </para>
///     <para>
///         <see cref="Scaling.none" /> keeps the aspect ratio by using a fixed size world (the world may not fill the
///         screen or some of the world may be offscreen).
///     </para>
/// </remarks>
public class ScalingViewport : Viewport
{
    private Scaling _scaling;

    /// <summary>
    ///     Creates a new viewport using a new <see cref="OrthographicCamera" />.
    /// </summary>
    /// <param name="scaling"></param>
    /// <param name="worldWidth"></param>
    /// <param name="worldHeight"></param>
    public ScalingViewport(Scaling scaling, float worldWidth, float worldHeight)
        : this(scaling, worldWidth, worldHeight, new OrthographicCamera())
    {
    }

    public ScalingViewport(Scaling scaling, float worldWidth, float worldHeight, Camera camera)
    {
        _scaling = scaling;
        SetWorldSize(worldWidth, worldHeight);
        SetCamera(camera);
    }

    public Scaling getScaling()
    {
        return _scaling;
    }

    public void setScaling(Scaling scaling)
    {
        _scaling = scaling;
    }

    public override void Update(int screenWidth, int screenHeight, bool centerCamera)
    {
        var scaled = _scaling.apply(GetWorldWidth(), GetWorldHeight(), screenWidth, screenHeight);
        var viewportWidth = (int)Math.Round(scaled.x);
        var viewportHeight = (int)Math.Round(scaled.y);

        // Center.
        SetScreenBounds((screenWidth - viewportWidth) / 2, (screenHeight - viewportHeight) / 2, viewportWidth,
            viewportHeight);

        Apply(centerCamera);
    }
}