using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Utils
{
	public class Collections
	{

		/** When true, {@link Iterable#iterator()} for {@link Array}, {@link ObjectMap}, and other collections will allocate a new
		 * iterator for each invocation. When false, the iterator is reused and nested use will throw an exception. Default is
		 * false. */
		public static bool allocateIterators;

	}
}
