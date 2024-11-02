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
///** Test that unchecked exceptions thrown from a runnable get posted and terminate the app. */
//public class RunnablePostTest : GdxTest {
//
//	private static readonly String TAG = "RunnablePostTest";
//	static bool expectIt = false;
//
//	static private Thread.UncaughtExceptionHandler exHandler = new Thread.UncaughtExceptionHandler() {
//		@Override
//		public void uncaughtException (Thread t, Throwable e) {
//			if (expectIt) {
//				Gdx.app.log(TAG, "PASSED: " + e.getMessage());
//			} else {
//				Gdx.app.log(TAG, "FAILED!  Unexpected exception received.");
//				e.printStackTrace(System.err);
//			}
//		}
//	};
//
//	public void create () {
//		Thread.setDefaultUncaughtExceptionHandler(exHandler);
//	}
//
//	public override void Render () {
//		if (Gdx.input.justTouched()) {
//			expectIt = true;
//			Gdx.app.postRunnable(new Runnable() {
//				@Override
//				public void run () {
//					throw new RuntimeException("This is a test of the uncaught exception handler.");
//				}
//			});
//		}
//	}
//}
