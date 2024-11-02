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
//public class InputTest : GdxTest , Input.IInputProcessor {
//
//	public override void Create () {
//// Gdx.input = new RemoteInput();
//		Gdx.input.setInputProcessor(this);
//// Gdx.input.setCursorCatched(true);
////
//// Gdx.input.getTextInput(new Input.TextInputListener() {
//// @Override
//// public void input(String text) {
//// Gdx.app.log("Input test", "Input value: " + text);
//// }
////
//// @Override
//// public void canceled() {
//// Gdx.app.log("Input test", "Canceled input text");
//// }
//// }, "Title", "Text", "Placeholder");
//	}
//
//	public override void Render () {
//		if (Gdx.input.justTouched()) {
//			Gdx.app.log("Input Test", "just touched, button: " + (Gdx.input.isButtonPressed(Buttons.LEFT) ? "left " : "")
//				+ (Gdx.input.isButtonPressed(Buttons.MIDDLE) ? "middle " : "")
//				+ (Gdx.input.isButtonPressed(Buttons.RIGHT) ? "right" : "") + (Gdx.input.isButtonPressed(Buttons.BACK) ? "back" : "")
//				+ (Gdx.input.isButtonPressed(Buttons.FORWARD) ? "forward" : ""));
//		}
//
//		for (int i = 0; i < 10; i++) {
//			if (Gdx.input.getDeltaX(i) != 0 || Gdx.input.getDeltaY(i) != 0) {
//				Gdx.app.log("Input Test", "delta[" + i + "]: " + Gdx.input.getDeltaX(i) + ", " + Gdx.input.getDeltaY(i));
//			}
//		}
//// Gdx.input.setCursorPosition(Gdx.graphics.getWidth() / 2, Gdx.graphics.getHeight() / 2);
//// if(Gdx.input.isTouched()) {
//// Gdx.app.log("Input Test", "is touched");
//// }
//	}
//
//	public override bool KeyDown (int keycode) {
//		Gdx.app.log("Input Test", "key down: " + keycode);
//		if (keycode == Keys.G) Gdx.input.setCursorCatched(!Gdx.input.isCursorCatched());
//		return false;
//	}
//
//	public override bool KeyTyped (char character) {
//		Gdx.app.log("Input Test", "key typed: '" + character + "'");
//		return false;
//	}
//
//	public override bool KeyUp (int keycode) {
//		Gdx.app.log("Input Test", "key up: " + keycode);
//		return false;
//	}
//
//	public override bool TouchDown (int x, int y, int pointer, int button) {
//		Gdx.app.log("Input Test", "touch down: " + x + ", " + y + ", button: " + getButtonString(button));
//		return false;
//	}
//
//	public override bool TouchDragged (int x, int y, int pointer) {
//		Gdx.app.log("Input Test", "touch dragged: " + x + ", " + y + ", pointer: " + pointer);
//		return false;
//	}
//
//	public override bool TouchUp (int x, int y, int pointer, int button) {
//		Gdx.app.log("Input Test", "touch up: " + x + ", " + y + ", button: " + getButtonString(button));
//		return false;
//	}
//
//	public override bool MouseMoved (int x, int y) {
//		Gdx.app.log("Input Test", "touch moved: " + x + ", " + y);
//		return false;
//	}
//
//	public override bool Scrolled (float amountX, float amountY) {
//		Gdx.app.log("Input Test", "scrolled: " + amountY);
//		return false;
//	}
//
//	private String getButtonString (int button) {
//		if (button == Buttons.LEFT) return "left";
//		if (button == Buttons.RIGHT) return "right";
//		if (button == Buttons.MIDDLE) return "middle";
//		if (button == Buttons.BACK) return "back";
//		if (button == Buttons.FORWARD) return "forward";
//		return "unknown";
//	}
//}
