using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** Objects implementing this interface will have {@link #reset()} called when passed to {@link Pool#free(Object)}. */
	public interface IPoolable
	{
		/** Resets the object for reuse. Object references should be nulled and fields may be set to default values. */
		public void reset();
	}
}
