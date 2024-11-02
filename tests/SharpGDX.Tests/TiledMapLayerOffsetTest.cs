//using SharpGDX.Tests.Utils;
//using SharpGDX.Assets.Loaders.Resolvers;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using SharpGDX.Maps.Tiled;
//using SharpGDX.Maps.Tiled.Renderers;
//using SharpGDX.Assets;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
//public class TiledMapLayerOffsetTest : GdxTest {
//	private readonly static String MAP_ORTHO = "data/maps/tiled-offsets/ortho.tmx";
//	private readonly static String MAP_ISO = "data/maps/tiled-offsets/iso.tmx";
//	private readonly static String MAP_ISO_STAG = "data/maps/tiled-offsets/iso_stag.tmx";
//	private readonly static String MAP_HEX_X = "data/maps/tiled-offsets/hex_x.tmx";
//	private readonly static String MAP_HEX_Y = "data/maps/tiled-offsets/hex_y.tmx";
//	private TiledMap map;
//	private TiledMapRenderer renderer;
//	private OrthographicCamera camera;
//	private OrthoCamController cameraController;
//	private AssetManager assetManager;
//	private BitmapFont font;
//	private SpriteBatch batch;
//	private ShapeRenderer shapeRenderer;
//	private int mapType = 0;
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
//		shapeRenderer = new ShapeRenderer();
//
//		assetManager = new AssetManager();
//		assetManager.setLoader(TiledMap.class, new TmxMapLoader(new InternalFileHandleResolver()));
//		assetManager.load(MAP_ORTHO, TiledMap.class);
//		assetManager.load(MAP_ISO, TiledMap.class);
//		assetManager.load(MAP_ISO_STAG, TiledMap.class);
//		assetManager.load(MAP_HEX_X, TiledMap.class);
//		assetManager.load(MAP_HEX_Y, TiledMap.class);
//		assetManager.finishLoading();
//
//		map = assetManager.get(MAP_ORTHO);
//		renderer = new OrthogonalTiledMapRenderer(map, 1f / 32f);
//	}
//
//	public override void Render () {
//		if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_1)) {
//			if (mapType != 0) {
//				if (renderer is Disposable) ((Disposable)renderer).Dispose();
//				mapType = 0;
//				map = assetManager.get(MAP_ORTHO);
//				renderer = new OrthogonalTiledMapRenderer(map, 1f / 32f);
//			}
//		} else if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_2)) {
//			if (mapType != 1) {
//				if (renderer is Disposable) ((Disposable)renderer).Dispose();
//				mapType = 1;
//				map = assetManager.get(MAP_ORTHO);
//				renderer = new OrthoCachedTiledMapRenderer(map, 1f / 32f);
//				((OrthoCachedTiledMapRenderer)renderer).setBlending(true);
//			}
//		} else if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_3)) {
//			if (mapType != 2) {
//				if (renderer is Disposable) ((Disposable)renderer).Dispose();
//				mapType = 2;
//				map = assetManager.get(MAP_ISO);
//				renderer = new IsometricTiledMapRenderer(map, 1f / 48f);
//			}
//		} else if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_4)) {
//			if (mapType != 3) {
//				if (renderer is Disposable) ((Disposable)renderer).Dispose();
//				mapType = 3;
//				map = assetManager.get(MAP_ISO_STAG);
//				renderer = new IsometricStaggeredTiledMapRenderer(map, 1f / 48f);
//			}
//		} else if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_5)) {
//			if (mapType != 4) {
//				if (renderer is Disposable) ((Disposable)renderer).Dispose();
//				mapType = 4;
//				map = assetManager.get(MAP_HEX_X);
//				renderer = new HexagonalTiledMapRenderer(map, 1f / 48f);
//			}
//		} else if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_6)) {
//			if (mapType != 5) {
//				if (renderer is Disposable) ((Disposable)renderer).Dispose();
//				mapType = 5;
//				map = assetManager.get(MAP_HEX_Y);
//				renderer = new HexagonalTiledMapRenderer(map, 1f / 48f);
//			}
//		}
//
//		ScreenUtils.clear(100f / 255f, 100f / 255f, 250f / 255f, 1f);
//		camera.update();
//
//		// add margin to view bounds so it is easy to see any issues with clipping, calculated same way as
//		// BatchTiledMapRenderer#setView (OrthographicCamera)
//		 float margin = 3;
//		 float width = camera.viewportWidth * camera.zoom - margin * 2;
//		 float height = camera.viewportHeight * camera.zoom - margin * 2;
//		 float w = width * Math.Abs(camera.up.y) + height * Math.Abs(camera.up.x);
//		 float h = height * Math.Abs(camera.up.y) + width * Math.Abs(camera.up.x);
//		 float x = camera.position.x - w / 2;
//		 float y = camera.position.y - h / 2;
//		renderer.setView(camera.combined, x, y, w, h);
//		// For parallax effect to work, we must invalidate cache.
//		if (mapType == 1) {
//			((OrthoCachedTiledMapRenderer)renderer).invalidateCache();
//		}
//		renderer.render();
//
//		shapeRenderer.setProjectionMatrix(camera.combined);
//		shapeRenderer.begin(ShapeRenderer.ShapeType.Line);
//		shapeRenderer.setColor(Color.RED);
//		shapeRenderer.rect(x, y, w, h);
//		shapeRenderer.end();
//
//		batch.begin();
//		font.draw(batch, "FPS: " + Gdx.graphics.getFramesPerSecond(), 10, 20);
//		font.draw(batch, "Switch type with 1-6", Gdx.graphics.getHeight() - 100, 50);
//		font.draw(batch, renderer.getClass().getSimpleName(), Gdx.graphics.getHeight() - 100, 20);
//		batch.end();
//	}
//}
