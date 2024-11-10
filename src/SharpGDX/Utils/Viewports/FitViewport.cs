﻿using SharpGDX.Graphics;

namespace SharpGDX.Utils.Viewports;

/// <summary>
///     A ScalingViewport that uses <see cref="Scaling.fit" /> so it keeps the aspect ratio by scaling the world up to fit
///     the screen, adding black bars (letterboxing) for the remaining space.
/// </summary>
public class FitViewport : ScalingViewport
{
    /// <summary>
    ///     Creates a new viewport using a new <see cref="OrthographicCamera" />.
    /// </summary>
    /// <param name="worldWidth"></param>
    /// <param name="worldHeight"></param>
    public FitViewport(float worldWidth, float worldHeight)
        : base(Scaling.fit, worldWidth, worldHeight)
    {
    }

    public FitViewport(float worldWidth, float worldHeight, Camera camera)
        : base(Scaling.fit, worldWidth, worldHeight, camera)
    {
    }
}