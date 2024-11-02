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
//public class DpiTest : GdxTest {
//	BitmapFont font;
//	SpriteBatch batch;
//
//	public override void Create () {
//		font = new BitmapFont();
//		batch = new SpriteBatch();
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		font.draw(batch,
//			"Density: " + Gdx.graphics.getDensity() + "\n" + "PPC-x: " + Gdx.graphics.getPpcX() + "\n" + "PPC-y: "
//				+ Gdx.graphics.getPpcY() + "\n" + "PPI-x: " + Gdx.graphics.getPpiX() + "\n" + "PPI-y: " + Gdx.graphics.getPpiY(),
//			0, Gdx.graphics.getHeight());
//		batch.end();
//	}
//
//	public override void Resize (int width, int height) {
//	}
//}
