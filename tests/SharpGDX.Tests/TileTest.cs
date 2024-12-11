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
//
//namespace SharpGDX.Tests;
//
//public class TileTest : GdxTest {
//	static readonly int LAYERS = 5;
//	static readonly int BLOCK_TILES = 25;
//	static readonly int WIDTH = 15;
//	static readonly int HEIGHT = 10;
//	static readonly int TILES_PER_LAYER = WIDTH * HEIGHT;
//
//	SpriteCache[] caches = new SpriteCache[LAYERS];
//	Texture texture;
//	int[] layers = new int[LAYERS];
//	OrthographicCamera cam;
//	OrthoCamController camController;
//	long startTime = TimeUtils.nanoTime();
//
//	public override void Create () {
//		cam = new OrthographicCamera(480, 320);
//		cam.position.set(WIDTH * 32 / 2, HEIGHT * 32 / 2, 0);
//		camController = new OrthoCamController(cam);
//		Gdx.input.setInputProcessor(camController);
//
//		texture = new Texture(Gdx.files.@internal("data/tiles.png"));
//
//		Random rand = new Random();
//		for (int i = 0; i < LAYERS; i++) {
//			caches[i] = new SpriteCache();
//			SpriteCache cache = caches[i];
//			cache.beginCache();
//			for (int y = 0; y < HEIGHT; y++) {
//				for (int x = 0; x < WIDTH; x++) {
//					int tileX = rand.nextInt(5);
//					int tileY = rand.nextInt(5);
//					cache.add(texture, x << 5, y << 5, 1 + tileX * 33, 1 + tileY * 33, 32, 32);
//				}
//			}
//			layers[i] = cache.endCache();
//		}
//
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		cam.update();
//
//		GDX.GL.glEnable(GL20.GL_BLEND);
//		GDX.GL.glBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
//		for (int i = 0; i < LAYERS; i++) {
//			SpriteCache cache = caches[i];
//			cache.setProjectionMatrix(cam.combined);
//			cache.begin();
//			for (int j = 0; j < TILES_PER_LAYER; j += BLOCK_TILES) {
//				cache.draw(layers[i], j, BLOCK_TILES);
//			}
//			cache.end();
//		}
//
//		if (TimeUtils.nanoTime() - startTime >= 1000000000) {
//			Gdx.app.log("TileTest", "fps: " + Gdx.graphics.getFramesPerSecond());
//			startTime = TimeUtils.nanoTime();
//		}
//	}
//}
