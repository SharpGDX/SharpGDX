using System;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Reflect
{
	/** Provides information about, and access to, a single constructor for a Class.
 * @author nexsoftware */
	public sealed class Constructor
	{

		private readonly ConstructorInfo constructor;

		internal Constructor(ConstructorInfo constructor)
		{
			this.constructor = constructor;
		}

		/** Returns an array of Class objects that represent the formal parameter types, in declaration order, of the constructor. */
		public Type[] getParameterTypes()
		{
			throw new NotImplementedException();
			// TODO: return constructor.getParameterTypes();
		}

		/** Returns the Class object representing the class or interface that declares the constructor. */
		public Type getDeclaringClass()
		{
			return constructor.DeclaringType;
		}

		public bool isAccessible()
		{
			throw new NotImplementedException();
			// TODO: return constructor.isAccessible();
		}

		public void setAccessible(bool accessible)
		{
			throw new NotImplementedException();
			// TODO: constructor.setAccessible(accessible);
		}

		/** Uses the constructor to create and initialize a new instance of the constructor's declaring class, with the supplied
		 * initialization parameters. */
		public Object newInstance(Object[] args) // TODO:  throws ReflectionException 
		{
			// TODO: try
			//{
				return constructor.Invoke(args);
			//}
			//catch (IllegalArgumentException e)
			//{
			//	throw new ReflectionException(
			//		"Illegal argument(s) supplied to constructor for class: " + getDeclaringClass().Name,
			//		e);
			//}
			//catch (InstantiationException e)
			//	{
			//		throw new ReflectionException("Could not instantiate instance of class: " + getDeclaringClass().Name, e);
			//	}
			//	catch (IllegalAccessException e)
			//	{
			//		throw new ReflectionException("Could not instantiate instance of class: " + getDeclaringClass().Name, e);
			//	}
			//	catch (InvocationTargetException e)
			//	{
			//		throw new ReflectionException("Exception occurred in constructor for class: " + getDeclaringClass().Name, e);
			//	}
		}

	}
}
