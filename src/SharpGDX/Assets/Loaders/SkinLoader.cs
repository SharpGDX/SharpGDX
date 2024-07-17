//using SharpGDX.Shims;
//using SharpGDX.Utils;
//using SharpGDX.Mathematics;

//namespace SharpGDX.Assets.Loaders;

///** {@link AssetLoader} for {@link Skin} instances. All {@link Texture} and {@link BitmapFont} instances will be loaded as
// * dependencies. Passing a {@link SkinParameter} allows the exact name of the texture associated with the skin to be specified.
// * Otherwise the skin texture is looked up just as with a call to {@link Skin#Skin(com.badlogic.gdx.files.FileHandle)}. A
// * {@link SkinParameter} also allows named resources to be set that will be added to the skin before loading the json file,
// * meaning that they can be referenced from inside the json file itself. This is useful for dynamic resources such as a BitmapFont
// * generated through FreeTypeFontGenerator.
// * @author Nathan Sweet */
//public class SkinLoader : AsynchronousAssetLoader<Skin, SkinLoader.SkinParameter> {
//	public SkinLoader (FileHandleResolver resolver) {
//		super(resolver);
//	}

//	@Override
//	public Array<AssetDescriptor> getDependencies (String fileName, FileHandle file, SkinParameter parameter) {
//		Array<AssetDescriptor> deps = new Array();
//		if (parameter == null || parameter.textureAtlasPath == null)
//			deps.add(new AssetDescriptor(file.pathWithoutExtension() + ".atlas", TextureAtlas.class));
//		else if (parameter.textureAtlasPath != null) deps.add(new AssetDescriptor(parameter.textureAtlasPath, TextureAtlas.class));
//		return deps;
//	}

//	@Override
//	public void loadAsync (AssetManager manager, String fileName, FileHandle file, SkinParameter parameter) {
//	}

//	@Override
//	public Skin loadSync (AssetManager manager, String fileName, FileHandle file, SkinParameter parameter) {
//		String textureAtlasPath = file.pathWithoutExtension() + ".atlas";
//		ObjectMap<String, Object> resources = null;
//		if (parameter != null) {
//			if (parameter.textureAtlasPath != null) {
//				textureAtlasPath = parameter.textureAtlasPath;
//			}
//			if (parameter.resources != null) {
//				resources = parameter.resources;
//			}
//		}
//		TextureAtlas atlas = manager.get(textureAtlasPath, TextureAtlas.class);
//		Skin skin = newSkin(atlas);
//		if (resources != null) {
//			for (Entry<String, Object> entry : resources.entries()) {
//				skin.add(entry.key, entry.value);
//			}
//		}
//		skin.load(file);
//		return skin;
//	}

//	/** Override to allow subclasses of Skin to be loaded or the skin instance to be configured.
//	 * @param atlas The TextureAtlas that the skin will use.
//	 * @return A new Skin (or subclass of Skin) instance based on the provided TextureAtlas. */
//	protected Skin newSkin (TextureAtlas atlas) {
//		return new Skin(atlas);
//	}

//	static public class SkinParameter extends AssetLoaderParameters<Skin> {
//		public final String textureAtlasPath;
//		public final ObjectMap<String, Object> resources;

//		public SkinParameter () {
//			this(null, null);
//		}

//		public SkinParameter (ObjectMap<String, Object> resources) {
//			this(null, resources);
//		}

//		public SkinParameter (String textureAtlasPath) {
//			this(textureAtlasPath, null);
//		}

//		public SkinParameter (String textureAtlasPath, ObjectMap<String, Object> resources) {
//			this.textureAtlasPath = textureAtlasPath;
//			this.resources = resources;
//		}
//	}
//}
