using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SharpGDX.Graphics.Pixmap;

namespace SharpGDX.Graphics.G2D
{
	/** @author mzechner */
	public class Gdx2DPixmap : Disposable
	{
		public const int GDX2D_FORMAT_ALPHA = 1;
		public const int GDX2D_FORMAT_LUMINANCE_ALPHA = 2;
		public const int GDX2D_FORMAT_RGB888 = 3;
		public const int GDX2D_FORMAT_RGBA8888 = 4;
		public const int GDX2D_FORMAT_RGB565 = 5;
		public const int GDX2D_FORMAT_RGBA4444 = 6;

		public const int GDX2D_SCALE_NEAREST = 0;
		public const int GDX2D_SCALE_LINEAR = 1;

		public const int GDX2D_BLEND_NONE = 0;
		public const int GDX2D_BLEND_SRC_OVER = 1;

		public static int toGlFormat(int format)
		{
			switch (format)
			{
				case GDX2D_FORMAT_ALPHA:
					return GL20.GL_ALPHA;
				case GDX2D_FORMAT_LUMINANCE_ALPHA:
					return GL20.GL_LUMINANCE_ALPHA;
				case GDX2D_FORMAT_RGB888:
				case GDX2D_FORMAT_RGB565:
					return GL20.GL_RGB;
				case GDX2D_FORMAT_RGBA8888:
				case GDX2D_FORMAT_RGBA4444:
					return GL20.GL_RGBA;
				default:
					throw new GdxRuntimeException("unknown format: " + format);
			}
		}

		public static int toGlType(int format)
		{
			switch (format)
			{
				case GDX2D_FORMAT_ALPHA:
				case GDX2D_FORMAT_LUMINANCE_ALPHA:
				case GDX2D_FORMAT_RGB888:
				case GDX2D_FORMAT_RGBA8888:
					return GL20.GL_UNSIGNED_BYTE;
				case GDX2D_FORMAT_RGB565:
					return GL20.GL_UNSIGNED_SHORT_5_6_5;
				case GDX2D_FORMAT_RGBA4444:
					return GL20.GL_UNSIGNED_SHORT_4_4_4_4;
				default:
					throw new GdxRuntimeException("unknown format: " + format);
			}
		}

		long basePtr;
		int width;
		int height;
		int format;
		ByteBuffer pixelPtr;
		long[] nativeData = new long[4];

		public Gdx2DPixmap(byte[] encodedData, int offset, int len, int requestedFormat) // TODO: throws IOException
		{
			pixelPtr = load(nativeData, encodedData, offset, len);
			if (pixelPtr == null) throw new IOException("Error loading pixmap: " + getFailureReason());

			basePtr = nativeData[0];
			width = (int)nativeData[1];
			height = (int)nativeData[2];
			format = (int)nativeData[3];

			if (requestedFormat != 0 && requestedFormat != format)
			{
				convert(requestedFormat);
			}
		}

		public Gdx2DPixmap(ByteBuffer encodedData, int offset, int len, int requestedFormat) // TODO: throws IOException
		{
			if (!encodedData.isDirect()) throw new IOException("Couldn't load pixmap from non-direct ByteBuffer");
			pixelPtr = loadByteBuffer(nativeData, encodedData, offset, len);
			if (pixelPtr == null) throw new IOException("Error loading pixmap: " + getFailureReason());

			basePtr = nativeData[0];
			width = (int)nativeData[1];
			height = (int)nativeData[2];
			format = (int)nativeData[3];

			if (requestedFormat != 0 && requestedFormat != format)
			{
				convert(requestedFormat);
			}
		}

		public Gdx2DPixmap(InputStream @in, int requestedFormat) // TODO: throws IOException
		{
			ByteArrayOutputStream bytes = new ByteArrayOutputStream(1024);
			byte[] buffer = new byte[1024];
			int readBytes = 0;

			while ((readBytes = @in.read(buffer)) != -1)
			{
				bytes.write(buffer, 0, readBytes);
			}

			buffer = bytes.toByteArray();
			pixelPtr = load(nativeData, buffer, 0, buffer.Length);
			if (pixelPtr == null) throw new IOException("Error loading pixmap: " + getFailureReason());

			basePtr = nativeData[0];
			width = (int)nativeData[1];
			height = (int)nativeData[2];
			format = (int)nativeData[3];

			if (requestedFormat != 0 && requestedFormat != format)
			{
				convert(requestedFormat);
			}
		}

		/** @throws GdxRuntimeException if allocation failed. */
		public Gdx2DPixmap(int width, int height, int format) // TODO:  throws GdxRuntimeException
		{
			pixelPtr = newPixmap(nativeData, width, height, format);
			if (pixelPtr == null)
				throw new GdxRuntimeException(
					"Unable to allocate memory for pixmap: " + width + "x" + height + ", " + getFormatString(format));

			this.basePtr = nativeData[0];
			this.width = (int)nativeData[1];
			this.height = (int)nativeData[2];
			this.format = (int)nativeData[3];
		}

		public Gdx2DPixmap(ByteBuffer pixelPtr, long[] nativeData)
		{
			this.pixelPtr = pixelPtr;
			this.basePtr = nativeData[0];
			this.width = (int)nativeData[1];
			this.height = (int)nativeData[2];
			this.format = (int)nativeData[3];
		}

		private void convert(int requestedFormat)
		{
			Gdx2DPixmap pixmap = new Gdx2DPixmap(width, height, requestedFormat);
			pixmap.setBlend(GDX2D_BLEND_NONE);
			pixmap.drawPixmap(this, 0, 0, 0, 0, width, height);
			dispose();
			this.basePtr = pixmap.basePtr;
			this.format = pixmap.format;
			this.height = pixmap.height;
			this.nativeData = pixmap.nativeData;
			this.pixelPtr = pixmap.pixelPtr;
			this.width = pixmap.width;
		}

		public void dispose()
		{
			free(basePtr);
		}

		public void clear(int color)
		{
			clear(basePtr, color);
		}

		public void setPixel(int x, int y, int color)
		{
			setPixel(basePtr, x, y, color);
		}

		public int getPixel(int x, int y)
		{
			return getPixel(basePtr, x, y);
		}

		public void drawLine(int x, int y, int x2, int y2, int color)
		{
			drawLine(basePtr, x, y, x2, y2, color);
		}

		public void drawRect(int x, int y, int width, int height, int color)
		{
			drawRect(basePtr, x, y, width, height, color);
		}

		public void drawCircle(int x, int y, int radius, int color)
		{
			drawCircle(basePtr, x, y, radius, color);
		}

		public void fillRect(int x, int y, int width, int height, int color)
		{
			fillRect(basePtr, x, y, width, height, color);
		}

		public void fillCircle(int x, int y, int radius, int color)
		{
			fillCircle(basePtr, x, y, radius, color);
		}

		public void fillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, int color)
		{
			fillTriangle(basePtr, x1, y1, x2, y2, x3, y3, color);
		}

		public void drawPixmap(Gdx2DPixmap src, int srcX, int srcY, int dstX, int dstY, int width, int height)
		{
			drawPixmap(src.basePtr, basePtr, srcX, srcY, width, height, dstX, dstY, width, height);
		}

		public void drawPixmap(Gdx2DPixmap src, int srcX, int srcY, int srcWidth, int srcHeight, int dstX, int dstY,
			int dstWidth,
			int dstHeight)
		{
			drawPixmap(src.basePtr, basePtr, srcX, srcY, srcWidth, srcHeight, dstX, dstY, dstWidth, dstHeight);
		}

		public void setBlend(int blend)
		{
			setBlend(basePtr, blend);
		}

		public void setScale(int scale)
		{
			setScale(basePtr, scale);
		}

		public static Gdx2DPixmap newPixmap(InputStream @in, int requestedFormat)
		{
			try
			{
				return new Gdx2DPixmap(@in, requestedFormat);
			}
			catch (IOException e)
			{
				return null;
			}
		}

		public static Gdx2DPixmap newPixmap(int width, int height, int format)
		{
			try
			{
				return new Gdx2DPixmap(width, height, format);
			}
			catch (IllegalArgumentException e)
			{
				return null;
			}
		}

		public ByteBuffer getPixels()
		{
			return pixelPtr;
		}

		public int getHeight()
		{
			return height;
		}

		public int getWidth()
		{
			return width;
		}

		public int getFormat()
		{
			return format;
		}

		public int getGLInternalFormat()
		{
			return toGlFormat(format);
		}

		public int getGLFormat()
		{
			return getGLInternalFormat();
		}

		public int getGLType()
		{
			return toGlType(format);
		}

		public String getFormatString()
		{
			return getFormatString(format);
		}

		static private String getFormatString(int format)
		{
			switch (format)
			{
				case GDX2D_FORMAT_ALPHA:
					return "alpha";
				case GDX2D_FORMAT_LUMINANCE_ALPHA:
					return "luminance alpha";
				case GDX2D_FORMAT_RGB888:
					return "rgb888";
				case GDX2D_FORMAT_RGBA8888:
					return "rgba8888";
				case GDX2D_FORMAT_RGB565:
					return "rgb565";
				case GDX2D_FORMAT_RGBA4444:
					return "rgba4444";
				default:
					return "unknown";
			}
		}

		// @off
		/*JNI
		#include <gdx2d/gdx2d.h>
		#include <stdlib.h>
		 */

		[StructLayout(LayoutKind.Sequential)]
		private struct gdx2d_pixmap
		{
			public uint width;
			public uint height;
			public uint format;
			public uint blend;
			public uint scale;
			public IntPtr pixels;
		}

		private static ByteBuffer load(long[] nativeData, byte[] buffer, int offset, int len)
		{
			var bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			var pixmapPtr = load(bufferHandle.AddrOfPinnedObject() + offset, len);
			var pixmap = Marshal.PtrToStructure<gdx2d_pixmap>(pixmapPtr);

			bufferHandle.Free();

			nativeData[0] = pixmapPtr.ToInt64();
			nativeData[1] = pixmap.width;
			nativeData[2] = pixmap.height;
			nativeData[3] = pixmap.format;

			var dataBlockSize = (int)(pixmap.width * pixmap.height * bytes_per_pixel(pixmap.format));
			var byteArray = new byte[dataBlockSize];
			Marshal.Copy(pixmap.pixels, byteArray, 0, dataBlockSize);

			var pixelBuffer = ByteBuffer.wrap(byteArray);

			return pixelBuffer;

			[DllImport("gdx2d.dll", EntryPoint = "gdx2d_load")]
			static extern IntPtr load(long nativeData, int len);

			[DllImport("gdx2d.dll", EntryPoint = "gdx2d_bytes_per_pixel")]
			static extern uint bytes_per_pixel(uint format);
		}

		/*MANUAL
	   const unsigned char* p_buffer = (const unsigned char*)env->GetPrimitiveArrayCritical(buffer, 0);
	   gdx2d_pixmap* pixmap = gdx2d_load(p_buffer + offset, len);
	   env->ReleasePrimitiveArrayCritical(buffer, (char*)p_buffer, 0);

	   if(pixmap==0)
		   return 0;

	   jobject pixel_buffer = env->NewDirectByteBuffer((void*)pixmap->pixels, pixmap->width * pixmap->height * gdx2d_bytes_per_pixel(pixmap->format));
	   jlong* p_native_data = (jlong*)env->GetPrimitiveArrayCritical(nativeData, 0);
	   p_native_data[0] = (jlong)pixmap;
	   p_native_data[1] = pixmap->width;
	   p_native_data[2] = pixmap->height;
	   p_native_data[3] = pixmap->format;
	   env->ReleasePrimitiveArrayCritical(nativeData, p_native_data, 0);

	   return pixel_buffer;
	*/
		[DllImport("gdx2d.dll")]
		private static extern ByteBuffer
			loadByteBuffer(long[] nativeData, ByteBuffer buffer, int offset, int len); /*MANUAL
			if(buffer==0)
				return 0;

			const unsigned char* p_buffer = (const unsigned char*)env->GetDirectBufferAddress(buffer);
			gdx2d_pixmap* pixmap = gdx2d_load(p_buffer + offset, len);

			if(pixmap==0)
				return 0;

			jobject pixel_buffer = env->NewDirectByteBuffer((void*)pixmap->pixels, pixmap->width * pixmap->height * gdx2d_bytes_per_pixel(pixmap->format));
			jlong* p_native_data = (jlong*)env->GetPrimitiveArrayCritical(nativeData, 0);
			p_native_data[0] = (jlong)pixmap;
			p_native_data[1] = pixmap->width;
			p_native_data[2] = pixmap->height;
			p_native_data[3] = pixmap->format;
			env->ReleasePrimitiveArrayCritical(nativeData, p_native_data, 0);

			return pixel_buffer;
		 */

		[DllImport("gdx2d.dll")]
		private static extern ByteBuffer newPixmap(long[] nativeData, int width, int height, int format); /*MANUAL
		gdx2d_pixmap* pixmap = gdx2d_new(width, height, format);
		if(pixmap==0)
			return 0;

		jobject pixel_buffer = env->NewDirectByteBuffer((void*)pixmap->pixels, pixmap->width * pixmap->height * gdx2d_bytes_per_pixel(pixmap->format));
		jlong* p_native_data = (jlong*)env->GetPrimitiveArrayCritical(nativeData, 0);
		p_native_data[0] = (jlong)pixmap;
		p_native_data[1] = pixmap->width;
		p_native_data[2] = pixmap->height;
		p_native_data[3] = pixmap->format;
		env->ReleasePrimitiveArrayCritical(nativeData, p_native_data, 0);

		return pixel_buffer;
	 */

		[DllImport("gdx2d.dll", EntryPoint = "gdx2d_free")]
		private static extern void free(long pixmap); /*
		gdx2d_free((gdx2d_pixmap*)pixmap);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void clear(long pixmap, int color); /*
		gdx2d_clear((gdx2d_pixmap*)pixmap, color);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void setPixel(long pixmap, int x, int y, int color); /*
		gdx2d_set_pixel((gdx2d_pixmap*)pixmap, x, y, color);
	 */

		[DllImport("gdx2d.dll")]
		private static extern int getPixel(long pixmap, int x, int y); /*
		return gdx2d_get_pixel((gdx2d_pixmap*)pixmap, x, y);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void drawLine(long pixmap, int x, int y, int x2, int y2, int color); /*
		gdx2d_draw_line((gdx2d_pixmap*)pixmap, x, y, x2, y2, color);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void drawRect(long pixmap, int x, int y, int width, int height, int color); /*
		gdx2d_draw_rect((gdx2d_pixmap*)pixmap, x, y, width, height, color);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void drawCircle(long pixmap, int x, int y, int radius, int color); /*
		gdx2d_draw_circle((gdx2d_pixmap*)pixmap, x, y, radius, color);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void fillRect(long pixmap, int x, int y, int width, int height, int color); /*
		gdx2d_fill_rect((gdx2d_pixmap*)pixmap, x, y, width, height, color);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void fillCircle(long pixmap, int x, int y, int radius, int color); /*
		gdx2d_fill_circle((gdx2d_pixmap*)pixmap, x, y, radius, color);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void
			fillTriangle(long pixmap, int x1, int y1, int x2, int y2, int x3, int y3, int color); /*
			gdx2d_fill_triangle((gdx2d_pixmap*)pixmap, x1, y1, x2, y2, x3, y3, color);
		 */

		[DllImport("gdx2d.dll")]
		private static extern void drawPixmap(long src, long dst, int srcX, int srcY, int srcWidth, int srcHeight,
			int dstX,
			int dstY, int dstWidth, int dstHeight); /*
				gdx2d_draw_pixmap((gdx2d_pixmap*)src, (gdx2d_pixmap*)dst, srcX, srcY, srcWidth, srcHeight, dstX, dstY, dstWidth, dstHeight);
				 */

		[DllImport("gdx2d.dll")]
		private static extern void setBlend(long src, int blend); /*
		gdx2d_set_blend((gdx2d_pixmap*)src, blend);
	 */

		[DllImport("gdx2d.dll")]
		private static extern void setScale(long src, int scale); /*
		gdx2d_set_scale((gdx2d_pixmap*)src, scale);
	 */

		[DllImport("gdx2d.dll")]
		public static extern String getFailureReason(); /*
     return env->NewStringUTF(gdx2d_get_failure_reason());
	 */
	}
}