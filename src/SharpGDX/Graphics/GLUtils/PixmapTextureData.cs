using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.GLUtils
{
	public class PixmapTextureData : ITextureData {
	readonly Pixmap pixmap;
	readonly Pixmap.Format? format;
	readonly bool _useMipMaps;
	readonly bool _disposePixmap;
	readonly bool managed;

	public PixmapTextureData (Pixmap pixmap, Pixmap.Format? format, bool useMipMaps, bool disposePixmap) 
	: this(pixmap, format, useMipMaps, disposePixmap, false)
	{
		
	}

	public PixmapTextureData (Pixmap pixmap, Pixmap.Format? format, bool useMipMaps, bool disposePixmap, bool managed) {
		this.pixmap = pixmap;
		this.format = format == null ? pixmap.getFormat() : format;
		this._useMipMaps = useMipMaps;
		this._disposePixmap = disposePixmap;
		this.managed = managed;
	}

	public bool disposePixmap () {
		return _disposePixmap;
	}

	public Pixmap consumePixmap () {
		return pixmap;
	}

	public int getWidth () {
		return pixmap.getWidth();
	}

	public int getHeight () {
		return pixmap.getHeight();
	}

	public Pixmap.Format? getFormat () {
		return format;
	}

	public bool useMipMaps () {
		return _useMipMaps;
	}

	public bool isManaged () {
		return managed;
	}

	public ITextureData.TextureDataType getType () {
		return ITextureData.TextureDataType.Pixmap;
	}

	public void consumeCustomData (int target) {
		throw new GdxRuntimeException("This TextureData implementation does not upload data itself");
	}

	public bool isPrepared () {
		return true;
	}

	public void prepare () {
		throw new GdxRuntimeException("prepare() must not be called on a PixmapTextureData instance as it is already prepared.");
	}
}
}
