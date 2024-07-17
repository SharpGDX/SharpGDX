using SharpGDX.Graphics.GLUtils;
using SharpGDX.Utils;
using static SharpGDX.IInput;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static OpenTK.Windowing.GraphicsLibraryFramework.GLFWCallbacks;

namespace SharpGDX.Desktop
{
	public class DefaultDesktopInput : AbstractInput , IDesktopInput
	{
		readonly DesktopWindow window;
	private IInputProcessor inputProcessor;
	readonly InputEventQueue eventQueue = new ();

	int mouseX, mouseY;
	int mousePressed;
	int deltaX, deltaY;
	bool _justTouched;
	readonly bool[] justPressedButtons = new bool[5];
	char lastCharacter;

	private KeyCallback keyCallback;


private unsafe void charCallback(Window* window, uint codepoint)
{
	if ((codepoint & 0xff00) == 0xf700) return;
	lastCharacter = (char)codepoint;
	this.window.getGraphics().requestRendering();
	eventQueue.keyTyped((char)codepoint, TimeUtils.nanoTime());
}
	

private unsafe void scrollCallback(Window* window, double scrollX, double scrollY)
{
	this.window.getGraphics().requestRendering();
	eventQueue.scrolled(-(float)scrollX, -(float)scrollY, TimeUtils.nanoTime());
}
private int logicalMouseY;
private int logicalMouseX;

private CursorPosCallback cursorPosCallback;
private MouseButtonCallback _mouseButtonCallback;

private unsafe void mouseButtonCallback(Window* window, MouseButton button, InputAction action, KeyModifiers mods)
{
	
	int gdxButton = toGdxButton((int)button);
	// TODO: This isn't possible???
			//if (button != -1 && gdxButton == -1) return;

			long time = TimeUtils.nanoTime();
	if (action == InputAction.Press)
	{
		mousePressed++;
		_justTouched = true;
		justPressedButtons[gdxButton] = true;
		this.window.getGraphics().requestRendering();
		eventQueue.touchDown(mouseX, mouseY, 0, gdxButton, time);
	}
	else
	{
		mousePressed = Math.Max(0, mousePressed - 1);
		this.window.getGraphics().requestRendering();
		eventQueue.touchUp(mouseX, mouseY, 0, gdxButton, time);
	}
}

private int toGdxButton(int button)
{
	if (button == 0) return Buttons.LEFT;
	if (button == 1) return Buttons.RIGHT;
	if (button == 2) return Buttons.MIDDLE;
	if (button == 3) return Buttons.BACK;
	if (button == 4) return Buttons.FORWARD;
	return -1;
}
	

public unsafe DefaultDesktopInput (DesktopWindow window)
{
	this.window = window;
	WindowHandleChanged(window.getWindowPtr());
}

public void ResetPollingStates()
{
	_justTouched = false;
	keyJustPressed = false;
	for (int i = 0; i < justPressedKeys.Length; i++)
	{
		justPressedKeys[i] = false;
	}
	for (int i = 0; i < justPressedButtons.Length; i++)
	{
		justPressedButtons[i] = false;
	}
	eventQueue.drain(null);
}

	public unsafe void WindowHandleChanged(Window* windowHandle)
{
	ResetPollingStates();
	GLFW.SetKeyCallback(window.getWindowPtr(), keyCallback= (Window* window, OpenTK.Windowing.GraphicsLibraryFramework.Keys key, int scancode, InputAction action, KeyModifiers mods) =>
	{
		var intKey = getGdxKeyCode((int)key);
		switch (action)
		{
			case InputAction.Press:
				eventQueue.keyDown((int)intKey, TimeUtils.nanoTime());
				pressedKeyCount++;
				keyJustPressed = true;
				pressedKeys[(int)intKey] = true;
				justPressedKeys[(int)intKey] = true;
				this.window.getGraphics().requestRendering();
				lastCharacter = (char)0;
				char character = characterForKeyCode((int)intKey);
				if (character != 0) charCallback(window, character);
				break;
			case InputAction.Release:
				pressedKeyCount--;
				pressedKeys[(int)intKey] = false;
				this.window.getGraphics().requestRendering();
				eventQueue.keyUp((int)intKey, TimeUtils.nanoTime());
				break;
			case InputAction.Repeat:
				if (lastCharacter != 0)
				{
					this.window.getGraphics().requestRendering();
					eventQueue.keyTyped(lastCharacter, TimeUtils.nanoTime());
				}
				break;
		}
	});
	GLFW.SetCharCallback(window.getWindowPtr(), charCallback);
	GLFW.SetScrollCallback(window.getWindowPtr(), scrollCallback);

	GLFW.SetCursorPosCallback
		(
			window.getWindowPtr(),
			cursorPosCallback = (Window* windowHandle, double x, double y) =>
	{
		deltaX = (int)x - logicalMouseX;
		deltaY = (int)y - logicalMouseY;
		mouseX = logicalMouseX = (int)x;
		mouseY = logicalMouseY = (int)y;

		if (window.getConfig().hdpiMode == HdpiMode.Pixels)
		{
			float xScale = window.getGraphics().getBackBufferWidth() / (float)window.getGraphics().getLogicalWidth();
			float yScale = window.getGraphics().getBackBufferHeight() / (float)window.getGraphics().getLogicalHeight();
			deltaX = (int)(deltaX * xScale);
			deltaY = (int)(deltaY * yScale);
			mouseX = (int)(mouseX * xScale);
			mouseY = (int)(mouseY * yScale);
		}

		this.window.getGraphics().requestRendering();
		long time = TimeUtils.nanoTime();
		if (mousePressed > 0)
		{
			eventQueue.touchDragged(mouseX, mouseY, 0, time);
		}
		else
		{
			eventQueue.mouseMoved(mouseX, mouseY, time);
		}
	}
			);

	// TODO: Clear this in the dispose method.
	GLFW.SetMouseButtonCallback(window.getWindowPtr(), _mouseButtonCallback= mouseButtonCallback);
}

	public void Update()
{
	eventQueue.drain(inputProcessor);
}

	public void PrepareNext()
{
	if (_justTouched)
	{
		_justTouched = false;
		for (int i = 0; i < justPressedButtons.Length; i++)
		{
			justPressedButtons[i] = false;
		}
	}

	if (keyJustPressed)
	{
		keyJustPressed = false;
		for (int i = 0; i < justPressedKeys.Length; i++)
		{
			justPressedKeys[i] = false;
		}
	}
	deltaX = 0;
	deltaY = 0;
}

public override int getMaxPointers()
{
	return 1;
}

public override int getX()
{
	return mouseX;
}

	public override int getX(int pointer)
{
	return pointer == 0 ? mouseX : 0;
}

public override int getDeltaX()
{
	return deltaX;
}

public override int getDeltaX(int pointer)
{
	return pointer == 0 ? deltaX : 0;
}

public override int getY()
{
	return mouseY;
}

public override int getY(int pointer)
{
	return pointer == 0 ? mouseY : 0;
}

public override int getDeltaY()
{
	return deltaY;
}

public override int getDeltaY(int pointer)
{
	return pointer == 0 ? deltaY : 0;
}

public override unsafe bool isTouched()
{
	return GLFW.GetMouseButton(window.getWindowPtr(), MouseButton.Button1) == InputAction.Press
		|| GLFW.GetMouseButton(window.getWindowPtr(), MouseButton.Button2) == InputAction.Press
		|| GLFW.GetMouseButton(window.getWindowPtr(), MouseButton.Button3) == InputAction.Press
		|| GLFW.GetMouseButton(window.getWindowPtr(), MouseButton.Button4) == InputAction.Press
		|| GLFW.GetMouseButton(window.getWindowPtr(), MouseButton.Button5) == InputAction.Press;
}

public override bool justTouched()
{
	return _justTouched;
}

public override bool isTouched(int pointer)
{
	return pointer == 0 && isTouched();
}

public override float getPressure()
{
	return getPressure(0);
}

public override float getPressure(int pointer)
{
	return isTouched(pointer) ? 1 : 0;
}

public override unsafe bool isButtonPressed(int button)
{
	return GLFW.GetMouseButton(window.getWindowPtr(), (MouseButton)button) == InputAction.Press;
}

public override bool isButtonJustPressed(int button)
{
	if (button < 0 || button >= justPressedButtons.Length)
	{
		return false;
	}
	return justPressedButtons[button];
}

public override void getTextInput(TextInputListener listener, String title, String text, String hint)
{
	getTextInput(listener, title, text, hint, OnscreenKeyboardType.Default);
}

public override void getTextInput(TextInputListener listener, String title, String text, String hint, OnscreenKeyboardType type)
{
	// FIXME getTextInput does nothing
	listener.canceled();
}

public override long getCurrentEventTime()
{
	// queue sets its event time for each event dequeued/processed
	return eventQueue.getCurrentEventTime();
}

public override void setInputProcessor(IInputProcessor processor)
{
	this.inputProcessor = processor;
}

public override IInputProcessor getInputProcessor()
{
	return inputProcessor;
}

public override unsafe void setCursorCatched(bool catched)
{
	GLFW.SetInputMode(window.getWindowPtr(), CursorStateAttribute.Cursor,
		catched ? CursorModeValue.CursorDisabled : CursorModeValue.CursorNormal);
}

public override unsafe bool isCursorCatched()
{
	return GLFW.GetInputMode(window.getWindowPtr(), CursorStateAttribute.Cursor) == CursorModeValue.CursorDisabled;
}

public override unsafe void setCursorPosition(int x, int y)
{
	if (window.getConfig().hdpiMode == HdpiMode.Pixels)
	{
		float xScale = window.getGraphics().getLogicalWidth() / (float)window.getGraphics().getBackBufferWidth();
		float yScale = window.getGraphics().getLogicalHeight() / (float)window.getGraphics().getBackBufferHeight();
		x = (int)(x * xScale);
		y = (int)(y * yScale);
	}
	GLFW.SetCursorPos(window.getWindowPtr(), x, y);
	cursorPosCallback(window.getWindowPtr(), x, y);
}

protected char characterForKeyCode(int key)
{
	// Map certain key codes to character codes.
	switch (key)
	{
		case IInput.Keys.BACKSPACE:
			return (char)8;
		case IInput.Keys.TAB:
			return '\t';
		case IInput.Keys.FORWARD_DEL:
			return (char)127;
		case IInput.Keys.NUMPAD_ENTER:
		case IInput.Keys.ENTER:
			return '\n';
	}
	return (char)0;
}

public int getGdxKeyCode(int lwjglKeyCode)
{
	switch ((OpenTK.Windowing.GraphicsLibraryFramework.Keys)lwjglKeyCode)
	{
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space:
			return IInput.Keys.SPACE;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Apostrophe:
			return IInput.Keys.APOSTROPHE;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Comma:
			return IInput.Keys.COMMA;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Minus:
			return IInput.Keys.MINUS;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Period:
			return IInput.Keys.PERIOD;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Slash:
			return IInput.Keys.SLASH;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D0:
			return IInput.Keys.NUM_0;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D1:
			return IInput.Keys.NUM_1;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D2:
			return IInput.Keys.NUM_2;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D3:
			return IInput.Keys.NUM_3;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D4:
			return IInput.Keys.NUM_4;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D5:
			return IInput.Keys.NUM_5;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D6:
			return IInput.Keys.NUM_6;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D7:
			return IInput.Keys.NUM_7;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D8:
			return IInput.Keys.NUM_8;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D9:
			return IInput.Keys.NUM_9;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Semicolon:
			return IInput.Keys.SEMICOLON;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Equal:
			return IInput.Keys.EQUALS;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.A:
			return IInput.Keys.A;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.B:
			return IInput.Keys.B;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.C:
			return IInput.Keys.C;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D:
			return IInput.Keys.D;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.E:
			return IInput.Keys.E;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F:
			return IInput.Keys.F;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.G:
			return IInput.Keys.G;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.H:
			return IInput.Keys.H;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.I:
			return IInput.Keys.I;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.J:
			return IInput.Keys.J;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.K:
			return IInput.Keys.K;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.L:
			return IInput.Keys.L;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.M:
			return IInput.Keys.M;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.N:
			return IInput.Keys.N;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.O:
			return IInput.Keys.O;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.P:
			return IInput.Keys.P;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q:
			return IInput.Keys.Q;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.R:
			return IInput.Keys.R;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.S:
			return IInput.Keys.S;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.T:
			return IInput.Keys.T;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.U:
			return IInput.Keys.U;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.V:
			return IInput.Keys.V;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.W:
			return IInput.Keys.W;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.X:
			return IInput.Keys.X;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Y:
			return IInput.Keys.Y;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Z:
			return IInput.Keys.Z;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftBracket:
			return IInput.Keys.LEFT_BRACKET;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backslash:
			return IInput.Keys.BACKSLASH;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightBracket:
			return IInput.Keys.RIGHT_BRACKET;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.GraveAccent:
			return IInput.Keys.GRAVE;
		//case OpenTK.Windowing.GraphicsLibraryFramework.Keys.WORLD_1:
		//case OpenTK.Windowing.GraphicsLibraryFramework.Keys.WORLD_2:
		//	return Input.Keys.UNKNOWN;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape:
			return IInput.Keys.ESCAPE;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Enter:
			return IInput.Keys.ENTER;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Tab:
			return IInput.Keys.TAB;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backspace:
			return IInput.Keys.BACKSPACE;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Insert:
			return IInput.Keys.INSERT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Delete:
			return IInput.Keys.FORWARD_DEL;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right:
			return IInput.Keys.RIGHT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left:
			return IInput.Keys.LEFT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down:
			return IInput.Keys.DOWN;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up:
			return IInput.Keys.UP;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.PageUp:
			return IInput.Keys.PAGE_UP;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.PageDown:
			return IInput.Keys.PAGE_DOWN;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Home:
			return IInput.Keys.HOME;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.End:
			return IInput.Keys.END;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.CapsLock:
			return IInput.Keys.CAPS_LOCK;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.ScrollLock:
			return IInput.Keys.SCROLL_LOCK;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.PrintScreen:
			return IInput.Keys.PRINT_SCREEN;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Pause:
			return IInput.Keys.PAUSE;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F1:
			return IInput.Keys.F1;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F2:
			return IInput.Keys.F2;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F3:
			return IInput.Keys.F3;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F4:
			return IInput.Keys.F4;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F5:
			return IInput.Keys.F5;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F6:
			return IInput.Keys.F6;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F7:
			return IInput.Keys.F7;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F8:
			return IInput.Keys.F8;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F9:
			return IInput.Keys.F9;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F10:
			return IInput.Keys.F10;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F11:
			return IInput.Keys.F11;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F12:
			return IInput.Keys.F12;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F13:
			return IInput.Keys.F13;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F14:
			return IInput.Keys.F14;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F15:
			return IInput.Keys.F15;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F16:
			return IInput.Keys.F16;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F17:
			return IInput.Keys.F17;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F18:
			return IInput.Keys.F18;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F19:
			return IInput.Keys.F19;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F20:
			return IInput.Keys.F20;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F21:
			return IInput.Keys.F21;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F22:
			return IInput.Keys.F22;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F23:
			return IInput.Keys.F23;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F24:
			return IInput.Keys.F24;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F25:
			return IInput.Keys.UNKNOWN;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.NumLock:
			return IInput.Keys.NUM_LOCK;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad0:
			return IInput.Keys.NUMPAD_0;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad1:
			return IInput.Keys.NUMPAD_1;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad2:
			return IInput.Keys.NUMPAD_2;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad3:
			return IInput.Keys.NUMPAD_3;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad4:
			return IInput.Keys.NUMPAD_4;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad5:
			return IInput.Keys.NUMPAD_5;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad6:
			return IInput.Keys.NUMPAD_6;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad7:
			return IInput.Keys.NUMPAD_7;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad8:
			return IInput.Keys.NUMPAD_8;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad9:
			return IInput.Keys.NUMPAD_9;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadDecimal:
			return IInput.Keys.NUMPAD_DOT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadDivide:
			return IInput.Keys.NUMPAD_DIVIDE;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadMultiply:
			return IInput.Keys.NUMPAD_MULTIPLY;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadSubtract:
			return IInput.Keys.NUMPAD_SUBTRACT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadAdd:
			return IInput.Keys.NUMPAD_ADD;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadEnter:
			return IInput.Keys.NUMPAD_ENTER;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadEqual:
			return IInput.Keys.NUMPAD_EQUALS;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftShift:
			return IInput.Keys.SHIFT_LEFT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl:
			return IInput.Keys.CONTROL_LEFT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftAlt:
			return IInput.Keys.ALT_LEFT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftSuper:
			return IInput.Keys.SYM;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightShift:
			return IInput.Keys.SHIFT_RIGHT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightControl:
			return IInput.Keys.CONTROL_RIGHT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightAlt:
			return IInput.Keys.ALT_RIGHT;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightSuper:
			return IInput.Keys.SYM;
		case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Menu:
			return IInput.Keys.MENU;
		default:
			return IInput.Keys.UNKNOWN;
	}
}

public void dispose()
{
	// TODO: Set to null
	//keyCallback.free();
	//charCallback.free();
	//scrollCallback.free();
	//cursorPosCallback.free();
	//mouseButtonCallback.free();
}

// --------------------------------------------------------------------------
// -------------------------- Nothing to see below this line except for stubs
// --------------------------------------------------------------------------

public override float getAccelerometerX()
{
	return 0;
}
public override float getAccelerometerY()
{
	return 0;
}

public override float getAccelerometerZ()
{
	return 0;
}

public override bool isPeripheralAvailable(Peripheral peripheral)
{
	return peripheral == Peripheral.HardwareKeyboard;
}

public override int getRotation()
{
	return 0;
}

public override Orientation getNativeOrientation()
{
	return Orientation.Landscape;
}
public override void setOnscreenKeyboardVisible(bool visible)
{
}

public override void setOnscreenKeyboardVisible(bool visible, OnscreenKeyboardType type)
{
}

public override void vibrate(int milliseconds)
{
}

public override void vibrate(int milliseconds, bool fallback)
{
}

public override void vibrate(int milliseconds, int amplitude, bool fallback)
{
}

public override void vibrate(VibrationType vibrationType)
{
}

public override float getAzimuth()
{
	return 0;
}

public override float getPitch()
{
	return 0;
}

public override float getRoll()
{
	return 0;
}

public override void getRotationMatrix(float[] matrix)
{
}

public override float getGyroscopeX()
{
	return 0;
}

public override float getGyroscopeY()
{
	return 0;
}

public override float getGyroscopeZ()
{
	return 0;
}
}
}
