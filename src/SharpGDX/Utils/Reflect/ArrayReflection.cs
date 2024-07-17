using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Reflect
{
	/** Utilities for Array reflection.
 * @author nexsoftware */
	public sealed class ArrayReflection
	{

		/** Creates a new array with the specified component type and length. */
		static public Object newInstance(Type c, int size)
		{
			return Array.CreateInstance(c, size);
		}

		/** Returns the length of the supplied array. */
		static public int getLength(Object array)
		{
			//return java.lang.reflect.Array.getLength(array);
			throw new NotImplementedException();
		}

		/** Returns the value of the indexed component in the supplied array. */
		static public Object get(Object array, int index)
		{
			//return java.lang.reflect.Array.get(array, index);
			throw new NotImplementedException();
		}

		/** Sets the value of the indexed component in the supplied array to the supplied value. */
		static public void set(Object array, int index, Object value)
		{
			//java.lang.reflect.Array.set(array, index, value);
			throw new NotImplementedException();
		}

	}
}
