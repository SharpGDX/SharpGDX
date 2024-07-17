using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Async
{
	/** Task to be submitted to an {@link AsyncExecutor}, returning a result of type T.
 * @author badlogic */
	public interface IAsyncTask<T>
	{
		public T? call();// TODO: throws Exception;
	}
}
