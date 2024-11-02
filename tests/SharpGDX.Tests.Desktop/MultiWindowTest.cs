//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SharpGDX.Desktop;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Mathematics;
//using SharpGDX.Utils;

//namespace SharpGDX.Tests.Desktop
//{
//    public class MultiWindowTest {
//	static Texture sharedTexture;
//	static SpriteBatch sharedSpriteBatch;

//	public class MainWindow : ApplicationAdapter {
//		Type[] childWindowClasses = {typeof(NoncontinuousRenderingTest), ShaderCollectionTest.class, Basic3DSceneTest.class,
//			UITest.class};
//		DesktopWindow latestWindow;
//		int index;

//		public override void Create ()
//        {
//            Console.WriteLine(Gdx.graphics.getGLVersion().GetRendererString());
//			sharedSpriteBatch = new SpriteBatch();
//			sharedTexture = new Texture("data/badlogic.jpg");
//		}

//		public override void Render () {
//			ScreenUtils.clear(1, 0, 0, 1);
//			sharedSpriteBatch.getProjectionMatrix().setToOrtho2D(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//			sharedSpriteBatch.begin();
//			sharedSpriteBatch.draw(sharedTexture, Gdx.input.getX(), Gdx.graphics.getHeight() - Gdx.input.getY() - 1);
//			sharedSpriteBatch.end();

//			if (Gdx.input.justTouched()) {
//				DesktopApplication app = (DesktopApplication)Gdx.app;
//				DesktopWindowConfiguration config = new DesktopWindowConfiguration();
//				IGraphics.DisplayMode mode = Gdx.graphics.getDisplayMode();
//				config.setWindowPosition(MathUtils.random(0, mode.width - 640), MathUtils.random(0, mode.height - 480));
//				config.setTitle("Child window");
//				config.useVsync(false);
//				config.setWindowListener(new DesktopWindowAdapter() {
//					@Override
//					public void created (DesktopWindow window) {
//						latestWindow = window;
//					}
//				});
//				Class clazz = childWindowClasses[index++ % childWindowClasses.length];
//				ApplicationListener listener = createChildWindowClass(clazz);
//				app.newWindow(listener, config);
//			}

//			if (Gdx.input.isKeyJustPressed(Input.Keys.SPACE) && latestWindow != null) {
//				latestWindow.setTitle("Retitled window");
//				int size = 48;
//				Pixmap icon = new Pixmap(size, size, Pixmap.Format.RGBA8888);
//				icon.setBlending(Pixmap.Blending.None);
//				icon.setColor(Color.BLUE);
//				icon.fill();
//				icon.setColor(Color.CLEAR);
//				for (int i = 0; i < size; i += 3)
//					for (int j = 0; j < size; j += 3)
//						icon.drawPixel(i, j);
//				latestWindow.setIcon(icon);
//				icon.dispose();
//			}
//		}

//		public IApplicationListener createChildWindowClass (Class clazz) {
//			try {
//				return (IApplicationListener)clazz.newInstance();
//			} catch (Throwable t) {
//				throw new GdxRuntimeException("Couldn't instantiate app listener", t);
//			}
//		}
//	}

//	public static void main (String[] argv) {
//		DesktopApplicationConfiguration config = new DesktopApplicationConfiguration();
//		config.setTitle("Multi-window test");
//		config.useVsync(true);
//		config.setOpenGLEmulation(DesktopApplicationConfiguration.GLEmulation.ANGLE_GLES20, 0, 0);
//		new DesktopApplication(new MultiWindowTest.MainWindow(), config);
//	}
//}
//}
