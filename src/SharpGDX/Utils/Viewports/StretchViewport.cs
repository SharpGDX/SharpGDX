using SharpGDX.Graphics;

namespace SharpGDX.Utils.Viewports;

/// <summary>
///     A ScalingViewport that uses <see cref="Scaling.stretch" /> so it does not keep the aspect ratio, the world is
///     scaled to take the whole screen.
/// </summary>
public class StretchViewport : ScalingViewport
{
    /// <summary>
    ///     Creates a new viewport using a new <see cref="OrthographicCamera" />.
    /// </summary>
    /// <param name="worldWidth"></param>
    /// <param name="worldHeight"></param>
    public StretchViewport(float worldWidth, float worldHeight)
        : base(Scaling.stretch, worldWidth, worldHeight)
    {
    }

    public StretchViewport(float worldWidth, float worldHeight, Camera camera)
        : base(Scaling.stretch, worldWidth, worldHeight, camera)
    {
    }
}