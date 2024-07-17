using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.GLUtils
{
	/** A {@link Texture3DData} implementation that addresses 2 use cases :
 * 
 * You can use it as a GL only texture (feed by a compute shader). In this case the texture is not managed.
 * 
 * Or you can use it to upload pixels to GPU. In this case you should call {@link #getPixels()} to fill the buffer prior to
 * consuming it (eg. before new Texture3D(data)).
 * 
 * @author mgsx */
public class CustomTexture3DData : ITexture3DData {

	private int width, height, depth;
	private int mipMapLevel;
	private int glFormat;
	private int glInternalFormat;
	private int glType;
	private ByteBuffer pixels;

	/** @see "https://registry.khronos.org/OpenGL-Refpages/es3.0/html/glTexImage3D.xhtml" */
	public CustomTexture3DData (int width, int height, int depth, int mipMapLevel, int glFormat, int glInternalFormat,
		int glType) {
		this.width = width;
		this.height = height;
		this.depth = depth;
		this.glFormat = glFormat;
		this.glInternalFormat = glInternalFormat;
		this.glType = glType;
		this.mipMapLevel = mipMapLevel;
	}

	public bool isPrepared () {
		return true;
	}

	public void prepare () {
	}

	public int getWidth () {
		return width;
	}

	public int getHeight () {
		return height;
	}

	public int getDepth () {
		return depth;
	}

	public bool useMipMaps () {
		return false;
	}

	public bool isManaged () {
		return pixels != null;
	}

	public int getInternalFormat () {
		return glInternalFormat;
	}

	public int getGLType () {
		return glType;
	}

	public int getGLFormat () {
		return glFormat;
	}

	public int getMipMapLevel () {
		return mipMapLevel;
	}

	public ByteBuffer getPixels () {
		if (pixels == null) {

			int numChannels;
			if (glFormat == GL30.GL_RED || glFormat == GL30.GL_RED_INTEGER || glFormat == GL20.GL_LUMINANCE
				|| glFormat == GL20.GL_ALPHA) {
				numChannels = 1;
			} else if (glFormat == GL30.GL_RG || glFormat == GL30.GL_RG_INTEGER || glFormat == GL20.GL_LUMINANCE_ALPHA) {
				numChannels = 2;
			} else if (glFormat == GL20.GL_RGB || glFormat == GL30.GL_RGB_INTEGER) {
				numChannels = 3;
			} else if (glFormat == GL20.GL_RGBA || glFormat == GL30.GL_RGBA_INTEGER) {
				numChannels = 4;
			} else {
				throw new GdxRuntimeException("unsupported glFormat: " + glFormat);
			}

			int bytesPerChannel;
			if (glType == GL20.GL_UNSIGNED_BYTE || glType == GL20.GL_BYTE) {
				bytesPerChannel = 1;
			} else if (glType == GL20.GL_UNSIGNED_SHORT || glType == GL20.GL_SHORT || glType == GL30.GL_HALF_FLOAT) {
				bytesPerChannel = 2;
			} else if (glType == GL20.GL_UNSIGNED_INT || glType == GL20.GL_INT || glType == GL20.GL_FLOAT) {
				bytesPerChannel = 4;
			} else {
				throw new GdxRuntimeException("unsupported glType: " + glType);
			}

			int bytesPerPixel = numChannels * bytesPerChannel;

			pixels = BufferUtils.newByteBuffer(width * height * depth * bytesPerPixel);
		}
		return pixels;
	}

	public void consume3DData () {
		Gdx.gl30.glTexImage3D(GL30.GL_TEXTURE_3D, mipMapLevel, glInternalFormat, width, height, depth, 0, glFormat, glType, pixels);
	}

}
}
