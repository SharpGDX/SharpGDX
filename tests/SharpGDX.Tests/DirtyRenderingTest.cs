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
///** Demonstrates how to use non-continuous (aka dirty-only) rendering. The application will clear the screen with a random color
// * every frame it renders. Rendering requests are issued automatically if new input events arrive.
// * 
// * @author mzechner */
//public class DirtyRenderingTest : GdxTest {
//	public override void Create () {
//		// disable continuous rendering
//		Gdx.graphics.setContinuousRendering(false);
//		Gdx.app.log("DirtyRenderingTest", "created");
//	}
//
//	public override void Resume () {
//		Gdx.app.log("DirtyRenderingTest", "resumed");
//	}
//
//	public override void Resize (int width, int height) {
//		Gdx.app.log("DirtyRenderingTest", "resized");
//	}
//
//	public override void Pause () {
//		Gdx.app.log("DirtyRenderingTest", "paused");
//	}
//
//	public override void Dispose () {
//		Gdx.app.log("DirtyRenderingTest", "disposed");
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(MathUtils.random(), MathUtils.random(), MathUtils.random(), MathUtils.random());
//	}
//}
