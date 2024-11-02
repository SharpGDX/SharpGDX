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
//public class DeltaTimeTest : GdxTest {
//
//	long lastFrameTime;
//
//	public override void Create () {
//		lastFrameTime = TimeUtils.nanoTime();
//	}
//
//	public override void Render () {
//		long frameTime = TimeUtils.nanoTime();
//		float deltaTime = (frameTime - lastFrameTime) / 1000000000.0f;
//		lastFrameTime = frameTime;
//
//		Gdx.app.log("DeltaTimeTest", "delta: " + deltaTime + ", gdx delta: " + Gdx.graphics.getDeltaTime());
//	}
//}
