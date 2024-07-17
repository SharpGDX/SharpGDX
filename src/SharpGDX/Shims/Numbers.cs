using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitConverter = System.BitConverter;

namespace SharpGDX.Shims
{
	public static class Numbers
	{

		public static int floatToIntBits(float f)
		{
			// TODO: Verify
			//wfa.set(0, f);
			//return wia.get(0);
			return BitConverter.SingleToInt32Bits(f);
		}

		//private static readonly Int8Array wba = Int8ArrayNative.create(8);
		//private static readonly Int32Array wia = Int32ArrayNative.create(wba.buffer(), 0, 2);
		//private static readonly Float32Array wfa = Float32ArrayNative.create(wba.buffer(), 0, 1);
		//private static readonly Float64Array wda = Float64ArrayNative.create(wba.buffer(), 0, 1);

		public static float intBitsToFloat(int i)
		{
			// TODO: Verify
			//wia.set(0, i);
			//return wfa.get(0);
			return BitConverter.Int32BitsToSingle(i);
		}

		public static long doubleToLongBits(double d)
		{
			// TODO: Verify
			//wda.set(0, d);
			//return ((long)wia.get(1) << 32) | (wia.get(0) & 0xffffffffL);
			return BitConverter.DoubleToInt64Bits(d);
		}

		public static double longBitsToDouble(long l)
		{
			// TODO: Verify
			//wia.set(1, (int)(l >>> 32));
			//wia.set(0, (int)(l & 0xffffffffL));
			//return wda.get(0);
			return BitConverter.Int64BitsToDouble(l);
		}

		public static long doubleToRawLongBits(double d)
		{
			// TODO: Verify
			//wda.set(0, d);
			//return ((long)wia.get(1) << 32) | (wia.get(0) & 0xffffffffL);
			return BitConverter.ToInt32(BitConverter.GetBytes(d));
		}
	}
}
