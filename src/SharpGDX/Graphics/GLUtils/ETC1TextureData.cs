using SharpGDX.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.GLUtils
{
	public class ETC1TextureData : ITextureData {
	FileHandle file;
	ETC1.ETC1Data data;
	bool _useMipMaps;
	int width = 0;
	int height = 0;
	bool _isPrepared = false;

	public ETC1TextureData (FileHandle file) 
	: this(file, false)
	{
		
	}

	public ETC1TextureData (FileHandle file, bool useMipMaps) {
		this.file = file;
		this._useMipMaps = useMipMaps;
	}

	public ETC1TextureData (ETC1.ETC1Data encodedImage, bool useMipMaps) {
		this.data = encodedImage;
		this._useMipMaps = useMipMaps;
	}

	public ITextureData.TextureDataType getType () {
		return ITextureData.TextureDataType.Custom;
	}

	public bool isPrepared () {
		return _isPrepared;
	}

	public void prepare () {
		if (_isPrepared) throw new GdxRuntimeException("Already prepared");
		if (file == null && data == null) throw new GdxRuntimeException("Can only load once from ETC1Data");
		if (file != null) {
			data = new ETC1.ETC1Data(file);
		}
		width = data.width;
		height = data.height;
		_isPrepared = true;
	}

	public void consumeCustomData (int target) {
		if (!_isPrepared) throw new GdxRuntimeException("Call prepare() before calling consumeCompressedData()");

		if (!Gdx.graphics.supportsExtension("GL_OES_compressed_ETC1_RGB8_texture")) {
			Pixmap pixmap = ETC1.decodeImage(data, Pixmap.Format.RGB565);
			Gdx.gl.glTexImage2D(target, 0, pixmap.getGLInternalFormat(), pixmap.getWidth(), pixmap.getHeight(), 0,
				pixmap.getGLFormat(), pixmap.getGLType(), pixmap.getPixels());
			if (_useMipMaps) MipMapGenerator.generateMipMap(target, pixmap, pixmap.getWidth(), pixmap.getHeight());
			pixmap.dispose();
			_useMipMaps = false;
		} else {
			Gdx.gl.glCompressedTexImage2D(target, 0, ETC1.ETC1_RGB8_OES, width, height, 0,
				data.compressedData.capacity() - data.dataOffset, data.compressedData);
			if (useMipMaps()) Gdx.gl20.glGenerateMipmap(GL20.GL_TEXTURE_2D);
		}
		data.dispose();
		data = null;
		_isPrepared = false;
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
		return Pixmap.Format.RGB565;
	}

	public bool useMipMaps () {
		return _useMipMaps;
	}

	public bool isManaged () {
		return true;
	}
}
}
