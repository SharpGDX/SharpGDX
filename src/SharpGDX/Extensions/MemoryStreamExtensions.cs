using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Extensions
{
	internal static class MemoryStreamExtensions
	{
		public static void Write(this MemoryStream stream, float[] values)
		{
			stream.Write(MemoryMarshal.Cast<float, byte>(values));
		}
	}
}
