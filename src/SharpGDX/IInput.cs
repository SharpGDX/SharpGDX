using SharpGDX.Input;

namespace SharpGDX;

/// <summary>
///     Interface to the input facilities.
/// </summary>
/// <remarks>
///     <para>
///         This allows polling the state of the keyboard, the touch screen and the accelerometer. On some backends
///         (desktop, gwt, etc.) the touch screen is replaced by mouse input. The accelerometer is of course not available
///         on all backends.
///     </para>
///     <para>
///         Instead of polling for events, one can process all input events with an <see cref="IInputProcessor" />. You can
///         set the InputProcessor via the <see cref="SetInputProcessor(IInputProcessor)" /> method. It will be called
///         before the <see cref="IApplicationListener.Render()" /> method in each frame.
///     </para>
///     <para>
///         Keyboard keys are translated to the constants in {@link Keys} transparently on all systems. Do not use system
///         specific key constants.
///     </para>
///     <para>
///         The class also offers methods to use (and test for the presence of) other input systems like vibration,
///         compass, on-screen keyboards, and cursor capture. Support for simple input dialogs is also provided.
///     </para>
/// </remarks>
public partial interface IInput
{
    /// <summary>
    ///     Closes the native input field and applies the result to the input wrapper.
    /// </summary>
    /// <param name="sendReturn">Whether a "return" key should be sent after processing.</param>
    public void CloseTextInputField(bool sendReturn);

    /// <summary>
    ///     Gets the acceleration force in m/s^2 applied to the device in the X axis, including the force of gravity.
    /// </summary>
    /// <returns>The acceleration force in m/s^2 applied to the device in the X axis, including the force of gravity.</returns>
    public float GetAccelerometerX();

    /// <summary>
    ///     Gets the acceleration force in m/s^2 applied to the device in the Y axis, including the force of gravity.
    /// </summary>
    /// <returns>The acceleration force in m/s^2 applied to the device in the Y axis, including the force of gravity.</returns>
    public float GetAccelerometerY();

    /// <summary>
    ///     Gets the acceleration force in m/s^2 applied to the device in the Z axis, including the force of gravity.
    /// </summary>
    /// <returns>The acceleration force in m/s^2 applied to the device in the Z axis, including the force of gravity.</returns>
    public float GetAccelerometerZ();

    /// <summary>
    ///     The azimuth is the angle of the device's orientation around the z-axis.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The positive z-axis points towards the earths center.
    ///     </para>
    ///     <para>
    ///         See
    ///         <a
    ///             href="http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],float[],float[],float[])">
    ///             http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],float[],float[],float[])
    ///         </a>
    ///     </para>
    /// </remarks>
    /// <returns>The azimuth in degrees.</returns>
    public float GetAzimuth();

    /// <summary>
    ///     Gets the time of the event currently reported to the <see cref="IInputProcessor" />.
    /// </summary>
    /// <returns>The time of the event currently reported to the <see cref="IInputProcessor" />.</returns>
    public long GetCurrentEventTime();

    /// <summary>
    ///     Gets the different between the current pointer location and the last pointer location on the x-axis.
    /// </summary>
    /// <returns>The different between the current pointer location and the last pointer location on the x-axis.</returns>
    public int GetDeltaX();

    /// <summary>
    ///     Gets the different between the current pointer location and the last pointer location on the x-axis.
    /// </summary>
    /// <param name="pointer"></param>
    /// <returns>The different between the current pointer location and the last pointer location on the x-axis.</returns>
    public int GetDeltaX(int pointer);

    /// <summary>
    ///     Gets the different between the current pointer location and the last pointer location on the y-axis.
    /// </summary>
    /// <returns>The different between the current pointer location and the last pointer location on the y-axis.</returns>
    public int GetDeltaY();

    /// <summary>
    ///     Gets the different between the current pointer location and the last pointer location on the y-axis.
    /// </summary>
    /// <param name="pointer"></param>
    /// <returns>The different between the current pointer location and the last pointer location on the y-axis.</returns>
    public int GetDeltaY(int pointer);

    /// <summary>
    ///     Gets the rate of rotation in rad/s around the X axis.
    /// </summary>
    /// <returns>The rate of rotation in rad/s around the X axis.</returns>
    public float GetGyroscopeX();

    /// <summary>
    ///     Gets the rate of rotation in rad/s around the Y axis.
    /// </summary>
    /// <returns>The rate of rotation in rad/s around the Y axis.</returns>
    public float GetGyroscopeY();

    /// <summary>
    ///     Gets the rate of rotation in rad/s around the Z axis.
    /// </summary>
    /// <returns>The rate of rotation in rad/s around the Z axis.</returns>
    public float GetGyroscopeZ();

    /// <summary>
    ///     Gets the currently set {@link InputProcessor} or null.
    /// </summary>
    /// <returns>The currently set {@link InputProcessor} or null.</returns>
    public IInputProcessor? GetInputProcessor();

    /// <summary>
    ///     Gets the maximum number of pointers supported.
    /// </summary>
    /// <returns>The maximum number of pointers supported.</returns>
    public int GetMaxPointers();

    /// <summary>
    ///     Gets the native orientation of the device.
    /// </summary>
    /// <returns>The native orientation of the device.</returns>
    public Orientation GetNativeOrientation();

    /// <summary>
    ///     The pitch is the angle of the device's orientation around the x-axis.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The positive x-axis roughly points to the west and is orthogonal to the z- and y-axis.
    ///     </para>
    ///     <para>
    ///         See
    ///         <a
    ///             href="http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],float[],float[],float[])">
    ///             http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],
    ///             float[], float[], float[])
    ///         </a>
    ///     </para>
    /// </remarks>
    /// <returns>The pitch in degrees.</returns>
    public float GetPitch();

    /// <summary>
    ///     Gets the pressure of the first pointer.
    /// </summary>
    /// <returns>The pressure of the first pointer.</returns>
    public float GetPressure();

    /// <summary>
    ///     Returns the pressure of the given pointer, where 0 is untouched.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         On Android it should be up to 1.0, but it can go above that slightly and is not consistent between devices.
    ///     </para>
    ///     <para>
    ///         On iOS 1.0 is the normal touch and significantly more of hard touch.
    ///     </para>
    ///     <para>
    ///         Check relevant manufacturer documentation for details.
    ///     </para>
    ///     <para>
    ///         Check availability with <see cref="IsPeripheralAvailable(Peripheral)" />. If not supported, returns 1.0 when
    ///         touched.
    ///     </para>
    /// </remarks>
    /// <param name="pointer">The pointer id.</param>
    /// <returns>The pressure.</returns>
    public float GetPressure(int pointer);

    /// <summary>
    ///     The roll is the angle of the device's orientation around the y-axis.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The positive y-axis points to the magnetic north pole of the earth.
    ///     </para>
    ///     <para>
    ///         See
    ///         <a
    ///             href="http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],float[],float[],float[])">
    ///             http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],
    ///             float[], float[], float[])
    ///         </a>
    ///     </para>
    /// </remarks>
    /// <returns>The roll in degrees.</returns>
    public float GetRoll();

    /// <summary>
    ///     Gets the rotation of the device with respect to its native orientation.
    /// </summary>
    /// <returns>The rotation of the device with respect to its native orientation.</returns>
    public int GetRotation();

    /// <summary>
    ///     Returns the rotation matrix describing the devices rotation as per
    ///     <a
    ///         href="http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],float[],float[],float[])">
    ///         SensorManager#getRotationMatrix(float[],
    ///         float[], float[], float[])
    ///     </a>
    ///     . Does not manipulate the matrix if the platform does not have an accelerometer.
    /// </summary>
    /// <param name="matrix"></param>
    public void GetRotationMatrix(float[] matrix);

    /// <summary>
    ///     System dependent method to input a string of text.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A dialog box will be created with the given title and the given text as a message for the user.
    ///     </para>
    ///     <para>
    ///         Will use the Default keyboard type.
    ///     </para>
    ///     <para>
    ///         Once the dialog has been closed the provided <see cref="ITextInputListener" /> will be called on the rendering
    ///         thread.
    ///     </para>
    /// </remarks>
    /// <param name="listener">The TextInputListener.</param>
    /// <param name="title">The title of the text input dialog.</param>
    /// <param name="text">The message presented to the user.</param>
    /// <param name="hint"></param>
    public void GetTextInput(ITextInputListener listener, string title, string text, string hint);

    /// <summary>
    ///     System dependent method to input a string of text.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A dialog box will be created with the given title and the given text as a message for the user.
    ///     </para>
    ///     <para>
    ///         Once the dialog has been closed the provided <see cref="ITextInputListener" /> will be called on the rendering
    ///         thread.
    ///     </para>
    /// </remarks>
    /// <param name="listener">The TextInputListener.</param>
    /// <param name="title">The title of the text input dialog.</param>
    /// <param name="text">The message presented to the user.</param>
    /// <param name="hint"></param>
    /// <param name="type">Which type of keyboard we wish to display.</param>
    public void GetTextInput(ITextInputListener listener, string title, string text, string hint,
        OnscreenKeyboardType type);

    /// <summary>
    ///     Gets the x coordinate of the last touch on touch screen devices and the current mouse position on desktop for the
    ///     first pointer in screen coordinates.
    /// </summary>
    /// <remarks>
    ///     The screen origin is the top left corner.
    /// </remarks>
    /// <returns>
    ///     The x coordinate of the last touch on touch screen devices and the current mouse position on desktop for the
    ///     first pointer in screen coordinates.
    /// </returns>
    public int GetX();

    /// <summary>
    ///     Returns the x coordinate in screen coordinates of the given pointer.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Pointers are indexed from 0 to n.
    ///     </para>
    ///     <para>
    ///         The pointer id identifies the order in which the fingers went down on the screen, e.g. 0 is the first finger, 1
    ///         is the second and so on.
    ///     </para>
    ///     <para>
    ///         When two fingers are touched down and the first one is lifted the second one keeps its index. If another finger
    ///         is placed on the touch screen the first free index will be used.
    ///     </para>
    /// </remarks>
    /// <param name="pointer">The pointer id.</param>
    /// <returns>The x coordinate.</returns>
    public int GetX(int pointer);

    /// <summary>
    ///     Gets the y coordinate of the last touch on touch screen devices and the current mouse position on desktop for the
    ///     first pointer in screen coordinates.
    /// </summary>
    /// <remarks>
    ///     The screen origin is the top left corner.
    /// </remarks>
    /// <returns>
    ///     The y coordinate of the last touch on touch screen devices and the current mouse position on desktop for the
    ///     first pointer in screen coordinates.
    /// </returns>
    public int GetY();

    /// <summary>
    ///     Returns the y coordinate in screen coordinates of the given pointer.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Pointers are indexed from 0 to n.
    ///     </para>
    ///     <para>
    ///         The pointer id identifies the order in which the fingers went down on the screen, e.g. 0 is the first finger, 1
    ///         is the second and so on.
    ///     </para>
    ///     <para>
    ///         When two fingers are touched down and the first one is lifted the second one keeps its index. If another finger
    ///         is placed on the touch screen the first free index will be used.
    ///     </para>
    /// </remarks>
    /// <param name="pointer">The pointer id.</param>
    /// <returns>The y coordinate.</returns>
    public int GetY(int pointer);

    /// <summary>
    ///     Returns whether a given button has just been pressed.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Button constants can be found in <see cref="Buttons" />.
    ///     </para>
    ///     <para>
    ///         On Android only the <see cref="Buttons.Left" /> constant is meaningful before version 4.0.
    ///     </para>
    ///     <para>
    ///         On WebGL (GWT), only LEFT, RIGHT and MIDDLE buttons are supported.
    ///     </para>
    /// </remarks>
    /// <param name="button">The button to check.</param>
    /// <returns>true or false.</returns>
    public bool IsButtonJustPressed(int button);

    /// <summary>
    ///     Whether a given button is pressed or not.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Button constants can be found in <see cref="Buttons" />.
    ///     </para>
    ///     <para>
    ///         On Android only the <see cref="Buttons.Left" /> constant is meaningful before version 4.0.
    ///     </para>
    /// </remarks>
    /// <param name="button">The button to check.</param>
    /// <returns>Whether the button is down or not.</returns>
    public bool IsButtonPressed(int button);

    /// <summary>
    ///     Returns if the given keycode is configured to be caught.
    /// </summary>
    /// <param name="keycode">Keycode to check if caught.</param>
    /// <returns>true if the given keycode is configured to be caught.</returns>
    public bool IsCatchKey(int keycode);

    /// <summary>
    ///     Gets whether the mouse cursor is catched.
    /// </summary>
    /// <returns>Whether the mouse cursor is catched.</returns>
    public bool IsCursorCatched();

    /// <summary>
    ///     Returns whether the key has just been pressed.
    /// </summary>
    /// <param name="key">The key code as found in <see cref="Keys" />.</param>
    /// <returns>true or false.</returns>
    public bool IsKeyJustPressed(int key);

    /// <summary>
    ///     Returns whether the key is pressed.
    /// </summary>
    /// <param name="key">The key code as found in <see cref="Keys" />.</param>
    /// <returns>true or false.</returns>
    public bool IsKeyPressed(int key);

    /// <summary>
    ///     Queries whether a <see cref="Peripheral" /> is currently available.
    /// </summary>
    /// <remarks>
    ///     In case of Android and the <see cref="Peripheral.HardwareKeyboard" /> this returns whether the keyboard is
    ///     currently slid out or not.
    /// </remarks>
    /// <param name="peripheral">The <see cref="Peripheral" />.</param>
    /// <returns>Whether the peripheral is available or not.</returns>
    public bool IsPeripheralAvailable(Peripheral peripheral);

    /// <summary>
    ///     Gets whether the screen is currently touched.
    /// </summary>
    /// <returns>Whether the screen is currently touched.</returns>
    public bool IsTouched();

    /// <summary>
    ///     Whether the screen is currently touched by the pointer with the given index.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Pointers are indexed from 0 to n. The pointer id identifies the order in which the fingers went down on the
    ///         screen, e.g. 0 is the first finger, 1 is the second and so on.
    ///     </para>
    ///     <para>
    ///         When two fingers are touched down and the first one is lifted the second one keeps its index. If another finger
    ///         is placed on the touch screen the first free index will be used.
    ///     </para>
    /// </remarks>
    /// <param name="pointer">The pointer.</param>
    /// <returns>Whether the screen is touched by the pointer.</returns>
    public bool IsTouched(int pointer);

    /// <summary>
    ///     Gets whether a new touch-down event just occurred.
    /// </summary>
    /// <returns>Whether a new touch-down event just occurred.</returns>
    public bool JustTouched();

    /// <summary>
    ///     Sets the on-screen keyboard visible if available.
    /// </summary>
    /// <param name="configuration">The configuration for the native input field.</param>
    public void OpenTextInputField(NativeInputConfiguration configuration);

    /// <summary>
    ///     Sets whether the given key on Android or GWT should be caught.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         No effect on other platforms.
    ///     </para>
    ///     <para>
    ///         All keys that are not caught may be handled by other apps or background processes on Android, or may trigger
    ///         default browser behavior on GWT. For example, media or volume buttons are handled by background media players
    ///         if present, or Space key triggers a scroll.
    ///     </para>
    ///     <para>
    ///         All keys you need to control your game should be caught to prevent unintended behavior.
    ///     </para>
    /// </remarks>
    /// <param name="keycode">Keycode to catch.</param>
    /// <param name="catchKey">Whether to catch the given keycode.</param>
    public void SetCatchKey(int keycode, bool catchKey);

    /// <summary>
    ///     Will confine the mouse cursor location to the window and hide the mouse cursor.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Only viable on the desktop.
    ///     </para>
    ///     <para>
    ///         X and y coordinates are still reported as if the mouse was not catched.
    ///     </para>
    /// </remarks>
    /// <param name="catched">Whether to catch or not to catch the mouse cursor.</param>
    public void SetCursorCatched(bool catched);

    /// <summary>
    ///     Will set the mouse cursor location to the given window coordinates (origin top-left corner).
    /// </summary>
    /// <remarks>
    ///     Only viable on the desktop.
    /// </remarks>
    /// <param name="x">The x-position.</param>
    /// <param name="y">The y-position.</param>
    public void SetCursorPosition(int x, int y);

    /// <summary>
    ///     Sets the <see cref="IInputProcessor" /> that will receive all touch and key input events.
    /// </summary>
    /// <remarks>
    ///     It will be called before the <see cref="IApplicationListener.Render()" /> method each frame.
    /// </remarks>
    /// <param name="processor">The InputProcessor.</param>
    public void SetInputProcessor(IInputProcessor processor);

    /// <summary>
    ///     This will set a keyboard height callback.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This will get called, whenever the keyboard height changes.
    ///     </para>
    ///     <para>
    ///         <b>Note:</b> When using openTextInputField, it will report the height of the native input field too.
    ///     </para>
    /// </remarks>
    /// <param name="observer"></param>
    public void SetKeyboardHeightObserver(IKeyboardHeightObserver observer);

    /// <summary>
    ///     Sets the on-screen keyboard visible if available.
    /// </summary>
    /// <remarks>
    ///     Will use the Default keyboard type.
    /// </remarks>
    /// <param name="visible">Visible or not.</param>
    public void SetOnscreenKeyboardVisible(bool visible);

    /// <summary>
    ///     Sets the on-screen keyboard visible if available.
    /// </summary>
    /// <param name="visible">Visible or not.</param>
    /// <param name="type">Which type of keyboard we wish to display. Can be null when hiding.</param>
    public void SetOnscreenKeyboardVisible(bool visible, OnscreenKeyboardType? type);

    /// <summary>
    ///     Generates a simple haptic effect of a given duration or a vibration effect on devices without haptic capabilities.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         On Android backend you'll need the permission
    ///         <code> <uses-permission android:name="android.permission.VIBRATE" /></code> in your manifest file in order for
    ///         this to work.
    ///     </para>
    ///     <para>
    ///         On iOS backend you'll need to set <code>useHaptics = true</code> for devices with haptics capabilities to use
    ///         them.
    ///     </para>
    /// </remarks>
    /// <param name="milliseconds">The number of milliseconds to vibrate.</param>
    public void Vibrate(int milliseconds);

    /// <summary>
    ///     Generates a simple haptic effect of a given duration and default amplitude.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         On Android backend you'll need the permission
    ///         <code> <uses-permission android:name="android.permission.VIBRATE" /></code> in your manifest file in order for
    ///         this to work.
    ///     </para>
    ///     <para>
    ///         On iOS backend you'll need to set <code>useHaptics = true</code> for devices with haptics capabilities to use
    ///         them.
    ///     </para>
    /// </remarks>
    /// <param name="milliseconds">The duration of the haptics effect.</param>
    /// <param name="fallback">
    ///     Whether to use non-haptic vibrator on devices without haptics capabilities (or haptics
    ///     disabled). Fallback non-haptic vibrations may ignore length parameter in some backends.
    /// </param>
    public void Vibrate(int milliseconds, bool fallback);

    /// <summary>
    ///     Generates a simple haptic effect of a given duration and amplitude.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         On Android backend you'll need the permission
    ///         <code> <uses-permission android:name="android.permission.VIBRATE" /></code> in your manifest file in order for
    ///         this to work.
    ///     </para>
    ///     <para>
    ///         On iOS backend you'll need to set <code>useHaptics = true</code> for devices with haptics capabilities to use
    ///         them.
    ///     </para>
    /// </remarks>
    /// <param name="milliseconds">The duration of the haptics effect.</param>
    /// <param name="amplitude">The amplitude/strength of the haptics effect. Valid values in the range [0, 255].</param>
    /// <param name="fallback">
    ///     Whether to use non-haptic vibrator on devices without haptics capabilities (or haptics
    ///     disabled). Fallback non-haptic vibrations may ignore length and/or amplitude parameters in some backends.
    /// </param>
    public void Vibrate(int milliseconds, int amplitude, bool fallback);

    /// <summary>
    ///     Generates a simple haptic effect of a type.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         VibrationTypes are length/amplitude haptic effect presets that depend on each device and are defined by
    ///         manufacturers. Should give most consistent results across devices and OSs.
    ///     </para>
    ///     <para>
    ///         On Android backend you'll need the permission
    ///         <code> <uses-permission android:name="android.permission.VIBRATE" /></code> in your manifest file in order for
    ///         this to work.
    ///     </para>
    ///     <para>
    ///         On iOS backend you'll need to set <code>useHaptics = true</code> for devices with haptics capabilities to use
    ///         them.
    ///     </para>
    /// </remarks>
    /// <param name="vibrationType">The type of vibration.</param>
    public void Vibrate(VibrationType vibrationType);
}