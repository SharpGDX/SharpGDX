using SharpGDX.Input;
using SharpGDX.Utils;
using Keys = SharpGDX.IInput.Keys;

namespace SharpGDX;

public abstract class AbstractInput : IInput
{
    private readonly IntSet _keysToCatch = new();

    protected readonly bool[] JustPressedKeys = new bool[Keys.MaxKeycode + 1];
    protected readonly bool[] PressedKeys = new bool[Keys.MaxKeycode + 1];

    protected bool KeyJustPressed;
    protected int PressedKeyCount;

    /// <inheritdoc cref="IInput.CloseTextInputField(bool)" />
    public abstract void CloseTextInputField(bool sendReturn);

    /// <inheritdoc cref="IInput.GetAccelerometerX" />
    public abstract float GetAccelerometerX();

    /// <inheritdoc cref="IInput.GetAccelerometerY" />
    public abstract float GetAccelerometerY();

    /// <inheritdoc cref="IInput.GetAccelerometerZ" />
    public abstract float GetAccelerometerZ();

    /// <inheritdoc cref="IInput.GetAzimuth()" />
    public abstract float GetAzimuth();

    /// <inheritdoc cref="IInput.GetCurrentEventTime()" />
    public abstract long GetCurrentEventTime();

    /// <inheritdoc cref="IInput.GetDeltaX()" />
    public abstract int GetDeltaX();

    public abstract int GetDeltaX(int pointer);

    public abstract int GetDeltaY();

    public abstract int GetDeltaY(int pointer);

    public abstract float GetGyroscopeX();

    public abstract float GetGyroscopeY();

    public abstract float GetGyroscopeZ();

    public abstract IInputProcessor GetInputProcessor();

    public abstract int GetMaxPointers();

    public abstract IInput.Orientation GetNativeOrientation();

    public abstract float GetPitch();

    public abstract float GetPressure();

    public abstract float GetPressure(int pointer);

    public abstract float GetRoll();

    public abstract int GetRotation();

    public abstract void GetRotationMatrix(float[] matrix);

    public abstract void GetTextInput(IInput.ITextInputListener listener, string title, string text, string hint);

    public abstract void GetTextInput
    (
        IInput.ITextInputListener listener,
        string title,
        string text,
        string hint,
        IInput.OnscreenKeyboardType type
    );

    public abstract int GetX();

    public abstract int GetX(int pointer);

    public abstract int GetY();

    public abstract int GetY(int pointer);

    public abstract bool IsButtonJustPressed(int button);

    public abstract bool IsButtonPressed(int button);

    public bool IsCatchKey(int keycode)
    {
        return _keysToCatch.contains(keycode);
    }

    public abstract bool IsCursorCatched();

    public bool IsKeyJustPressed(int key)
    {
        if (key == Keys.AnyKey) return KeyJustPressed;

        if (key < 0 || key > Keys.MaxKeycode) return false;

        return JustPressedKeys[key];
    }

    public bool IsKeyPressed(int key)
    {
        if (key == Keys.AnyKey) return PressedKeyCount > 0;

        if (key is < 0 or > Keys.MaxKeycode) return false;

        return PressedKeys[key];
    }

    public abstract bool IsPeripheralAvailable(IInput.Peripheral peripheral);
    public abstract bool IsTouched();
    public abstract bool IsTouched(int pointer);
    public abstract bool JustTouched();
    public abstract void OpenTextInputField(NativeInputConfiguration configuration);

    public void SetCatchKey(int keycode, bool catchKey)
    {
        if (!catchKey)
            _keysToCatch.remove(keycode);
        else
            _keysToCatch.add(keycode);
    }

    public abstract void SetCursorCatched(bool catched);

    public abstract void SetCursorPosition(int x, int y);

    public abstract void SetInputProcessor(IInputProcessor processor);
    public abstract void SetKeyboardHeightObserver(IInput.IKeyboardHeightObserver observer);

    public abstract void SetOnscreenKeyboardVisible(bool visible);

    public abstract void SetOnscreenKeyboardVisible(bool visible, IInput.OnscreenKeyboardType? type);

    public abstract void Vibrate(int milliseconds);

    public abstract void Vibrate(int milliseconds, bool fallback);

    public abstract void Vibrate(int milliseconds, int amplitude, bool fallback);

    public abstract void Vibrate(IInput.VibrationType vibrationType);
}