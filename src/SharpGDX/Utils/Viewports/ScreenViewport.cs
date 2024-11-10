using SharpGDX.Graphics;

namespace SharpGDX.Utils.Viewports;

/// <summary>
///     A viewport where the world size is based on the size of the screen.
/// </summary>
/// <remarks>
///     By default, 1 world unit == 1 screen pixel, but this ratio can be changed by calling
///     <see cref="SetUnitsPerPixel(float)" />.
/// </remarks>
public class ScreenViewport : Viewport
{
    private float _unitsPerPixel = 1;

    /// <summary>
    ///     Creates a new viewport using a new <see cref="OrthographicCamera" />.
    /// </summary>
    public ScreenViewport()
        : this(new OrthographicCamera())
    {
    }

    public ScreenViewport(Camera camera)
    {
        SetCamera(camera);
    }

    public float GetUnitsPerPixel()
    {
        return _unitsPerPixel;
    }

    /// <summary>
    ///     Sets the number of pixels for each world unit.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Eg, a scale of 2.5 means there are 2.5 world units for every 1 screen pixel.
    ///     </para>
    ///     <para>
    ///         Default is 1.
    ///     </para>
    /// </remarks>
    /// <param name="unitsPerPixel"></param>
    public void SetUnitsPerPixel(float unitsPerPixel)
    {
        _unitsPerPixel = unitsPerPixel;
    }

    public override void Update(int screenWidth, int screenHeight, bool centerCamera)
    {
        SetScreenBounds(0, 0, screenWidth, screenHeight);
        SetWorldSize(screenWidth * _unitsPerPixel, screenHeight * _unitsPerPixel);
        Apply(centerCamera);
    }
}