namespace SharpGDX;

public partial interface IGraphics
{
    /// <summary>
    ///     Enumeration describing different types of <see cref="IGraphics" /> implementations.
    /// </summary>
    enum GraphicsType
    {
        AndroidGL,
        WebGL,
        iOSGL,
        JGLFW,
        Mock,
        OpenGL
    }
}