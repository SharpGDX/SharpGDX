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
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
//public class IsometricTileTest : GdxTest {
//	static final int LAYERS = 1;
//	static final int WIDTH = 4;
//	static final int HEIGHT = 5;
//	static final int TILES_PER_LAYER = WIDTH * HEIGHT;
//	static final int TILE_WIDTH = 54;
//	static final int TILE_HEIGHT = 54;
//	static final int TILE_HEIGHT_DIAMOND = 28;
//	static final int BOUND_X = HEIGHT * TILE_WIDTH / 2 + WIDTH * TILE_WIDTH / 2;
//	static final int BOUND_Y = HEIGHT * TILE_HEIGHT_DIAMOND / 2 + WIDTH * TILE_HEIGHT_DIAMOND / 2;
//
//	Texture texture;
//	SpriteCache[] caches = new SpriteCache[LAYERS];
//	int[] layers = new int[LAYERS];
//	OrthographicCamera cam;
//	OrthoCamController camController;
//	ShapeRenderer renderer;
//	long startTime = TimeUtils.nanoTime();
//
//	public override void Create () {
//		cam = new OrthographicCamera(480, 320);
//		camController = new OrthoCamController(cam);
//		Gdx.input.setInputProcessor(camController);
//
//		renderer = new ShapeRenderer();
//		texture = new Texture(Gdx.files.@internal("data/isotile.png"));
//
//		Random rand = new Random();
//		for (int i = 0; i < LAYERS; i++) {
//			caches[i] = new SpriteCache();
//			SpriteCache cache = caches[i];
//			cache.beginCache();
//
//			int colX = HEIGHT * TILE_WIDTH / 2 - TILE_WIDTH / 2;
//			int colY = BOUND_Y - TILE_HEIGHT_DIAMOND;
//			for (int x = 0; x < WIDTH; x++) {
//				for (int y = 0; y < HEIGHT; y++) {
//					int tileX = colX - y * TILE_WIDTH / 2;
//					int tileY = colY - y * TILE_HEIGHT_DIAMOND / 2;
//					cache.add(texture, tileX, tileY, rand.nextInt(2) * 54, 0, TILE_WIDTH, TILE_HEIGHT);
//				}
//				colX += TILE_WIDTH / 2;
//				colY -= TILE_HEIGHT_DIAMOND / 2;
//			}
//
//			layers[i] = cache.endCache();
//		}
//	}
//
//	public override void Dispose () {
//		renderer.Dispose();
//		texture.Dispose();
//		for (SpriteCache cache : caches)
//			cache.Dispose();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.7f, 0.7f, 0.7f, 1f);
//		cam.update();
//
//		GDX.GL.glEnable(GL20.GL_BLEND);
//		GDX.GL.glBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
//		for (int i = 0; i < LAYERS; i++) {
//			SpriteCache cache = caches[i];
//			cache.setProjectionMatrix(cam.combined);
//			cache.begin();
//			cache.draw(layers[i]);
//			cache.end();
//		}
//
//		renderer.setProjectionMatrix(cam.combined);
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.setColor(1, 0, 0, 1);
//		renderer.line(0, 0, 500, 0);
//		renderer.line(0, 0, 0, 500);
//
//		renderer.setColor(0, 0, 1, 1);
//		renderer.line(0, BOUND_Y, BOUND_X, BOUND_Y);
//
//		renderer.setColor(0, 0, 1, 1);
//		renderer.line(BOUND_X, 0, BOUND_X, BOUND_Y);
//
//		renderer.end();
//	}
//}
