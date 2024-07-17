using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} for {@link Cubemap} instances. The pixel data is loaded asynchronously. The texture is then created on the
 * rendering thread, synchronously. Passing a {@link CubemapParameter} to
 * {@link AssetManager#load(String, Class, AssetLoaderParameters)} allows one to specify parameters as can be passed to the
 * various Cubemap constructors, e.g. filtering and so on.
 * @author mzechner, Vincent Bousquet */
public class CubemapLoader : AsynchronousAssetLoader<Cubemap, CubemapLoader.CubemapParameter>
{
	public class CubemapLoaderInfo
	{
		internal String filename;
		internal ICubemapData data;
		internal Cubemap cubemap;
	};

	CubemapLoaderInfo info = new CubemapLoaderInfo();

	public CubemapLoader(IFileHandleResolver resolver)
	: base(resolver)
	{
		
	}

	public override void loadAsync(AssetManager manager, String fileName, FileHandle file, CubemapParameter parameter)
	{
		info.filename = fileName;
		if (parameter == null || parameter.cubemapData == null)
		{
			Pixmap.Format? format = null;
			bool genMipMaps = false;
			info.cubemap = null;

			if (parameter != null)
			{
				format = parameter.format;
				info.cubemap = parameter.cubemap;
			}

			if (fileName.Contains(".ktx") || fileName.Contains(".zktx"))
			{
				info.data = new KTXTextureData(file, genMipMaps);
			}
		}
		else
		{
			info.data = parameter.cubemapData;
			info.cubemap = parameter.cubemap;
		}
		if (!info.data.isPrepared()) info.data.prepare();
	}

	public override Cubemap loadSync(AssetManager manager, String fileName, FileHandle file, CubemapParameter parameter)
	{
		if (info == null) return null;
		Cubemap cubemap = info.cubemap;
		if (cubemap != null)
		{
			cubemap.load(info.data);
		}
		else
		{
			cubemap = new Cubemap(info.data);
		}
		if (parameter != null)
		{
			cubemap.setFilter(parameter.minFilter, parameter.magFilter);
			cubemap.setWrap(parameter.wrapU, parameter.wrapV);
		}
		return cubemap;
	}

	public override Array<AssetDescriptor<Cubemap>> getDependencies(String fileName, FileHandle file, CubemapParameter parameter)
	{
		return null;
	}

	public class CubemapParameter : AssetLoaderParameters<Cubemap> {
		/** the format of the final Texture. Uses the source images format if null **/
		public Pixmap.Format? format = null;
	/** The texture to put the {@link TextureData} in, optional. **/
	public Cubemap cubemap = null;
	/** CubemapData for textures created on the fly, optional. When set, all format and genMipMaps are ignored */
	public ICubemapData cubemapData = null;
	public Texture.TextureFilter minFilter = Texture.TextureFilter.Nearest;
	public Texture.TextureFilter magFilter = Texture.TextureFilter.Nearest;
	public Texture.TextureWrap wrapU = Texture.TextureWrap.ClampToEdge;
	public Texture.TextureWrap wrapV = Texture.TextureWrap.ClampToEdge;
}
}
