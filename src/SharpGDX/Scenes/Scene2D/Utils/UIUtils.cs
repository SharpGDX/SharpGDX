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
		return GDX.Input.IsButtonPressed(IInput.Buttons.Left);
	}

	static public bool left (int button) {
		return button == IInput.Buttons.Left;
	}

	static public bool right () {
		return GDX.Input.IsButtonPressed(IInput.Buttons.Right);
	}

	static public bool right (int button) {
		return button == IInput.Buttons.Right;
	}

	static public bool middle () {
		return GDX.Input.IsButtonPressed(IInput.Buttons.Middle);
	}

	static public bool middle (int button) {
		return button == IInput.Buttons.Middle;
	}

	static public bool shift () {
		return GDX.Input.IsKeyPressed(IInput.Keys.ShiftLeft) || GDX.Input.IsKeyPressed(IInput.Keys.ShiftRight);
	}

	static public bool shift (int keycode) {
		return keycode == IInput.Keys.ShiftLeft || keycode == IInput.Keys.ShiftRight;
	}

	static public bool ctrl () {
		if (isMac)
			return GDX.Input.IsKeyPressed(IInput.Keys.Sym);
		else
			return GDX.Input.IsKeyPressed(IInput.Keys.ControlLeft) || GDX.Input.IsKeyPressed(IInput.Keys.ControlRight);
	}

	static public bool ctrl (int keycode) {
		if (isMac)
			return keycode == IInput.Keys.Sym;
		else
			return keycode == IInput.Keys.ControlLeft || keycode == IInput.Keys.ControlRight;
	}

	static public bool alt () {
		return GDX.Input.IsKeyPressed(IInput.Keys.AltLeft) || GDX.Input.IsKeyPressed(IInput.Keys.AltRight);
	}

	static public bool alt (int keycode) {
		return keycode == IInput.Keys.AltLeft || keycode == IInput.Keys.AltRight;
	}
}
}
