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
///** A simple test to demonstrate the life cycle of an application.
// * 
// * @author mzechner */
//public class LifeCycleTest : GdxTest {
//
//	public override void Dispose () {
//		Gdx.app.log("Test", "app destroyed");
//	}
//
//	public override void Pause () {
//		Gdx.app.log("Test", "app paused");
//	}
//
//	public override void Resume () {
//		Gdx.app.log("Test", "app resumed");
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//	}
//
//	public override void Create () {
//		Gdx.app.log("Test", "app created: " + Gdx.graphics.getWidth() + "x" + Gdx.graphics.getHeight());
//	}
//
//	public override void Resize (int width, int height) {
//		Gdx.app.log("Test",
//			"app resized: " + width + "x" + height + ", Graphics says: " + Gdx.graphics.getWidth() + "x" + Gdx.graphics.getHeight());
//	}
//}
