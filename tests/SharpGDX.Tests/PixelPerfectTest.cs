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
//public class PixelPerfectTest : GdxTest {
//	SpriteBatch batch;
//	OrthographicCamera cam;
//	Texture tex;
//
//	public override void Create () {
//		Pixmap pixmap = new Pixmap(16, 16, Pixmap.Format.RGBA8888);
//		pixmap.setColor(Color.BLUE);
//		pixmap.fill();
//		pixmap.setColor(Color.RED);
//		pixmap.drawLine(0, 0, 15, 15);
//		pixmap.drawLine(0, 15, 15, 0);
//
//		tex = new Texture(pixmap);
//		batch = new SpriteBatch();
//		cam = new OrthographicCamera();
//		cam.setToOrtho(false, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//	}
//
//	public override void Resize (int width, int height) {
//		cam.setToOrtho(false, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(1, 0, 1, 1);
//		cam.update();
//		batch.setProjectionMatrix(cam.combined);
//		batch.begin();
//		batch.draw(tex, 1, 1);
//		batch.end();
//	}
//}
