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
//public class ETC1Test : GdxTest {
//	OrthographicCamera camera;
//	OrthoCamController controller;
//	Texture img1;
//	Texture img2;
//	SpriteBatch batch;
//	BitmapFont font;
//
//	public override void Create () {
//		font = new BitmapFont();
//		camera = new OrthographicCamera(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		controller = new OrthoCamController(camera);
//		Gdx.input.setInputProcessor(controller);
//
//		Pixmap pixmap = new Pixmap(32, 32, Format.RGB565);
//		pixmap.setColor(1, 0, 0, 1);
//		pixmap.fill();
//		pixmap.setColor(0, 1, 0, 1);
//		pixmap.drawLine(0, 0, 32, 32);
//		pixmap.drawLine(0, 32, 32, 0);
//		ETC1Data encodedImage = ETC1.encodeImagePKM(pixmap);
//		pixmap.Dispose();
//		pixmap = ETC1.decodeImage(encodedImage, Format.RGB565);
//		encodedImage.Dispose();
//
//		img1 = new Texture(pixmap);
//		img2 = new Texture("data/test.etc1");
//		batch = new SpriteBatch();
//		pixmap.Dispose();
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		camera.update();
//
//		batch.setProjectionMatrix(camera.combined);
//		batch.begin();
//		batch.draw(img2, -100, 0);
//		batch.draw(img1, 0, 0);
//		batch.end();
//
//		batch.getProjectionMatrix().setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		batch.begin();
//		font.draw(batch, "fps: " + Gdx.graphics.getFramesPerSecond(), 0, 30);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		font.Dispose();
//		img1.Dispose();
//		img2.Dispose();
//	}
//}
