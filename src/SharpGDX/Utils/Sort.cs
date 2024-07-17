using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** Provides methods to sort arrays of objects. Sorting requires working memory and this class allows that memory to be reused to
 * avoid allocation. The sorting is otherwise identical to the Arrays.sort methods (uses timsort).<br>
 * <br>
 * Note that sorting primitive arrays with the Arrays.sort methods does not allocate memory (unless sorting large arrays of char,
 * short, or byte).
 * @author Nathan Sweet */
	public class Sort<T>
	{
		static private Sort<T> _instance;

		private TimSort<T> timSort;
		private ComparableTimSort<T> comparableTimSort;

		public  void sort(Array<T> a)
		
		{
			if (comparableTimSort == null) comparableTimSort = new ComparableTimSort<T>();
			comparableTimSort.doSort(a.items, 0, a.size);
		}

		/** The specified objects must implement {@link Comparable}. */
		public void sort(T[] a)
		{
			if (comparableTimSort == null) comparableTimSort = new ComparableTimSort<T>();
			comparableTimSort.doSort(a, 0, a.Length);
		}

		/** The specified objects must implement {@link Comparable}. */
		public void sort(T[] a, int fromIndex, int toIndex)
		{
			if (comparableTimSort == null) comparableTimSort = new ComparableTimSort<T>();
			comparableTimSort.doSort(a, fromIndex, toIndex);
		}

		public  void sort(Array<T> a, IComparer< T> c)
		{
			if (timSort == null) timSort = new TimSort<T>();
			timSort.doSort(a.items, c, 0, a.size);
		}

		public  void sort(T[] a, IComparer< T> c)
		{
			if (timSort == null) timSort = new TimSort<T>();
			timSort.doSort(a, c, 0, a.Length);
		}

		public  void sort(T[] a, IComparer< T> c, int fromIndex, int toIndex)
		{
			if (timSort == null) timSort = new TimSort<T>();
			timSort.doSort(a, c, fromIndex, toIndex);
		}

		/** Returns a Sort instance for convenience. Multiple threads must not use this instance at the same time. */
		static public Sort<T> instance()
		{
			if (_instance == null) _instance = new Sort<T>();
			return _instance;
		}
	}
}
