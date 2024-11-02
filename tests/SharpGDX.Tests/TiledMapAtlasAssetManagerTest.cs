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
//public class TiledMapAtlasAssetManagerTest : GdxTest {
//
//	private TiledMap map;
//	private TiledMapRenderer renderer;
//	private OrthographicCamera camera;
//	private OrthoCamController cameraController;
//	private AssetManager assetManager;
//	private BitmapFont font;
//	private SpriteBatch batch;
//	String errorMessage;
//	private String fileName = "data/maps/tiled-atlas-processed/test.tmx";
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
//		AtlasTiledMapLoaderParameters params = new AtlasTiledMapLoaderParameters();
//		params.forceTextureFilters = true;
//		params.textureMinFilter = TextureFilter.Linear;
//		params.textureMagFilter = TextureFilter.Linear;
//
//		assetManager = new AssetManager();
//		assetManager.setErrorListener(new AssetErrorListener() {
//			@Override
//			public void error (AssetDescriptor asset, Throwable throwable) {
//				errorMessage = throwable.getMessage();
//			}
//		});
//
//		assetManager.setLoader(TiledMap.class, new AtlasTmxMapLoader(new InternalFileHandleResolver()));
//		assetManager.load(fileName, TiledMap.class);
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(100f / 255f, 100f / 255f, 250f / 255f, 1f);
//		camera.update();
//		assetManager.update(16);
//		if (renderer == null && assetManager.isLoaded(fileName)) {
//			map = assetManager.get(fileName);
//			renderer = new OrthogonalTiledMapRenderer(map, 1f / 32f);
//		} else if (renderer != null) {
//			renderer.setView(camera);
//			renderer.render();
//		}
//		batch.begin();
//		if (errorMessage != null) font.draw(batch, "ERROR (OK if running in GWT): " + errorMessage, 10, 50);
//		font.draw(batch, "FPS: " + Gdx.graphics.getFramesPerSecond(), 10, 20);
//		batch.end();
//	}
//}
