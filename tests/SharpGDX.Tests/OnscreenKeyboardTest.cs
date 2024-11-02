//using SharpGDX.Tests.Utils;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using SharpGDX.Input;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
//public class OnscreenKeyboardTest : GdxTest , Input.IInputProcessor {
//
//	BitmapFont font;
//	String text;
//	SpriteBatch batch;
//	OnscreenKeyboardType type = OnscreenKeyboardType.Default;
//
//	public void create () {
//		batch = new SpriteBatch();
//		font = new BitmapFont();
//		text = "";
//		Gdx.input.setInputProcessor(this);
//	}
//
//	public void render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		font.draw(batch, "input [" + type + "]: " + text, 0, Gdx.graphics.getHeight());
//		batch.end();
//
//		if (Gdx.input.justTouched()) {
//			type = OnscreenKeyboardType.values()[(type.ordinal() + 1) % OnscreenKeyboardType.values().length];
//			Gdx.input.setOnscreenKeyboardVisible(true, type);
//		}
//	}
//
//	public override bool KeyDown (int keycode) {
//
//		return false;
//	}
//
//	public override bool KeyUp (int keycode) {
//		return false;
//	}
//
//	public override bool KeyTyped (char character) {
//		if (character == '\b' && text.Length >= 1) {
//			text = text.Substring(0, text.Length - 1);
//		} else if (character == '\n') {
//			Gdx.input.setOnscreenKeyboardVisible(false);
//		} else {
//			text += character;
//		}
//		return false;
//	}
//
//	public override bool TouchDown (int x, int y, int pointer, int button) {
//		return false;
//	}
//
//	public override bool TouchUp (int x, int y, int pointer, int button) {
//		// TODO Auto-generated method stub
//		return false;
//	}
//
//	public override bool TouchDragged (int x, int y, int pointer) {
//		// TODO Auto-generated method stub
//		return false;
//	}
//
//	public override bool MouseMoved (int x, int y) {
//		// TODO Auto-generated method stub
//		return false;
//	}
//
//	public override bool Scrolled (float amountX, float amountY) {
//		// TODO Auto-generated method stub
//		return false;
//	}
//}
