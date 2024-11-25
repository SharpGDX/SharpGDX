using SharpGDX.Shims;
using SharpGDX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX
{
    public partial interface IInput
    {
        public static class Keys
        {
            public const int AnyKey = -1;
            public const int Num0 = 7;
            public const int Num1 = 8;
            public const int Num2 = 9;
            public const int Num3 = 10;
            public const int Num4 = 11;
            public const int Num5 = 12;
            public const int Num6 = 13;
            public const int Num7 = 14;
            public const int Num8 = 15;
            public const int Num9 = 16;
            public const int A = 29;
            public const int AltLeft = 57;
            public const int AltRight = 58;
            public const int Apostrophe = 75;
            public const int At = 77;
            public const int B = 30;
            public const int Back = 4;
            public const int Backslash = 73;
            public const int C = 31;
            public const int Call = 5;
            public const int Camera = 27;
            public const int CapsLock = 115;
            public const int Clear = 28;
            public const int Comma = 55;
            public const int D = 32;
            public const int Del = 67;
            public const int Backspace = 67;
            public const int ForwardDel = 112;
            public const int DPadCenter = 23;
            public const int DPadDown = 20;
            public const int DPadLeft = 21;
            public const int DPadRight = 22;
            public const int DPadUp = 19;
            public const int Center = 23;
            public const int Down = 20;
            public const int Left = 21;
            public const int Right = 22;
            public const int Up = 19;
            public const int E = 33;
            public const int EndCall = 6;
            public const int Enter = 66;
            public const int Envelope = 65;
            public const int EQUALS = 70;
            public const int Explorer = 64;
            public const int F = 34;
            public const int Focus = 80;
            public const int G = 35;
            public const int Grave = 68;
            public const int H = 36;
            public const int HeadsetHook = 79;
            public const int Home = 3;
            public const int I = 37;
            public const int J = 38;
            public const int K = 39;
            public const int L = 40;
            public const int LeftBracket = 71;
            public const int M = 41;
            public const int MediaFastForward = 90;
            public const int MediaNext = 87;
            public const int MediaPlayPause = 85;
            public const int MediaPrevious = 88;
            public const int MediaRewind = 89;
            public const int MediaStop = 86;
            public const int Menu = 82;
            public const int Minus = 69;
            public const int Mute = 91;
            public const int N = 42;
            public const int Notification = 83;
            public const int Num = 78;
            public const int O = 43;
            public const int P = 44;
            public const int Pause = 121; // aka break
            public const int Period = 56;
            public const int Plus = 81;
            public const int Pound = 18;
            public const int Power = 26;
            public const int PrintScreen = 120; // aka SYSRQ
            public const int Q = 45;
            public const int R = 46;
            public const int RightBracket = 72;
            public const int S = 47;
            public const int ScrollLock = 116;
            public const int Search = 84;
            public const int Semicolon = 74;
            public const int ShiftLeft = 59;
            public const int ShiftRight = 60;
            public const int Slash = 76;
            public const int SoftLeft = 1;
            public const int SoftRight = 2;
            public const int Space = 62;
            public const int Star = 17;
            public const int Sym = 63; // on MacOS, this is Command (⌘)
            public const int T = 48;
            public const int Tab = 61;
            public const int U = 49;
            public const int Unknown = 0;
            public const int V = 50;
            public const int VolumeDown = 25;
            public const int VolumeUp = 24;
            public const int W = 51;
            public const int X = 52;
            public const int Y = 53;
            public const int Z = 54;
            public const int MetaAltLeftOn = 16;
            public const int MetaAltOn = 2;
            public const int MetaAltRightOn = 32;
            public const int MetaShiftLeftOn = 64;
            public const int MetaShiftOn = 1;
            public const int MetaShiftRightOn = 128;
            public const int MetaSymOn = 4;
            public const int ControlLeft = 129;
            public const int ControlRight = 130;
            public const int Escape = 111;
            public const int End = 123;
            public const int Insert = 124;
            public const int PageUp = 92;
            public const int PageDown = 93;
            public const int PictSymbols = 94;
            public const int SwitchCharset = 95;
            public const int ButtonCircle = 255;
            public const int ButtonA = 96;
            public const int ButtonB = 97;
            public const int ButtonC = 98;
            public const int ButtonX = 99;
            public const int ButtonY = 100;
            public const int ButtonZ = 101;
            public const int ButtonL1 = 102;
            public const int ButtonR1 = 103;
            public const int ButtonL2 = 104;
            public const int ButtonR2 = 105;
            public const int ButtonThumbL = 106;
            public const int ButtonThumbR = 107;
            public const int ButtonStart = 108;
            public const int ButtonSelect = 109;
            public const int ButtonMode = 110;

            public const int Numpad0 = 144;
            public const int Numpad1 = 145;
            public const int Numpad2 = 146;
            public const int Numpad3 = 147;
            public const int Numpad4 = 148;
            public const int Numpad5 = 149;
            public const int Numpad6 = 150;
            public const int Numpad7 = 151;
            public const int Numpad8 = 152;
            public const int Numpad9 = 153;

            public const int NumpadDivide = 154;
            public const int NumpadMultiply = 155;
            public const int NumpadSubtract = 156;
            public const int NumpadAdd = 157;
            public const int NumpadDot = 158;
            public const int NumpadComma = 159;
            public const int NumpadEnter = 160;
            public const int NumpadEquals = 161;
            public const int NumpadLeftParen = 162;
            public const int NumpadRightParen = 163;
            public const int NumLock = 143;

            // public const int BACKTICK = 0;
            // public const int TILDE = 0;
            // public const int UNDERSCORE = 0;
            // public const int DOT = 0;
            // public const int BREAK = 0;
            // public const int PIPE = 0;
            // public const int EXCLAMATION = 0;
            // public const int QUESTIONMARK = 0;

            // ` | VK_BACKTICK
            // ~ | VK_TILDE
            // : | VK_COLON
            // _ | VK_UNDERSCORE
            // . | VK_DOT
            // (break) | VK_BREAK
            // | | VK_PIPE
            // ! | VK_EXCLAMATION
            // ? | VK_QUESTION
            public const int Colon = 243;
            public const int F1 = 131;
            public const int F2 = 132;
            public const int F3 = 133;
            public const int F4 = 134;
            public const int F5 = 135;
            public const int F6 = 136;
            public const int F7 = 137;
            public const int F8 = 138;
            public const int F9 = 139;
            public const int F10 = 140;
            public const int F11 = 141;
            public const int F12 = 142;
            public const int F13 = 183;
            public const int F14 = 184;
            public const int F15 = 185;
            public const int F16 = 186;
            public const int F17 = 187;
            public const int F18 = 188;
            public const int F19 = 189;
            public const int F20 = 190;
            public const int F21 = 191;
            public const int F22 = 192;
            public const int F23 = 193;
            public const int F24 = 194;

            public const int MaxKeycode = 255;

            /** @return a human readable representation of the keycode. The returned value can be used in
             *         {@link Input.Keys#valueOf(String)} */
            public static string toString(int keycode)
            {
                if (keycode < 0) throw new IllegalArgumentException("keycode cannot be negative, keycode: " + keycode);
                if (keycode > MaxKeycode)
                    throw new IllegalArgumentException("keycode cannot be greater than 255, keycode: " + keycode);
                switch (keycode)
                {
                    // META* variables should not be used with this method.
                    case Unknown:
                        return "Unknown";
                    case SoftLeft:
                        return "Soft Left";
                    case SoftRight:
                        return "Soft Right";
                    case Home:
                        return "Home";
                    case Back:
                        return "Back";
                    case Call:
                        return "Call";
                    case EndCall:
                        return "End Call";
                    case Num0:
                        return "0";
                    case Num1:
                        return "1";
                    case Num2:
                        return "2";
                    case Num3:
                        return "3";
                    case Num4:
                        return "4";
                    case Num5:
                        return "5";
                    case Num6:
                        return "6";
                    case Num7:
                        return "7";
                    case Num8:
                        return "8";
                    case Num9:
                        return "9";
                    case Star:
                        return "*";
                    case Pound:
                        return "#";
                    case Up:
                        return "Up";
                    case Down:
                        return "Down";
                    case Left:
                        return "Left";
                    case Right:
                        return "Right";
                    case Center:
                        return "Center";
                    case VolumeUp:
                        return "Volume Up";
                    case VolumeDown:
                        return "Volume Down";
                    case Power:
                        return "Power";
                    case Camera:
                        return "Camera";
                    case Clear:
                        return "Clear";
                    case A:
                        return "A";
                    case B:
                        return "B";
                    case C:
                        return "C";
                    case D:
                        return "D";
                    case E:
                        return "E";
                    case F:
                        return "F";
                    case G:
                        return "G";
                    case H:
                        return "H";
                    case I:
                        return "I";
                    case J:
                        return "J";
                    case K:
                        return "K";
                    case L:
                        return "L";
                    case M:
                        return "M";
                    case N:
                        return "N";
                    case O:
                        return "O";
                    case P:
                        return "P";
                    case Q:
                        return "Q";
                    case R:
                        return "R";
                    case S:
                        return "S";
                    case T:
                        return "T";
                    case U:
                        return "U";
                    case V:
                        return "V";
                    case W:
                        return "W";
                    case X:
                        return "X";
                    case Y:
                        return "Y";
                    case Z:
                        return "Z";
                    case Comma:
                        return ",";
                    case Period:
                        return ".";
                    case AltLeft:
                        return "L-Alt";
                    case AltRight:
                        return "R-Alt";
                    case ShiftLeft:
                        return "L-Shift";
                    case ShiftRight:
                        return "R-Shift";
                    case Tab:
                        return "Tab";
                    case Space:
                        return "Space";
                    case Sym:
                        return "SYM";
                    case Explorer:
                        return "Explorer";
                    case Envelope:
                        return "Envelope";
                    case Enter:
                        return "Enter";
                    case Del:
                        return "Delete"; // also BACKSPACE
                    case Grave:
                        return "`";
                    case Minus:
                        return "-";
                    case EQUALS:
                        return "=";
                    case LeftBracket:
                        return "[";
                    case RightBracket:
                        return "]";
                    case Backslash:
                        return "\\";
                    case Semicolon:
                        return ";";
                    case Apostrophe:
                        return "'";
                    case Slash:
                        return "/";
                    case At:
                        return "@";
                    case Num:
                        return "Num";
                    case HeadsetHook:
                        return "Headset Hook";
                    case Focus:
                        return "Focus";
                    case Plus:
                        return "Plus";
                    case Menu:
                        return "Menu";
                    case Notification:
                        return "Notification";
                    case Search:
                        return "Search";
                    case MediaPlayPause:
                        return "Play/Pause";
                    case MediaStop:
                        return "Stop Media";
                    case MediaNext:
                        return "Next Media";
                    case MediaPrevious:
                        return "Prev Media";
                    case MediaRewind:
                        return "Rewind";
                    case MediaFastForward:
                        return "Fast Forward";
                    case Mute:
                        return "Mute";
                    case PageUp:
                        return "Page Up";
                    case PageDown:
                        return "Page Down";
                    case PictSymbols:
                        return "PICTSYMBOLS";
                    case SwitchCharset:
                        return "SWITCH_CHARSET";
                    case ButtonA:
                        return "A Button";
                    case ButtonB:
                        return "B Button";
                    case ButtonC:
                        return "C Button";
                    case ButtonX:
                        return "X Button";
                    case ButtonY:
                        return "Y Button";
                    case ButtonZ:
                        return "Z Button";
                    case ButtonL1:
                        return "L1 Button";
                    case ButtonR1:
                        return "R1 Button";
                    case ButtonL2:
                        return "L2 Button";
                    case ButtonR2:
                        return "R2 Button";
                    case ButtonThumbL:
                        return "Left Thumb";
                    case ButtonThumbR:
                        return "Right Thumb";
                    case ButtonStart:
                        return "Start";
                    case ButtonSelect:
                        return "Select";
                    case ButtonMode:
                        return "Button Mode";
                    case ForwardDel:
                        return "Forward Delete";
                    case ControlLeft:
                        return "L-Ctrl";
                    case ControlRight:
                        return "R-Ctrl";
                    case Escape:
                        return "Escape";
                    case End:
                        return "End";
                    case Insert:
                        return "Insert";
                    case Numpad0:
                        return "Numpad 0";
                    case Numpad1:
                        return "Numpad 1";
                    case Numpad2:
                        return "Numpad 2";
                    case Numpad3:
                        return "Numpad 3";
                    case Numpad4:
                        return "Numpad 4";
                    case Numpad5:
                        return "Numpad 5";
                    case Numpad6:
                        return "Numpad 6";
                    case Numpad7:
                        return "Numpad 7";
                    case Numpad8:
                        return "Numpad 8";
                    case Numpad9:
                        return "Numpad 9";
                    case Colon:
                        return ":";
                    case F1:
                        return "F1";
                    case F2:
                        return "F2";
                    case F3:
                        return "F3";
                    case F4:
                        return "F4";
                    case F5:
                        return "F5";
                    case F6:
                        return "F6";
                    case F7:
                        return "F7";
                    case F8:
                        return "F8";
                    case F9:
                        return "F9";
                    case F10:
                        return "F10";
                    case F11:
                        return "F11";
                    case F12:
                        return "F12";
                    case F13:
                        return "F13";
                    case F14:
                        return "F14";
                    case F15:
                        return "F15";
                    case F16:
                        return "F16";
                    case F17:
                        return "F17";
                    case F18:
                        return "F18";
                    case F19:
                        return "F19";
                    case F20:
                        return "F20";
                    case F21:
                        return "F21";
                    case F22:
                        return "F22";
                    case F23:
                        return "F23";
                    case F24:
                        return "F24";
                    case NumpadDivide:
                        return "Num /";
                    case NumpadMultiply:
                        return "Num *";
                    case NumpadSubtract:
                        return "Num -";
                    case NumpadAdd:
                        return "Num +";
                    case NumpadDot:
                        return "Num .";
                    case NumpadComma:
                        return "Num ,";
                    case NumpadEnter:
                        return "Num Enter";
                    case NumpadEquals:
                        return "Num =";
                    case NumpadLeftParen:
                        return "Num (";
                    case NumpadRightParen:
                        return "Num )";
                    case NumLock:
                        return "Num Lock";
                    case CapsLock:
                        return "Caps Lock";
                    case ScrollLock:
                        return "Scroll Lock";
                    case Pause:
                        return "Pause";
                    case PrintScreen:
                        return "Print";
                    // BUTTON_CIRCLE unhandled, as it conflicts with the more likely to be pressed F12
                    default:
                        // key name not found
                        return null;
                }
            }

            private static ObjectIntMap<string> keyNames;

            /** @param keyname the keyname returned by the {@link Keys#toString(int)} method
             * @return the int keycode */
            public static int valueOf(string keyname)
            {
                if (keyNames == null) initializeKeyNames();
                return keyNames.get(keyname, -1);
            }

            /** lazily intialized in {@link Keys#valueOf(String)} */
            private static void initializeKeyNames()
            {
                keyNames = new ObjectIntMap<string>();
                for (int i = 0; i < 256; i++)
                {
                    string name = toString(i);
                    if (name != null) keyNames.put(name, i);
                }
            }
        }
    }

}