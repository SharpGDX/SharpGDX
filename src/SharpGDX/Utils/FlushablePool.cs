using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** A {@link Pool} which keeps track of the obtained items (see {@link #obtain()}), which can be free'd all at once using the
 * {@link #flush()} method.
 * @author Xoppa */
public abstract class FlushablePool<T> : Pool<T> {
	protected Array<T> obtained = new Array<T>();

	public FlushablePool () {
	}

	public FlushablePool (int initialCapacity) 
	: base(initialCapacity)
	{
		
	}

	public FlushablePool (int initialCapacity, int max) 
	: base(initialCapacity, max)
	{
		
	}

		public override T obtain () {
		T result = base.obtain();
		obtained.add(result);
		return result;
	}

	/** Frees all obtained instances. */
	public void flush () {
		base.freeAll(obtained);
		obtained.clear();
	}

		public override void free (T obj) {
		obtained.removeValue(obj, true);
		base.free(obj);
	}

		public override void freeAll (Array<T> objects) {
		obtained.removeAll(objects, true);
		base.freeAll(objects);
	}
}
}
