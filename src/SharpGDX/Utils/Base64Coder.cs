using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Utils
{
	// TODO: Should this be byte or sbyte?
	public class Base64Coder
	{
		public class CharMap
		{
			internal readonly char[] encodingMap = new char[64];
			internal readonly sbyte[] decodingMap = new sbyte[128];

			public CharMap(char char63, char char64)
			{
				int i = 0;
				for (char c = 'A'; c <= 'Z'; c++)
				{
					encodingMap[i++] = c;
				}

				for (char c = 'a'; c <= 'z'; c++)
				{
					encodingMap[i++] = c;
				}

				for (char c = '0'; c <= '9'; c++)
				{
					encodingMap[i++] = c;
				}

				encodingMap[i++] = char63;
				encodingMap[i++] = char64;
				for (i = 0; i < decodingMap.Length; i++)
				{
					decodingMap[i] = -1;
				}

				for (i = 0; i < 64; i++)
				{
					decodingMap[encodingMap[i]] = (sbyte)i;
				}
			}

			public sbyte[] getDecodingMap()
			{
				return decodingMap;
			}

			public char[] getEncodingMap()
			{
				return encodingMap;
			}
		}

		// The line separator string of the operating system.
		private static readonly String systemLineSeparator = "\n";

		public static readonly CharMap regularMap = new CharMap('+', '/'), urlsafeMap = new CharMap('-', '_');

		/** Encodes a string into Base64 format. No blanks or line breaks are inserted.
		 * @param s A String to be encoded.
		 * @return A String containing the Base64 encoded data. */
		public static String encodeString(String s)
		{
			return encodeString(s, false);
		}

		/** Encodes a string into Base64 format, optionally using URL-safe encoding instead of the "regular" Base64 encoding. No blanks
		 * or line breaks are inserted.
		 * @param s A String to be encoded.
		 * @param useUrlsafeEncoding If true, this encodes the result with an alternate URL-safe set of characters.
		 * @return A String containing the Base64 encoded data. */
		public static String encodeString(String s, bool useUrlsafeEncoding)
		{
			try
			{
				throw new NotImplementedException();
				// TODO: return new String(encode(s.getBytes("UTF-8"), useUrlsafeEncoding ? urlsafeMap.encodingMap : regularMap.encodingMap));
			}
			catch (UnsupportedEncodingException e)
			{
				// shouldn't ever happen; only needed because we specify an encoding with a String
				return "";
			}
		}

		/** Encodes a byte array into Base64 format and breaks the output into lines of 76 characters. This method is compatible with
		 * <code>sun.misc.BASE64Encoder.encodeBuffer(byte[])</code>.
		 * @param in An array containing the data bytes to be encoded.
		 * @return A String containing the Base64 encoded data, broken into lines. */
		public static String encodeLines(sbyte[] @in)
		{
			return encodeLines(@in, 0, @in.Length, 76, systemLineSeparator, regularMap.encodingMap);
		}

		public static String encodeLines(sbyte[] @in, int iOff, int iLen, int lineLen, String lineSeparator,
			CharMap charMap)
		{
			return encodeLines(@in, iOff, iLen, lineLen, lineSeparator, charMap.encodingMap);
		}

		/** Encodes a byte array into Base64 format and breaks the output into lines.
		 * @param in An array containing the data bytes to be encoded.
		 * @param iOff Offset of the first byte in <code>in</code> to be processed.
		 * @param iLen Number of bytes to be processed in <code>in</code>, starting at <code>iOff</code>.
		 * @param lineLen Line length for the output data. Should be a multiple of 4.
		 * @param lineSeparator The line separator to be used to separate the output lines.
		 * @param charMap char map to use
		 * @return A String containing the Base64 encoded data, broken into lines. */
		public static String encodeLines(sbyte[] @in, int iOff, int iLen, int lineLen, String lineSeparator,
			char[] charMap)
		{
			int blockLen = (lineLen * 3) / 4;
			if (blockLen <= 0)
			{
				throw new IllegalArgumentException();
			}

			int lines = (iLen + blockLen - 1) / blockLen;
			int bufLen = ((iLen + 2) / 3) * 4 + lines * lineSeparator.Length;
			StringBuilder buf = new StringBuilder(bufLen);
			int ip = 0;
			while (ip < iLen)
			{
				int l = Math.Min(iLen - ip, blockLen);
				buf.Append(encode(@in, iOff + ip, l, charMap));
				buf.Append(lineSeparator);
				ip += l;
			}

			return buf.ToString();
		}

		/** Encodes a byte array into Base64 format. No blanks or line breaks are inserted in the output.
		 * @param in An array containing the data bytes to be encoded.
		 * @return A character array containing the Base64 encoded data. */
		public static char[] encode(sbyte[] @in)
		{
			return encode(@in, regularMap.encodingMap);
		}

		public static char[] encode(sbyte[] @in, CharMap charMap)
		{
			return encode(@in, 0, @in.Length, charMap);
		}

		public static char[] encode(sbyte[] @in, char[] charMap)
		{
			return encode(@in, 0, @in.Length, charMap);
		}

		/** Encodes a byte array into Base64 format. No blanks or line breaks are inserted in the output.
		 * @param in An array containing the data bytes to be encoded.
		 * @param iLen Number of bytes to process in <code>in</code>.
		 * @return A character array containing the Base64 encoded data. */
		public static char[] encode(sbyte[] @in, int iLen)
		{
			return encode(@in, 0, iLen, regularMap.encodingMap);
		}

		public static char[] encode(sbyte[] @in, int iOff, int iLen, CharMap charMap)
		{
			return encode(@in, iOff, iLen, charMap.encodingMap);
		}

		/** Encodes a byte array into Base64 format. No blanks or line breaks are inserted in the output.
		 * @param in An array containing the data bytes to be encoded.
		 * @param iOff Offset of the first byte in <code>in</code> to be processed.
		 * @param iLen Number of bytes to process in <code>in</code>, starting at <code>iOff</code>.
		 * @param charMap char map to use
		 * @return A character array containing the Base64 encoded data. */
		public static char[] encode(sbyte[] @in, int iOff, int iLen, char[] charMap)
		{
			int oDataLen = (iLen * 4 + 2) / 3; // output length without padding
			int oLen = ((iLen + 2) / 3) * 4; // output length including padding
			char[] @out = new char[oLen];
			int ip = iOff;
			int iEnd = iOff + iLen;
			int op = 0;
			while (ip < iEnd)
			{
				int i0 = @in[ip++] & 0xff;
				int i1 = ip < iEnd ? @in[ip++] & 0xff : 0;
				int i2 = ip < iEnd ? @in[ip++] & 0xff : 0;
				int o0 = i0 >>> 2;
				int o1 = ((i0 & 3) << 4) | (i1 >>> 4);
				int o2 = ((i1 & 0xf) << 2) | (i2 >>> 6);
				int o3 = i2 & 0x3F;
				@out[op++] = charMap[o0];
				@out[op++] = charMap[o1];
				@out[op] = op < oDataLen ? charMap[o2] : '=';
				op++;
				@out[op] = op < oDataLen ? charMap[o3] : '=';
				op++;
			}

			return @out;
		}

		/** Decodes a string from Base64 format. No blanks or line breaks are allowed within the Base64 encoded input data.
		 * @param s A Base64 String to be decoded.
		 * @return A String containing the decoded data.
		 * @throws IllegalArgumentException If the input is not valid Base64 encoded data. */
		public static String decodeString(String s)
		{
			return decodeString(s, false);
		}

		public static String decodeString(String s, bool useUrlSafeEncoding)
		{
			throw new NotImplementedException();
			// TODO: return new String((decode(s.ToCharArray(), useUrlSafeEncoding ? urlsafeMap.decodingMap : regularMap.decodingMap));
		}

		public static sbyte[] decodeLines(String s)
		{
			return decodeLines(s, regularMap.decodingMap);
		}

		public static sbyte[] decodeLines(String s, CharMap inverseCharMap)
		{
			return decodeLines(s, inverseCharMap.decodingMap);
		}

		/** Decodes a byte array from Base64 format and ignores line separators, tabs and blanks. CR, LF, Tab and Space characters are
		 * ignored in the input data. This method is compatible with <code>sun.misc.BASE64Decoder.decodeBuffer(String)</code>.
		 * @param s A Base64 String to be decoded.
		 * @param inverseCharMap
		 * @return An array containing the decoded data bytes.
		 * @throws IllegalArgumentException If the input is not valid Base64 encoded data. */
		public static sbyte[] decodeLines(String s, sbyte[] inverseCharMap)
		{
			char[] buf = new char[s.Length];
			int p = 0;
			for (int ip = 0; ip < s.Length; ip++)
			{
				char c = s[ip];
				if (c != ' ' && c != '\r' && c != '\n' && c != '\t')
				{
					buf[p++] = c;
				}
			}

			return decode(buf, 0, p, inverseCharMap);
		}

		/** Decodes a byte array from Base64 format. No blanks or line breaks are allowed within the Base64 encoded input data.
		 * @param s A Base64 String to be decoded.
		 * @return An array containing the decoded data bytes.
		 * @throws IllegalArgumentException If the input is not valid Base64 encoded data. */
		public static sbyte[] decode(String s)
		{
			return decode(s.ToCharArray());
		}

		/** Decodes a byte array from Base64 format. No blanks or line breaks are allowed within the Base64 encoded input data.
		 * @param s A Base64 String to be decoded.
		 * @param inverseCharMap
		 * @return An array containing the decoded data bytes.
		 * @throws IllegalArgumentException If the input is not valid Base64 encoded data. */
		public static sbyte[] decode(String s, CharMap inverseCharMap)
		{
			return decode(s.ToCharArray(), inverseCharMap);
		}

		public static sbyte[] decode(char[] @in, sbyte[] inverseCharMap)
		{
			return decode(@in, 0, @in.Length, inverseCharMap);
		}

		public static sbyte[] decode(char[] @in, CharMap inverseCharMap)
		{
			return decode(@in, 0, @in.Length, inverseCharMap);
		}

		/** Decodes a byte array from Base64 format. No blanks or line breaks are allowed within the Base64 encoded input data.
		 * @param in A character array containing the Base64 encoded data.
		 * @return An array containing the decoded data bytes.
		 * @throws IllegalArgumentException If the input is not valid Base64 encoded data. */
		public static sbyte[] decode(char[] @in)
		{
			return decode(@in, 0, @in.Length, regularMap.decodingMap);
		}

		public static sbyte[] decode(char[] @in, int iOff, int iLen, CharMap inverseCharMap)
		{
			return decode(@in, iOff, iLen, inverseCharMap.decodingMap);
		}

		/** Decodes a byte array from Base64 format. No blanks or line breaks are allowed within the Base64 encoded input data.
		 * @param in A character array containing the Base64 encoded data.
		 * @param iOff Offset of the first character in <code>in</code> to be processed.
		 * @param iLen Number of characters to process in <code>in</code>, starting at <code>iOff</code>.
		 * @param inverseCharMap charMap to use
		 * @return An array containing the decoded data bytes.
		 * @throws IllegalArgumentException If the input is not valid Base64 encoded data. */
		public static sbyte[] decode(char[] @in, int iOff, int iLen, sbyte[] inverseCharMap)
		{
			if (iLen % 4 != 0)
			{
				throw new IllegalArgumentException("Length of Base64 encoded input string is not a multiple of 4.");
			}

			while (iLen > 0 && @in[iOff + iLen - 1] == '=')
			{
				iLen--;
			}

			int oLen = (iLen * 3) / 4;
			sbyte[] @out = new sbyte[oLen];
			int ip = iOff;
			int iEnd = iOff + iLen;
			int op = 0;
			while (ip < iEnd)
			{
				int i0 = @in[ip++];
				int i1 = @in[ip++];
				int i2 = ip < iEnd ? @in[ip++] : 'A';
				int i3 = ip < iEnd ? @in[ip++] : 'A';
				if (i0 > 127 || i1 > 127 || i2 > 127 || i3 > 127)
				{
					throw new IllegalArgumentException("Illegal character in Base64 encoded data.");
				}

				int b0 = inverseCharMap[i0];
				int b1 = inverseCharMap[i1];
				int b2 = inverseCharMap[i2];
				int b3 = inverseCharMap[i3];
				if (b0 < 0 || b1 < 0 || b2 < 0 || b3 < 0)
				{
					throw new IllegalArgumentException("Illegal character in Base64 encoded data.");
				}

				int o0 = (b0 << 2) | (b1 >>> 4);
				int o1 = ((b1 & 0xf) << 4) | (b2 >>> 2);
				int o2 = ((b2 & 3) << 6) | b3;
				@out[op++] = (sbyte)o0;
				if (op < oLen)
				{
					@out[op++] = (sbyte)o1;
				}

				if (op < oLen)
				{
					@out[op++] = (sbyte)o2;
				}
			}

			return @out;
		}

		// Dummy constructor.
		private Base64Coder()
		{
		}
	}
}