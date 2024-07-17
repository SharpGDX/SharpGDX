using SharpGDX.Files;
using System.Collections;
using static SharpGDX.Maps.Tiled.TiledMapTileLayer;
using SharpGDX.Maps;
using SharpGDX.Maps.Objects;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Assets.Loaders.Resolvers;
using static SharpGDX.Maps.ImageResolver;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;
using SharpGDX.Assets;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using static SharpGDX.Utils.XmlReader;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;

namespace SharpGDX.Maps.Tiled;

public class TideMapLoader : SynchronousAssetLoader<TiledMap, TideMapLoader.Parameters> {

	public  class Parameters : AssetLoaderParameters<TiledMap> {

	}

	private XmlReader xml = new XmlReader();
	private Element root;

	public TideMapLoader () 
	: base(new InternalFileHandleResolver())
	{
		
	}

	public TideMapLoader (IFileHandleResolver resolver) 
	: base(resolver)
	{
		
	}

	public TiledMap load (String fileName)
	{
		throw new NotImplementedException();
		//try {
		//	FileHandle tideFile = resolve(fileName);
		//	root = xml.parse(tideFile);
		//	ObjectMap<String, Texture> textures = new ObjectMap<String, Texture>();
		//	foreach (FileHandle textureFile in loadTileSheets(root, tideFile)) {
		//		textures.put(textureFile.path(), new Texture(textureFile));
		//	}
		//	DirectImageResolver imageResolver = new DirectImageResolver(textures);
		//	TiledMap map = loadMap(root, tideFile, imageResolver);
		//	map.setOwnedResources(textures.values().toArray());
		//	return map;
		//} catch (IOException e) {
		//	throw new GdxRuntimeException("Couldn't load tilemap '" + fileName + "'", e);
		//}

	}

	public override TiledMap load (AssetManager assetManager, String fileName, FileHandle tideFile, Parameters parameter) {
		try {
			return loadMap(root, tideFile, new AssetManagerImageResolver(assetManager));
		} catch (Exception e) {
			throw new GdxRuntimeException("Couldn't load tilemap '" + fileName + "'", e);
		}
	}

	public override Array<AssetDescriptor<TiledMap>> getDependencies (String fileName, FileHandle tmxFile, Parameters parameter)
	{
		throw new NotImplementedException();
		//Array<AssetDescriptor> dependencies = new Array<AssetDescriptor>();
		//try {
		//	root = xml.parse(tmxFile);
		//	foreach (FileHandle image in loadTileSheets(root, tmxFile)) {
		//		dependencies.add(new AssetDescriptor(image.path(), typeof(Texture)));
		//	}
		//	return dependencies;
		//} catch (IOException e) {
		//	throw new GdxRuntimeException("Couldn't load tilemap '" + fileName + "'", e);
		//}
	}

	/** Loads the map data, given the XML root element and an {@link ImageResolver} used to return the tileset Textures
	 * @param root the XML root element
	 * @param tmxFile the Filehandle of the tmx file
	 * @param imageResolver the {@link ImageResolver}
	 * @return the {@link TiledMap} */
	private TiledMap loadMap (Element root, FileHandle tmxFile, ImageResolver imageResolver) {
		TiledMap map = new TiledMap();
		Element properties = root.getChildByName("Properties");
		if (properties != null) {
			loadProperties(map.getProperties(), properties);
		}
		Element tilesheets = root.getChildByName("TileSheets");
		foreach (Element tilesheet in tilesheets.getChildrenByName("TileSheet")) {
			loadTileSheet(map, tilesheet, tmxFile, imageResolver);
		}
		Element layers = root.getChildByName("Layers");
		foreach (Element layer in layers.getChildrenByName("Layer")) {
			loadLayer(map, layer);
		}
		return map;
	}

	/** Loads the tilesets
	 * @param root the root XML element
	 * @return a list of filenames for images containing tiles
	 * @throws IOException */
	private Array<FileHandle> loadTileSheets (Element root, FileHandle tideFile) // TODO: throws IOException 
	{
		Array<FileHandle> images = new Array<FileHandle>();
		Element tilesheets = root.getChildByName("TileSheets");
		foreach (Element tileset in tilesheets.getChildrenByName("TileSheet")) {
			Element imageSource = tileset.getChildByName("ImageSource");
			FileHandle image = getRelativeFileHandle(tideFile, imageSource.getText());
			images.add(image);
		}
		return images;
	}

	private void loadTileSheet (TiledMap map, Element element, FileHandle tideFile, ImageResolver imageResolver)
	{
		throw new NotImplementedException();
		//if (element.getName().Equals("TileSheet")) {
		//	String id = element.getAttribute("Id");
		//	String description = element.getChildByName("Description").getText();
		//	String imageSource = element.getChildByName("ImageSource").getText();

		//	Element alignment = element.getChildByName("Alignment");
		//	String sheetSize = alignment.getAttribute("SheetSize");
		//	String tileSize = alignment.getAttribute("TileSize");
		//	String margin = alignment.getAttribute("Margin");
		//	String spacing = alignment.getAttribute("Spacing");

		//	String[] sheetSizeParts = sheetSize.Split(" x ");
		//	int sheetSizeX = int.Parse(sheetSizeParts[0]);
		//	int sheetSizeY = int.Parse(sheetSizeParts[1]);

		//	String[] tileSizeParts = tileSize.Split(" x ");
		//	int tileSizeX = int.Parse(tileSizeParts[0]);
		//	int tileSizeY = int.Parse(tileSizeParts[1]);

		//	String[] marginParts = margin.Split(" x ");
		//	int marginX = int.Parse(marginParts[0]);
		//	int marginY = int.Parse(marginParts[1]);

		//	String[] spacingParts = margin.Split(" x ");
		//	int spacingX = int.Parse(spacingParts[0]);
		//	int spacingY = int.Parse(spacingParts[1]);

		//	FileHandle image = getRelativeFileHandle(tideFile, imageSource);
		//	TextureRegion texture = imageResolver.getImage(image.path());

		//	TiledMapTileSets tilesets = map.getTileSets();
		//	int firstgid = 1;
		//	foreach (TiledMapTileSet tileset in tilesets) {
		//		firstgid += tileset.size();
		//	}

		//	TiledMapTileSet tileset = new TiledMapTileSet();
		//	tileset.setName(id);
		//	tileset.getProperties().put("firstgid", firstgid);
		//	int gid = firstgid;

		//	int stopWidth = texture.getRegionWidth() - tileSizeX;
		//	int stopHeight = texture.getRegionHeight() - tileSizeY;

		//	for (int y = marginY; y <= stopHeight; y += tileSizeY + spacingY) {
		//		for (int x = marginX; x <= stopWidth; x += tileSizeX + spacingX) {
		//			TiledMapTile tile = new StaticTiledMapTile(new TextureRegion(texture, x, y, tileSizeX, tileSizeY));
		//			tile.setId(gid);
		//			tileset.putTile(gid++, tile);
		//		}
		//	}

		//	Element properties = element.getChildByName("Properties");
		//	if (properties != null) {
		//		loadProperties(tileset.getProperties(), properties);
		//	}

		//	tilesets.addTileSet(tileset);
		//}
	}

	private void loadLayer (TiledMap map, Element element)
	{
		throw new NotImplementedException();
		//if (element.getName().Equals("Layer")) {
		//	String id = element.getAttribute("Id");
		//	String visible = element.getAttribute("Visible");

		//	Element dimensions = element.getChildByName("Dimensions");
		//	String layerSize = dimensions.getAttribute("LayerSize");
		//	String tileSize = dimensions.getAttribute("TileSize");

		//	String[] layerSizeParts = layerSize.Split(" x ");
		//	int layerSizeX = int.Parse(layerSizeParts[0]);
		//	int layerSizeY = int.Parse(layerSizeParts[1]);

		//	String[] tileSizeParts = tileSize.Split(" x ");
		//	int tileSizeX = int.Parse(tileSizeParts[0]);
		//	int tileSizeY = int.Parse(tileSizeParts[1]);

		//	TiledMapTileLayer layer = new TiledMapTileLayer(layerSizeX, layerSizeY, tileSizeX, tileSizeY);
		//	layer.setName(id);
		//	layer.setVisible(visible.equalsIgnoreCase("True"));
		//	Element tileArray = element.getChildByName("TileArray");
		//	Array<Element> rows = tileArray.getChildrenByName("Row");
		//	TiledMapTileSets tilesets = map.getTileSets();
		//	TiledMapTileSet currentTileSet = null;
		//	int firstgid = 0;
		//	int x, y;
		//	for (int row = 0, rowCount = rows.size; row < rowCount; row++) {
		//		Element currentRow = rows.get(row);
		//		y = rowCount - 1 - row;
		//		x = 0;
		//		for (int child = 0, childCount = currentRow.getChildCount(); child < childCount; child++) {
		//			Element currentChild = currentRow.getChild(child);
		//			String name = currentChild.getName();
		//			if (name.Equals("TileSheet")) {
		//				currentTileSet = tilesets.getTileSet(currentChild.getAttribute("Ref"));
		//				firstgid = currentTileSet.getProperties().get("firstgid", typeof(int));
		//			} else if (name.Equals("Null")) {
		//				x += currentChild.getIntAttribute("Count");
		//			} else if (name.Equals("Static")) {
		//				Cell cell = new Cell();
		//				cell.setTile(currentTileSet.getTile(firstgid + currentChild.getIntAttribute("Index")));
		//				layer.setCell(x++, y, cell);
		//			} else if (name.Equals("Animated")) {
		//				// Create an AnimatedTile
		//				int interval = currentChild.getInt("Interval");
		//				Element frames = currentChild.getChildByName("Frames");
		//				Array<StaticTiledMapTile> frameTiles = new Array<StaticTiledMapTile>();
		//				for (int frameChild = 0, frameChildCount = frames.getChildCount(); frameChild < frameChildCount; frameChild++) {
		//					Element frame = frames.getChild(frameChild);
		//					String frameName = frame.getName();
		//					if (frameName.equals("TileSheet")) {
		//						currentTileSet = tilesets.getTileSet(frame.getAttribute("Ref"));
		//						firstgid = currentTileSet.getProperties().get("firstgid", typeof(int));
		//					} else if (frameName.equals("Static")) {
		//						frameTiles.add((StaticTiledMapTile)currentTileSet.getTile(firstgid + frame.getIntAttribute("Index")));
		//					}
		//				}
		//				Cell cell = new Cell();
		//				cell.setTile(new AnimatedTiledMapTile(interval / 1000f, frameTiles));
		//				layer.setCell(x++, y, cell); // TODO: Reuse existing animated tiles
		//			}
		//		}
		//	}

		//	Element properties = element.getChildByName("Properties");
		//	if (properties != null) {
		//		loadProperties(layer.getProperties(), properties);
		//	}

		//	map.getLayers().add(layer);
		//}
	}

	private void loadProperties (MapProperties properties, Element element)
	{
		throw new NotImplementedException();
		//if (element.getName().Equals("Properties")) {
		//	foreach (Element property in element.getChildrenByName("Property")) {
		//		String key = property.getAttribute("Key", null);
		//		String type = property.getAttribute("Type", null);
		//		String value = property.getText();

		//		if (type.Equals("Int32")) {
		//			properties.put(key, int.Parse(value));
		//		} else if (type.Equals("String")) {
		//			properties.put(key, value);
		//		} else if (type.Equals("Boolean")) {
		//			properties.put(key, value.equalsIgnoreCase("true"));
		//		} else {
		//			properties.put(key, value);
		//		}
		//	}
		//}
	}

	private static FileHandle getRelativeFileHandle (FileHandle file, String path)
	{
		throw new NotImplementedException();
		//StringTokenizer tokenizer = new StringTokenizer(path, "\\/");
		//FileHandle result = file.parent();
		//while (tokenizer.hasMoreElements()) {
		//	String token = tokenizer.nextToken();
		//	if (token.Equals(".."))
		//		result = result.parent();
		//	else {
		//		result = result.child(token);
		//	}
		//}
		//return result;
	}

}
