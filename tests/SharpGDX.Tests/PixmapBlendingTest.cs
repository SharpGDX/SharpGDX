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
//public class PixmapBlendingTest : GdxTest {
//	private SpriteBatch spriteBatch;
//	private Sprite logoSprite;
//	private Pixmap pixD, pixS1, pixS2;
//
//	public override void Create () {
//		if (spriteBatch != null) return;
//		spriteBatch = new SpriteBatch();
//
//		Matrix4 transform = new Matrix4();
//		transform.setToTranslation(0, Gdx.graphics.getHeight(), 0);
//		transform.mul(new Matrix4().setToScaling(1, -1, 1));
//		spriteBatch.setTransformMatrix(transform);
//
//		pixS1 = new Pixmap(Gdx.files.@internal("data/test4.png"));
//		pixS2 = new Pixmap(Gdx.files.@internal("data/test3.png"));
//		pixD = new Pixmap(512, 1024, Pixmap.Format.RGBA8888);
//
//		pixD.setBlending(Pixmap.Blending.SourceOver);
//		pixD.setFilter(Pixmap.Filter.NearestNeighbour);
//
//		pixD.drawPixmap(pixS1, 0, 0, 38, 76, 0, 0, 512, 1024);
//		pixD.drawPixmap(pixS2, 0, 0, 38, 76, 0, 0, 512, 1024);
//
//		logoSprite = new Sprite(new Texture(pixD));
//		logoSprite.flip(false, true);
//
//		pixS1.Dispose();
//		pixS2.Dispose();
//		pixD.Dispose();
//	}
//
//	public override void Render () {
//
//		ScreenUtils.clear(0, 1, 0, 1);
//
//		spriteBatch.begin();
//		logoSprite.setSize(256, 256);
//		logoSprite.draw(spriteBatch);
//		spriteBatch.end();
//
//	}
//
//	public bool needsGL20 () {
//		return false;
//	}
//}
