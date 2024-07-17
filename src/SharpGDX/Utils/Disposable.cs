using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** Interface for disposable resources.
 * @author mzechner */
	public interface Disposable
	{
		/** Releases all resources of this object. */
		public void dispose();
	}
}
