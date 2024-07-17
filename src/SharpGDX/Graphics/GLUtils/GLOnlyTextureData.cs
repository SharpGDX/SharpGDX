using System;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Graphics.GLUtils
{
	/** A {@link TextureData} implementation which should be used to create gl only textures. This TextureData fits perfectly for
 * FrameBuffer. The data is not managed. */
public class GLOnlyTextureData : ITextureData {

	/** width and height */
	int width = 0;
	int height = 0;
	bool _isPrepared = false;

	/** properties of opengl texture */
	int mipLevel = 0;
	int internalFormat;
	int format;
	int type;

	/** @see "https://www.khronos.org/opengles/sdk/docs/man/xhtml/glTexImage2D.xml"
	 * @param internalFormat Specifies the internal format of the texture. Must be one of the following symbolic constants:
	 *           {@link GL20#GL_ALPHA}, {@link GL20#GL_LUMINANCE}, {@link GL20#GL_LUMINANCE_ALPHA}, {@link GL20#GL_RGB},
	 *           {@link GL20#GL_RGBA}.
	 * @param format Specifies the format of the texel data. Must match internalformat. The following symbolic values are accepted:
	 *           {@link GL20#GL_ALPHA}, {@link GL20#GL_RGB}, {@link GL20#GL_RGBA}, {@link GL20#GL_LUMINANCE}, and
	 *           {@link GL20#GL_LUMINANCE_ALPHA}.
	 * @param type Specifies the data type of the texel data. The following symbolic values are accepted:
	 *           {@link GL20#GL_UNSIGNED_BYTE}, {@link GL20#GL_UNSIGNED_SHORT_5_6_5}, {@link GL20#GL_UNSIGNED_SHORT_4_4_4_4}, and
	 *           {@link GL20#GL_UNSIGNED_SHORT_5_5_5_1}. */
	public GLOnlyTextureData (int width, int height, int mipMapLevel, int internalFormat, int format, int type) {
		this.width = width;
		this.height = height;
		this.mipLevel = mipMapLevel;
		this.internalFormat = internalFormat;
		this.format = format;
		this.type = type;
	}

	public ITextureData.TextureDataType getType () {
		return ITextureData.TextureDataType.Custom;
	}

	public bool isPrepared () {
		return _isPrepared;
	}

	public void prepare () {
		if (_isPrepared) throw new GdxRuntimeException("Already prepared");
		_isPrepared = true;
	}

	public void consumeCustomData (int target) {
		Gdx.gl.glTexImage2D(target, mipLevel, internalFormat, width, height, 0, format, type, null);
	}

	public Pixmap consumePixmap () {
		throw new GdxRuntimeException("This TextureData implementation does not return a Pixmap");
	}

	public bool disposePixmap () {
		throw new GdxRuntimeException("This TextureData implementation does not return a Pixmap");
	}

	public int getWidth () {
		return width;
	}

	public int getHeight () {
		return height;
	}

	public Pixmap.Format? getFormat () {
		return Pixmap.Format.RGBA8888;
	}

	public bool useMipMaps () {
		return false;
	}

	public bool isManaged () {
		return false;
	}
}
}
