﻿using SharpGDX.Shims;
using SharpGDX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Input
{
    public static class Keys
    {
        // TODO: Fix casing. -RP
        public const int ANY_KEY = -1;
        public const int NUM_0 = 7;
        public const int NUM_1 = 8;
        public const int NUM_2 = 9;
        public const int NUM_3 = 10;
        public const int NUM_4 = 11;
        public const int NUM_5 = 12;
        public const int NUM_6 = 13;
        public const int NUM_7 = 14;
        public const int NUM_8 = 15;
        public const int NUM_9 = 16;
        public const int A = 29;
        public const int ALT_LEFT = 57;
        public const int ALT_RIGHT = 58;
        public const int APOSTROPHE = 75;
        public const int AT = 77;
        public const int B = 30;
        public const int BACK = 4;
        public const int BACKSLASH = 73;
        public const int C = 31;
        public const int CALL = 5;
        public const int CAMERA = 27;
        public const int CAPS_LOCK = 115;
        public const int CLEAR = 28;
        public const int COMMA = 55;
        public const int D = 32;
        public const int DEL = 67;
        public const int BACKSPACE = 67;
        public const int FORWARD_DEL = 112;
        public const int DPAD_CENTER = 23;
        public const int DPAD_DOWN = 20;
        public const int DPAD_LEFT = 21;
        public const int DPAD_RIGHT = 22;
        public const int DPAD_UP = 19;
        public const int CENTER = 23;
        public const int DOWN = 20;
        public const int LEFT = 21;
        public const int RIGHT = 22;
        public const int UP = 19;
        public const int E = 33;
        public const int ENDCALL = 6;
        public const int ENTER = 66;
        public const int ENVELOPE = 65;
        public const int EQUALS = 70;
        public const int EXPLORER = 64;
        public const int F = 34;
        public const int FOCUS = 80;
        public const int G = 35;
        public const int GRAVE = 68;
        public const int H = 36;
        public const int HEADSETHOOK = 79;
        public const int HOME = 3;
        public const int I = 37;
        public const int J = 38;
        public const int K = 39;
        public const int L = 40;
        public const int LEFT_BRACKET = 71;
        public const int M = 41;
        public const int MEDIA_FAST_FORWARD = 90;
        public const int MEDIA_NEXT = 87;
        public const int MEDIA_PLAY_PAUSE = 85;
        public const int MEDIA_PREVIOUS = 88;
        public const int MEDIA_REWIND = 89;
        public const int MEDIA_STOP = 86;
        public const int MENU = 82;
        public const int MINUS = 69;
        public const int MUTE = 91;
        public const int N = 42;
        public const int NOTIFICATION = 83;
        public const int NUM = 78;
        public const int O = 43;
        public const int P = 44;
        public const int PAUSE = 121; // aka break
        public const int PERIOD = 56;
        public const int PLUS = 81;
        public const int POUND = 18;
        public const int POWER = 26;
        public const int PRINT_SCREEN = 120; // aka SYSRQ
        public const int Q = 45;
        public const int R = 46;
        public const int RIGHT_BRACKET = 72;
        public const int S = 47;
        public const int SCROLL_LOCK = 116;
        public const int SEARCH = 84;
        public const int SEMICOLON = 74;
        public const int SHIFT_LEFT = 59;
        public const int SHIFT_RIGHT = 60;
        public const int SLASH = 76;
        public const int SOFT_LEFT = 1;
        public const int SOFT_RIGHT = 2;
        public const int SPACE = 62;
        public const int STAR = 17;
        public const int SYM = 63; // on MacOS, this is Command (⌘)
        public const int T = 48;
        public const int TAB = 61;
        public const int U = 49;
        public const int UNKNOWN = 0;
        public const int V = 50;
        public const int VOLUME_DOWN = 25;
        public const int VOLUME_UP = 24;
        public const int W = 51;
        public const int X = 52;
        public const int Y = 53;
        public const int Z = 54;
        public const int META_ALT_LEFT_ON = 16;
        public const int META_ALT_ON = 2;
        public const int META_ALT_RIGHT_ON = 32;
        public const int META_SHIFT_LEFT_ON = 64;
        public const int META_SHIFT_ON = 1;
        public const int META_SHIFT_RIGHT_ON = 128;
        public const int META_SYM_ON = 4;
        public const int CONTROL_LEFT = 129;
        public const int CONTROL_RIGHT = 130;
        public const int ESCAPE = 111;
        public const int END = 123;
        public const int INSERT = 124;
        public const int PAGE_UP = 92;
        public const int PAGE_DOWN = 93;
        public const int PICTSYMBOLS = 94;
        public const int SWITCH_CHARSET = 95;
        public const int BUTTON_CIRCLE = 255;
        public const int BUTTON_A = 96;
        public const int BUTTON_B = 97;
        public const int BUTTON_C = 98;
        public const int BUTTON_X = 99;
        public const int BUTTON_Y = 100;
        public const int BUTTON_Z = 101;
        public const int BUTTON_L1 = 102;
        public const int BUTTON_R1 = 103;
        public const int BUTTON_L2 = 104;
        public const int BUTTON_R2 = 105;
        public const int BUTTON_THUMBL = 106;
        public const int BUTTON_THUMBR = 107;
        public const int BUTTON_START = 108;
        public const int BUTTON_SELECT = 109;
        public const int BUTTON_MODE = 110;

        public const int NUMPAD_0 = 144;
        public const int NUMPAD_1 = 145;
        public const int NUMPAD_2 = 146;
        public const int NUMPAD_3 = 147;
        public const int NUMPAD_4 = 148;
        public const int NUMPAD_5 = 149;
        public const int NUMPAD_6 = 150;
        public const int NUMPAD_7 = 151;
        public const int NUMPAD_8 = 152;
        public const int NUMPAD_9 = 153;

        public const int NUMPAD_DIVIDE = 154;
        public const int NUMPAD_MULTIPLY = 155;
        public const int NUMPAD_SUBTRACT = 156;
        public const int NUMPAD_ADD = 157;
        public const int NUMPAD_DOT = 158;
        public const int NUMPAD_COMMA = 159;
        public const int NUMPAD_ENTER = 160;
        public const int NUMPAD_EQUALS = 161;
        public const int NUMPAD_LEFT_PAREN = 162;
        public const int NUMPAD_RIGHT_PAREN = 163;
        public const int NUM_LOCK = 143;

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
        public const int COLON = 243;
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

        public const int MAX_KEYCODE = 255;

        /** @return a human readable representation of the keycode. The returned value can be used in
         *         {@link Input.Keys#valueOf(String)} */
        public static String toString(int keycode)
        {
            if (keycode < 0) throw new IllegalArgumentException("keycode cannot be negative, keycode: " + keycode);
            if (keycode > MAX_KEYCODE) throw new IllegalArgumentException("keycode cannot be greater than 255, keycode: " + keycode);
            switch (keycode)
            {
                // META* variables should not be used with this method.
                case UNKNOWN:
                    return "Unknown";
                case SOFT_LEFT:
                    return "Soft Left";
                case SOFT_RIGHT:
                    return "Soft Right";
                case HOME:
                    return "Home";
                case BACK:
                    return "Back";
                case CALL:
                    return "Call";
                case ENDCALL:
                    return "End Call";
                case NUM_0:
                    return "0";
                case NUM_1:
                    return "1";
                case NUM_2:
                    return "2";
                case NUM_3:
                    return "3";
                case NUM_4:
                    return "4";
                case NUM_5:
                    return "5";
                case NUM_6:
                    return "6";
                case NUM_7:
                    return "7";
                case NUM_8:
                    return "8";
                case NUM_9:
                    return "9";
                case STAR:
                    return "*";
                case POUND:
                    return "#";
                case UP:
                    return "Up";
                case DOWN:
                    return "Down";
                case LEFT:
                    return "Left";
                case RIGHT:
                    return "Right";
                case CENTER:
                    return "Center";
                case VOLUME_UP:
                    return "Volume Up";
                case VOLUME_DOWN:
                    return "Volume Down";
                case POWER:
                    return "Power";
                case CAMERA:
                    return "Camera";
                case CLEAR:
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
                case COMMA:
                    return ",";
                case PERIOD:
                    return ".";
                case ALT_LEFT:
                    return "L-Alt";
                case ALT_RIGHT:
                    return "R-Alt";
                case SHIFT_LEFT:
                    return "L-Shift";
                case SHIFT_RIGHT:
                    return "R-Shift";
                case TAB:
                    return "Tab";
                case SPACE:
                    return "Space";
                case SYM:
                    return "SYM";
                case EXPLORER:
                    return "Explorer";
                case ENVELOPE:
                    return "Envelope";
                case ENTER:
                    return "Enter";
                case DEL:
                    return "Delete"; // also BACKSPACE
                case GRAVE:
                    return "`";
                case MINUS:
                    return "-";
                case EQUALS:
                    return "=";
                case LEFT_BRACKET:
                    return "[";
                case RIGHT_BRACKET:
                    return "]";
                case BACKSLASH:
                    return "\\";
                case SEMICOLON:
                    return ";";
                case APOSTROPHE:
                    return "'";
                case SLASH:
                    return "/";
                case AT:
                    return "@";
                case NUM:
                    return "Num";
                case HEADSETHOOK:
                    return "Headset Hook";
                case FOCUS:
                    return "Focus";
                case PLUS:
                    return "Plus";
                case MENU:
                    return "Menu";
                case NOTIFICATION:
                    return "Notification";
                case SEARCH:
                    return "Search";
                case MEDIA_PLAY_PAUSE:
                    return "Play/Pause";
                case MEDIA_STOP:
                    return "Stop Media";
                case MEDIA_NEXT:
                    return "Next Media";
                case MEDIA_PREVIOUS:
                    return "Prev Media";
                case MEDIA_REWIND:
                    return "Rewind";
                case MEDIA_FAST_FORWARD:
                    return "Fast Forward";
                case MUTE:
                    return "Mute";
                case PAGE_UP:
                    return "Page Up";
                case PAGE_DOWN:
                    return "Page Down";
                case PICTSYMBOLS:
                    return "PICTSYMBOLS";
                case SWITCH_CHARSET:
                    return "SWITCH_CHARSET";
                case BUTTON_A:
                    return "A Button";
                case BUTTON_B:
                    return "B Button";
                case BUTTON_C:
                    return "C Button";
                case BUTTON_X:
                    return "X Button";
                case BUTTON_Y:
                    return "Y Button";
                case BUTTON_Z:
                    return "Z Button";
                case BUTTON_L1:
                    return "L1 Button";
                case BUTTON_R1:
                    return "R1 Button";
                case BUTTON_L2:
                    return "L2 Button";
                case BUTTON_R2:
                    return "R2 Button";
                case BUTTON_THUMBL:
                    return "Left Thumb";
                case BUTTON_THUMBR:
                    return "Right Thumb";
                case BUTTON_START:
                    return "Start";
                case BUTTON_SELECT:
                    return "Select";
                case BUTTON_MODE:
                    return "Button Mode";
                case FORWARD_DEL:
                    return "Forward Delete";
                case CONTROL_LEFT:
                    return "L-Ctrl";
                case CONTROL_RIGHT:
                    return "R-Ctrl";
                case ESCAPE:
                    return "Escape";
                case END:
                    return "End";
                case INSERT:
                    return "Insert";
                case NUMPAD_0:
                    return "Numpad 0";
                case NUMPAD_1:
                    return "Numpad 1";
                case NUMPAD_2:
                    return "Numpad 2";
                case NUMPAD_3:
                    return "Numpad 3";
                case NUMPAD_4:
                    return "Numpad 4";
                case NUMPAD_5:
                    return "Numpad 5";
                case NUMPAD_6:
                    return "Numpad 6";
                case NUMPAD_7:
                    return "Numpad 7";
                case NUMPAD_8:
                    return "Numpad 8";
                case NUMPAD_9:
                    return "Numpad 9";
                case COLON:
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
                case NUMPAD_DIVIDE:
                    return "Num /";
                case NUMPAD_MULTIPLY:
                    return "Num *";
                case NUMPAD_SUBTRACT:
                    return "Num -";
                case NUMPAD_ADD:
                    return "Num +";
                case NUMPAD_DOT:
                    return "Num .";
                case NUMPAD_COMMA:
                    return "Num ,";
                case NUMPAD_ENTER:
                    return "Num Enter";
                case NUMPAD_EQUALS:
                    return "Num =";
                case NUMPAD_LEFT_PAREN:
                    return "Num (";
                case NUMPAD_RIGHT_PAREN:
                    return "Num )";
                case NUM_LOCK:
                    return "Num Lock";
                case CAPS_LOCK:
                    return "Caps Lock";
                case SCROLL_LOCK:
                    return "Scroll Lock";
                case PAUSE:
                    return "Pause";
                case PRINT_SCREEN:
                    return "Print";
                // BUTTON_CIRCLE unhandled, as it conflicts with the more likely to be pressed F12
                default:
                    // key name not found
                    return null;
            }
        }

        private static ObjectIntMap<String> keyNames;

        /** @param keyname the keyname returned by the {@link Keys#toString(int)} method
         * @return the int keycode */
        public static int valueOf(String keyname)
        {
            if (keyNames == null) initializeKeyNames();
            return keyNames.get(keyname, -1);
        }

        /** lazily intialized in {@link Keys#valueOf(String)} */
        private static void initializeKeyNames()
        {
            keyNames = new ObjectIntMap<String>();
            for (int i = 0; i < 256; i++)
            {
                String name = toString(i);
                if (name != null) keyNames.put(name, i);
            }
        }
    }
}
