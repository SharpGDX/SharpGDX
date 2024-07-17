using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** This class is for selecting a ranked element (kth ordered statistic) from an unordered list in faster time than sorting the
 * whole array. Typical applications include finding the nearest enemy unit(s), and other operations which are likely to run as
 * often as every x frames. Certain values of k will result in a partial sorting of the Array.
 * <p>
 * The lowest ranking element starts at 1, not 0. 1 = first, 2 = second, 3 = third, etc. calling with a value of zero will result
 * in a {@link GdxRuntimeException}
 * </p>
 * <p>
 * This class uses very minimal extra memory, as it makes no copies of the array. The underlying algorithms used are a naive
 * single-pass for k=min and k=max, and Hoare's quickselect for values in between.
 * </p>
 * @author Jon Renner */
	public class Select<T>
	{
		private static Select<T> _instance;
		private QuickSelect<T> quickSelect;

		/** Provided for convenience */
		public static Select<T> instance()
		{
			if (_instance == null) _instance = new Select<T>();
			return _instance;
		}

		public T select(T[] items, IComparer<T> comp, int kthLowest, int size)
		{
			int idx = selectIndex(items, comp, kthLowest, size);
			return items[idx];
		}

		public int selectIndex(T[] items, IComparer<T> comp, int kthLowest, int size)
		{
			if (size < 1)
			{
				throw new GdxRuntimeException("cannot select from empty array (size < 1)");
			}
			else if (kthLowest > size)
			{
				throw new GdxRuntimeException("Kth rank is larger than size. k: " + kthLowest + ", size: " + size);
			}
			int idx;
			// naive partial selection sort almost certain to outperform quickselect where n is min or max
			if (kthLowest == 1)
			{
				// find min
				idx = fastMin(items, comp, size);
			}
			else if (kthLowest == size)
			{
				// find max
				idx = fastMax(items, comp, size);
			}
			else
			{
				// quickselect a better choice for cases of k between min and max
				if (quickSelect == null) quickSelect = new QuickSelect<T>();
				idx = quickSelect.select(items, comp, kthLowest, size);
			}
			return idx;
		}

		/** Faster than quickselect for n = min */
		private int fastMin(T[] items, IComparer<T> comp, int size)
		{
			int lowestIdx = 0;
			for (int i = 1; i < size; i++)
			{
				int comparison = comp.Compare(items[i], items[lowestIdx]);
				if (comparison < 0)
				{
					lowestIdx = i;
				}
			}
			return lowestIdx;
		}

		/** Faster than quickselect for n = max */
		private int fastMax(T[] items, IComparer<T> comp, int size)
		{
			int highestIdx = 0;
			for (int i = 1; i < size; i++)
			{
				int comparison = comp.Compare(items[i], items[highestIdx]);
				if (comparison > 0)
				{
					highestIdx = i;
				}
			}
			return highestIdx;
		}
	}
}
