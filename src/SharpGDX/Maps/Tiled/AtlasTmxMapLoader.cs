using SharpGDX.Files;
using System.Collections;
using static SharpGDX.Graphics.G2D.TextureAtlas;
using static SharpGDX.Utils.XmlReader;
using SharpGDX.Assets.Loaders.Resolvers;
using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;

namespace SharpGDX.Maps.Tiled;

/** A TiledMap Loader which loads tiles from a TextureAtlas instead of separate images.
 * 
 * It requires a map-level property called 'atlas' with its value being the relative path to the TextureAtlas. The atlas must have
 * in it indexed regions named after the tilesets used in the map. The indexes shall be local to the tileset (not the global id).
 * Strip whitespace and rotation should not be used when creating the atlas.
 * 
 * @author Justin Shapcott
 * @author Manuel Bua */
public class AtlasTmxMapLoader : BaseTmxMapLoader<AtlasTmxMapLoader.AtlasTiledMapLoaderParameters> {

	public class AtlasTiledMapLoaderParameters : BaseTmxMapLoader<AtlasTiledMapLoaderParameters>.Parameters {
		/** force texture filters? **/
		public bool forceTextureFilters = false;
	}

	protected interface IAtlasResolver : ImageResolver {

		public TextureAtlas getAtlas ();

		public  class DirectAtlasResolver : AtlasTmxMapLoader.IAtlasResolver {
			private readonly TextureAtlas atlas;

			public DirectAtlasResolver (TextureAtlas atlas) {
				this.atlas = atlas;
			}

			public TextureAtlas getAtlas () {
				return atlas;
			}

			public TextureRegion getImage (String name) {
				return atlas.findRegion(name);
			}
		}

		public  class AssetManagerAtlasResolver : AtlasTmxMapLoader.IAtlasResolver {
			private readonly AssetManager assetManager;
			private readonly String atlasName;

			public AssetManagerAtlasResolver (AssetManager assetManager, String atlasName) {
				this.assetManager = assetManager;
				this.atlasName = atlasName;
			}

			public TextureAtlas getAtlas () {
				return assetManager.get< TextureAtlas>(atlasName, typeof(TextureAtlas));
			}

			public TextureRegion getImage (String name) {
				return getAtlas().findRegion(name);
			}
		}
	}

	protected Array<Texture> trackedTextures = new Array<Texture>();

	protected IAtlasResolver atlasResolver;

	public AtlasTmxMapLoader () 
	: base(new InternalFileHandleResolver())
	{
		
	}

	public AtlasTmxMapLoader (IFileHandleResolver resolver) 
	: base(resolver)
	{
		
	}

	public TiledMap load (String fileName) {
		return load(fileName, new AtlasTiledMapLoaderParameters());
	}

	public TiledMap load (String fileName, AtlasTiledMapLoaderParameters parameter)
	{
		throw new NotImplementedException();
		//FileHandle tmxFile = resolve(fileName);

		//this.root = xml.parse(tmxFile);

		//FileHandle atlasFileHandle = getAtlasFileHandle(tmxFile);
		//TextureAtlas atlas = new TextureAtlas(atlasFileHandle);
		//this.atlasResolver = new AtlasResolver.DirectAtlasResolver(atlas);

		//TiledMap map = loadTiledMap(tmxFile, parameter, atlasResolver);
		//map.setOwnedResources(new Array<TextureAtlas>(new TextureAtlas[] {atlas}));
		//setTextureFilters(parameter.textureMinFilter, parameter.textureMagFilter);
		//return map;
	}

	public override void loadAsync (AssetManager manager, String fileName, FileHandle tmxFile, AtlasTiledMapLoaderParameters parameter)
	{
		throw new NotImplementedException();
		//FileHandle atlasHandle = getAtlasFileHandle(tmxFile);
		//this.atlasResolver = new AtlasResolver.AssetManagerAtlasResolver(manager, atlasHandle.path());

		//this.map = loadTiledMap(tmxFile, parameter, atlasResolver);
	}

	public override TiledMap loadSync (AssetManager manager, String fileName, FileHandle file, AtlasTiledMapLoaderParameters parameter) {
		if (parameter != null) {
			setTextureFilters(parameter.textureMinFilter, parameter.textureMagFilter);
		}

		return map;
	}

	protected override Array<IAssetDescriptor> getDependencyAssetDescriptors (FileHandle tmxFile,
		TextureLoader.TextureParameter textureParameter)
	{
		throw new NotImplementedException();
		//Array<AssetDescriptor> descriptors = new Array<AssetDescriptor>();

		//// Atlas dependencies
		// FileHandle atlasFileHandle = getAtlasFileHandle(tmxFile);
		//if (atlasFileHandle != null) {
		//	descriptors.add(new AssetDescriptor(atlasFileHandle, TextureAtlas.class));
		//}

		//return descriptors;
	}

	protected override void addStaticTiles (FileHandle tmxFile, ImageResolver imageResolver, TiledMapTileSet tileSet, Element element,
		Array<Element> tileElements, String name, int firstgid, int tilewidth, int tileheight, int spacing, int margin,
		String source, int offsetX, int offsetY, String imageSource, int imageWidth, int imageHeight, FileHandle image) {

		TextureAtlas atlas = atlasResolver.getAtlas();
		String regionsName = name;

		foreach (Texture texture in atlas.getTextures()) {
			trackedTextures.add(texture);
		}

		MapProperties props = tileSet.getProperties();
		props.put("imagesource", imageSource);
		props.put("imagewidth", imageWidth);
		props.put("imageheight", imageHeight);
		props.put("tilewidth", tilewidth);
		props.put("tileheight", tileheight);
		props.put("margin", margin);
		props.put("spacing", spacing);

		if (imageSource != null && imageSource.Length > 0) {
			int lastgid = firstgid + ((imageWidth / tilewidth) * (imageHeight / tileheight)) - 1;
			foreach (AtlasRegion region in atlas.findRegions(regionsName)) {
				// Handle unused tileIds
				if (region != null) {
					int tileId = firstgid + region.index;
					if (tileId >= firstgid && tileId <= lastgid) {
						addStaticTiledMapTile(tileSet, region, tileId, offsetX, offsetY);
					}
				}
			}
		}

		// Add tiles with individual image sources
		foreach (Element tileElement in tileElements) {
			int tileId = firstgid + tileElement.getIntAttribute("id", 0);
			ITiledMapTile tile = tileSet.getTile(tileId);
			if (tile == null) {
				Element imageElement = tileElement.getChildByName("image");
				if (imageElement != null) {
					String regionName = imageElement.getAttribute("source");
					regionName = regionName.Substring(0, regionName.LastIndexOf('.'));
					AtlasRegion region = atlas.findRegion(regionName);
					if (region == null) throw new GdxRuntimeException("Tileset atlasRegion not found: " + regionName);
					addStaticTiledMapTile(tileSet, region, tileId, offsetX, offsetY);
				}
			}
		}
	}

	protected FileHandle getAtlasFileHandle (FileHandle tmxFile) {
		Element properties = root.getChildByName("properties");

		String atlasFilePath = null;
		if (properties != null) {
			foreach (Element property in properties.getChildrenByName("property")) {
				String name = property.getAttribute("name");
				if (name.StartsWith("atlas")) {
					atlasFilePath = property.getAttribute("value");
					break;
				}
			}
		}
		if (atlasFilePath == null) {
			throw new GdxRuntimeException("The map is missing the 'atlas' property");
		} else {
			 FileHandle fileHandle = getRelativeFileHandle(tmxFile, atlasFilePath);
			if (!fileHandle.exists()) {
				throw new GdxRuntimeException("The 'atlas' file could not be found: '" + atlasFilePath + "'");
			}
			return fileHandle;
		}
	}

	protected void setTextureFilters (Texture.TextureFilter min, Texture.TextureFilter mag) {
		foreach (Texture texture in trackedTextures) {
			texture.setFilter(min, mag);
		}
		trackedTextures.clear();
	}
}
