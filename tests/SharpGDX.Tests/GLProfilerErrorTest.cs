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
//public class GLProfilerErrorTest : GdxTest {
//	SpriteBatch batch;
//	BitmapFont font;
//
//	GLProfiler glProfiler;
//
//	String message = "GLProfiler is currently disabled";
//	boolean makeGlError = false;
//	final GLErrorListener customListener = new GLErrorListener() {
//		@Override
//		public void onError (int error) {
//			if (error == GL20.GL_INVALID_VALUE) {
//				message = "Correctly raised GL_INVALID_VALUE";
//			} else {
//				message = "Raised error but something unexpected: " + error;
//			}
//		}
//	};
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		Gdx.input.setInputProcessor(this);
//
//		glProfiler = new GLProfiler(Gdx.graphics);
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0.2f, 0.2f, 0.2f, 1);
//
//		batch.begin();
//
//		if (makeGlError) {
//			makeGlError = false;
//			try {
//				GDX.GL.glClear(42); // Random invalid value, will raise GL_INVALID_VALUE (0x501, 1281)
//			} catch (GdxRuntimeException glError) {
//				if ("GLProfiler: Got gl error GL_INVALID_VALUE".equals(glError.getMessage())) {
//					message = "Got expected exception.";
//				} else {
//					message = "Got GdxRuntimeException (correct) but with unexpected message: " + glError.getMessage();
//				}
//				Gdx.app.log("GLProfilerTest", "Caught exception: ", glError);
//			}
//		}
//
//		int x = 10;
//		int y = Gdx.graphics.getHeight() - 10;
//		y -= font.draw(batch, "e - Enable debugging\n" + "d - Disable debugging\n" + "l - Test log error listener\n"
//			+ "t - Test throw error listener\n" + "c - Test custom listener\n\n" + "Expected error: GL_INVALID_VALUE (0x501, 1281)",
//			x, y).height;
//		y -= 10;
//		font.draw(batch, message, x, y);
//		batch.end();
//	}
//
//	public override bool KeyTyped (char character) {
//		String DEBUGGER_DISABLED_MESSAGE = "Error will be detected after enabling the debugger";
//		switch (character) {
//		case 'e':
//			glProfiler.enable();
//			message = "GLProfiler enabled (isEnabled(): " + glProfiler.isEnabled() + ")";
//			break;
//		case 'd':
//			glProfiler.disable();
//			message = "GLProfiler disabled (isEnabled(): " + glProfiler.isEnabled() + ")";
//			break;
//		case 'l':
//			glProfiler.setListener(GLErrorListener.LOGGING_LISTENER);
//			makeGlError = true;
//			if (glProfiler.isEnabled()) {
//				message = "Log should contain info about error, which happened in glClear.";
//			} else {
//				message = DEBUGGER_DISABLED_MESSAGE;
//			}
//			break;
//		case 't':
//			glProfiler.setListener(GLErrorListener.THROWING_LISTENER);
//			makeGlError = true;
//			if (glProfiler.isEnabled()) {
//				message = "This should be soon replaced with info about caught exception.";
//			} else {
//				message = DEBUGGER_DISABLED_MESSAGE;
//			}
//			break;
//		case 'c':
//			glProfiler.setListener(customListener);
//			makeGlError = true;
//			if (glProfiler.isEnabled()) {
//				message = "This should be soon replaced about info about success.";
//			} else {
//				message = DEBUGGER_DISABLED_MESSAGE;
//			}
//			break;
//		default:
//			return false;
//		}
//		return true;
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		font.Dispose();
//	}
//}
