using SharpGDX.Graphics;

namespace SharpGDX.Utils.Viewports;

/// <summary>
///     A ScalingViewport that uses <see cref="Scaling.fill" /> so it keeps the aspect ratio by scaling the world up to
///     take the whole screen.
/// </summary>
/// <remarks>
///     Some of the world may be offscreen.
/// </remarks>
public class FillViewport : ScalingViewport
{
    /// <summary>
    ///     Creates a new viewport using a new <see cref="OrthographicCamera" />.
    /// </summary>
    /// <param name="worldWidth"></param>
    /// <param name="worldHeight"></param>
    public FillViewport(float worldWidth, float worldHeight)
        : base(Scaling.fill, worldWidth, worldHeight)
    {
    }

    public FillViewport(float worldWidth, float worldHeight, Camera camera)
        : base(Scaling.fill, worldWidth, worldHeight, camera)
    {
    }
}