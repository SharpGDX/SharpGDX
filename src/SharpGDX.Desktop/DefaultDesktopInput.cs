﻿using SharpGDX.Graphics.GLUtils;
using SharpGDX.Utils;
using static SharpGDX.IInput;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpGDX.Input;
using static OpenTK.Windowing.GraphicsLibraryFramework.GLFWCallbacks;
using Keys = SharpGDX.IInput.Keys;

namespace SharpGDX.Desktop
{
    public class DefaultDesktopInput : AbstractInput, IDesktopInput
    {
        readonly DesktopWindow window;
        private IInputProcessor inputProcessor;
        readonly InputEventQueue eventQueue = new();

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
            this.window.GetGraphics().RequestRendering();
            eventQueue.KeyTyped((char)codepoint, TimeUtils.nanoTime());
        }

        private int logicalMouseY;
        private int logicalMouseX;

        private CursorPosCallback _cursorPosCallback;
        private MouseButtonCallback _mouseButtonCallback;
        private CharCallback _charCallback;
        private ScrollCallback _scrollCallback;

        private int toGdxButton(int button)
        {
            if (button == 0) return Buttons.Left;
            if (button == 1) return Buttons.Right;
            if (button == 2) return Buttons.Middle;
            if (button == 3) return Buttons.Back;
            if (button == 4) return Buttons.Forward;
            return -1;
        }


        public unsafe DefaultDesktopInput(DesktopWindow window)
        {
            this.window = window;
            WindowHandleChanged(window.GetWindowPtr());
        }

        public void ResetPollingStates()
        {
            _justTouched = false;
            KeyJustPressed = false;
            for (int i = 0; i < JustPressedKeys.Length; i++)
            {
                JustPressedKeys[i] = false;
            }

            for (int i = 0; i < justPressedButtons.Length; i++)
            {
                justPressedButtons[i] = false;
            }

            eventQueue.Drain(null);
        }

        public unsafe void WindowHandleChanged(Window* windowHandle)
        {
            ResetPollingStates();
            GLFW.SetKeyCallback(window.GetWindowPtr(), keyCallback =
                (Window* window, OpenTK.Windowing.GraphicsLibraryFramework.Keys key, int scancode, InputAction action,
                    KeyModifiers mods) =>
                {
                    var intKey = getGdxKeyCode((int)key);
                    switch (action)
                    {
                        case InputAction.Press:
                            eventQueue.KeyDown((int)intKey, TimeUtils.nanoTime());
                            PressedKeyCount++;
                            KeyJustPressed = true;
                            PressedKeys[(int)intKey] = true;
                            JustPressedKeys[(int)intKey] = true;
                            this.window.GetGraphics().RequestRendering();
                            lastCharacter = (char)0;
                            char character = characterForKeyCode((int)intKey);
                            if (character != 0) charCallback(window, character);
                            break;
                        case InputAction.Release:
                            PressedKeyCount--;
                            PressedKeys[(int)intKey] = false;
                            this.window.GetGraphics().RequestRendering();
                            eventQueue.KeyUp((int)intKey, TimeUtils.nanoTime());
                            break;
                        case InputAction.Repeat:
                            if (lastCharacter != 0)
                            {
                                this.window.GetGraphics().RequestRendering();
                                eventQueue.KeyTyped(lastCharacter, TimeUtils.nanoTime());
                            }

                            break;
                    }
                });
            GLFW.SetCharCallback(window.GetWindowPtr(), _charCallback = charCallback);
            GLFW.SetScrollCallback
            (
                window.GetWindowPtr(),
                _scrollCallback = (_, scrollX, scrollY) =>
                {
                    this.window.GetGraphics().RequestRendering();
                    eventQueue.Scrolled(-(float)scrollX, -(float)scrollY, TimeUtils.nanoTime());
                }
            );

            GLFW.SetCursorPosCallback
            (
                window.GetWindowPtr(),
                _cursorPosCallback = (_, x, y) =>
                {
                    deltaX = (int)x - logicalMouseX;
                    deltaY = (int)y - logicalMouseY;
                    mouseX = logicalMouseX = (int)x;
                    mouseY = logicalMouseY = (int)y;

                    if (window.GetConfig().hdpiMode == HdpiMode.Pixels)
                    {
                        float xScale = window.GetGraphics().GetBackBufferWidth() /
                                       (float)window.GetGraphics().getLogicalWidth();
                        float yScale = window.GetGraphics().GetBackBufferHeight() /
                                       (float)window.GetGraphics().getLogicalHeight();
                        deltaX = (int)(deltaX * xScale);
                        deltaY = (int)(deltaY * yScale);
                        mouseX = (int)(mouseX * xScale);
                        mouseY = (int)(mouseY * yScale);
                    }

                    this.window.GetGraphics().RequestRendering();
                    long time = TimeUtils.nanoTime();
                    if (mousePressed > 0)
                    {
                        eventQueue.TouchDragged(mouseX, mouseY, 0, time);
                    }
                    else
                    {
                        eventQueue.MouseMoved(mouseX, mouseY, time);
                    }
                }
            );

            // TODO: Clear this in the dispose method.
            GLFW.SetMouseButtonCallback
            (
                window.GetWindowPtr(),
                _mouseButtonCallback = (_, button, action, _) =>
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
                        this.window.GetGraphics().RequestRendering();
                        eventQueue.TouchDown(mouseX, mouseY, 0, gdxButton, time);
                    }
                    else
                    {
                        mousePressed = Math.Max(0, mousePressed - 1);
                        this.window.GetGraphics().RequestRendering();
                        eventQueue.TouchUp(mouseX, mouseY, 0, gdxButton, time);
                    }
                }
            );
        }

        public void Update()
        {
            eventQueue.Drain(inputProcessor);
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

            if (KeyJustPressed)
            {
                KeyJustPressed = false;
                for (int i = 0; i < JustPressedKeys.Length; i++)
                {
                    JustPressedKeys[i] = false;
                }
            }

            deltaX = 0;
            deltaY = 0;
        }

        public override int GetMaxPointers()
        {
            return 1;
        }

        public override int GetX()
        {
            return mouseX;
        }

        public override int GetX(int pointer)
        {
            return pointer == 0 ? mouseX : 0;
        }

        public override int GetDeltaX()
        {
            return deltaX;
        }

        public override int GetDeltaX(int pointer)
        {
            return pointer == 0 ? deltaX : 0;
        }

        public override int GetY()
        {
            return mouseY;
        }

        public override int GetY(int pointer)
        {
            return pointer == 0 ? mouseY : 0;
        }

        public override int GetDeltaY()
        {
            return deltaY;
        }

        public override int GetDeltaY(int pointer)
        {
            return pointer == 0 ? deltaY : 0;
        }

        public override unsafe bool IsTouched()
        {
            return GLFW.GetMouseButton(window.GetWindowPtr(), MouseButton.Button1) == InputAction.Press
                   || GLFW.GetMouseButton(window.GetWindowPtr(), MouseButton.Button2) == InputAction.Press
                   || GLFW.GetMouseButton(window.GetWindowPtr(), MouseButton.Button3) == InputAction.Press
                   || GLFW.GetMouseButton(window.GetWindowPtr(), MouseButton.Button4) == InputAction.Press
                   || GLFW.GetMouseButton(window.GetWindowPtr(), MouseButton.Button5) == InputAction.Press;
        }

        public override bool JustTouched()
        {
            return _justTouched;
        }

        public override bool IsTouched(int pointer)
        {
            return pointer == 0 && IsTouched();
        }

        public override float GetPressure()
        {
            return GetPressure(0);
        }

        public override float GetPressure(int pointer)
        {
            return IsTouched(pointer) ? 1 : 0;
        }

        public override unsafe bool IsButtonPressed(int button)
        {
            return GLFW.GetMouseButton(window.GetWindowPtr(), (MouseButton)button) == InputAction.Press;
        }

        public override bool IsButtonJustPressed(int button)
        {
            if (button < 0 || button >= justPressedButtons.Length)
            {
                return false;
            }

            return justPressedButtons[button];
        }

        public override void GetTextInput(ITextInputListener listener, String title, String text, String hint)
        {
            GetTextInput(listener, title, text, hint, OnscreenKeyboardType.Default);
        }

        public override void GetTextInput(ITextInputListener listener, String title, String text, String hint,
            OnscreenKeyboardType type)
        {
            // FIXME getTextInput does nothing
            listener.Canceled();
        }

        public override long GetCurrentEventTime()
        {
            // queue sets its event time for each event dequeued/processed
            return eventQueue.GetCurrentEventTime();
        }

        public override void SetInputProcessor(IInputProcessor processor)
        {
            this.inputProcessor = processor;
        }

        public override IInputProcessor GetInputProcessor()
        {
            return inputProcessor;
        }

        public override unsafe void SetCursorCatched(bool catched)
        {
            GLFW.SetInputMode(window.GetWindowPtr(), CursorStateAttribute.Cursor,
                catched ? CursorModeValue.CursorDisabled : CursorModeValue.CursorNormal);
        }

        public override unsafe bool IsCursorCatched()
        {
            return GLFW.GetInputMode(window.GetWindowPtr(), CursorStateAttribute.Cursor) ==
                   CursorModeValue.CursorDisabled;
        }

        public override unsafe void SetCursorPosition(int x, int y)
        {
            if (window.GetConfig().hdpiMode == HdpiMode.Pixels)
            {
                float xScale = window.GetGraphics().getLogicalWidth() /
                               (float)window.GetGraphics().GetBackBufferWidth();
                float yScale = window.GetGraphics().getLogicalHeight() /
                               (float)window.GetGraphics().GetBackBufferHeight();
                x = (int)(x * xScale);
                y = (int)(y * yScale);
            }

            GLFW.SetCursorPos(window.GetWindowPtr(), x, y);
            _cursorPosCallback(window.GetWindowPtr(), x, y);
        }

        protected char characterForKeyCode(int key)
        {
            // Map certain key codes to character codes.
            switch (key)
            {
                case Keys.Backspace:
                    return (char)8;
                case Keys.Tab:
                    return '\t';
                case Keys.ForwardDel:
                    return (char)127;
                case Keys.NumpadEnter:
                case Keys.Enter:
                    return '\n';
            }

            return (char)0;
        }

        public int getGdxKeyCode(int lwjglKeyCode)
        {
            switch ((OpenTK.Windowing.GraphicsLibraryFramework.Keys)lwjglKeyCode)
            {
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space:
                    return Keys.Space;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Apostrophe:
                    return Keys.Apostrophe;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Comma:
                    return Keys.Comma;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Minus:
                    return Keys.Minus;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Period:
                    return Keys.Period;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Slash:
                    return Keys.Slash;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D0:
                    return Keys.Num0;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D1:
                    return Keys.Num1;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D2:
                    return Keys.Num2;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D3:
                    return Keys.Num3;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D4:
                    return Keys.Num4;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D5:
                    return Keys.Num5;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D6:
                    return Keys.Num6;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D7:
                    return Keys.Num7;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D8:
                    return Keys.Num8;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D9:
                    return Keys.Num9;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Semicolon:
                    return Keys.Semicolon;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Equal:
                    return Keys.EQUALS;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.A:
                    return Keys.A;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.B:
                    return Keys.B;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.C:
                    return Keys.C;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.D:
                    return Keys.D;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.E:
                    return Keys.E;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F:
                    return Keys.F;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.G:
                    return Keys.G;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.H:
                    return Keys.H;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.I:
                    return Keys.I;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.J:
                    return Keys.J;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.K:
                    return Keys.K;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.L:
                    return Keys.L;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.M:
                    return Keys.M;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.N:
                    return Keys.N;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.O:
                    return Keys.O;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.P:
                    return Keys.P;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q:
                    return Keys.Q;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.R:
                    return Keys.R;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.S:
                    return Keys.S;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.T:
                    return Keys.T;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.U:
                    return Keys.U;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.V:
                    return Keys.V;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.W:
                    return Keys.W;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.X:
                    return Keys.X;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Y:
                    return Keys.Y;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Z:
                    return Keys.Z;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftBracket:
                    return Keys.LeftBracket;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backslash:
                    return Keys.Backslash;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightBracket:
                    return Keys.RightBracket;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.GraveAccent:
                    return Keys.Grave;
                //case OpenTK.Windowing.GraphicsLibraryFramework.Keys.WORLD_1:
                //case OpenTK.Windowing.GraphicsLibraryFramework.Keys.WORLD_2:
                //	return Input.Keys.UNKNOWN;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape:
                    return Keys.Escape;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Enter:
                    return Keys.Enter;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Tab:
                    return Keys.Tab;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backspace:
                    return Keys.Backspace;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Insert:
                    return Keys.Insert;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Delete:
                    return Keys.ForwardDel;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right:
                    return Keys.Right;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left:
                    return Keys.Left;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down:
                    return Keys.Down;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up:
                    return Keys.Up;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.PageUp:
                    return Keys.PageUp;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.PageDown:
                    return Keys.PageDown;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Home:
                    return Keys.Home;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.End:
                    return Keys.End;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.CapsLock:
                    return Keys.CapsLock;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.ScrollLock:
                    return Keys.ScrollLock;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.PrintScreen:
                    return Keys.PrintScreen;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Pause:
                    return Keys.Pause;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F1:
                    return Keys.F1;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F2:
                    return Keys.F2;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F3:
                    return Keys.F3;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F4:
                    return Keys.F4;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F5:
                    return Keys.F5;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F6:
                    return Keys.F6;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F7:
                    return Keys.F7;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F8:
                    return Keys.F8;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F9:
                    return Keys.F9;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F10:
                    return Keys.F10;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F11:
                    return Keys.F11;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F12:
                    return Keys.F12;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F13:
                    return Keys.F13;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F14:
                    return Keys.F14;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F15:
                    return Keys.F15;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F16:
                    return Keys.F16;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F17:
                    return Keys.F17;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F18:
                    return Keys.F18;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F19:
                    return Keys.F19;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F20:
                    return Keys.F20;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F21:
                    return Keys.F21;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F22:
                    return Keys.F22;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F23:
                    return Keys.F23;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F24:
                    return Keys.F24;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.F25:
                    return Keys.Unknown;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.NumLock:
                    return Keys.NumLock;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad0:
                    return Keys.Numpad0;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad1:
                    return Keys.Numpad1;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad2:
                    return Keys.Numpad2;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad3:
                    return Keys.Numpad3;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad4:
                    return Keys.Numpad4;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad5:
                    return Keys.Numpad5;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad6:
                    return Keys.Numpad6;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad7:
                    return Keys.Numpad7;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad8:
                    return Keys.Numpad8;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPad9:
                    return Keys.Numpad9;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadDecimal:
                    return Keys.NumpadDot;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadDivide:
                    return Keys.NumpadDivide;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadMultiply:
                    return Keys.NumpadMultiply;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadSubtract:
                    return Keys.NumpadSubtract;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadAdd:
                    return Keys.NumpadAdd;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadEnter:
                    return Keys.NumpadEnter;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadEqual:
                    return Keys.NumpadEquals;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftShift:
                    return Keys.ShiftLeft;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl:
                    return Keys.ControlLeft;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftAlt:
                    return Keys.AltLeft;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftSuper:
                    return Keys.Sym;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightShift:
                    return Keys.ShiftRight;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightControl:
                    return Keys.ControlRight;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightAlt:
                    return Keys.AltRight;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.RightSuper:
                    return Keys.Sym;
                case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Menu:
                    return Keys.Menu;
                default:
                    return Keys.Unknown;
            }
        }

        public void Dispose()
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

        public override float GetAccelerometerX()
        {
            return 0;
        }

        public override float GetAccelerometerY()
        {
            return 0;
        }

        public override float GetAccelerometerZ()
        {
            return 0;
        }

        public override bool IsPeripheralAvailable(Peripheral peripheral)
        {
            return peripheral == Peripheral.HardwareKeyboard;
        }

        public override int GetRotation()
        {
            return 0;
        }

        public override Orientation GetNativeOrientation()
        {
            return Orientation.Landscape;
        }

        public override void SetOnscreenKeyboardVisible(bool visible)
        {
        }

        public override void SetOnscreenKeyboardVisible(bool visible, OnscreenKeyboardType? type)
        {
        }

        public override void Vibrate(int milliseconds)
        {
        }

        public override void Vibrate(int milliseconds, bool fallback)
        {
        }

        public override void Vibrate(int milliseconds, int amplitude, bool fallback)
        {
        }

        public override void Vibrate(VibrationType vibrationType)
        {
        }

        public override float GetAzimuth()
        {
            return 0;
        }

        public override float GetPitch()
        {
            return 0;
        }

        public override float GetRoll()
        {
            return 0;
        }

        public override void GetRotationMatrix(float[] matrix)
        {
        }

        public override float GetGyroscopeX()
        {
            return 0;
        }

        public override float GetGyroscopeY()
        {
            return 0;
        }

        public override float GetGyroscopeZ()
        {
            return 0;
        }

        public override void OpenTextInputField(NativeInputConfiguration configuration)
        {

        }

        public override void CloseTextInputField(bool sendReturn)
        {

        }

        public override void SetKeyboardHeightObserver(IKeyboardHeightObserver observer)
        {

        }
    }
}