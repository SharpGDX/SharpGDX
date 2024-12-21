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

        public static T[] copyOfRange<T>(T[] source, int pos, int count)
        {
            T[] result = new T[count - pos];
			Array.Copy(source, pos, result, 0, count - pos);
            return result;
        }

        public static T[] copyOf<T>(T[] source, int capacity)
        {
            var result = new T[capacity];
            Array.Copy(source, 0, source, 0, Math.Min(source.Length, capacity));
            return result;
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
