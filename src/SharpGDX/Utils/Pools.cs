using System;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** Stores a map of {@link Pool}s (usually {@link ReflectionPool}s) by type for convenient static access.
 * @author Nathan Sweet */
public class Pools {
	static private readonly ObjectMap<Type, Pool> typePools = new ();

	/** Returns a new or existing pool for the specified type, stored in a Class to {@link Pool} map. Note the max size is ignored
	 * if this is not the first time this pool has been requested. */
	static public  Pool<T> get<T>(Type type, int max) {
		Pool<T> pool = typePools.get(type) as Pool<T>;
		if (pool == null) {
			pool = new ReflectionPool<T>(type, 4, max);
			typePools.put(type, pool);
		}
		return pool;
	}

	/** Returns a new or existing pool for the specified type, stored in a Class to {@link Pool} map. The max size of the pool used
	 * is 100. */
	static public Pool<T> get <T>(Type type) {
		return get<T>(type, 100);
	}

	/** Sets an existing pool for the specified type, stored in a Class to {@link Pool} map. */
	static public void set<T> (Type type, Pool<T> pool) {
		typePools.put(type, pool);
	}

	/** Obtains an object from the {@link #get(Class) pool}. */
	static public  T obtain<T>(Type type) {
		return get<T>(type).obtain();
	}

	/** Frees an object from the {@link #get(Class) pool}. */
	static public void free <T>(T obj) {
		if (obj == null) throw new IllegalArgumentException("object cannot be null.");
		Pool<T> pool = typePools.get(obj.GetType()) as Pool<T>;
		if (pool == null) return; // Ignore freeing an object that was never retained.
		pool.free(obj);
	}

	/** Frees the specified objects from the {@link #get(Class) pool}. Null objects within the array are silently ignored. Objects
	 * don't need to be from the same pool. */
	static public void freeAll<T>(Array<T> objects) {
		freeAll(objects, false);
	}

	/** Frees the specified objects from the {@link #get(Class) pool}. Null objects within the array are silently ignored.
	 * @param samePool If true, objects don't need to be from the same pool but the pool must be looked up for each object. */
	static public void freeAll<T>(Array<T> objects, bool samePool) {
		if (objects == null) throw new IllegalArgumentException("objects cannot be null.");
		Pool<T>? pool = null;
		for (int i = 0, n = objects.size; i < n; i++) {
			T obj = objects.get(i);
			if (obj == null) continue;
			if (pool == null) {
				pool = typePools.get(obj.GetType()) as Pool<T>;
				if (pool == null) continue; // Ignore freeing an object that was never retained.
			}
			pool.free(obj);
			if (!samePool) pool = null;
		}
	}

	private Pools () {
	}
}
}
