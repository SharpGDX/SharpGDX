using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Utils;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} for {@link Texture} instances. The pixel data is loaded asynchronously. The texture is then created on the
 * rendering thread, synchronously. Passing a {@link TextureParameter} to
 * {@link AssetManager#load(String, Class, AssetLoaderParameters)} allows one to specify parameters as can be passed to the
 * various Texture constructors, e.g. filtering, whether to generate mipmaps and so on.
 * @author mzechner */
public class TextureLoader : AsynchronousAssetLoader<Texture, TextureLoader.TextureParameter> {
	 public class TextureLoaderInfo {
		internal String filename;
		internal ITextureData data;
		internal Texture texture;
	};

	TextureLoaderInfo info = new TextureLoaderInfo();

	public TextureLoader (IFileHandleResolver resolver) 
	: base(resolver)
	{
		
	}

	public override void loadAsync (AssetManager manager, String fileName, FileHandle file, TextureParameter parameter) {
		info.filename = fileName;
		if (parameter == null || parameter.textureData == null) {
			Pixmap.Format? format = null;
			bool genMipMaps = false;
			info.texture = null;

			if (parameter != null) {
				format = parameter.format;
				genMipMaps = parameter.genMipMaps;
				info.texture = parameter.texture;
			}

			info.data = ITextureData.Factory.LoadFromFile(file, format, genMipMaps);
		} else {
			info.data = parameter.textureData;
			info.texture = parameter.texture;
		}
		if (!info.data.isPrepared()) info.data.prepare();
	}

	public override Texture loadSync (AssetManager manager, String fileName, FileHandle file, TextureParameter parameter) {
		if (info == null) return null;
		Texture texture = info.texture;
		if (texture != null) {
			texture.load(info.data);
		} else {
			texture = new Texture(info.data);
		}
		if (parameter != null) {
			texture.setFilter(parameter.minFilter, parameter.magFilter);
			texture.setWrap(parameter.wrapU, parameter.wrapV);
		}
		return texture;
	}

	public override Array<AssetDescriptor<Texture>>? getDependencies (String fileName, FileHandle file, TextureParameter parameter) {
		return null;
	}

	 public class TextureParameter : AssetLoaderParameters<Texture> {
		/** the format of the final Texture. Uses the source images format if null **/
		public Pixmap.Format? format = null;
		/** whether to generate mipmaps **/
		public bool genMipMaps = false;
		/** The texture to put the {@link TextureData} in, optional. **/
		public Texture texture = null;
		/** TextureData for textures created on the fly, optional. When set, all format and genMipMaps are ignored */
		public ITextureData textureData = null;
		public Texture.TextureFilter minFilter = Texture.TextureFilter.Nearest;
		public Texture.TextureFilter magFilter = Texture.TextureFilter.Nearest;
		public Texture.TextureWrap wrapU = Texture.TextureWrap.ClampToEdge;
		public Texture.TextureWrap wrapV = Texture.TextureWrap.ClampToEdge;
	}
}
