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
///** Test for issue http://code.google.com/p/libgdx/issues/detail?id=493
// * @author mzechner */
//public class SpriteBatchOriginScaleTest : GdxTest {
//	SpriteBatch batch;
//	TextureRegion region;
//	ShapeRenderer renderer;
//
//	public override void Create () {
//		region = new TextureRegion(new Texture("data/badlogicsmall.jpg"));
//		batch = new SpriteBatch();
//		renderer = new ShapeRenderer();
//		renderer.setProjectionMatrix(batch.getProjectionMatrix());
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//        renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.setColor(1, 1, 1, 1);
//		renderer.line(0, 100, Gdx.graphics.getWidth(), 100);
//		renderer.line(100, 0, 100, Gdx.graphics.getHeight());
//		renderer.end();
//
//		batch.begin();
//		batch.draw(region, 100, 100, 0, 0, 32, 32, 2, 2, 20);
//		batch.end();
//	}
//
//}
