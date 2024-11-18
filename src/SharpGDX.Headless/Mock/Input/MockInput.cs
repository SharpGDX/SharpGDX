using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Input;
using static SharpGDX.IInput;

namespace SharpGDX.Headless.Mock.Input
{
    /** The headless backend does its best to mock elements. This is intended to make code-sharing between server and client as simple
 * as possible. */
    public class MockInput : IInput
	{

		public float GetAccelerometerX()
	{
		return 0;
	}

	public float GetAccelerometerY()
	{
		return 0;
	}

	public float GetAccelerometerZ()
	{
		return 0;
	}

	public float GetGyroscopeX()
	{
		return 0;
	}

	public float GetGyroscopeY()
	{
		return 0;
	}

	public float GetGyroscopeZ()
	{
		return 0;
	}

	public int GetMaxPointers()
	{
		return 0;
	}

	public int GetX()
	{
		return 0;
	}

	public int GetX(int pointer)
	{
		return 0;
	}

	public int GetDeltaX()
	{
		return 0;
	}

	public int GetDeltaX(int pointer)
	{
		return 0;
	}

	public int GetY()
	{
		return 0;
	}

	public int GetY(int pointer)
	{
		return 0;
	}

	public int GetDeltaY()
	{
		return 0;
	}

	public int GetDeltaY(int pointer)
	{
		return 0;
	}

	public bool IsTouched()
	{
		return false;
	}

	public bool JustTouched()
	{
		return false;
	}

	public bool IsTouched(int pointer)
	{
		return false;
	}

	public float GetPressure()
	{
		return 0;
	}

	public float GetPressure(int pointer)
	{
		return 0;
	}

	public bool IsButtonPressed(int button)
	{
		return false;
	}

	public bool IsButtonJustPressed(int button)
	{
		return false;
	}

	public bool IsKeyPressed(int key)
	{
		return false;
	}

	public bool IsKeyJustPressed(int key)
	{
		return false;
	}

	public void GetTextInput(ITextInputListener listener, String title, String text, String hint)
	{

	}

	public void GetTextInput(ITextInputListener listener, String title, String text, String hint, OnscreenKeyboardType type)
	{

	}
	
	public void SetOnscreenKeyboardVisible(bool visible)
	{
	}

	public void SetOnscreenKeyboardVisible(bool visible, OnscreenKeyboardType? type)
	{
	}

	public void Vibrate(int milliseconds)
	{

	}

	public void Vibrate(int milliseconds, bool fallback)
	{
	}

	public void Vibrate(int milliseconds, int amplitude, bool fallback)
	{
	}

	public void Vibrate(VibrationType vibrationType)
	{
	}

	public float GetAzimuth()
	{
		return 0;
	}

	public float GetPitch()
	{
		return 0;
	}

	public float GetRoll()
	{
		return 0;
	}

	public void GetRotationMatrix(float[] matrix)
	{

	}

	public long GetCurrentEventTime()
	{
		return 0;
	}
        
	public bool IsCatchMenuKey()
	{
		return false;
	}

	public void SetCatchKey(int keycode, bool catchKey)
	{

	}

	public bool IsCatchKey(int keycode)
	{
		return false;
	}

	public void SetInputProcessor(IInputProcessor processor)
	{

	}

	private IInputProcessor mockInputProcessor;

	public IInputProcessor GetInputProcessor()
	{
		if (mockInputProcessor == null)
		{
			mockInputProcessor = new InputAdapter();
		}
		return mockInputProcessor;
	}

	public bool IsPeripheralAvailable(Peripheral peripheral)
	{
		return false;
	}

	public int GetRotation()
	{
		return 0;
	}

	public Orientation GetNativeOrientation()
	{
		return Orientation.Landscape;
	}

	public void SetCursorCatched(bool catched)
	{

	}

	public bool IsCursorCatched()
	{
		return false;
	}

	public void SetCursorPosition(int x, int y)
	{

	}

    public void OpenTextInputField(NativeInputConfiguration configuration)
    {

    }

    public void CloseTextInputField(bool sendReturn)
    {

    }

    public void SetKeyboardHeightObserver(IKeyboardHeightObserver observer)
    {

    }
    }
}
