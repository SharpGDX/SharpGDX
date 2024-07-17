using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharpGDX.IInput;

namespace SharpGDX.Headless.Mock.Input
{
	/** The headless backend does its best to mock elements. This is intended to make code-sharing between server and client as simple
 * as possible. */
	public class MockInput : SharpGDX.IInput
	{

		public float getAccelerometerX()
	{
		return 0;
	}

	public float getAccelerometerY()
	{
		return 0;
	}

	public float getAccelerometerZ()
	{
		return 0;
	}

	public float getGyroscopeX()
	{
		return 0;
	}

	public float getGyroscopeY()
	{
		return 0;
	}

	public float getGyroscopeZ()
	{
		return 0;
	}

	public int getMaxPointers()
	{
		return 0;
	}

	public int getX()
	{
		return 0;
	}

	public int getX(int pointer)
	{
		return 0;
	}

	public int getDeltaX()
	{
		return 0;
	}

	public int getDeltaX(int pointer)
	{
		return 0;
	}

	public int getY()
	{
		return 0;
	}

	public int getY(int pointer)
	{
		return 0;
	}

	public int getDeltaY()
	{
		return 0;
	}

	public int getDeltaY(int pointer)
	{
		return 0;
	}

	public bool isTouched()
	{
		return false;
	}

	public bool justTouched()
	{
		return false;
	}

	public bool isTouched(int pointer)
	{
		return false;
	}

	public float getPressure()
	{
		return 0;
	}

	public float getPressure(int pointer)
	{
		return 0;
	}

	public bool isButtonPressed(int button)
	{
		return false;
	}

	public bool isButtonJustPressed(int button)
	{
		return false;
	}

	public bool isKeyPressed(int key)
	{
		return false;
	}

	public bool isKeyJustPressed(int key)
	{
		return false;
	}

	public void getTextInput(TextInputListener listener, String title, String text, String hint)
	{

	}

	public void getTextInput(TextInputListener listener, String title, String text, String hint, OnscreenKeyboardType type)
	{

	}
	
	public void setOnscreenKeyboardVisible(bool visible)
	{
	}

	public void setOnscreenKeyboardVisible(bool visible, OnscreenKeyboardType type)
	{
	}

	public void vibrate(int milliseconds)
	{

	}

	public void vibrate(int milliseconds, bool fallback)
	{
	}

	public void vibrate(int milliseconds, int amplitude, bool fallback)
	{
	}

	public void vibrate(VibrationType vibrationType)
	{
	}

	public float getAzimuth()
	{
		return 0;
	}

	public float getPitch()
	{
		return 0;
	}

	public float getRoll()
	{
		return 0;
	}

	public void getRotationMatrix(float[] matrix)
	{

	}

	public long getCurrentEventTime()
	{
		return 0;
	}

	public void setCatchBackKey(bool catchBack)
	{

	}

	public bool isCatchBackKey()
	{
		return false;
	}

	public void setCatchMenuKey(bool catchMenu)
	{

	}

	public bool isCatchMenuKey()
	{
		return false;
	}

	public void setCatchKey(int keycode, bool catchKey)
	{

	}

	public bool isCatchKey(int keycode)
	{
		return false;
	}

	public void setInputProcessor(IInputProcessor processor)
	{

	}

	private IInputProcessor mockInputProcessor;

	public IInputProcessor getInputProcessor()
	{
		if (mockInputProcessor == null)
		{
			mockInputProcessor = new InputAdapter();
		}
		return mockInputProcessor;
	}

	public bool isPeripheralAvailable(Peripheral peripheral)
	{
		return false;
	}

	public int getRotation()
	{
		return 0;
	}

	public Orientation getNativeOrientation()
	{
		return Orientation.Landscape;
	}

	public void setCursorCatched(bool catched)
	{

	}

	public bool isCursorCatched()
	{
		return false;
	}

	public void setCursorPosition(int x, int y)
	{

	}
}
}
