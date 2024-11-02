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
///** Cycles viewports while rendering with SpriteBatch, also shows how to draw in the black bars. */
//public class ViewportTest2 : GdxTest {
//	Array<Viewport> viewports;
//	Viewport viewport;
//	Array<String> names;
//	String name;
//
//	private SpriteBatch batch;
//	private Texture texture;
//	private BitmapFont font;
//	private OrthographicCamera camera;
//
//	public void create () {
//		font = new BitmapFont();
//		font.setColor(0, 0, 0, 1);
//
//		Pixmap pixmap = new Pixmap(16, 16, Pixmap.Format.RGBA8888);
//		pixmap.setColor(1, 1, 1, 1);
//		pixmap.fill();
//		texture = new Texture(pixmap);
//
//		batch = new SpriteBatch();
//
//		camera = new OrthographicCamera();
//		camera.position.set(100, 100, 0);
//		camera.update();
//
//		viewports = ViewportTest1.getViewports(camera);
//		viewport = viewports.first();
//
//		names = ViewportTest1.getViewportNames();
//		name = names.first();
//
//		Gdx.input.setInputProcessor(new InputAdapter() {
//			public boolean keyDown (int keycode) {
//				if (keycode == Input.Keys.SPACE) {
//					int index = (viewports.indexOf(viewport, true) + 1) % viewports.size;
//					name = names.get(index);
//					viewport = viewports.get(index);
//					resize(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//				}
//				return false;
//			}
//		});
//	}
//
//	public void render () {
//		batch.setProjectionMatrix(camera.projection);
//		batch.setTransformMatrix(camera.view);
//
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		// draw a white background so we are able to see the black bars
//		batch.setColor(1, 1, 1, 1);
//		batch.draw(texture, -4096, -4096, 4096, 4096, 8192, 8192, 1, 1, 0, 0, 0, 16, 16, false, false);
//
//		batch.setColor(1, 0, 0, 1);
//		batch.draw(texture, 150, 100, 16, 16, 32, 32, 1, 1, 45, 0, 0, 16, 16, false, false);
//
//		font.draw(batch, viewport.getClass().getSimpleName(), 150, 100);
//		batch.end();
//
//		if (viewport is ScalingViewport) {
//			// This shows how to set the viewport to the whole screen and draw within the black bars.
//			ScalingViewport scalingViewport = (ScalingViewport)viewport;
//			int screenWidth = Gdx.graphics.getWidth();
//			int screenHeight = Gdx.graphics.getHeight();
//			HdpiUtils.glViewport(0, 0, screenWidth, screenHeight);
//			batch.getProjectionMatrix().idt().setToOrtho2D(0, 0, screenWidth, screenHeight);
//			batch.getTransformMatrix().idt();
//			batch.begin();
//			float leftGutterWidth = scalingViewport.getLeftGutterWidth();
//			if (leftGutterWidth > 0) {
//				batch.draw(texture, 0, 0, leftGutterWidth, screenHeight);
//				batch.draw(texture, scalingViewport.getRightGutterX(), 0, scalingViewport.getRightGutterWidth(), screenHeight);
//			}
//			float bottomGutterHeight = scalingViewport.getBottomGutterHeight();
//			if (bottomGutterHeight > 0) {
//				batch.draw(texture, 0, 0, screenWidth, bottomGutterHeight);
//				batch.draw(texture, 0, scalingViewport.getTopGutterY(), screenWidth, scalingViewport.getTopGutterHeight());
//			}
//			batch.end();
//			viewport.update(screenWidth, screenHeight, true); // Restore viewport.
//		}
//	}
//
//	public void resize (int width, int height) {
//		Console.WriteLine(name);
//		viewport.update(width, height);
//	}
//
//	public void dispose () {
//		texture.Dispose();
//		batch.Dispose();
//	}
//}
