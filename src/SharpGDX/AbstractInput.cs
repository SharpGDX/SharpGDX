using SharpGDX.Utils;

namespace SharpGDX;

public abstract class AbstractInput : IInput
{
	protected readonly bool[] justPressedKeys = new bool[IInput.Keys.MAX_KEYCODE + 1];
	protected readonly bool[] pressedKeys = new bool[IInput.Keys.MAX_KEYCODE + 1];
	protected bool keyJustPressed;
	protected int pressedKeyCount;
	private readonly IntSet keysToCatch = new();

	public abstract float getAccelerometerX();

	public abstract float getAccelerometerY();

	public abstract float getAccelerometerZ();

	public abstract float getAzimuth();

	public abstract long getCurrentEventTime();

	public abstract int getDeltaX();

	public abstract int getDeltaX(int pointer);

	public abstract int getDeltaY();

	public abstract int getDeltaY(int pointer);

	public abstract float getGyroscopeX();

	public abstract float getGyroscopeY();

	public abstract float getGyroscopeZ();

	public abstract IInputProcessor getInputProcessor();

	public abstract int getMaxPointers();

	public abstract IInput.Orientation getNativeOrientation();

	public abstract float getPitch();

	public abstract float getPressure();

	public abstract float getPressure(int pointer);

	public abstract float getRoll();

	public abstract int getRotation();

	public abstract void getRotationMatrix(float[] matrix);

	public abstract void getTextInput(IInput.TextInputListener listener, string title, string text, string hint);

	public abstract void getTextInput
	(
		IInput.TextInputListener listener,
		string title,
		string text,
		string hint,
		IInput.OnscreenKeyboardType type
	);

	public abstract int getX();

	public abstract int getX(int pointer);

	public abstract int getY();

	public abstract int getY(int pointer);

	public abstract bool isButtonJustPressed(int button);

	public abstract bool isButtonPressed(int button);

	public bool isCatchBackKey()
	{
		return keysToCatch.contains(IInput.Keys.BACK);
	}

	public bool isCatchKey(int keycode)
	{
		return keysToCatch.contains(keycode);
	}

	public bool isCatchMenuKey()
	{
		return keysToCatch.contains(IInput.Keys.MENU);
	}

	public abstract bool isCursorCatched();

	public bool isKeyJustPressed(int key)
	{
		if (key == IInput.Keys.ANY_KEY)
		{
			return keyJustPressed;
		}

		if (key < 0 || key > IInput.Keys.MAX_KEYCODE)
		{
			return false;
		}

		return justPressedKeys[key];
	}

	public bool isKeyPressed(int key)
	{
		if (key == IInput.Keys.ANY_KEY)
		{
			return pressedKeyCount > 0;
		}

		if (key is < 0 or > IInput.Keys.MAX_KEYCODE)
		{
			return false;
		}

		return pressedKeys[key];
	}

	public abstract bool isPeripheralAvailable(IInput.Peripheral peripheral);
	public abstract bool isTouched();
	public abstract bool isTouched(int pointer);
	public abstract bool justTouched();

	public void setCatchBackKey(bool catchBack)
	{
		setCatchKey(IInput.Keys.BACK, catchBack);
	}

	public void setCatchKey(int keycode, bool catchKey)
	{
		if (!catchKey)
		{
			keysToCatch.remove(keycode);
		}
		else
		{
			keysToCatch.add(keycode);
		}
	}

	public void setCatchMenuKey(bool catchMenu)
	{
		setCatchKey(IInput.Keys.MENU, catchMenu);
	}

	public abstract void setCursorCatched(bool catched);

	public abstract void setCursorPosition(int x, int y);

	public abstract void setInputProcessor(IInputProcessor processor);

	public abstract void setOnscreenKeyboardVisible(bool visible);

	public abstract void setOnscreenKeyboardVisible(bool visible, IInput.OnscreenKeyboardType type);

	public abstract void vibrate(int milliseconds);

	public abstract void vibrate(int milliseconds, bool fallback);

	public abstract void vibrate(int milliseconds, int amplitude, bool fallback);

	public abstract void vibrate(IInput.VibrationType vibrationType);
}