using SharpGDX.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;

namespace SharpGDX.Utils
{
    /** Lightweight JSON parser.<br>
 * <br>
 * The default behavior is to parse the JSON into a DOM containing {@link JsonValue} objects. Extend this class and override
 * methods to perform event driven parsing. When this is done, the parse methods will return null.
 * @author Nathan Sweet */
    public class JsonReader : BaseJsonReader
    {
        public JsonValue parse(String json)
        {
            char[] data = json.ToCharArray();
            return parse(data, 0, data.Length);
        }

        public JsonValue parse(Reader reader)
        {
            char[] data = new char[1024];
            int offset = 0;
            try
            {
                while (true)
                {
                    int length = reader.read(data, offset, data.Length - offset);
                    if (length == -1) break;
                    if (length == 0)
                    {
                        char[] newData = new char[data.Length * 2];
                        System.Array.Copy(data, 0, newData, 0, data.Length);
                        data = newData;
                    }
                    else
                        offset += length;
                }
            }
            catch (IOException ex)
            {
                throw new SerializationException("Error reading input.", ex);
            }
            finally
            {
                StreamUtils.closeQuietly(reader);
            }

            return parse(data, 0, offset);
        }

        public JsonValue parse(InputStream input)
        {
            Reader reader;
            try
            {
                reader = new InputStreamReader(input, "UTF-8");
            }
            catch (Exception ex)
            {
                throw new SerializationException("Error reading stream.", ex);
            }

            return parse(reader);
        }

        public JsonValue parse(FileHandle file)
        {
            Reader reader;
            try
            {
                reader = file.reader("UTF-8");
            }
            catch (Exception ex)
            {
                throw new SerializationException("Error reading file: " + file, ex);
            }

            try
            {
                return parse(reader);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Error parsing file: " + file, ex);
            }
        }

        public JsonValue parse(char[] data, int offset, int length)
        {
            _stop = false;
            int cs, p = offset, pe = length, eof = pe, top = 0;
            int[] stack = new int[4];

            int s = 0;
            String name = null;
            bool needsUnescape = false, stringIsName = false, stringIsUnquoted = false;
            RuntimeException parseRuntimeEx = null;

            bool debug = false;
            if (debug) Console.WriteLine();

            try
            {

                // line 103 "../../../../../src/com/badlogic/gdx/utils/JsonReader.java"
                {
                    cs = json_start;
                    top = 0;
                }

                // line 108 "../../../../../src/com/badlogic/gdx/utils/JsonReader.java"
                {
                    int _klen;
                    int _trans = 0;
                    int _acts;
                    int _nacts;
                    int _keys;
                    int _goto_targ = 0;

                    _goto:
                    while (true)
                    {
                        switch (_goto_targ)
                        {
                            case 0:
                                if (p == pe)
                                {
                                    _goto_targ = 4;
                                    goto _goto;
                                }

                                if (cs == 0)
                                {
                                    _goto_targ = 5;
                                    goto _goto;
                                }

                                goto case 1;
                            case 1:
                                _match:
                                do
                                {
                                    _keys = _json_key_offsets[cs];
                                    _trans = _json_index_offsets[cs];
                                    _klen = _json_single_lengths[cs];
                                    if (_klen > 0)
                                    {
                                        int _lower = _keys;
                                        int _mid;
                                        int _upper = _keys + _klen - 1;
                                        while (true)
                                        {
                                            if (_upper < _lower) break;

                                            _mid = _lower + ((_upper - _lower) >> 1);
                                            if (data[p] < _json_trans_keys[_mid])
                                                _upper = _mid - 1;
                                            else if (data[p] > _json_trans_keys[_mid])
                                                _lower = _mid + 1;
                                            else
                                            {
                                                _trans += (_mid - _keys);
                                                goto endOfMatch;
                                            }
                                        }

                                        _keys += _klen;
                                        _trans += _klen;
                                    }

                                    _klen = _json_range_lengths[cs];
                                    if (_klen > 0)
                                    {
                                        int _lower = _keys;
                                        int _mid;
                                        int _upper = _keys + (_klen << 1) - 2;
                                        while (true)
                                        {
                                            if (_upper < _lower) break;

                                            _mid = _lower + (((_upper - _lower) >> 1) & ~1);
                                            if (data[p] < _json_trans_keys[_mid])
                                                _upper = _mid - 2;
                                            else if (data[p] > _json_trans_keys[_mid + 1])
                                                _lower = _mid + 2;
                                            else
                                            {
                                                _trans += ((_mid - _keys) >> 1);
                                                goto endOfMatch;
                                            }
                                        }

                                        _trans += _klen;
                                    }
                                } while (false);

                                endOfMatch:

                                _trans = _json_indicies[_trans];
                                cs = _json_trans_targs[_trans];

                                if (_json_trans_actions[_trans] != 0)
                                {
                                    _acts = _json_trans_actions[_trans];
                                    _nacts = (int)_json_actions[_acts++];
                                    while (_nacts-- > 0)
                                    {
                                        switch (_json_actions[_acts++])
                                        {
                                            case 0:
                                                // line 108 "JsonReader.rl"
                                            {
                                                stringIsName = true;
                                            }
                                                break;
                                            case 1:
                                                // line 111 "JsonReader.rl"
                                            {
                                                String value = new String(data, s, p - s);
                                                if (needsUnescape) value = unescape(value);
                                                outer:
                                                if (stringIsName)
                                                {
                                                    stringIsName = false;
                                                    if (debug) Console.WriteLine("name: " + value);
                                                    name = value;
                                                }
                                                else
                                                {
                                                    String valueName = name;
                                                    name = null;
                                                    if (stringIsUnquoted)
                                                    {
                                                        if (value.Equals("true"))
                                                        {
                                                            if (debug)
                                                                Console.WriteLine("boolean: " + valueName + "=true");
                                                            @bool(valueName, true);
                                                            goto endOfOuter;
                                                        }
                                                        else if (value.Equals("false"))
                                                        {
                                                            if (debug)
                                                                Console.WriteLine("boolean: " + valueName + "=false");
                                                            @bool(valueName, false);
                                                            goto endOfOuter;
                                                        }
                                                        else if (value.Equals("null"))
                                                        {
                                                            @string(valueName, null);
                                                            goto endOfOuter;
                                                        }

                                                        bool couldBeDouble = false, couldBeLong = true;
                                                        outer2:
                                                        for (int i = s; i < p; i++)
                                                        {
                                                            switch (data[i])
                                                            {
                                                                case '0':
                                                                case '1':
                                                                case '2':
                                                                case '3':
                                                                case '4':
                                                                case '5':
                                                                case '6':
                                                                case '7':
                                                                case '8':
                                                                case '9':
                                                                case '-':
                                                                case '+':
                                                                    break;
                                                                case '.':
                                                                case 'e':
                                                                case 'E':
                                                                    couldBeDouble = true;
                                                                    couldBeLong = false;
                                                                    break;
                                                                default:
                                                                    couldBeDouble = false;
                                                                    couldBeLong = false;
                                                                    goto endOfOuter2;
                                                            }
                                                        }

                                                        endOfOuter2:
                                                        if (couldBeDouble)
                                                        {
                                                            try
                                                            {
                                                                if (debug)
                                                                    Console.WriteLine("double: " + valueName + "=" +
                                                                        double.Parse(value));
                                                                number(valueName, double.Parse(value), value);
                                                                goto endOfOuter;
                                                            }
                                                            catch (FormatException ignored)
                                                            {
                                                            }
                                                        }
                                                        else if (couldBeLong)
                                                        {
                                                            if (debug)
                                                                Console.WriteLine("double: " + valueName + "=" +
                                                                    double.Parse(value));
                                                            try
                                                            {
                                                                number(valueName, long.Parse(value), value);
                                                                goto endOfOuter;
                                                            }
                                                            catch (FormatException ignored)
                                                            {
                                                            }
                                                        }
                                                    }

                                                    if (debug) Console.WriteLine("string: " + valueName + "=" + value);
                                                    @string(valueName, value);
                                                }

                                                endOfOuter:

                                                if (_stop) goto endOfGoTo;
                                                stringIsUnquoted = false;
                                                s = p;
                                            }
                                                break;
                                            case 2:
                                                // line 187 "JsonReader.rl"
                                            {
                                                if (debug) Console.WriteLine("startObject: " + name);
                                                startObject(name);
                                                if (_stop) goto endOfGoTo;
                                                name = null;
                                                {
                                                    if (top == stack.Length) Array.Resize(ref stack, stack.Length * 2);
                                                    {
                                                        stack[top++] = cs;
                                                        cs = 5;
                                                        _goto_targ = 2;
                                                        if (true) goto _goto;
                                                    }
                                                }
                                            }
                                                break;
                                            case 3:
                                                // line 194 "JsonReader.rl"
                                            {
                                                if (debug) Console.WriteLine("endObject");
                                                pop();
                                                if (_stop) goto endOfGoTo;
                                                {
                                                    cs = stack[--top];
                                                    _goto_targ = 2;
                                                    if (true) goto _goto;
                                                }
                                            }
                                                break;
                                            case 4:
                                                // line 200 "JsonReader.rl"
                                            {
                                                if (debug) Console.WriteLine("startArray: " + name);
                                                startArray(name);
                                                if (_stop) goto endOfGoTo;
                                                name = null;
                                                {
                                                    if (top == stack.Length) Array.Resize(ref stack, stack.Length * 2);
                                                    {
                                                        stack[top++] = cs;
                                                        cs = 23;
                                                        _goto_targ = 2;
                                                        if (true) goto _goto;
                                                    }
                                                }
                                            }
                                                break;
                                            case 5:
                                                // line 207 "JsonReader.rl"
                                            {
                                                if (debug) Console.WriteLine("endArray");
                                                pop();
                                                if (_stop) goto endOfGoTo;
                                                {
                                                    cs = stack[--top];
                                                    _goto_targ = 2;
                                                    if (true) goto _goto;
                                                }
                                            }
                                                break;
                                            case 6:
                                                // line 213 "JsonReader.rl"
                                            {
                                                int start = p - 1;
                                                if (data[p++] == '/')
                                                {
                                                    while (p != eof && data[p] != '\n')
                                                        p++;
                                                    p--;
                                                }
                                                else
                                                {
                                                    while (p + 1 < eof && (data[p] != '*' || data[p + 1] != '/'))
                                                        p++;
                                                    p++;
                                                }

                                                if (debug)
                                                    Console.WriteLine("comment " + new String(data, start, p - start));
                                            }
                                                break;
                                            case 7:
                                                // line 226 "JsonReader.rl"
                                            {
                                                if (debug) Console.WriteLine("unquotedChars");
                                                s = p;
                                                needsUnescape = false;
                                                stringIsUnquoted = true;
                                                if (stringIsName)
                                                {
                                                    outer:
                                                    while (true)
                                                    {
                                                        switch (data[p])
                                                        {
                                                            case '\\':
                                                                needsUnescape = true;
                                                                break;
                                                            case '/':
                                                                if (p + 1 == eof) break;
                                                                char c = data[p + 1];
                                                                if (c == '/' || c == '*') goto endOfOuter;
                                                                break;
                                                            case ':':
                                                            case '\r':
                                                            case '\n':
                                                                goto endOfOuter;
                                                        }

                                                        if (debug)
                                                            Console.WriteLine("unquotedChar (name): '" + data[p] + "'");
                                                        p++;
                                                        if (p == eof) break;
                                                    }

                                                    endOfOuter:
                                                    {
                                                    }
                                                }
                                                else
                                                {
                                                    outer:
                                                    while (true)
                                                    {
                                                        switch (data[p])
                                                        {
                                                            case '\\':
                                                                needsUnescape = true;
                                                                break;
                                                            case '/':
                                                                if (p + 1 == eof) break;
                                                                char c = data[p + 1];
                                                                if (c == '/' || c == '*') goto endOfOuter;
                                                                break;
                                                            case '}':
                                                            case ']':
                                                            case ',':
                                                            case '\r':
                                                            case '\n':
                                                                goto endOfOuter;
                                                        }

                                                        if (debug)
                                                            Console.WriteLine("unquotedChar (value): '" + data[p] +
                                                                              "'");
                                                        p++;
                                                        if (p == eof) break;
                                                    }

                                                    endOfOuter:
                                                    {
                                                    }
                                                }

                                                p--;
                                                while (char.IsWhiteSpace(data[p]))
                                                    p--;
                                            }
                                                break;
                                            case 8:
                                                // line 280 "JsonReader.rl"
                                            {
                                                if (debug) Console.WriteLine("quotedChars");
                                                s = ++p;
                                                needsUnescape = false;
                                                outer:
                                                while (true)
                                                {
                                                    switch (data[p])
                                                    {
                                                        case '\\':
                                                            needsUnescape = true;
                                                            p++;
                                                            break;
                                                        case '"':
                                                            goto endOfOuter;
                                                    }

                                                    if (debug) Console.WriteLine("quotedChar: '" + data[p] + "'");
                                                    p++;
                                                    if (p == eof) break;
                                                }

                                                endOfOuter:
                                                p--;
                                            }
                                                break;
                                            // line 411 "../../../../../src/com/badlogic/gdx/utils/JsonReader.java"
                                        }
                                    }
                                }

                                goto case 2;
                            case 2:
                                if (cs == 0)
                                {
                                    _goto_targ = 5;
                                    goto _goto;
                                }

                                if (++p != pe)
                                {
                                    _goto_targ = 1;
                                    goto _goto;
                                }

                                goto case 4;
                            case 4:
                                if (p == eof)
                                {
                                    int __acts = _json_eof_actions[cs];
                                    int __nacts = (int)_json_actions[__acts++];
                                    while (__nacts-- > 0)
                                    {
                                        switch (_json_actions[__acts++])
                                        {
                                            case 1:
                                                // line 111 "JsonReader.rl"
                                            {
                                                String value = new String(data, s, p - s);
                                                if (needsUnescape) value = unescape(value);
                                                outer:
                                                if (stringIsName)
                                                {
                                                    stringIsName = false;
                                                    if (debug) Console.WriteLine("name: " + value);
                                                    name = value;
                                                }
                                                else
                                                {
                                                    String valueName = name;
                                                    name = null;
                                                    if (stringIsUnquoted)
                                                    {
                                                        if (value.Equals("true"))
                                                        {
                                                            if (debug)
                                                                Console.WriteLine("boolean: " + valueName + "=true");
                                                            @bool(valueName, true);
                                                            goto endOfOuter;
                                                        }
                                                        else if (value.Equals("false"))
                                                        {
                                                            if (debug)
                                                                Console.WriteLine("boolean: " + valueName + "=false");
                                                            @bool(valueName, false);
                                                            goto endOfOuter;
                                                        }
                                                        else if (value.Equals("null"))
                                                        {
                                                            @string(valueName, null);
                                                            goto endOfOuter;
                                                        }

                                                        bool couldBeDouble = false, couldBeLong = true;
                                                        outer2:
                                                        for (int i = s; i < p; i++)
                                                        {
                                                            switch (data[i])
                                                            {
                                                                case '0':
                                                                case '1':
                                                                case '2':
                                                                case '3':
                                                                case '4':
                                                                case '5':
                                                                case '6':
                                                                case '7':
                                                                case '8':
                                                                case '9':
                                                                case '-':
                                                                case '+':
                                                                    break;
                                                                case '.':
                                                                case 'e':
                                                                case 'E':
                                                                    couldBeDouble = true;
                                                                    couldBeLong = false;
                                                                    break;
                                                                default:
                                                                    couldBeDouble = false;
                                                                    couldBeLong = false;
                                                                    goto endOfOuter2;
                                                            }
                                                        }

                                                        endOfOuter2:
                                                        if (couldBeDouble)
                                                        {
                                                            try
                                                            {
                                                                if (debug)
                                                                    Console.WriteLine("double: " + valueName + "=" +
                                                                        double.Parse(value));
                                                                number(valueName, double.Parse(value), value);
                                                                goto endOfOuter;
                                                            }
                                                            catch (FormatException ignored)
                                                            {
                                                            }
                                                        }
                                                        else if (couldBeLong)
                                                        {
                                                            if (debug)
                                                                Console.WriteLine("double: " + valueName + "=" +
                                                                    double.Parse(value));
                                                            try
                                                            {
                                                                number(valueName, long.Parse(value), value);
                                                                goto endOfOuter;
                                                            }
                                                            catch (FormatException ignored)
                                                            {
                                                            }
                                                        }
                                                    }

                                                    if (debug) Console.WriteLine("string: " + valueName + "=" + value);
                                                    @string(valueName, value);
                                                }

                                                endOfOuter:
                                                if (_stop) goto endOfGoTo;
                                                stringIsUnquoted = false;
                                                s = p;
                                            }
                                                break;
                                            // line 511 "../../../../../src/com/badlogic/gdx/utils/JsonReader.java"
                                        }
                                    }
                                }

                                goto case 5;

                            case 5:
                                break;
                        }

                        break;
                    }

                    endOfGoTo:
                    {
                    }
                }

                // line 316 "JsonReader.rl"

            }
            catch (RuntimeException ex)
            {
                parseRuntimeEx = ex;
            }

            JsonValue root = this.root;
            this.root = null;
            current = null;
            lastChild.clear();

            if (!_stop)
            {
                if (p < pe)
                {
                    int lineNumber = 1;
                    for (int i = 0; i < p; i++)
                        if (data[i] == '\n')
                            lineNumber++;
                    int start = Math.Max(0, p - 32);
                    throw new SerializationException("Error parsing JSON on line " + lineNumber + " near: "
                                                     + new String(data, start, p - start) + "*ERROR*" +
                                                     new String(data, p, Math.Min(64, pe - p)), parseRuntimeEx);
                }

                if (elements.size != 0)
                {
                    JsonValue element = elements.peek();
                    elements.clear();
                    if (element != null && element.isObject())
                        throw new SerializationException("Error parsing JSON, unmatched brace.");
                    else
                        throw new SerializationException("Error parsing JSON, unmatched bracket.");
                }

                if (parseRuntimeEx != null)
                    throw new SerializationException("Error parsing JSON: " + new String(data), parseRuntimeEx);
            }

            return root;
        }

        // line 553 "../../../../../src/com/badlogic/gdx/utils/JsonReader.java"
        private static byte[] init__json_actions_0()
        {
            return new byte[] { 0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6, 1, 7, 1, 8, 2, 0, 7, 2, 0, 8, 2, 1, 3, 2, 1, 5 };
        }

        private static readonly byte[] _json_actions = init__json_actions_0();

        private static short[] init__json_key_offsets_0()
        {
            return new short[]
            {
                0, 0, 11, 13, 14, 16, 25, 31, 37, 39, 50, 57, 64, 73, 74, 83, 85, 87, 96, 98, 100, 101, 103, 105, 116,
                123, 130, 141, 142, 153, 155, 157, 168, 170, 172, 174, 179, 184, 184
            };
        }

        private static readonly short[] _json_key_offsets = init__json_key_offsets_0();

        private static char[] init__json_trans_keys_0()
        {
            return new char[]
            {
                (char)13, (char)(char)32, (char)(char)34, (char)44, (char)47, (char)58, (char)91, (char)93, (char)123,
                (char)9, (char)10, (char)42, (char)47, (char)34, (char)42, (char)47, (char)13, (char)32, (char)34,
                (char)44,
                (char)47, (char)58, (char)125, (char)9, (char)10, (char)13, (char)32,
                (char)47, (char)58, (char)9, (char)10, (char)13, (char)32, (char)47, (char)58, (char)9, (char)10,
                (char)42,
                (char)47, (char)13, (char)32, (char)34, (char)44, (char)47, (char)58, (char)91, (char)93, (char)123,
                (char)9, (char)10, (char)9, (char)10, (char)13, (char)32, (char)44, (char)47, (char)125, (char)9,
                (char)10, (char)13, (char)32, (char)44, (char)47, (char)125, (char)13, (char)32, (char)34, (char)44,
                (char)47, (char)58, (char)125, (char)9, (char)10, (char)34, (char)13, (char)32, (char)34, (char)44,
                (char)47, (char)58, (char)125, (char)9, (char)10, (char)42, (char)47, (char)42, (char)47, (char)13,
                (char)32, (char)34, (char)44, (char)47, (char)58, (char)125, (char)9, (char)10, (char)42, (char)47,
                (char)42, (char)47, (char)34, (char)42, (char)47, (char)42, (char)47, (char)13, (char)32, (char)34,
                (char)44, (char)47, (char)58, (char)91, (char)93, (char)123, (char)9, (char)10, (char)9, (char)10,
                (char)13, (char)32, (char)44, (char)47, (char)93, (char)9, (char)10, (char)13, (char)32, (char)44,
                (char)47,
                (char)93, (char)13, (char)32, (char)34, (char)44, (char)47, (char)58, (char)91, (char)93, (char)123,
                (char)9, (char)10, (char)34, (char)13, (char)32, (char)34, (char)44, (char)47, (char)58,
                (char)91, (char)93, (char)123, (char)9, (char)10, (char)42, (char)47, (char)42, (char)47, (char)13,
                (char)32, (char)34, (char)44, (char)47, (char)58, (char)91, (char)93, (char)123, (char)9, (char)10,
                (char)42, (char)47, (char)42, (char)47, (char)42, (char)47, (char)13, (char)32, (char)47, (char)9,
                (char)10, (char)13, (char)32, (char)47, (char)9, (char)10, (char)0
            };
        }

        private static readonly char[] _json_trans_keys = init__json_trans_keys_0();

        private static byte[] init__json_single_lengths_0()
        {
            return new byte[]
            {
                0, 9, 2, 1, 2, 7, 4, 4, 2, 9, 7, 7, 7, 1, 7, 2, 2, 7, 2, 2, 1, 2, 2, 9, 7, 7, 9, 1, 9, 2, 2, 9, 2, 2, 2,
                3, 3, 0, 0
            };
        }

        private static readonly byte[] _json_single_lengths = init__json_single_lengths_0();

        private static byte[] init__json_range_lengths_0()
        {
            return new byte[]
            {
                0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0,
                1, 1, 0, 0
            };
        }

        private static readonly byte[] _json_range_lengths = init__json_range_lengths_0();

        private static short[] init__json_index_offsets_0()
        {
            return new short[]
            {
                0, 0, 11, 14, 16, 19, 28, 34, 40, 43, 54, 62, 70, 79, 81, 90, 93, 96, 105, 108, 111, 113, 116, 119, 130,
                138, 146, 157, 159, 170, 173, 176, 187, 190, 193, 196, 201, 206, 207
            };
        }

        private static readonly short[] _json_index_offsets = init__json_index_offsets_0();

        private static byte[] init__json_indicies_0()
        {
            return new byte[]
            {
                1, 1, 2, 3, 4, 3, 5, 3, 6, 1, 0, 7, 7, 3, 8, 3, 9, 9, 3, 11, 11, 12, 13, 14, 3, 15, 11, 10, 16, 16, 17,
                18, 16, 3, 19, 19, 20, 21, 19, 3, 22, 22, 3, 21, 21, 24, 3, 25, 3, 26, 3, 27, 21, 23, 28, 29, 29, 28,
                30, 31, 32, 3, 33,
                34, 34, 33, 13, 35, 15, 3, 34, 34, 12, 36, 37, 3, 15, 34, 10, 16, 3, 36, 36, 12, 3, 38, 3, 3, 36, 10,
                39, 39, 3, 40, 40,
                3, 13, 13, 12, 3, 41, 3, 15, 13, 10, 42, 42, 3, 43, 43, 3, 28, 3, 44, 44, 3, 45, 45, 3, 47, 47, 48, 49,
                50, 3, 51, 52,
                53, 47, 46, 54, 55, 55, 54, 56, 57, 58, 3, 59, 60, 60, 59, 49, 61, 52, 3, 60, 60, 48, 62, 63, 3, 51, 52,
                53, 60, 46, 54,
                3, 62, 62, 48, 3, 64, 3, 51, 3, 53, 62, 46, 65, 65, 3, 66, 66, 3, 49, 49, 48, 3, 67, 3, 51, 52, 53, 49,
                46, 68, 68, 3,
                69, 69, 3, 70, 70, 3, 8, 8, 71, 8, 3, 72, 72, 73, 72, 3, 3, 3, 0
            };
        }

        private static readonly byte[] _json_indicies = init__json_indicies_0();

        private static byte[] init__json_trans_targs_0()
        {
            return new byte[]
            {
                35, 1, 3, 0, 4, 36, 36, 36, 36, 1, 6, 5, 13, 17, 22, 37, 7, 8, 9, 7, 8, 9, 7, 10, 20, 21, 11, 11, 11,
                12,
                17, 19, 37, 11, 12, 19, 14, 16, 15, 14, 12, 18, 17, 11, 9, 5, 24, 23, 27, 31, 34, 25, 38, 25, 25, 26,
                31, 33, 38, 25, 26,
                33, 28, 30, 29, 28, 26, 32, 31, 25, 23, 2, 36, 2
            };
        }

        private static readonly byte[] _json_trans_targs = init__json_trans_targs_0();

        private static byte[] init__json_trans_actions_0()
        {
            return new byte[]
            {
                13, 0, 15, 0, 0, 7, 3, 11, 1, 11, 17, 0, 20, 0, 0, 5, 1, 1, 1, 0, 0, 0, 11, 13, 15, 0, 7, 3, 1, 1, 1, 1,
                23, 0, 0, 0, 0, 0, 0, 11, 11, 0, 11, 11, 11, 11, 13, 0, 15, 0, 0, 7, 9, 3, 1, 1, 1, 1, 26, 0, 0, 0, 0,
                0, 0, 11, 11, 0,
                11, 11, 11, 1, 0, 0
            };
        }

        private static readonly byte[] _json_trans_actions = init__json_trans_actions_0();

        private static byte[] init__json_eof_actions_0()
        {
            return new byte[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1, 0, 0, 0
            };
        }

        private static readonly byte[] _json_eof_actions = init__json_eof_actions_0();

        static readonly int json_start = 1;
        static readonly int json_first_final = 35;
        static readonly int json_error = 0;

        static readonly int json_en_object = 5;
        static readonly int json_en_array = 23;
        static readonly int json_en_main = 1;

        // line 349 "JsonReader.rl"

        private readonly Array<JsonValue> elements = new(8);
        private readonly Array<JsonValue> lastChild = new(8);
        private JsonValue root, current;
        private bool _stop;

        /** Causes parsing to stop after the current or next object, array, or value. */
        public void stop()
        {
            _stop = true;
        }

        public bool isStopped()
        {
            return _stop;
        }

        private void addChild(String? name, JsonValue child)
        {
            child.setName(name);
            if (current == null)
            {
                current = child;
                root = child;
            }
            else if (current.isArray() || current.isObject())
            {
                child._parent = current;
                if (current.Size == 0)
                    current._child = child;
                else
                {
                    JsonValue last = lastChild.pop();
                    last._next = child;
                    child._prev = last;
                }

                lastChild.Add(child);
                current.Size++;
            }
            else
                root = current;
        }

        /** Called when an object is encountered in the JSON. */
        protected void startObject(String? name)
        {
            JsonValue value = new JsonValue(JsonValue.ValueType.@object);
            if (current != null) addChild(name, value);
            elements.Add(value);
            current = value;
        }

        /** Called when an array is encountered in the JSON. */
        protected void startArray(String? name)
        {
            JsonValue value = new JsonValue(JsonValue.ValueType.array);
            if (current != null) addChild(name, value);
            elements.Add(value);
            current = value;
        }

        /** Called when the end of an object or array is encountered in the JSON. */
        protected void pop()
        {
            root = elements.pop();
            if (current.Size > 0) lastChild.pop();
            current = elements.size > 0 ? elements.peek() : null;
        }

        /** Called when a string value is encountered in the JSON. */
        protected void @string(String? name, String value)
        {
            addChild(name, new JsonValue(value));
        }

        /** Called when a double value is encountered in the JSON. */
        protected void number(String? name, double value, String stringValue)
        {
            addChild(name, new JsonValue(value, stringValue));
        }

        /** Called when a long value is encountered in the JSON. */
        protected void number(String? name, long value, String stringValue)
        {
            addChild(name, new JsonValue(value, stringValue));
        }

        /** Called when a boolean value is encountered in the JSON. */
        protected void @bool(String? name, bool value)
        {
            addChild(name, new JsonValue(value));
        }

        /** Called to unescape string values. The default implementation does standard JSON unescaping. */
        protected String unescape(String value)
        {
            int length = value.Length;
            StringBuilder buffer = new StringBuilder(length + 16);
            for (int i = 0; i < length;)
            {
                char c = value[i++];
                if (c != '\\')
                {
                    buffer.Append(c);
                    continue;
                }

                if (i == length) break;
                c = value[i++];
                if (c == 'u')
                {
                    // TODO: Ensure that this is correct. -RP
                    buffer.Append(char.ConvertFromUtf32(Convert.ToInt32(value.Substring(i, i + 4), 16)));
                    i += 4;
                    continue;
                }

                switch (c)
                {
                    case '"':
                    case '\\':
                    case '/':
                        break;
                    case 'b':
                        c = '\b';
                        break;
                    case 'f':
                        c = '\f';
                        break;
                    case 'n':
                        c = '\n';
                        break;
                    case 'r':
                        c = '\r';
                        break;
                    case 't':
                        c = '\t';
                        break;
                    default:
                        throw new SerializationException("Illegal escaped character: \\" + c);
                }

                buffer.Append(c);
            }

            return buffer.ToString();
        }
    }
}