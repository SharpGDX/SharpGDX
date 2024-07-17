using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.GLUtils
{
	public class FileTextureData : ITextureData {

	readonly FileHandle file;
	int width = 0;
	int height = 0;
	Pixmap.Format? format;
	Pixmap pixmap;
	bool _useMipMaps;
	bool _isPrepared = false;

	public FileTextureData (FileHandle file, Pixmap preloadedPixmap, Pixmap.Format? format, bool useMipMaps) {
		this.file = file;
		this.pixmap = preloadedPixmap;
		this.format = format;
		this._useMipMaps = useMipMaps;
		if (pixmap != null) {
			width = pixmap.getWidth();
			height = pixmap.getHeight();
			if (format == null) this.format = pixmap.getFormat();
		}
	}

	public bool isPrepared () {
		return _isPrepared;
	}

	public void prepare () {
		if (_isPrepared) throw new GdxRuntimeException("Already prepared");
		if (pixmap == null) {
			if (file.extension().Equals("cim"))
				pixmap = PixmapIO.readCIM(file);
			else
				pixmap = new Pixmap(file);
			width = pixmap.getWidth();
			height = pixmap.getHeight();
			if (format == null) format = pixmap.getFormat();
		}
		_isPrepared = true;
	}

	public Pixmap consumePixmap () {
		if (!_isPrepared) throw new GdxRuntimeException("Call prepare() before calling getPixmap()");
		_isPrepared = false;
		Pixmap pixmap = this.pixmap;
		this.pixmap = null;
		return pixmap;
	}

	public bool disposePixmap () {
		return true;
	}

	public int getWidth () {
		return width;
	}

	public int getHeight () {
		return height;
	}

	public Pixmap.Format? getFormat () {
		return format;
	}

	public bool useMipMaps () {
		return _useMipMaps;
	}

	public bool isManaged () {
		return true;
	}

	public FileHandle getFileHandle () {
		return file;
	}

	public ITextureData.TextureDataType getType () {
		return ITextureData.TextureDataType.Pixmap;
	}

	public void consumeCustomData (int target) {
		throw new GdxRuntimeException("This TextureData implementation does not upload data itself");
	}

	public override String ToString () {
		return file.ToString();
	}
}
}
