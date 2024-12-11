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
//public class AlphaTest : GdxTest {
//	SpriteBatch batch;
//	Texture texture;
//
//	public override void Create () {
//		Pixmap pixmap = new Pixmap(256, 256, Pixmap.Format.RGBA8888);
//		pixmap.setColor(1, 0, 0, 1);
//		pixmap.fill();
//
//		texture = new Texture(pixmap, false);
//		texture.setFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
//		batch = new SpriteBatch();
//		pixmap.Dispose();
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		batch.draw(texture, 0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		batch.end();
//
//		Pixmap pixmap = Pixmap.createFromFrameBuffer(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		int color = pixmap.getPixel(0, pixmap.getHeight() - 1);
//		Gdx.app.log("AlphaTest", Integer.toHexString(color));
//		pixmap.Dispose();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		texture.Dispose();
//	}
//}
