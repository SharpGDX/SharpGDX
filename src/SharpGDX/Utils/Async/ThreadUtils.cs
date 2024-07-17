using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Async
{
	/** Utilities for threaded programming.
 * @author badlogic */
	public class ThreadUtils
	{
		public static void yield()
		{
			Thread.Yield();
		}
	}
}
