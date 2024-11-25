using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Mathematics;
using SharpGDX.Shims;

namespace SharpGDX.Utils;

/// <summary>
///     Class with static helper methods related to currently bound OpenGL frame buffer, including access to the current
///     OpenGL FrameBuffer.
/// </summary>
/// <remarks>
///     These methods can be used to get the entire screen content or a portion thereof.
/// </remarks>
public static class ScreenUtils
{
    /// <summary>
    ///     Clears the color buffers with the specified Color.
    /// </summary>
    /// <param name="color">Color to clear the color buffers with.</param>
    public static void Clear(Color color)
    {
        Clear(color.R, color.G, color.B, color.A, false);
    }

    /// <summary>
    ///     Clears the color buffers with the specified color.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    public static void Clear(float r, float g, float b, float a)
    {
        Clear(r, g, b, a, false);
    }

    /// <summary>
    ///     Clears the color buffers and optionally the depth buffer.
    /// </summary>
    /// <param name="color">Color to clear the color buffers with.</param>
    /// <param name="clearDepth">Clears the depth buffer if true.</param>
    public static void Clear(Color color, bool clearDepth)
    {
        Clear(color.R, color.G, color.B, color.A, clearDepth);
    }

    /// <summary>
    ///     Clears the color buffers and optionally the depth buffer.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <param name="clearDepth">Clears the depth buffer if true.</param>
    public static void Clear(float r, float g, float b, float a, bool clearDepth)
    {
        Clear(r, g, b, a, clearDepth, false);
    }

    /// <summary>
    ///     Clears the color buffers, optionally the depth buffer and whether to apply antialiasing (requires to set number of
    ///     samples in the launcher class).
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <param name="clearDepth">Clears the depth buffer if true.</param>
    /// <param name="applyAntialiasing">Applies multi-sampling for antialiasing if true.</param>
    public static void Clear(float r, float g, float b, float a, bool clearDepth, bool applyAntialiasing)
    {
        GDX.GL.glClearColor(r, g, b, a);
        var mask = IGL20.GL_COLOR_BUFFER_BIT;

        if (clearDepth)
        {
            mask |= IGL20.GL_DEPTH_BUFFER_BIT;
        }

        if (applyAntialiasing && GDX.Graphics.GetBufferFormat().CoverageSampling)
        {
            mask |= IGL20.GL_COVERAGE_BUFFER_BIT_NV;
        }

        GDX.GL.glClear(mask);
    }

    /// <summary>
    ///     Returns the current frame buffer contents as a byte[] array with a length equal to screen width * height * 4.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The byte[] will always contain RGBA8888 data.
    ///     </para>
    ///     <para>
    ///         Because of differences in screen and image origins the frame buffer contents should be flipped along the Y axis
    ///         if you intend save them to disk as a bitmap.
    ///     </para>
    ///     <para>
    ///         Flipping is not a cheap operation, so use this functionality wisely.
    ///     </para>
    /// </remarks>
    /// <param name="flipY">Whether to flip pixels along Y axis.</param>
    /// <returns></returns>
    public static byte[] GetFrameBufferPixels(bool flipY)
    {
        var w = GDX.Graphics.GetBackBufferWidth();
        var h = GDX.Graphics.GetBackBufferHeight();
        return GetFrameBufferPixels(0, 0, w, h, flipY);
    }

    /// <summary>
    ///     Returns a portion of the current frame buffer contents specified by x, y, width and height, as a byte[] array with
    ///     a length equal to the specified width * height * 4.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The byte[] will always contain RGBA8888 data.
    ///     </para>
    ///     <para>
    ///         If the width and height specified are larger than the frame buffer dimensions, the Texture will be padded
    ///         accordingly. Pixels that fall outside the current screen will have RGBA values of 0.
    ///     </para>
    ///     <para>
    ///         Because of differences in screen and image origins the frame buffer contents should be flipped along the Y axis
    ///         if you intend save them to disk as a bitmap.
    ///     </para>
    ///     <para>
    ///         Flipping is not a cheap operation, so use this functionality wisely.
    ///     </para>
    /// </remarks>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="flipY">Whether to flip pixels along Y axis.</param>
    /// <returns></returns>
    public static byte[] GetFrameBufferPixels(int x, int y, int w, int h, bool flipY)
    {
        GDX.GL.glPixelStorei(IGL20.GL_PACK_ALIGNMENT, 1);
        var pixels = BufferUtils.newByteBuffer(w * h * 4);
        GDX.GL.glReadPixels(x, y, w, h, IGL20.GL_RGBA, IGL20.GL_UNSIGNED_BYTE, pixels);
        var numBytes = w * h * 4;
        var lines = new byte[numBytes];

        if (flipY)
        {
            var numBytesPerLine = w * 4;

            for (var i = 0; i < h; i++)
            {
                pixels.position((h - i - 1) * numBytesPerLine);
                pixels.get(lines, i * numBytesPerLine, numBytesPerLine);
            }
        }
        else
        {
            pixels.clear();
            pixels.get(lines);
        }

        return lines;
    }

    /// <summary>
    ///     Returns the current frame buffer contents as a <see cref="TextureRegion" /> with a width and height equal to the
    ///     current screen size.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The base <see cref="Texture" /> always has <see cref="MathUtils.nextPowerOfTwo" /> dimensions and RGBA8888
    ///         <see cref="Pixmap.Format" />. It can be accessed via <see cref="TextureRegion.getTexture()" />.
    ///     </para>
    ///     <para>
    ///         The texture is not managed and has to be reloaded manually on a context loss.
    ///     </para>
    ///     <para>
    ///         The returned TextureRegion is flipped along the Y axis by default.
    ///     </para>
    /// </remarks>
    /// <returns></returns>
    public static TextureRegion GetFrameBufferTexture()
    {
        var w = GDX.Graphics.GetBackBufferWidth();
        var h = GDX.Graphics.GetBackBufferHeight();

        return GetFrameBufferTexture(0, 0, w, h);
    }

    /// <summary>
    ///     Returns a portion of the current frame buffer contents specified by x, y, width and height as a
    ///     <see cref="TextureRegion" /> with the same dimensions.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The base <see cref="Texture" /> always has <see cref="MathUtils.nextPowerOfTwo" /> dimensions and RGBA8888
    ///         <see cref="Pixmap.Format" />. It can be accessed via <see cref="TextureRegion.getTexture()" />.
    ///     </para>
    ///     <para>
    ///         This texture is not managed and has to be reloaded manually on a context loss.
    ///     </para>
    ///     <para>
    ///         If the width and height specified are larger than the frame buffer dimensions, the Texture will be padded
    ///         accordingly. Pixels that fall outside the current screen will have RGBA values of 0.
    ///     </para>
    /// </remarks>
    /// <param name="x">The x position of the frame buffer contents to capture.</param>
    /// <param name="y">The y position of the frame buffer contents to capture.</param>
    /// <param name="w">The width of the frame buffer contents to capture.</param>
    /// <param name="h">The height of the frame buffer contents to capture.</param>
    /// <returns></returns>
    public static TextureRegion GetFrameBufferTexture(int x, int y, int w, int h)
    {
        var potW = MathUtils.nextPowerOfTwo(w);
        var potH = MathUtils.nextPowerOfTwo(h);
        var pixmap = Pixmap.createFromFrameBuffer(x, y, w, h);
        var potPixmap = new Pixmap(potW, potH, Pixmap.Format.RGBA8888);
        potPixmap.setBlending(Pixmap.Blending.None);
        potPixmap.drawPixmap(pixmap, 0, 0);
        var texture = new Texture(potPixmap);
        var textureRegion = new TextureRegion(texture, 0, h, w, -h);
        potPixmap.Dispose();
        pixmap.Dispose();

        return textureRegion;
    }
}