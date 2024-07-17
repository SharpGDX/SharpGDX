using SharpGDX.Files;
using SharpGDX.Shims;
using static SharpGDX.Graphics.G2D.TextureAtlas;
using static SharpGDX.Graphics.Texture;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;
using static SharpGDX.Graphics.G2D.BitmapFont;
using SharpGDX.Mathematics;

namespace SharpGDX.Assets.Loaders;

/** {@link AssetLoader} for {@link BitmapFont} instances. Loads the font description file (.fnt) asynchronously, loads the
 * {@link Texture} containing the glyphs as a dependency. The {@link BitmapFontParameter} allows you to set things like texture
 * filters or whether to flip the glyphs vertically.
 * @author mzechner */
public class BitmapFontLoader : AsynchronousAssetLoader<BitmapFont, BitmapFontLoader.BitmapFontParameter>
{
	public BitmapFontLoader(IFileHandleResolver resolver)
		: base(resolver)
	{

	}

	BitmapFontData data;

	public override Array<AssetDescriptor<BitmapFont>> getDependencies(String fileName, FileHandle file,
		BitmapFontParameter parameter)
	{
		throw new NotImplementedException();
//		Array<AssetDescriptor> deps = new();
//		if (parameter != null && parameter.bitmapFontData != null)
//		{
//			data = parameter.bitmapFontData;
//			return deps;
//		}

//		data = new BitmapFontData(file, parameter != null && parameter.flip);
//		if (parameter != null && parameter.atlasName != null)
//		{
//			deps.add(new AssetDescriptor<TextureAtlas>(parameter.atlasName, typeof(TextureAtlas)));
//		} else {
//			for (int i = 0; i<data.getImagePaths().Length; i++) {
//				String path = data.getImagePath(i);
//FileHandle resolved = resolve(path);

//TextureLoader.TextureParameter textureParams = new TextureLoader.TextureParameter();

//				if (parameter != null) {
//					textureParams.genMipMaps = parameter.genMipMaps;
//					textureParams.minFilter = parameter.minFilter;
//					textureParams.magFilter = parameter.magFilter;
//				}

//				AssetDescriptor descriptor = new AssetDescriptor<Texture>(resolved, typeof(Texture), textureParams);
//deps.add(descriptor);
//			}
//		}

//		return deps;
	}

	public override void loadAsync(AssetManager manager, String fileName, FileHandle file,
		BitmapFontParameter parameter)
	{
	}

	public override BitmapFont loadSync(AssetManager manager, String fileName, FileHandle file,
		BitmapFontParameter parameter)
	{
		if (parameter != null && parameter.atlasName != null)
		{
			TextureAtlas atlas = manager.get<TextureAtlas>(parameter.atlasName, typeof(TextureAtlas));
			String name = file.sibling(data.imagePaths[0]).nameWithoutExtension().ToString();
			AtlasRegion region = atlas.findRegion(name);

			if (region == null)
				throw new GdxRuntimeException("Could not find font region " + name + " in atlas " +
				                              parameter.atlasName);
			return new BitmapFont(file, region);
		}
		else
		{
			int n = data.getImagePaths().Length;
			Array<TextureRegion> regs = new(n);
			for (int i = 0; i < n; i++)
			{
				regs.add(new TextureRegion(manager.get<Texture>(data.getImagePath(i), typeof(Texture))));
			}

			return new BitmapFont(data, regs, true);
		}
	}

	/** Parameter to be passed to {@link AssetManager#load(String, Class, AssetLoaderParameters)} if additional configuration is
	 * necessary for the {@link BitmapFont}.
	 * @author mzechner */
	public class BitmapFontParameter : AssetLoaderParameters<BitmapFont>
	{
		/** Flips the font vertically if {@code true}. Defaults to {@code false}. **/
		public bool flip = false;

		/** Generates mipmaps for the font if {@code true}. Defaults to {@code false}. **/
		public bool genMipMaps = false;

		/** The {@link TextureFilter} to use when scaling down the {@link BitmapFont}. Defaults to {@link TextureFilter#Nearest}. */
		public TextureFilter minFilter = TextureFilter.Nearest;

		/** The {@link TextureFilter} to use when scaling up the {@link BitmapFont}. Defaults to {@link TextureFilter#Nearest}. */
		public TextureFilter magFilter = TextureFilter.Nearest;

		/** optional {@link BitmapFontData} to be used instead of loading the {@link Texture} directly. Use this if your font is
		 * embedded in a {@link Skin}. **/
		public BitmapFontData bitmapFontData = null;

		/** The name of the {@link TextureAtlas} to load the {@link BitmapFont} itself from. Optional; if {@code null}, will look
		 * for a separate image */
		public String atlasName = null;
	}
}