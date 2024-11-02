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
//public class FullscreenTest : GdxTest {
//	SpriteBatch batch;
//	Texture tex;
//	boolean fullscreen = false;
//	BitmapFont font;
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		tex = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//		DisplayMode[] modes = Gdx.graphics.getDisplayModes();
//		for (DisplayMode mode : modes) {
//			Console.WriteLine(mode);
//		}
//		Gdx.app.log("FullscreenTest", Gdx.graphics.getBufferFormat().toString());
//	}
//
//	public override void Resume () {
//
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0, 0, 0, 1);
//
//		batch.begin();
//		batch.setColor(Gdx.input.getX() < Gdx.graphics.getSafeInsetLeft()
//			|| Gdx.input.getX() + tex.getWidth() > Gdx.graphics.getWidth() - Gdx.graphics.getSafeInsetRight() ? Color.RED
//				: Color.WHITE);
//		batch.draw(tex, Gdx.input.getX(), Gdx.graphics.getHeight() - Gdx.input.getY());
//		font.draw(batch, "" + Gdx.graphics.getWidth() + ", " + Gdx.graphics.getHeight(), 0, 20);
//		batch.end();
//
//		if (Gdx.input.justTouched()) {
//			if (fullscreen) {
//				Gdx.graphics.setWindowedMode(480, 320);
//				batch.getProjectionMatrix().setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//				Gdx.gl.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
//				fullscreen = false;
//			} else {
//				DisplayMode m = null;
//				for (DisplayMode mode : Gdx.graphics.getDisplayModes()) {
//					if (m == null) {
//						m = mode;
//					} else {
//						if (m.width < mode.width) {
//							m = mode;
//						}
//					}
//				}
//
//				Gdx.graphics.setFullscreenMode(Gdx.graphics.getDisplayMode());
//				batch.getProjectionMatrix().setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//				Gdx.gl.glViewport(0, 0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getBackBufferHeight());
//				fullscreen = true;
//			}
//		}
//	}
//
//	public override void Resize (int width, int height) {
//		Gdx.app.log("FullscreenTest", "resized: " + width + ", " + height);
//		Gdx.app.log("FullscreenTest", "safe insets: " + Gdx.graphics.getSafeInsetLeft() + "/" + Gdx.graphics.getSafeInsetRight());
//		batch.getProjectionMatrix().setToOrtho2D(0, 0, width, height);
//	}
//
//	public override void Pause () {
//		Gdx.app.log("FullscreenTest", "paused");
//	}
//
//	public override void Dispose () {
//		Gdx.app.log("FullscreenTest", "disposed");
//	}
//}
