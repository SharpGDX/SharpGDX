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
///** Check if predictive back gesture works, loosely modeled upon Android's back stack. Tap the screen to increment the counter. Go
// * back to decrement the counter. If the counter is 0, the test will be exited. */
//public class BackTest : GdxTest {
//
//	private SpriteBatch batch;
//	private BitmapFont font;
//	private final Viewport viewport = new FitViewport(160, 90);
//
//	private int stackDepth;
//
//	public override void Create () {
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		Gdx.input.setInputProcessor(new InputAdapter() {
//
//			@Override
//			public boolean touchDown (int screenX, int screenY, int pointer, int button) {
//				int screenWidth = Gdx.graphics.getBackBufferWidth();
//				float safeZone = screenWidth * .1f;
//				if (screenX >= safeZone && screenX < screenWidth - safeZone) {
//					stackDepth++;
//					Gdx.input.setCatchKey(Input.Keys.BACK, stackDepth > 0);
//					return true;
//				}
//				return false;
//			}
//
//			@Override
//			public boolean keyDown (int keycode) {
//				if (keycode == Input.Keys.BACK) {
//					stackDepth--;
//					Gdx.input.setCatchKey(Input.Keys.BACK, stackDepth > 0);
//					return true;
//				}
//				return false;
//			}
//		});
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(Color.BLACK);
//		batch.begin();
//		font.draw(batch, "Stack depth: " + stackDepth, 20, 50);
//		batch.end();
//	}
//
//	public override void Resize (int width, int height) {
//		viewport.update(width, height, true);
//		batch.setProjectionMatrix(viewport.getCamera().combined);
//	}
//
//}
