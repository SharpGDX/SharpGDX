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
//public class ShapeRendererTest : GdxTest {
//
//	ShapeRenderer renderer;
//	PerspectiveCamera cam;
//	PerspectiveCamController controller;
//	SpriteBatch batch;
//	BitmapFont font;
//
//	public void create () {
//		renderer = new ShapeRenderer();
//		cam = new PerspectiveCamera(47, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		cam.position.set(0, 0, 2);
//		cam.near = 0.1f;
//		controller = new PerspectiveCamController(cam);
//		Gdx.input.setInputProcessor(controller);
//		batch = new SpriteBatch();
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//	}
//
//	public void render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//		Gdx.gl.glEnable(GL20.GL_DEPTH_TEST);
//		cam.update();
//		renderer.setProjectionMatrix(cam.combined);
//		renderer.identity();
//		renderer.rotate(0, 1, 0, 20);
//		renderer.translate(-0.5f, -0.5f, 0);
//
//		MathUtils.random.setSeed(0);
//
//		renderer.begin(ShapeRenderer.ShapeType.Point);
//
//		renderer.setColor(Color.PINK);
//		for (int i = 0; i < 100; i++)
//			renderer.point(MathUtils.random(0.0f, 1.0f), MathUtils.random(0.0f, 1.0f), 0);
//
//		renderer.end();
//
//		if (Gdx.input.isKeyPressed(Input.Keys.F)) {
//			renderer.begin(ShapeRenderer.ShapeType.Filled);
//
//			renderer.setColor(Color.RED);
//			renderer.rect(0, 0, 1, 1);
//
//			renderer.setColor(Color.BLUE);
//			renderer.circle(0.2f, 0.2f, 0.5f, 40);
//
//			renderer.setColor(Color.WHITE);
//			renderer.box(0.1f, 0.1f, 0.1f, 0.3f, 0.25f, 0.1f);
//
//			renderer.setColor(Color.GREEN);
//			renderer.cone(0.6f, 0.6f, 0, 0.3f, 0.75f, 20);
//
//			renderer.setColor(Color.MAGENTA);
//			renderer.triangle(-0.1f, 0.1f, -0.6f, -0.1f, -0.3f, 0.5f);
//
//			renderer.setColor(Color.GOLD);
//			renderer.ellipse(0.7f, -0.1f, 0.3f, 0.1f, 45f, 40);
//			renderer.ellipse(0.7f, -0.1f, 0.3f, 0.1f, 135f);
//
//			renderer.end();
//		} else {
//			renderer.begin(ShapeRenderer.ShapeType.Line);
//
//			renderer.setColor(Color.RED);
//			renderer.rect(0, 0, 1, 1);
//
//			renderer.setColor(Color.BLUE);
//			renderer.circle(0.2f, 0.2f, 0.5f, 40);
//
//			renderer.setColor(Color.YELLOW);
//			renderer.line(0, 0, 1, 1);
//
//			renderer.setColor(Color.WHITE);
//			renderer.box(0.1f, 0.1f, 0.1f, 0.3f, 0.25f, 0.1f);
//
//			renderer.setColor(Color.GREEN);
//			renderer.cone(0.6f, 0.6f, 0, 0.3f, 0.75f, 20);
//
//			renderer.setColor(Color.MAGENTA);
//			renderer.triangle(-0.1f, 0.1f, -0.6f, -0.1f, -0.3f, 0.5f);
//
//			renderer.setColor(Color.CYAN);
//			renderer.curve(0.0f, 0.25f, 0.2f, 0.3f, 0.3f, 0.6f, 0.1f, 0.5f, 30);
//
//			renderer.setColor(Color.GOLD);
//			renderer.ellipse(0.7f, -0.1f, 0.3f, 0.1f, 45f, 40);
//			renderer.ellipse(0.7f, -0.1f, 0.3f, 0.1f, 135f);
//
//			renderer.end();
//		}
//
//		batch.begin();
//		font.draw(batch, "fps: " + Gdx.graphics.getFramesPerSecond(), 0, 20);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		font.Dispose();
//		renderer.Dispose();
//	}
//}
