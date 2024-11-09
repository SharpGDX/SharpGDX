using SharpGDX;
using SharpGDX.Input;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	public sealed class UIUtils {
	private UIUtils () {
	}

	static public bool isAndroid = SharedLibraryLoader.isAndroid;
	static public bool isMac = SharedLibraryLoader.isMac;
	static public bool isWindows = SharedLibraryLoader.isWindows;
	static public bool isLinux = SharedLibraryLoader.isLinux;
	static public bool isIos = SharedLibraryLoader.isIos;

	static public bool left () {
		return Gdx.Input.isButtonPressed(Buttons.Left);
	}

	static public bool left (int button) {
		return button == Buttons.Left;
	}

	static public bool right () {
		return Gdx.Input.isButtonPressed(Buttons.Right);
	}

	static public bool right (int button) {
		return button == Buttons.Right;
	}

	static public bool middle () {
		return Gdx.Input.isButtonPressed(Buttons.Middle);
	}

	static public bool middle (int button) {
		return button == Buttons.Middle;
	}

	static public bool shift () {
		return Gdx.Input.isKeyPressed(Keys.SHIFT_LEFT) || Gdx.Input.isKeyPressed(Keys.SHIFT_RIGHT);
	}

	static public bool shift (int keycode) {
		return keycode == Keys.SHIFT_LEFT || keycode == Keys.SHIFT_RIGHT;
	}

	static public bool ctrl () {
		if (isMac)
			return Gdx.Input.isKeyPressed(Keys.SYM);
		else
			return Gdx.Input.isKeyPressed(Keys.CONTROL_LEFT) || Gdx.Input.isKeyPressed(Keys.CONTROL_RIGHT);
	}

	static public bool ctrl (int keycode) {
		if (isMac)
			return keycode == Keys.SYM;
		else
			return keycode == Keys.CONTROL_LEFT || keycode == Keys.CONTROL_RIGHT;
	}

	static public bool alt () {
		return Gdx.Input.isKeyPressed(Keys.ALT_LEFT) || Gdx.Input.isKeyPressed(Keys.ALT_RIGHT);
	}

	static public bool alt (int keycode) {
		return keycode == Keys.ALT_LEFT || keycode == Keys.ALT_RIGHT;
	}
}
}
