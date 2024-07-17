using SharpGDX.Files;
using SharpGDX.Graphics;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} for {@link Pixmap} instances. The Pixmap is loaded asynchronously.
 * @author mzechner */
public class PixmapLoader : AsynchronousAssetLoader<Pixmap, PixmapLoader.PixmapParameter> {
	public PixmapLoader (IFileHandleResolver resolver) 
	: base(resolver)
	{
		
	}

	Pixmap pixmap;

	public override void loadAsync (AssetManager manager, String fileName, FileHandle file, PixmapParameter parameter) {
		pixmap = null;
		pixmap = new Pixmap(file);
	}

	public override Pixmap loadSync (AssetManager manager, String fileName, FileHandle file, PixmapParameter parameter) {
		Pixmap pixmap = this.pixmap;
		this.pixmap = null;
		return pixmap;
	}

	public override Array<AssetDescriptor<Pixmap>> getDependencies (String fileName, FileHandle file, PixmapParameter parameter) {
		return null;
	}

	public class PixmapParameter : AssetLoaderParameters<Pixmap> {
	}
}
