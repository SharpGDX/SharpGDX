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
//public class ParallaxTest : GdxTest {
//	class ParallaxCamera : OrthographicCamera {
//		Matrix4 parallaxView = new Matrix4();
//		Matrix4 parallaxCombined = new Matrix4();
//		Vector3 tmp = new Vector3();
//		Vector3 tmp2 = new Vector3();
//
//		public ParallaxCamera (float viewportWidth, float viewportHeight) 
//        : base(viewportWidth, viewportHeight)
//        {
//			
//		}
//
//		public Matrix4 calculateParallaxMatrix (float parallaxX, float parallaxY) {
//			update();
//			tmp.set(position);
//			tmp.x *= parallaxX;
//			tmp.y *= parallaxY;
//
//			parallaxView.setToLookAt(tmp, tmp2.set(tmp).add(direction), up);
//			parallaxCombined.set(projection);
//			Matrix4.mul(parallaxCombined.val, parallaxView.val);
//			return parallaxCombined;
//		}
//	}
//
//	TextureRegion[] layers;
//	ParallaxCamera camera;
//	OrthoCamController controller;
//	SpriteBatch batch;
//	BitmapFont font;
//
//	public override void Create () {
//		Texture texture = new Texture(Gdx.files.@internal("data/layers.png"));
//		layers = new TextureRegion[3];
//		layers[0] = new TextureRegion(texture, 0, 0, 542, 363);
//		layers[1] = new TextureRegion(texture, 0, 363, 1024, 149);
//		layers[2] = new TextureRegion(texture, 547, 0, 224, 51);
//
//		camera = new ParallaxCamera(480, 320);
//		controller = new OrthoCamController(camera);
//		Gdx.input.setInputProcessor(controller);
//		batch = new SpriteBatch();
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//	}
//
//	public override void Dispose () {
//		layers[0].getTexture().Dispose();
//		batch.Dispose();
//		font.Dispose();
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(242 / 255.0f, 210 / 255.0f, 111 / 255.0f, 1);
//
//		// keep camera in foreground layer bounds
//		bool updateCamera = false;
//		if (camera.position.x < -1024 + camera.viewportWidth / 2) {
//			camera.position.x = -1024 + (int)(camera.viewportWidth / 2);
//			updateCamera = true;
//		}
//
//		if (camera.position.x > 1024 - camera.viewportWidth / 2) {
//			camera.position.x = 1024 - (int)(camera.viewportWidth / 2);
//			updateCamera = true;
//		}
//
//		if (camera.position.y < 0) {
//			camera.position.y = 0;
//			updateCamera = true;
//		}
//		// arbitrary height of scene
//		if (camera.position.y > 400 - camera.viewportHeight / 2) {
//			camera.position.y = 400 - (int)(camera.viewportHeight / 2);
//			updateCamera = true;
//		}
//
//		// background layer, no parallax, centered around origin
//		batch.setProjectionMatrix(camera.calculateParallaxMatrix(0, 0));
//		batch.disableBlending();
//		batch.begin();
//		batch.draw(layers[0], -(int)(layers[0].getRegionWidth() / 2), -(int)(layers[0].getRegionHeight() / 2));
//		batch.end();
//		batch.enableBlending();
//
//		// midground layer, 0.5 parallax (move at half speed on x, full speed on y)
//		// layer is 1024x320
//		batch.setProjectionMatrix(camera.calculateParallaxMatrix(0.5f, 1));
//		batch.begin();
//		batch.draw(layers[1], -512, -160);
//		batch.end();
//
//		// foreground layer, 1.0 parallax (move at full speed)
//		// layer is 2048x320
//		batch.setProjectionMatrix(camera.calculateParallaxMatrix(1f, 1));
//		batch.begin();
//		for (int i = 0; i < 9; i++) {
//			batch.draw(layers[2], i * layers[2].getRegionWidth() - 1024, -160);
//		}
//		batch.end();
//
//		// draw fps
//		batch.getProjectionMatrix().setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		batch.begin();
//		font.draw(batch, "fps: " + Gdx.graphics.getFramesPerSecond(), 0, 30);
//		batch.end();
//	}
//}
