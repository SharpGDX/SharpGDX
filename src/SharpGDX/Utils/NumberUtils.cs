namespace SharpGDX.Utils
{
	public static class NumberUtils
	{
		public static int floatToIntBits(float value)
		{
			return BitConverter.SingleToInt32Bits(value);
		}

		public static int floatToRawIntBits(float value)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}

		/** Converts the color from a float ABGR encoding to an int ABGR encoding. The alpha is expanded from 0-254 in the float
		 * encoding (see {@link #intToFloatColor(int)}) to 0-255, which means converting from int to float and back to int can be
		 * lossy. */
		public static int floatToIntColor(float value)
		{
			int intBits = floatToRawIntBits(value);
			intBits |= (int)((intBits >>> 24) * (255f / 254f)) << 24;
			return intBits;
		}

		/** Encodes the ABGR int color as a float. The alpha is compressed to use only even numbers between 0-254 to avoid using bits
		 * in the NaN range (see {@link Float#intBitsToFloat(int)} javadocs). Rendering which uses colors encoded as floats should
		 * expand the 0-254 back to 0-255, else colors cannot be fully opaque. */
		public static float intToFloatColor(int value)
		{
			return BitConverter.Int32BitsToSingle(value & unchecked((int)0xfeffffff));
		}

		public static float intBitsToFloat(int value)
		{
			return BitConverter.Int32BitsToSingle(value);
		}

		public static long doubleToLongBits(double value)
		{
			return BitConverter.DoubleToInt64Bits(value);
		}

		public static double longBitsToDouble(long value)
		{
			return BitConverter.Int64BitsToDouble(value);
		}
	}
}
