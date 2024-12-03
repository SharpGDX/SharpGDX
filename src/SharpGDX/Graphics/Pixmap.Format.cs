using SharpGDX.Graphics.G2D;
using SharpGDX.Utils;

namespace SharpGDX.Graphics;

public partial class Pixmap
{
    /// <summary>
    ///     Different pixel formats.
    /// </summary>
    public enum Format
    {
        Alpha,
        Intensity,
        LuminanceAlpha,
        RGB565,
        RGBA4444,
        RGB888,
        RGBA8888
    }

    public class FormatUtils
    {
        public static int ToGdx2DPixmapFormat(Format? format)
        {
            if (format == Format.Alpha)
            {
                return Gdx2DPixmap.GDX2D_FORMAT_ALPHA;
            }

            if (format == Format.Intensity)
            {
                return Gdx2DPixmap.GDX2D_FORMAT_ALPHA;
            }

            if (format == Format.LuminanceAlpha)
            {
                return Gdx2DPixmap.GDX2D_FORMAT_LUMINANCE_ALPHA;
            }

            if (format == Format.RGB565)
            {
                return Gdx2DPixmap.GDX2D_FORMAT_RGB565;
            }

            if (format == Format.RGBA4444)
            {
                return Gdx2DPixmap.GDX2D_FORMAT_RGBA4444;
            }

            if (format == Format.RGB888)
            {
                return Gdx2DPixmap.GDX2D_FORMAT_RGB888;
            }

            if (format == Format.RGBA8888)
            {
                return Gdx2DPixmap.GDX2D_FORMAT_RGBA8888;
            }

            throw new GdxRuntimeException("Unknown Format: " + format);
        }

        public static Format FromGdx2DPixmapFormat(int format)
        {
            if (format == Gdx2DPixmap.GDX2D_FORMAT_ALPHA)
            {
                return Format.Alpha;
            }

            if (format == Gdx2DPixmap.GDX2D_FORMAT_LUMINANCE_ALPHA)
            {
                return Format.LuminanceAlpha;
            }

            if (format == Gdx2DPixmap.GDX2D_FORMAT_RGB565)
            {
                return Format.RGB565;
            }

            if (format == Gdx2DPixmap.GDX2D_FORMAT_RGBA4444)
            {
                return Format.RGBA4444;
            }

            if (format == Gdx2DPixmap.GDX2D_FORMAT_RGB888)
            {
                return Format.RGB888;
            }

            if (format == Gdx2DPixmap.GDX2D_FORMAT_RGBA8888)
            {
                return Format.RGBA8888;
            }

            throw new GdxRuntimeException("Unknown Gdx2DPixmap Format: " + format);
        }

        public static int ToGlFormat(Format format)
        {
            return Gdx2DPixmap.toGlFormat(ToGdx2DPixmapFormat(format));
        }

        public static int ToGlType(Format format)
        {
            return Gdx2DPixmap.toGlType(ToGdx2DPixmapFormat(format));
        }
    }
}