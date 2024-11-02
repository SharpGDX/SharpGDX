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
//using SharpGDX.Maps.Tiled;
//using SharpGDX.Assets;
//using SharpGDX.Assets.Loaders.Resolvers;
//using SharpGDX.Maps;
//using SharpGDX.Maps.Objects;
//using SharpGDX.Maps.Tiled;
//using SharpGDX.Maps.Tiled.Renderers;
//using SharpGDX.Assets.Loaders;
//using SharpGDX.Assets;
//using SharpGDX.Assets.Loaders.Resolvers;
//
//namespace SharpGDX.Tests;
//
//public class TideMapAssetManagerTest : GdxTest {
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
//		assetManager.setLoader(TiledMap.class, new TideMapLoader(new InternalFileHandleResolver()));
//		assetManager.load("data/maps/tide/Map01.tide", TiledMap.class);
//		assetManager.finishLoading();
//		map = assetManager.get("data/maps/tide/Map01.tide");
//		renderer = new OrthogonalTiledMapRenderer(map, 1f / 32f);
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.55f, 0.55f, 0.55f, 1f);
//		camera.update();
//		renderer.setView(camera);
//		renderer.render();
//		batch.begin();
//		font.draw(batch, "FPS: " + Gdx.graphics.getFramesPerSecond(), 10, 20);
//		batch.end();
//	}
//}
