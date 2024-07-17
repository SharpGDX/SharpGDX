using SharpGDX.Shims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	public abstract class Pool
	{

	}
	
	/** A pool of objects that can be reused to avoid allocation.
 * @see Pools
 * @author Nathan Sweet */
	abstract public class Pool<T>:Pool
	{
		/** The maximum number of objects that will be pooled. */
		public readonly int max;
		/** The highest number of free objects. Can be reset any time. */
		public int peak;

		private readonly Array<T> freeObjects;

		/** Creates a pool with an initial capacity of 16 and no maximum. */
		public Pool()
		: this(16, int.MaxValue)
		{
			
		}

		/** Creates a pool with the specified initial capacity and no maximum. */
		public Pool(int initialCapacity)
		: this(initialCapacity, int.MaxValue)
		{
			
		}

		/** @param initialCapacity The initial size of the array supporting the pool. No objects are created/pre-allocated. Use
		 *           {@link #fill(int)} after instantiation if needed.
		 * @param max The maximum number of free objects to store in this pool. */
		public Pool(int initialCapacity, int max)
		{
			freeObjects = new (false, initialCapacity);
			this.max = max;
		}

		abstract protected T newObject();

		/** Returns an object from this pool. The object may be new (from {@link #newObject()}) or reused (previously
		 * {@link #free(Object) freed}). */
		public virtual T obtain()
		{
			return freeObjects.size == 0 ? newObject() : freeObjects.pop();
		}

		/** Puts the specified object in the pool, making it eligible to be returned by {@link #obtain()}. If the pool already contains
		 * {@link #max} free objects, the specified object is {@link #discard(Object) discarded}, it is not reset and not added to the
		 * pool.
		 * <p>
		 * The pool does not check if an object is already freed, so the same object must not be freed multiple times. */
		public virtual void free(T obj)
		{
			if (obj == null) throw new IllegalArgumentException("object cannot be null.");
			if (freeObjects.size < max)
			{
				freeObjects.add(obj);
				peak = Math.Max(peak, freeObjects.size);
				reset(obj);
			}
			else
				discard(obj);
		}

		/** Adds the specified number of new free objects to the pool. Usually called early on as a pre-allocation mechanism but can be
		 * used at any time.
		 *
		 * @param size the number of objects to be added */
		public void fill(int size)
		{
			for (int i = 0; i < size; i++)
				if (freeObjects.size < max) freeObjects.add(newObject());
			peak = Math.Max(peak, freeObjects.size);
		}

		/** Called when an object is freed to clear the state of the object for possible later reuse. The default implementation calls
		 * {@link Poolable#reset()} if the object is {@link Poolable}. */
		protected void reset(T obj)
		{
			if (obj is IPoolable) ((IPoolable)obj).reset();
		}

		/** Called when an object is discarded. This is the case when an object is freed, but the maximum capacity of the pool is
		 * reached, and when the pool is {@link #clear() cleared} */
		protected void discard(T obj)
		{
			reset(obj);
		}

		/** Puts the specified objects in the pool. Null objects within the array are silently ignored.
		 * <p>
		 * The pool does not check if an object is already freed, so the same object must not be freed multiple times.
		 * @see #free(Object) */
		public virtual void freeAll(Array<T> objects)
		{
			if (objects == null) throw new IllegalArgumentException("objects cannot be null.");
			Array<T> freeObjects = this.freeObjects;
			int max = this.max;
			for (int i = 0, n = objects.size; i < n; i++)
			{
				T obj = objects.get(i);
				if (obj == null) continue;
				if (freeObjects.size < max)
				{
					freeObjects.add(obj);
					reset(obj);
				}
				else
				{
					discard(obj);
				}
			}
			peak = Math.Max(peak, freeObjects.size);
		}

		/** Removes and discards all free objects from this pool. */
		public void clear()
		{
			Array<T> freeObjects = this.freeObjects;
			for (int i = 0, n = freeObjects.size; i < n; i++)
				discard(freeObjects.get(i));
			freeObjects.clear();
		}

		/** The number of objects available to be obtained. */
		public int getFree()
		{
			return freeObjects.size;
		}
	}
}
