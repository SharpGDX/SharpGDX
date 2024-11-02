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
//public class BlitTest : GdxTest {
//
//	Texture rgb888;
//	Texture rgba8888;
//	Texture psRgb888;
//	Texture psRgba8888;
//	SpriteBatch batch;
//
//	public void create () {
//		rgb888 = new Texture("data/bobrgb888-32x32.png");
//		rgba8888 = new Texture("data/bobargb8888-32x32.png");
//		psRgb888 = new Texture("data/alpha.png");
//		psRgba8888 = new Texture("data/rgb.png");
//		batch = new SpriteBatch();
//	}
//
//	public void render () {
//		ScreenUtils.clear(0.4f, 0.4f, 0.4f, 1);
//
//		batch.begin();
//		batch.draw(rgb888, 0, 0);
//		batch.draw(rgba8888, 60, 0);
//		batch.draw(psRgb888, 0, 60);
//		batch.draw(psRgba8888, psRgb888.getWidth() + 20, 60);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		rgb888.Dispose();
//		rgba8888.Dispose();
//		psRgb888.Dispose();
//		psRgba8888.Dispose();
//	}
//}
