using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Utils;
using SharpGDX;
using SharpGDX.Mathematics;
using SharpGDX.Assets;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} to load {@link TextureAtlas} instances. Passing a {@link TextureAtlasParameter} to
 * {@link AssetManager#load(String, Class, AssetLoaderParameters)} allows to specify whether the atlas regions should be flipped
 * on the y-axis or not.
 * @author mzechner */
public class TextureAtlasLoader : SynchronousAssetLoader<TextureAtlas, TextureAtlasLoader.TextureAtlasParameter>
{
	public TextureAtlasLoader(IFileHandleResolver resolver)
		: base(resolver)
	{

	}

	TextureAtlas.TextureAtlasData data;

	public override TextureAtlas load(AssetManager assetManager, String fileName, FileHandle file,
		TextureAtlasParameter parameter)
	{
		foreach (var page in data.getPages())
		{
			Texture texture = assetManager.get<Texture>(page.textureFile.path().Replace("\\\\", "/"), typeof(Texture));
			page.texture = texture;
		}

		TextureAtlas atlas = new TextureAtlas(data);
		data = null;
		return atlas;
	}

	public override Array<AssetDescriptor<TextureAtlas>> getDependencies(String fileName, FileHandle atlasFile,
		TextureAtlasParameter parameter)
	{
		FileHandle imgDir = atlasFile.parent();

		if (parameter != null)
			data = new TextureAtlas.TextureAtlasData(atlasFile, imgDir, parameter.flip);
		else
		{
			data = new TextureAtlas.TextureAtlasData(atlasFile, imgDir, false);
		}

		Array<IAssetDescriptor> dependencies = new();
		foreach (var page in data.getPages())
		{
			TextureLoader.TextureParameter @params = new TextureLoader.TextureParameter();
			@params.format = page.format;
			@params.genMipMaps = page.useMipMaps;
			@params.minFilter = page.minFilter;
			@params.magFilter = page.magFilter;
			// TODO: Is AssetDescriptor<Texture> right? -RP
			dependencies.add(new AssetDescriptor<Texture>(page.textureFile, typeof(Texture), @params));
		}

		throw new NotImplementedException();
		// TODO: You can't return all of these as Array<AssetDescriptor<TextureAtlas>>, because they aren't. -RP
		// return dependencies;
	}

	public class TextureAtlasParameter : AssetLoaderParameters<TextureAtlas>
	{
		/** whether to flip the texture atlas vertically **/
		public bool flip = false;

		public TextureAtlasParameter()
		{
		}

		public TextureAtlasParameter(bool flip)
		{
			this.flip = flip;
		}
	}
}