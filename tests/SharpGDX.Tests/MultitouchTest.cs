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
//public class MultitouchTest : GdxTest {
//	ShapeRenderer renderer;
//	ScreenViewport viewport;
//	OrthographicCamera camera;
//	long startTime = TimeUtils.nanoTime();
//
//	Color[] colors = {Color.RED, Color.BLUE, Color.GREEN, Color.WHITE, Color.PINK, Color.ORANGE, Color.YELLOW, Color.MAGENTA,
//		Color.CYAN, Color.LIGHT_GRAY, Color.GRAY, Color.DARK_GRAY};
//
//	Vector2 tp = new Vector2();
//
//	public override void Render () {
//		Gdx.gl.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		camera.update();
//		renderer.setProjectionMatrix(camera.combined);
//		renderer.begin(ShapeRenderer.ShapeType.Filled);
//		int size = Math.max(Gdx.graphics.getWidth(), Gdx.graphics.getHeight()) / 10;
//		for (int i = 0; i < 10; i++) {
//			if (!Gdx.input.isTouched(i)) continue;
//			viewport.unproject(tp.set(Gdx.input.getX(i), Gdx.input.getY(i)));
//			Color color = colors[i % colors.length];
//			renderer.setColor(color);
//			float sSize = size * Gdx.input.getPressure(i);
//			renderer.triangle(tp.x, tp.y + sSize, tp.x + sSize, tp.y - sSize, tp.x - sSize, tp.y - sSize);
//		}
//		renderer.end();
//	}
//
//	public override void Create () {
//		Gdx.app.log("Multitouch", "multitouch supported: " + Gdx.input.isPeripheralAvailable(Peripheral.MultitouchScreen));
//		renderer = new ShapeRenderer();
//		camera = new OrthographicCamera();
//		viewport = new ScreenViewport(camera);
//		Gdx.input.setInputProcessor(this);
//	}
//
//	public override void Resize (int width, int height) {
//		viewport.update(width, height);
//	}
//
//	public override void Dispose () {
//		renderer.Dispose();
//	}
//}
