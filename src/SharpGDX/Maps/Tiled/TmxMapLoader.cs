using SharpGDX.Files;
using System.Collections;
using static SharpGDX.Maps.ImageResolver;
using static SharpGDX.Utils.XmlReader;
using SharpGDX.Maps.Tiled;
using SharpGDX.Maps.Objects;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
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

/** @brief synchronous loader for TMX maps created with the Tiled tool */
public class TmxMapLoader : BaseTmxMapLoader<TmxMapLoader.Parameters> {

	public new class Parameters : BaseTmxMapLoader<Parameters>.Parameters {

	}

	public TmxMapLoader () 
	: base(new InternalFileHandleResolver())
	{
		
	}

	/** Creates loader
	 * 
	 * @param resolver */
	public TmxMapLoader (IFileHandleResolver resolver) 
	: base(resolver)
	{
		
	}

	/** Loads the {@link TiledMap} from the given file. The file is resolved via the {@link FileHandleResolver} set in the
	 * constructor of this class. By default it will resolve to an internal file. The map will be loaded for a y-up coordinate
	 * system.
	 * @param fileName the filename
	 * @return the TiledMap */
	public TiledMap load (String fileName) {
		return load(fileName, new TmxMapLoader.Parameters());
	}

	/** Loads the {@link TiledMap} from the given file. The file is resolved via the {@link FileHandleResolver} set in the
	 * constructor of this class. By default it will resolve to an internal file.
	 * @param fileName the filename
	 * @param parameter specifies whether to use y-up, generate mip maps etc.
	 * @return the TiledMap */
	public TiledMap load (String fileName, TmxMapLoader.Parameters parameter)
	{
		throw new NotImplementedException();
		//FileHandle tmxFile = resolve(fileName);

		//this.root = xml.parse(tmxFile);

		//ObjectMap<String, Texture> textures = new ObjectMap<String, Texture>();

		// Array<FileHandle> textureFiles = getDependencyFileHandles(tmxFile);
		//foreach (FileHandle textureFile in textureFiles) {
		//	Texture texture = new Texture(textureFile, parameter.generateMipMaps);
		//	texture.setFilter(parameter.textureMinFilter, parameter.textureMagFilter);
		//	textures.put(textureFile.path(), texture);
		//}

		//TiledMap map = loadTiledMap(tmxFile, parameter, new DirectImageResolver(textures));
		//map.setOwnedResources(textures.values().toArray());
		//return map;
	}

	public override void loadAsync (AssetManager manager, String fileName, FileHandle tmxFile, Parameters parameter) {
		this.map = loadTiledMap(tmxFile, parameter, new AssetManagerImageResolver(manager));
	}

	public override TiledMap loadSync (AssetManager manager, String fileName, FileHandle file, Parameters parameter) {
		return map;
	}

	protected override Array<IAssetDescriptor> getDependencyAssetDescriptors (FileHandle tmxFile,
		TextureLoader.TextureParameter textureParameter)
	{
		throw new NotImplementedException();
		//Array<AssetDescriptor> descriptors = new Array<AssetDescriptor>();

		//Array<FileHandle> fileHandles = getDependencyFileHandles(tmxFile);
		//foreach (FileHandle handle in fileHandles) {
		//	descriptors.add(new AssetDescriptor(handle, typeof(Texture), textureParameter));
		//}

		//return descriptors;
	}

	protected Array<FileHandle> getDependencyFileHandles (FileHandle tmxFile)
	{
		throw new NotImplementedException();
		//Array<FileHandle> fileHandles = new Array<FileHandle>();

		//// TileSet descriptors
		//foreach (var tileset in root.getChildrenByName("tileset")) {
		//	String source = tileset.getAttribute("source", null);
		//	if (source != null) {
		//		FileHandle tsxFile = getRelativeFileHandle(tmxFile, source);
		//		tileset = xml.parse(tsxFile);
		//		Element imageElement = tileset.getChildByName("image");
		//		if (imageElement != null) {
		//			String imageSource = tileset.getChildByName("image").getAttribute("source");
		//			FileHandle image = getRelativeFileHandle(tsxFile, imageSource);
		//			fileHandles.add(image);
		//		} else {
		//			foreach (Element tile in tileset.getChildrenByName("tile")) {
		//				String imageSource = tile.getChildByName("image").getAttribute("source");
		//				FileHandle image = getRelativeFileHandle(tsxFile, imageSource);
		//				fileHandles.add(image);
		//			}
		//		}
		//	} else {
		//		Element imageElement = tileset.getChildByName("image");
		//		if (imageElement != null) {
		//			String imageSource = tileset.getChildByName("image").getAttribute("source");
		//			FileHandle image = getRelativeFileHandle(tmxFile, imageSource);
		//			fileHandles.add(image);
		//		} else {
		//			foreach (Element tile in tileset.getChildrenByName("tile")) {
		//				String imageSource = tile.getChildByName("image").getAttribute("source");
		//				FileHandle image = getRelativeFileHandle(tmxFile, imageSource);
		//				fileHandles.add(image);
		//			}
		//		}
		//	}
		//}

		//// ImageLayer descriptors
		//foreach (Element imageLayer in root.getChildrenByName("imagelayer")) {
		//	Element image = imageLayer.getChildByName("image");
		//	String source = image.getAttribute("source", null);

		//	if (source != null) {
		//		FileHandle handle = getRelativeFileHandle(tmxFile, source);
		//		fileHandles.add(handle);
		//	}
		//}

		//return fileHandles;
	}

	protected override void addStaticTiles (FileHandle tmxFile, ImageResolver imageResolver, TiledMapTileSet tileSet, Element element,
		Array<Element> tileElements, String name, int firstgid, int tilewidth, int tileheight, int spacing, int margin,
		String source, int offsetX, int offsetY, String imageSource, int imageWidth, int imageHeight, FileHandle image) {

		MapProperties props = tileSet.getProperties();
		if (image != null) {
			// One image for the whole tileSet
			TextureRegion texture = imageResolver.getImage(image.path());

			props.put("imagesource", imageSource);
			props.put("imagewidth", imageWidth);
			props.put("imageheight", imageHeight);
			props.put("tilewidth", tilewidth);
			props.put("tileheight", tileheight);
			props.put("margin", margin);
			props.put("spacing", spacing);

			int stopWidth = texture.getRegionWidth() - tilewidth;
			int stopHeight = texture.getRegionHeight() - tileheight;

			int id = firstgid;

			for (int y = margin; y <= stopHeight; y += tileheight + spacing) {
				for (int x = margin; x <= stopWidth; x += tilewidth + spacing) {
					TextureRegion tileRegion = new TextureRegion(texture, x, y, tilewidth, tileheight);
					int tileId = id++;
					addStaticTiledMapTile(tileSet, tileRegion, tileId, offsetX, offsetY);
				}
			}
		} else {
			// Every tile has its own image source
			foreach (Element tileElement in tileElements) {
				Element imageElement = tileElement.getChildByName("image");
				if (imageElement != null) {
					imageSource = imageElement.getAttribute("source");

					if (source != null) {
						image = getRelativeFileHandle(getRelativeFileHandle(tmxFile, source), imageSource);
					} else {
						image = getRelativeFileHandle(tmxFile, imageSource);
					}
				}
				TextureRegion texture = imageResolver.getImage(image.path());
				int tileId = firstgid + tileElement.getIntAttribute("id");
				addStaticTiledMapTile(tileSet, texture, tileId, offsetX, offsetY);
			}
		}
	}
}
