using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public static class Arrays
	{
		public static string toString(float[]? a)
		{
			if (a == null)
			{
				return "null";
			}
			return $"[{string.Join(", ", a)}]";
		}

		public static void sort<T>(T[] array)
		{
			Array.Sort(array);
		}

		public static List<T> asList<T>(T[] array)
		{
			return array.ToList();
		}

		public static bool equals<T>(T[]? a, T[]? b)
		{
			if (a == null)
			{
				return b == null;
			}

			return a.SequenceEqual(b!);
		}
	}
}
