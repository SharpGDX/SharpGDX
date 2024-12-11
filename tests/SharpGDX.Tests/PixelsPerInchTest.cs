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
//public class PixelsPerInchTest : GdxTest {
//
//	BitmapFont font;
//	SpriteBatch batch;
//	Texture texture;
//
//	public override void Create () {
//		font = new BitmapFont(Gdx.files.@internal("data/lsans-15.fnt"), false);
//		batch = new SpriteBatch();
//		texture = new Texture(Gdx.files.@internal("data/badlogicsmall.jpg"));
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		batch.begin();
//		float width = (int)(Gdx.graphics.getPpcX() * 2);
//		float height = (int)(Gdx.graphics.getPpcY() * 1);
//		batch.draw(texture, 10, 100, width, height, 0, 0, 64, 32, false, false);
//		font.draw(batch, "button is 2x1 cm (" + width + "x" + height + "px), ppi: (" + Gdx.graphics.getPpiX() + ","
//			+ Gdx.graphics.getPpiY() + "), ppc: (" + Gdx.graphics.getPpcX() + "," + Gdx.graphics.getPpcY() + ")", 10, 50);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		font.Dispose();
//		batch.Dispose();
//		texture.Dispose();
//	}
//}
