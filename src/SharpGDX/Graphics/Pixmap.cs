using SharpGDX.Files;
using System;
using SharpGDX.Graphics.G2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX;
using SharpGDX.Utils;

namespace SharpGDX.Graphics
{
    /**
 * <p>
 * A Pixmap represents an image in memory. It has a width and height expressed in pixels as well as a {@link Format} specifying
 * the number and order of color components per pixel. Coordinates of pixels are specified with respect to the top left corner of
 * the image, with the x-axis pointing to the right and the y-axis pointing downwards.
 * <p>
 * By default all methods use blending. You can disable blending with {@link Pixmap#setBlending(Blending)}, which may reduce
 * blitting time by ~30%. The {@link Pixmap#drawPixmap(Pixmap, int, int, int, int, int, int, int, int)} method will scale and
 * stretch the source image to a target image. There either nearest neighbour or bilinear filtering can be used.
 * <p>
 * A Pixmap stores its data in native heap memory. It is mandatory to call {@link Pixmap#dispose()} when the pixmap is no longer
 * needed, otherwise memory leaks will result
 * @author badlogicgames@gmail.com */
    public partial class Pixmap : IDisposable
    {
        /** Creates a Pixmap from a part of the current framebuffer.
         * @param x framebuffer region x
         * @param y framebuffer region y
         * @param w framebuffer region width
         * @param h framebuffer region height
         * @return the pixmap */
        public static Pixmap createFromFrameBuffer(int x, int y, int w, int h)
        {
            GDX.GL.glPixelStorei(IGL20.GL_PACK_ALIGNMENT, 1);

            Pixmap pixmap = new Pixmap(w, h, Format.RGBA8888);
            ByteBuffer pixels = pixmap.GetPixels();
            GDX.GL.glReadPixels(x, y, w, h, IGL20.GL_RGBA, IGL20.GL_UNSIGNED_BYTE, pixels);

            return pixmap;
        }

        private Blending blending = Blending.SourceOver;
        private Filter filter = Filter.BiLinear;

        readonly Gdx2DPixmap pixmap;
        int color = 0;

        private bool disposed;

        /** Sets the type of {@link Blending} to be used for all operations. Default is {@link Blending#SourceOver}.
         * @param blending the blending type */
        public void SetBlending(Blending blending)
        {
            this.blending = blending;
            pixmap.setBlend(blending == Blending.None ? 0 : 1);
        }

        /** Sets the type of interpolation {@link Filter} to be used in conjunction with
         * {@link Pixmap#drawPixmap(Pixmap, int, int, int, int, int, int, int, int)}.
         * @param filter the filter. */
        public void SetFilter(Filter filter)
        {
            this.filter = filter;
            pixmap.setScale(filter == Filter.NearestNeighbour
                ? Gdx2DPixmap.GDX2D_SCALE_NEAREST
                : Gdx2DPixmap.GDX2D_SCALE_LINEAR);
        }

        /** Creates a new Pixmap instance with the given width, height and format.
         * @param width the width in pixels
         * @param height the height in pixels
         * @param format the {@link Format} */
        public Pixmap(int width, int height, Format? format)
        {
            pixmap = new Gdx2DPixmap(width, height, FormatUtils.ToGdx2DPixmapFormat(format));
            SetColor(0, 0, 0, 0);
            Fill();
        }

        /** Creates a new Pixmap instance from the given encoded image data. The image can be encoded as JPEG, PNG or BMP. Not
         * available on GWT backend.
         *
         * @param encodedData the encoded image data
         * @param offset the offset
         * @param len the length */
        public Pixmap(byte[] encodedData, int offset, int len)
        {
            try
            {
                pixmap = new Gdx2DPixmap(encodedData, offset, len, 0);
            }
            catch (IOException e)
            {
                throw new GdxRuntimeException("Couldn't load pixmap from image data", e);
            }
        }

        /** Creates a new Pixmap instance from the given encoded image data. The image can be encoded as JPEG, PNG or BMP. Not
         * available on GWT backend.
         *
         * @param encodedData the encoded image data
         * @param offset the offset relative to the base address of encodedData
         * @param len the length */
        public Pixmap(ByteBuffer encodedData, int offset, int len)
        {
            if (!encodedData.isDirect())
                throw new GdxRuntimeException("Couldn't load pixmap from non-direct ByteBuffer");
            try
            {
                pixmap = new Gdx2DPixmap(encodedData, offset, len, 0);
            }
            catch (IOException e)
            {
                throw new GdxRuntimeException("Couldn't load pixmap from image data", e);
            }
        }

        /** Creates a new Pixmap instance from the given encoded image data. The image can be encoded as JPEG, PNG or BMP. Not
         * available on GWT backend.
         *
         * Offset is based on the position of the buffer. Length is based on the remaining bytes of the buffer.
         *
         * @param encodedData the encoded image data */
        public Pixmap(ByteBuffer encodedData)
            : this(encodedData, encodedData.position(), encodedData.remaining())
        {

        }

        /** Creates a new Pixmap instance from the given file. The file must be a Png, Jpeg or Bitmap. Paletted formats are not
         * supported.
         *
         * @param file the {@link FileHandle} */
        public Pixmap(FileHandle file)
        {
            try
            {
                byte[] bytes = file.readBytes();
                pixmap = new Gdx2DPixmap(bytes, 0, bytes.Length, 0);
            }
            catch (Exception e)
            {
                throw new GdxRuntimeException("Couldn't load file: " + file, e);
            }
        }

        /** Constructs a new Pixmap from a {@link Gdx2DPixmap}.
         * @param pixmap */
        public Pixmap(Gdx2DPixmap pixmap)
        {
            this.pixmap = pixmap;
        }

        /** Downloads an image from http(s) url and passes it as a {@link Pixmap} to the specified
         * {@link DownloadPixmapResponseListener}
         *
         * @param url http url to download the image from
         * @param responseListener the listener to call once the image is available as a {@link Pixmap} */
        public static void DownloadFromUrl(String url, IDownloadPixmapResponseListener responseListener)
        {
            //Net.HttpRequest request = new Net.HttpRequest(Net.HttpMethods.GET);
            //request.setUrl(url);
            //Gdx.net.sendHttpRequest(request, new Net.HttpResponseListener() {
            //		@Override
            //		public void handleHttpResponse(Net.HttpResponse httpResponse)
            //{
            //	final byte[] result = httpResponse.getResult();
            //	Gdx.app.postRunnable(new Runnable() {
            //				@Override
            //				public void run()
            //	{
            //		try
            //		{
            //			Pixmap pixmap = new Pixmap(result, 0, result.length);
            //			responseListener.downloadComplete(pixmap);
            //		}
            //		catch (Throwable t)
            //		{
            //			failed(t);
            //		}
            //	}
            //});
//}

//			public void failed(Exception t)
//{
//	responseListener.downloadFailed(t);
//}

//			public void cancelled()
//{
//	// no way to cancel, will never get called
//}
//		});
        }

        /** Sets the color for the following drawing operations
         * @param color the color, encoded as RGBA8888 */
        public void SetColor(int color)
        {
            this.color = color;
        }

        /** Sets the color for the following drawing operations.
         *
         * @param r The red component.
         * @param g The green component.
         * @param b The blue component.
         * @param a The alpha component. */
        public void SetColor(float r, float g, float b, float a)
        {
            color = Color.RGBA8888(r, g, b, a);
        }

        /** Sets the color for the following drawing operations.
         * @param color The color. */
        public void SetColor(Color color)
        {
            this.color = Color.RGBA8888(color.R, color.G, color.B, color.A);
        }

        /** Fills the complete bitmap with the currently set color. */
        public void Fill()
        {
            pixmap.clear(color);
        }

// /**
// * Sets the width in pixels of strokes.
// *
// * @param width The stroke width in pixels.
// */
// public void setStrokeWidth (int width);

        /** Draws a line between the given coordinates using the currently set color.
         *
         * @param x The x-coodinate of the first point
         * @param y The y-coordinate of the first point
         * @param x2 The x-coordinate of the first point
         * @param y2 The y-coordinate of the first point */
        public void DrawLine(int x, int y, int x2, int y2)
        {
            pixmap.drawLine(x, y, x2, y2, color);
        }

        /** Draws a rectangle outline starting at x, y extending by width to the right and by height downwards (y-axis points
         * downwards) using the current color.
         *
         * @param x The x coordinate
         * @param y The y coordinate
         * @param width The width in pixels
         * @param height The height in pixels */
        public void DrawRectangle(int x, int y, int width, int height)
        {
            pixmap.drawRect(x, y, width, height, color);
        }

        /** Draws an area from another Pixmap to this Pixmap.
         *
         * @param pixmap The other Pixmap
         * @param x The target x-coordinate (top left corner)
         * @param y The target y-coordinate (top left corner) */
        public void DrawPixmap(Pixmap pixmap, int x, int y)
        {
            DrawPixmap(pixmap, x, y, 0, 0, pixmap.GetWidth(), pixmap.GetHeight());
        }

        /** Draws an area from another Pixmap to this Pixmap.
         *
         * @param pixmap The other Pixmap
         * @param x The target x-coordinate (top left corner)
         * @param y The target y-coordinate (top left corner)
         * @param srcx The source x-coordinate (top left corner)
         * @param srcy The source y-coordinate (top left corner);
         * @param srcWidth The width of the area from the other Pixmap in pixels
         * @param srcHeight The height of the area from the other Pixmap in pixels */
        public void DrawPixmap(Pixmap pixmap, int x, int y, int srcx, int srcy, int srcWidth, int srcHeight)
        {
            this.pixmap.drawPixmap(pixmap.pixmap, srcx, srcy, x, y, srcWidth, srcHeight);
        }

        /** Draws an area from another Pixmap to this Pixmap. This will automatically scale and stretch the source image to the
         * specified target rectangle. Use {@link Pixmap#setFilter(Filter)} to specify the type of filtering to be used (nearest
         * neighbour or bilinear).
         *
         * @param pixmap The other Pixmap
         * @param srcx The source x-coordinate (top left corner)
         * @param srcy The source y-coordinate (top left corner);
         * @param srcWidth The width of the area from the other Pixmap in pixels
         * @param srcHeight The height of the area from the other Pixmap in pixels
         * @param dstx The target x-coordinate (top left corner)
         * @param dsty The target y-coordinate (top left corner)
         * @param dstWidth The target width
         * @param dstHeight the target height */
        public void DrawPixmap(Pixmap pixmap, int srcx, int srcy, int srcWidth, int srcHeight, int dstx, int dsty,
            int dstWidth,
            int dstHeight)
        {
            this.pixmap.drawPixmap(pixmap.pixmap, srcx, srcy, srcWidth, srcHeight, dstx, dsty, dstWidth, dstHeight);
        }

        /** Fills a rectangle starting at x, y extending by width to the right and by height downwards (y-axis points downwards) using
         * the current color.
         *
         * @param x The x coordinate
         * @param y The y coordinate
         * @param width The width in pixels
         * @param height The height in pixels */
        public void FillRectangle(int x, int y, int width, int height)
        {
            pixmap.fillRect(x, y, width, height, color);
        }

        /** Draws a circle outline with the center at x,y and a radius using the current color and stroke width.
         *
         * @param x The x-coordinate of the center
         * @param y The y-coordinate of the center
         * @param radius The radius in pixels */
        public void DrawCircle(int x, int y, int radius)
        {
            pixmap.drawCircle(x, y, radius, color);
        }

        /** Fills a circle with the center at x,y and a radius using the current color.
         *
         * @param x The x-coordinate of the center
         * @param y The y-coordinate of the center
         * @param radius The radius in pixels */
        public void FillCircle(int x, int y, int radius)
        {
            pixmap.fillCircle(x, y, radius, color);
        }

        /** Fills a triangle with vertices at x1,y1 and x2,y2 and x3,y3 using the current color.
         *
         * @param x1 The x-coordinate of vertex 1
         * @param y1 The y-coordinate of vertex 1
         * @param x2 The x-coordinate of vertex 2
         * @param y2 The y-coordinate of vertex 2
         * @param x3 The x-coordinate of vertex 3
         * @param y3 The y-coordinate of vertex 3 */
        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            pixmap.fillTriangle(x1, y1, x2, y2, x3, y3, color);
        }

        /** Returns the 32-bit RGBA8888 value of the pixel at x, y. For Alpha formats the RGB components will be one.
         *
         * @param x The x-coordinate
         * @param y The y-coordinate
         * @return The pixel color in RGBA8888 format. */
        public int GetPixel(int x, int y)
        {
            return pixmap.getPixel(x, y);
        }

        /** @return The width of the Pixmap in pixels. */
        public int GetWidth()
        {
            return pixmap.getWidth();
        }

        /** @return The height of the Pixmap in pixels. */
        public int GetHeight()
        {
            return pixmap.getHeight();
        }

        /** Releases all resources associated with this Pixmap. */
        public void Dispose()
        {
            if (disposed)
            {
                GDX.App.Error("Pixmap", "Pixmap already disposed!");
                return;
            }

            pixmap.Dispose();
            disposed = true;
        }

        public bool IsDisposed()
        {
            return disposed;
        }

        /** Draws a pixel at the given location with the current color.
         *
         * @param x the x-coordinate
         * @param y the y-coordinate */
        public void DrawPixel(int x, int y)
        {
            pixmap.setPixel(x, y, color);
        }

        /** Draws a pixel at the given location with the given color.
         *
         * @param x the x-coordinate
         * @param y the y-coordinate
         * @param color the color in RGBA8888 format. */
        public void DrawPixel(int x, int y, int color)
        {
            pixmap.setPixel(x, y, color);
        }

        /** Returns the OpenGL ES format of this Pixmap. Used as the seventh parameter to
         * {@link GL20#glTexImage2D(int, int, int, int, int, int, int, int, java.nio.Buffer)}.
         * @return one of GL_ALPHA, GL_RGB, GL_RGBA, GL_LUMINANCE, or GL_LUMINANCE_ALPHA. */
        public int GetGLFormat()
        {
            return pixmap.getGLFormat();
        }

        /** Returns the OpenGL ES format of this Pixmap. Used as the third parameter to
         * {@link GL20#glTexImage2D(int, int, int, int, int, int, int, int, java.nio.Buffer)}.
         * @return one of GL_ALPHA, GL_RGB, GL_RGBA, GL_LUMINANCE, or GL_LUMINANCE_ALPHA. */
        public int GetGLInternalFormat()
        {
            return pixmap.getGLInternalFormat();
        }

        /** Returns the OpenGL ES type of this Pixmap. Used as the eighth parameter to
         * {@link GL20#glTexImage2D(int, int, int, int, int, int, int, int, java.nio.Buffer)}.
         * @return one of GL_UNSIGNED_BYTE, GL_UNSIGNED_SHORT_5_6_5, GL_UNSIGNED_SHORT_4_4_4_4 */
        public int GetGLType()
        {
            return pixmap.getGLType();
        }

        /** Returns the direct ByteBuffer holding the pixel data. For the format Alpha each value is encoded as a byte. For the format
         * LuminanceAlpha the luminance is the first byte and the alpha is the second byte of the pixel. For the formats RGB888 and
         * RGBA8888 the color components are stored in a single byte each in the order red, green, blue (alpha). For the formats RGB565
         * and RGBA4444 the pixel colors are stored in shorts in machine dependent order.
         * @return the direct {@link ByteBuffer} holding the pixel data. */
        public ByteBuffer GetPixels()
        {
            if (disposed) throw new GdxRuntimeException("Pixmap already disposed");
            return pixmap.getPixels();
        }

        /** Sets pixels from a provided direct byte buffer.
         * @param pixels Pixels to copy from, should be a direct ByteBuffer and match Pixmap data size (see {@link #getPixels()}). */
        public void SetPixels(ByteBuffer pixels)
        {
            if (!pixels.isDirect()) throw new GdxRuntimeException("Couldn't setPixels from non-direct ByteBuffer");
            ByteBuffer dst = pixmap.getPixels();
            BufferUtils.copy(pixels, dst, dst.limit());
        }

        /** @return the {@link Format} of this Pixmap. */
        public Format GetFormat()
        {
            return FormatUtils.FromGdx2DPixmapFormat(pixmap.getFormat());
        }

        /** @return the currently set {@link Blending} */
        public Blending GetBlending()
        {
            return blending;
        }

        /** @return the currently set {@link Filter} */
        public Filter GetFilter()
        {
            return filter;
        }
    }
}