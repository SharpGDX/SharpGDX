namespace SharpGDX.Input
{
    /**
 * <p>
 * Interface to the input facilities. This allows polling the state of the keyboard, the touch screen and the accelerometer. On
 * some backends (desktop, gwt, etc) the touch screen is replaced by mouse input. The accelerometer is of course not available on
 * all backends.
 * </p>
 *
 * <p>
 * Instead of polling for events, one can process all input events with an {@link InputProcessor}. You can set the InputProcessor
 * via the {@link #setInputProcessor(InputProcessor)} method. It will be called before the {@link ApplicationListener#render()}
 * method in each frame.
 * </p>
 *
 * <p>
 * Keyboard keys are translated to the constants in {@link Keys} transparently on all systems. Do not use system specific key
 * constants.
 * </p>
 *
 * <p>
 * The class also offers methods to use (and test for the presence of) other input systems like vibration, compass, on-screen
 * keyboards, and cursor capture. Support for simple input dialogs is also provided.
 * </p>
 *
 * @author mzechner */
    public interface IInput
    {
        /** Callback interface for {@link Input#getTextInput(TextInputListener, String, String, String)}
         *
         * @author mzechner */
        public interface TextInputListener
        {
            public void input(String text);

            public void canceled();
        }

        /** @return The acceleration force in m/s^2 applied to the device in the X axis, including the force of gravity */
        public float getAccelerometerX();

        /** @return The acceleration force in m/s^2 applied to the device in the Y axis, including the force of gravity */
        public float getAccelerometerY();

        /** @return The acceleration force in m/s^2 applied to the device in the Z axis, including the force of gravity */
        public float getAccelerometerZ();

        /** @return The rate of rotation in rad/s around the X axis */
        public float getGyroscopeX();

        /** @return The rate of rotation in rad/s around the Y axis */
        public float getGyroscopeY();

        /** @return The rate of rotation in rad/s around the Z axis */
        public float getGyroscopeZ();

        /** @return The maximum number of pointers supported */
        public int getMaxPointers();

        /** @return The x coordinate of the last touch on touch screen devices and the current mouse position on desktop for the first
         *         pointer in screen coordinates. The screen origin is the top left corner. */
        public int getX();

        /** Returns the x coordinate in screen coordinates of the given pointer. Pointers are indexed from 0 to n. The pointer id
         * identifies the order in which the fingers went down on the screen, e.g. 0 is the first finger, 1 is the second and so on.
         * When two fingers are touched down and the first one is lifted the second one keeps its index. If another finger is placed on
         * the touch screen the first free index will be used.
         *
         * @param pointer the pointer id.
         * @return the x coordinate */
        public int getX(int pointer);

        /** @return the different between the current pointer location and the last pointer location on the x-axis. */
        public int getDeltaX();

        /** @return the different between the current pointer location and the last pointer location on the x-axis. */
        public int getDeltaX(int pointer);

        /** @return The y coordinate of the last touch on touch screen devices and the current mouse position on desktop for the first
         *         pointer in screen coordinates. The screen origin is the top left corner. */
        public int getY();

        /** Returns the y coordinate in screen coordinates of the given pointer. Pointers are indexed from 0 to n. The pointer id
         * identifies the order in which the fingers went down on the screen, e.g. 0 is the first finger, 1 is the second and so on.
         * When two fingers are touched down and the first one is lifted the second one keeps its index. If another finger is placed on
         * the touch screen the first free index will be used.
         *
         * @param pointer the pointer id.
         * @return the y coordinate */
        public int getY(int pointer);

        /** @return the different between the current pointer location and the last pointer location on the y-axis. */
        public int getDeltaY();

        /** @return the different between the current pointer location and the last pointer location on the y-axis. */
        public int getDeltaY(int pointer);

        /** @return whether the screen is currently touched. */
        public bool isTouched();

        /** @return whether a new touch down event just occurred. */
        public bool justTouched();

        /** Whether the screen is currently touched by the pointer with the given index. Pointers are indexed from 0 to n. The pointer
         * id identifies the order in which the fingers went down on the screen, e.g. 0 is the first finger, 1 is the second and so on.
         * When two fingers are touched down and the first one is lifted the second one keeps its index. If another finger is placed on
         * the touch screen the first free index will be used.
         *
         * @param pointer the pointer
         * @return whether the screen is touched by the pointer */
        public bool isTouched(int pointer);

        /** @return the pressure of the first pointer */
        public float getPressure();

        /** Returns the pressure of the given pointer, where 0 is untouched. On Android it should be up to 1.0, but it can go above
         * that slightly and its not consistent between devices. On iOS 1.0 is the normal touch and significantly more of hard touch.
         * Check relevant manufacturer documentation for details. Check availability with
         * {@link Input#isPeripheralAvailable(Peripheral)}. If not supported, returns 1.0 when touched.
         *
         * @param pointer the pointer id.
         * @return the pressure */
        public float getPressure(int pointer);

        /** Whether a given button is pressed or not. Button constants can be found in {@link Buttons}. On Android only the
         * Buttons#LEFT constant is meaningful before version 4.0.
         * @param button the button to check.
         * @return whether the button is down or not. */
        public bool isButtonPressed(int button);

        /** Returns whether a given button has just been pressed. Button constants can be found in {@link Buttons}. On Android only the
         * Buttons#LEFT constant is meaningful before version 4.0. On WebGL (GWT), only LEFT, RIGHT and MIDDLE buttons are supported.
         *
         * @param button the button to check.
         * @return true or false. */
        public bool isButtonJustPressed(int button);

        /** Returns whether the key is pressed.
         *
         * @param key The key code as found in {@link Input.Keys}.
         * @return true or false. */
        public bool isKeyPressed(int key);

        /** Returns whether the key has just been pressed.
         *
         * @param key The key code as found in {@link Input.Keys}.
         * @return true or false. */
        public bool isKeyJustPressed(int key);

        /** System dependent method to input a string of text. A dialog box will be created with the given title and the given text as
         * a message for the user. Will use the Default keyboard type. Once the dialog has been closed the provided
         * {@link TextInputListener} will be called on the rendering thread.
         *
         * @param listener The TextInputListener.
         * @param title The title of the text input dialog.
         * @param text The message presented to the user. */
        public void getTextInput(TextInputListener listener, String title, String text, String hint);

        /** System dependent method to input a string of text. A dialog box will be created with the given title and the given text as
         * a message for the user. Once the dialog has been closed the provided {@link TextInputListener} will be called on the
         * rendering thread.
         *
         * @param listener The TextInputListener.
         * @param title The title of the text input dialog.
         * @param text The message presented to the user.
         * @param type which type of keyboard we wish to display */
        public void getTextInput(TextInputListener listener, String title, String text, String hint,
            OnscreenKeyboardType type);

        /** Sets the on-screen keyboard visible if available. Will use the Default keyboard type.
         *
         * @param visible visible or not */
        public void setOnscreenKeyboardVisible(bool visible);

         interface InputStringValidator
        {
            /** @param toCheck The string that should be validated
             * @return true, if the string is acceptable, false if not. */
            bool validate(String toCheck);
        }

        /** Sets the on-screen keyboard visible if available.
         *
         * @param configuration The configuration for the native input field */
        public void openTextInputField(NativeInputConfiguration configuration);

        /** Closes the native input field and applies the result to the input wrapper.
         * @param sendReturn Whether a "return" key should be send after processing */
        public void closeTextInputField(bool sendReturn);

        interface KeyboardHeightObserver
        {
            void onKeyboardHeightChanged(int height);
        }

        /** This will set a keyboard height callback. This will get called, whenever the keyboard height changes. Note: When using
         * openTextInputField, it will report the height of the native input field too. */
        public void setKeyboardHeightObserver(KeyboardHeightObserver observer);


        /** Sets the on-screen keyboard visible if available.
         *
         * @param visible visible or not
         * @param type which type of keyboard we wish to display. Can be null when hiding */
        public void setOnscreenKeyboardVisible(bool visible, OnscreenKeyboardType type);

        /** Generates a simple haptic effect of a given duration or a vibration effect on devices without haptic capabilities. Note
         * that on Android backend you'll need the permission
         * <code> <uses-permission android:name="android.permission.VIBRATE" /></code> in your manifest file in order for this to work.
         * On iOS backend you'll need to set <code>useHaptics = true</code> for devices with haptics capabilities to use them.
         *
         * @param milliseconds the number of milliseconds to vibrate. */
        public void vibrate(int milliseconds);

        /** Generates a simple haptic effect of a given duration and default amplitude. Note that on Android backend you'll need the
         * permission <code> <uses-permission android:name="android.permission.VIBRATE" /></code> in your manifest file in order for
         * this to work. On iOS backend you'll need to set <code>useHaptics = true</code> for devices with haptics capabilities to use
         * them.
         *
         * @param milliseconds the duration of the haptics effect
         * @param fallback whether to use non-haptic vibrator on devices without haptics capabilities (or haptics disabled). Fallback
         *           non-haptic vibrations may ignore length parameter in some backends. */
        public void vibrate(int milliseconds, bool fallback);

        /** Generates a simple haptic effect of a given duration and amplitude. Note that on Android backend you'll need the permission
         * <code> <uses-permission android:name="android.permission.VIBRATE" /></code> in your manifest file in order for this to work.
         * On iOS backend you'll need to set <code>useHaptics = true</code> for devices with haptics capabilities to use them.
         *
         * @param milliseconds the duration of the haptics effect
         * @param amplitude the amplitude/strength of the haptics effect. Valid values in the range [0, 255].
         * @param fallback whether to use non-haptic vibrator on devices without haptics capabilities (or haptics disabled). Fallback
         *           non-haptic vibrations may ignore length and/or amplitude parameters in some backends. */
        public void vibrate(int milliseconds, int amplitude, bool fallback);

        /** Generates a simple haptic effect of a type. VibrationTypes are length/amplitude haptic effect presets that depend on each
         * device and are defined by manufacturers. Should give most consistent results across devices and OSs. Note that on Android
         * backend you'll need the permission <code> <uses-permission android:name="android.permission.VIBRATE" /></code> in your
         * manifest file in order for this to work. On iOS backend you'll need to set <code>useHaptics = true</code> for devices with
         * haptics capabilities to use them.
         *
         * @param vibrationType the type of vibration */
        public void vibrate(VibrationType vibrationType);

        /** The azimuth is the angle of the device's orientation around the z-axis. The positive z-axis points towards the earths
         * center.
         *
         * @see <a
         *      href="http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[], float[], float[], float[])">http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],
         *      float[], float[], float[])</a>
         * @return the azimuth in degrees */
        public float getAzimuth();

        /** The pitch is the angle of the device's orientation around the x-axis. The positive x-axis roughly points to the west and is
         * orthogonal to the z- and y-axis.
         * @see <a
         *      href="http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[], float[], float[], float[])">http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],
         *      float[], float[], float[])</a>
         * @return the pitch in degrees */
        public float getPitch();

        /** The roll is the angle of the device's orientation around the y-axis. The positive y-axis points to the magnetic north pole
         * of the earth.
         * @see <a
         *      href="http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[], float[], float[], float[])">http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[],
         *      float[], float[], float[])</a>
         * @return the roll in degrees */
        public float getRoll();

        /** Returns the rotation matrix describing the devices rotation as per
         * <a href= "http://developer.android.com/reference/android/hardware/SensorManager.html#getRotationMatrix(float[], float[],
         * float[], float[])" >SensorManager#getRotationMatrix(float[], float[], float[], float[])</a>. Does not manipulate the matrix
         * if the platform does not have an accelerometer.
         * @param matrix */
        public void getRotationMatrix(float[] matrix);

        /** @return the time of the event currently reported to the {@link InputProcessor}. */
        public long getCurrentEventTime();

        /** Sets whether the given key on Android or GWT should be caught. No effect on other platforms. All keys that are not caught
         * may be handled by other apps or background processes on Android, or may trigger default browser behaviour on GWT. For
         * example, media or volume buttons are handled by background media players if present, or Space key triggers a scroll. All
         * keys you need to control your game should be caught to prevent unintended behaviour.
         *
         * @param keycode keycode to catch
         * @param catchKey whether to catch the given keycode */
        public void setCatchKey(int keycode, bool catchKey);

        /** @param keycode keycode to check if caught
         * @return true if the given keycode is configured to be caught */
        public bool isCatchKey(int keycode);

        /** Sets the {@link InputProcessor} that will receive all touch and key input events. It will be called before the
         * {@link ApplicationListener#render()} method each frame.
         *
         * @param processor the InputProcessor */
        public void setInputProcessor(IInputProcessor processor);

        /** @return the currently set {@link InputProcessor} or null. */
        public IInputProcessor getInputProcessor();

        /** Queries whether a {@link Peripheral} is currently available. In case of Android and the {@link Peripheral#HardwareKeyboard}
         * this returns the whether the keyboard is currently slid out or not.
         *
         * @param peripheral the {@link Peripheral}
         * @return whether the peripheral is available or not. */
        public bool isPeripheralAvailable(Peripheral peripheral);

        /** @return the rotation of the device with respect to its native orientation. */
        public int getRotation();

        /** @return the native orientation of the device. */
        public Orientation getNativeOrientation();

        /** Only viable on the desktop. Will confine the mouse cursor location to the window and hide the mouse cursor. X and y
         * coordinates are still reported as if the mouse was not catched.
         * @param catched whether to catch or not to catch the mouse cursor */
        public void setCursorCatched(bool catched);

        /** @return whether the mouse cursor is catched. */
        public bool isCursorCatched();

        /** Only viable on the desktop. Will set the mouse cursor location to the given window coordinates (origin top-left corner).
         * @param x the x-position
         * @param y the y-position */
        public void setCursorPosition(int x, int y);
    }
}