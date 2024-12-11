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
//using SharpGDX.Files;
//
//namespace SharpGDX.Tests;
//
//public class PngTest : GdxTest {
//	SpriteBatch batch;
//	Texture badlogic, screenshot;
//
//	public void create () {
//		batch = new SpriteBatch();
//		badlogic = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		if (screenshot == null) {
//			int width = Gdx.graphics.getWidth(), height = Gdx.graphics.getHeight();
//			for (int i = 0; i < 100; i++)
//				batch.draw(badlogic, MathUtils.random(width), MathUtils.random(height));
//			batch.flush();
//
//			FileHandle file = FileHandle.tempFile("screenshot-");
//			Console.WriteLine(file.file().getAbsolutePath());
//			Pixmap pixmap = Pixmap.createFromFrameBuffer(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//			try {
//				PNG writer = new PNG((int)(pixmap.getWidth() * pixmap.getHeight() * 1.5f));
//				// writer.setCompression(Deflater.NO_COMPRESSION);
//				writer.write(file, pixmap);
//				writer.write(file, pixmap); // Write twice to make sure the object is reusable.
//				writer.Dispose();
//			} catch (IOException ex) {
//				throw new RuntimeException(ex);
//			}
//			screenshot = new Texture(file);
//		}
//		batch.draw(screenshot, 0, 0);
//		batch.end();
//	}
//}
