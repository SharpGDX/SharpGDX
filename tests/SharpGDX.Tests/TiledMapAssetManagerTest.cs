//using SharpGDX.Tests.Utils;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using SharpGDX.Mathematics;
//using SharpGDX.Maps;
//using SharpGDX.Maps.Tiled;
//using SharpGDX.Maps.Tiled.Renderers;
//using SharpGDX.Assets.Loaders;
//using SharpGDX.Assets;
//using SharpGDX.Assets.Loaders.Resolvers;
//
//namespace SharpGDX.Tests;
//
//public class TiledMapAssetManagerTest : GdxTest {
//
//	private static readonly String MAP_PROPERTY_NAME = "mapCustomProperty";
//	private static readonly String BOOL_PROPERTY_NAME = "boolCustomProperty";
//	private static readonly String INT_PROPERTY_NAME = "intCustomProperty";
//	private static readonly String FLOAT_PROPERTY_NAME = "floatCustomProperty";
//
//	private static readonly String TILESET_PROPERTY_NAME = "tilesetCustomProperty";
//	private static readonly String TILE_PROPERTY_NAME = "tileCustomProperty";
//	private static readonly String LAYER_PROPERTY_NAME = "layerCustomProperty";
//
//	private static readonly String MAP_PROPERTY_VALUE = "mapCustomValue";
//	private static readonly bool BOOL_PROPERTY_VALUE = true;
//	private static readonly int INT_PROPERTY_VALUE = 5;
//	private static readonly float FLOAT_PROPERTY_VALUE = 1.56f;
//
//	private static readonly String TILESET_PROPERTY_VALUE = "tilesetCustomValue";
//	private static readonly String TILE_PROPERTY_VALUE = "tileCustomValue";
//	private static readonly String LAYER_PROPERTY_VALUE = "layerCustomValue";
//
//	private TiledMap map;
//	private TiledMapRenderer renderer;
//	private OrthographicCamera camera;
//	private OrthoCamController cameraController;
//	private AssetManager assetManager;
//	private BitmapFont font;
//	private SpriteBatch batch;
//
//	public override void Create () {
//		float w = Gdx.graphics.getWidth();
//		float h = Gdx.graphics.getHeight();
//
//		camera = new OrthographicCamera();
//		camera.setToOrtho(false, (w / h) * 10, 10);
//		camera.zoom = 2;
//		camera.update();
//
//		cameraController = new OrthoCamController(camera);
//		Gdx.input.setInputProcessor(cameraController);
//
//		font = new BitmapFont();
//		batch = new SpriteBatch();
//
//		assetManager = new AssetManager();
//		assetManager.setLoader(TiledMap.class, new TmxMapLoader(new InternalFileHandleResolver()));
//		assetManager.load("data/maps/tiled/isometric_grass_and_water.tmx", TiledMap.class);
//		assetManager.finishLoading();
//		map = assetManager.get("data/maps/tiled/isometric_grass_and_water.tmx");
//		renderer = new IsometricTiledMapRenderer(map, 1f / 64f);
//
//		String mapCustomValue = map.getProperties().get(MAP_PROPERTY_NAME, String.class);
//		Gdx.app.log("TiledMapAssetManagerTest", "Property : " + MAP_PROPERTY_NAME + ", Value : " + mapCustomValue);
//		if (!MAP_PROPERTY_VALUE.Equals(mapCustomValue)) {
//			throw new RuntimeException("Failed to get map properties");
//		}
//
//		bool boolCustomValue = map.getProperties().get(BOOL_PROPERTY_NAME, Boolean.class);
//		Gdx.app.log("TiledMapAssetManagerTest", "Property : " + BOOL_PROPERTY_NAME + ", Value : " + boolCustomValue);
//		if (boolCustomValue != BOOL_PROPERTY_VALUE) {
//			throw new RuntimeException("Failed to get boolean map properties");
//		}
//
//		int intCustomValue = map.getProperties().get(INT_PROPERTY_NAME, Integer.class);
//		Gdx.app.log("TiledMapAssetManagerTest", "Property : " + INT_PROPERTY_NAME + ", Value : " + intCustomValue);
//		if (intCustomValue != INT_PROPERTY_VALUE) {
//			throw new RuntimeException("Failed to get int map properties");
//		}
//
//		float floatCustomValue = map.getProperties().get(FLOAT_PROPERTY_NAME, Float.class);
//		Gdx.app.log("TiledMapAssetManagerTest", "Property : " + FLOAT_PROPERTY_NAME + ", Value : " + floatCustomValue);
//		if (floatCustomValue != FLOAT_PROPERTY_VALUE) {
//			throw new RuntimeException("Failed to get float map properties");
//		}
//
//		TiledMapTileSet tileset = map.getTileSets().getTileSet(0);
//		String tilesetCustomValue = tileset.getProperties().get(TILESET_PROPERTY_NAME, String.class);
//		if (!TILESET_PROPERTY_VALUE.Equals(tilesetCustomValue)) {
//			throw new RuntimeException("Failed to get tileset properties");
//		}
//
//		TiledMapTile tile = tileset.getTile(1);
//		String tileCustomValue = tile.getProperties().get(TILE_PROPERTY_NAME, String.class);
//		if (!TILE_PROPERTY_VALUE.Equals(tileCustomValue)) {
//			throw new RuntimeException("Failed to get tile properties");
//		}
//
//		MapLayer layer = map.getLayers().get(0);
//		String layerCustomValue = layer.getProperties().get(LAYER_PROPERTY_NAME, String.class);
//		if (!LAYER_PROPERTY_VALUE.Equals(layerCustomValue)) {
//			throw new RuntimeException("Failed to get layer properties");
//		}
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(100f / 255f, 100f / 255f, 250f / 255f, 1f);
//		camera.update();
//		renderer.setView(camera);
//		renderer.render();
//		batch.begin();
//		font.draw(batch, "FPS: " + Gdx.graphics.getFramesPerSecond(), 10, 20);
//		batch.end();
//	}
//}
