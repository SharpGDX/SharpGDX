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
///** Tests alpha blending with all ShapeRenderer shapes.
// * @author mzechner */
//public class ShapeRendererAlphaTest : GdxTest {
//	ShapeRenderer renderer;
//
//	public override void Create () {
//		renderer = new ShapeRenderer();
//	}
//
//	public override void Render () {
//		Gdx.gl.glClearColor(1, 1, 1, 1);
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		Gdx.gl.glEnable(GL20.GL_BLEND);
//		Gdx.gl.glBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
//
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.setColor(1, 0, 0, 0.5f);
//		renderer.rect(0, 0, 100, 200);
//		renderer.end();
//
//		renderer.begin(ShapeRenderer.ShapeType.Filled);
//		renderer.setColor(0, 1, 0, 0.5f);
//		renderer.rect(200, 0, 100, 100);
//		renderer.end();
//
//		renderer.begin(ShapeRenderer.ShapeType.Filled);
//		renderer.setColor(new Color(0x000000ff));
//		renderer.rect(300, 0, 100, 100);
//		renderer.end();
//
//		renderer.begin(ShapeRenderer.ShapeType.Line);
//		renderer.setColor(0, 1, 0, 0.5f);
//		renderer.circle(400, 50, 50);
//		renderer.end();
//
//		renderer.begin(ShapeRenderer.ShapeType.Filled);
//		renderer.setColor(1, 0, 1, 0.5f);
//		renderer.circle(500, 50, 50);
//		renderer.end();
//	}
//
//	public override void Dispose () {
//		renderer.Dispose();
//	}
//}
