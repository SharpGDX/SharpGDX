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
//using SharpGDX.Input;
//
//namespace SharpGDX.Tests;
//
//public class TextInputDialogTest : GdxTest {
//	String message;
//	SpriteBatch batch;
//	BitmapFont font;
//	int inputType = 0;
//
//	public void create () {
//		message = "Touch screen for dialog";
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//	}
//
//	public void render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		font.draw(batch, message, 10, 40);
//		batch.end();
//
//		if (Gdx.input.justTouched()) {
//			Gdx.input.getTextInput(new IInput.TextInputListener() {
//				@Override
//				public void input (String text) {
//					message = "message: " + text + ", type: " + Input.OnscreenKeyboardType.values()[inputType]
//						+ ", touch screen for new dialog";
//				}
//
//				@Override
//				public void canceled () {
//					message = "cancled by user";
//				}
//			}, "enter something funny", "funny", "something funny", Input.OnscreenKeyboardType.values()[inputType]);
//			inputType = (inputType + 1) % Input.OnscreenKeyboardType.values().length;
//		}
//	}
//}
