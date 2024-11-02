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
//public class GestureDetectorTest : GdxTest implements ApplicationListener {
//	Texture texture;
//	SpriteBatch batch;
//	OrthographicCamera camera;
//	CameraController controller;
//	GestureDetector gestureDetector;
//
//	class CameraController implements GestureListener {
//		float velX, velY;
//		boolean flinging = false;
//		float initialScale = 1;
//
//		public boolean touchDown (float x, float y, int pointer, int button) {
//			flinging = false;
//			initialScale = camera.zoom;
//			return false;
//		}
//
//		@Override
//		public boolean tap (float x, float y, int count, int button) {
//			Gdx.app.log("GestureDetectorTest", "tap at " + x + ", " + y + ", count: " + count);
//			return false;
//		}
//
//		@Override
//		public boolean longPress (float x, float y) {
//			Gdx.app.log("GestureDetectorTest", "long press at " + x + ", " + y);
//			return false;
//		}
//
//		@Override
//		public boolean fling (float velocityX, float velocityY, int button) {
//			Gdx.app.log("GestureDetectorTest", "fling " + velocityX + ", " + velocityY);
//			flinging = true;
//			velX = camera.zoom * velocityX * 0.5f;
//			velY = camera.zoom * velocityY * 0.5f;
//			return false;
//		}
//
//		@Override
//		public boolean pan (float x, float y, float deltaX, float deltaY) {
//			// Gdx.app.log("GestureDetectorTest", "pan at " + x + ", " + y);
//			camera.position.add(-deltaX * camera.zoom, deltaY * camera.zoom, 0);
//			return false;
//		}
//
//		@Override
//		public boolean panStop (float x, float y, int pointer, int button) {
//			Gdx.app.log("GestureDetectorTest", "pan stop at " + x + ", " + y);
//			return false;
//		}
//
//		@Override
//		public boolean zoom (float originalDistance, float currentDistance) {
//			float ratio = originalDistance / currentDistance;
//			camera.zoom = initialScale * ratio;
//			Console.WriteLine(camera.zoom);
//			return false;
//		}
//
//		@Override
//		public boolean pinch (Vector2 initialFirstPointer, Vector2 initialSecondPointer, Vector2 firstPointer,
//			Vector2 secondPointer) {
//			return false;
//		}
//
//		public void update () {
//			if (flinging) {
//				velX *= 0.98f;
//				velY *= 0.98f;
//				camera.position.add(-velX * Gdx.graphics.getDeltaTime(), velY * Gdx.graphics.getDeltaTime(), 0);
//				if (Math.abs(velX) < 0.01f) velX = 0;
//				if (Math.abs(velY) < 0.01f) velY = 0;
//			}
//		}
//
//		@Override
//		public void pinchStop () {
//		}
//	}
//
//	public override void Create () {
//		texture = new Texture("data/stones.jpg");
//		batch = new SpriteBatch();
//		camera = new OrthographicCamera(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		controller = new CameraController();
//		gestureDetector = new GestureDetector(20, 40, 0.5f, 2, 0.15f, controller);
//		Gdx.input.setInputProcessor(gestureDetector);
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		controller.update();
//		camera.update();
//		batch.setProjectionMatrix(camera.combined);
//		batch.begin();
//		batch.draw(texture, 0, 0, texture.getWidth() * 2, texture.getHeight() * 2);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		texture.Dispose();
//		batch.Dispose();
//	}
//}
