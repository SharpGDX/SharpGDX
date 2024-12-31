using System.Reflection;
using SharpGDX.Shims;
using System.Security;
using SharpGDX.Graphics.G2D;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using System.Security.Claims;

// TODO: Re-Port this class. -RP
namespace SharpGDX.Utils.Reflect
{
	/** Utilities for Class reflection.
 * @author nexsoftware */
	public sealed class ClassReflection
	{

		//	/** Returns the Class object associated with the class or interface with the supplied string name. */
        static public Type forName(String name) // throws ReflectionException
        {
            try
            {
                // TODO: Not really sure how Class.nameFor("string") works in Java, need to explore. -RP
                // TODO: return Type.GetType(name);

				// TODO: This isn't really efficient, but for 'when' it is used, it probably doesn't really matter. -RP
                var type = AppDomain.CurrentDomain.GetAssemblies()
                    .Reverse()
                    .SelectMany(assembly => assembly.GetTypes())
                    .First(type => type.Name.EndsWith(name));
                return type;
            }
            catch (Exception e)
            {
                // TODO: This exception is really too generic. -RP
                throw new ReflectionException("Class not found: " + name, e);
            }
        }

        /** Returns a {@link Field} that represents the specified public member field for the supplied class. */
        static public Field getField(Type c, String name)
        {
            try
            {
                return new Field(c.GetField(name));
                //} catch (SecurityException e) {
                //throw new ReflectionException("Security violation while getting field: " + name + ", for class: " + c.Name, e);
                //} catch (NoSuchFieldException e) {
            }
            catch (Exception e)
            {

                throw new ReflectionException("Field not found: " + name + ", for class: " + c.Name, e);
            }
        }

        /** Returns an array of {@link Field} containing the public fields of the class represented by the supplied Class. */
        static public Field[] getFields(Type c)
        {
            var fields = c.GetFields();
            Field[] result = new Field[fields.Length];
            for (int i = 0, j = fields.Length; i < j; i++)
            {
                result[i] = new Field(fields[i]);
            }
            return result;
        }

        /** Returns the simple name of the underlying class as supplied in the source code. */
        static public String getSimpleName(Type c)
		{
			return c.Name;
		}

		/** Determines if the supplied Object is assignment-compatible with the object represented by supplied Class. */
		static public bool isInstance(Type c, Object obj)
		{
			// TODO: Verify
			return c.IsInstanceOfType(obj);
		}

		/** Determines if the class or interface represented by first Class parameter is either the same as, or is a superclass or
		 * superinterface of, the class or interface represented by the second Class parameter. */
		static public bool isAssignableFrom(Type c1, Type c2)
		{
			return c1.IsAssignableFrom(c2);
		}

		//	/** Returns true if the class or interface represented by the supplied Class is a member class. */
		static public bool isMemberClass(Type c)
		{
			return c.IsNested;
		}

		//	/** Returns true if the class or interface represented by the supplied Class is a static class. */
		static public bool isStaticClass(Type c)
		{
			return c is { IsAbstract: true, IsSealed: true };
		}

		//	/** Determines if the supplied Class object represents an array class. */
		//	static public boolean isArray (Class c) {
		//		return c.isArray();
		//	}

		//	/** Determines if the supplied Class object represents a primitive type. */
		//	static public boolean isPrimitive (Class c) {
		//		return c.isPrimitive();
		//	}

		//	/** Determines if the supplied Class object represents an enum type. */
		//	static public boolean isEnum (Class c) {
		//		return c.isEnum();
		//	}

		//	/** Determines if the supplied Class object represents an annotation type. */
		//	static public boolean isAnnotation (Class c) {
		//		return c.isAnnotation();
		//	}

		//	/** Determines if the supplied Class object represents an interface type. */
		//	static public boolean isInterface (Class c) {
		//		return c.isInterface();
		//	}

		//	/** Determines if the supplied Class object represents an abstract type. */
		//	static public boolean isAbstract (Class c) {
		//		return Modifier.isAbstract(c.getModifiers());
		//	}

		/** Creates a new instance of the class represented by the supplied Class. */
		// TODO: Does this really need 'Type c'? -RP
		// TODO: Can it just use the generic type and call Activator.CreateInstance<T>()? -RP
		static public T newInstance<T>(Type c) //throws ReflectionException
        {
            try
            {
                return (T)Activator.CreateInstance(c);
            }
            catch (Exception e)
            {
                throw new ReflectionException("Could not instantiate instance of class: " + c.Name, e);
            }
			// TODO: Need to properly mimic. -RP
            //try
            //{
            //    return c.newInstance();
            //}
            //catch (InstantiationException e)
            //{
            //    throw new ReflectionException("Could not instantiate instance of class: " + c.getName(), e);
            //}
            //catch (IllegalAccessException e)
            //{
            //    throw new ReflectionException("Could not instantiate instance of class: " + c.getName(), e);
            //}
        }

        /** Creates a new instance of the class represented by the supplied Class. */
        // TODO: Does this really need 'Type c'? -RP
        // TODO: Can it just use the generic type and call Activator.CreateInstance<T>()? -RP
        static public object newInstance(Type c) //throws ReflectionException
        {
            try
            {
                return Activator.CreateInstance(c);
            }
            catch (Exception e)
            {
                throw new ReflectionException("Could not instantiate instance of class: " + c.Name, e);
            }
            // TODO: Need to properly mimic. -RP
            //try
            //{
            //    return c.newInstance();
            //}
            //catch (InstantiationException e)
            //{
            //    throw new ReflectionException("Could not instantiate instance of class: " + c.getName(), e);
            //}
            //catch (IllegalAccessException e)
            //{
            //    throw new ReflectionException("Could not instantiate instance of class: " + c.getName(), e);
            //}
        }

        //	/** Returns the Class representing the component type of an array. If this class does not represent an array class this method
        //	 * returns null. */
        //	static public Class getComponentType (Class c) {
        //		return c.getComponentType();
        //	}

        //	/** Returns an array of {@link Constructor} containing the public constructors of the class represented by the supplied
        //	 * Class. */
        //	static public Constructor[] getConstructors (Class c) {
        //		java.lang.reflect.Constructor[] constructors = c.getConstructors();
        //		Constructor[] result = new Constructor[constructors.length];
        //		for (int i = 0, j = constructors.length; i < j; i++) {
        //			result[i] = new Constructor(constructors[i]);
        //		}
        //		return result;
        //	}

        /** Returns a {@link Constructor} that represents the public constructor for the supplied class which takes the supplied
		 * parameter types. */
        static public Constructor getConstructor(Type c, Type[] parameterTypes) // TODO: throws ReflectionException
		{
			try
			{
				// TODO: I think this should maybe search private/internal?
				return new Constructor(c.GetConstructor(parameterTypes ?? []));
			}
			// TODO: Will it actually throw this? -RP
			catch (SecurityException e)
			{
				throw new ReflectionException(
					"Security violation occurred while getting constructor for class: '" + c.Name + "'.",
					e);
			}
			// TODO: Will it actually throw this? -RP
			// TODO:catch (NoSuchMethodException e) {
			catch (Exception e)
			{
				throw new ReflectionException("Constructor not found for class: " + c.Name, e);
			}
		}

		/** Returns a {@link Constructor} that represents the constructor for the supplied class which takes the supplied parameter
		 * types. */
		static public Constructor
			getDeclaredConstructor(Type c, params Type[] parameterTypes) // TODO: throws ReflectionException
		{
			try
			{
				// TODO: I think this should only search public/protected?
				return new Constructor(c.GetConstructor(parameterTypes));
			}
			catch (SecurityException e)
			{
				throw new ReflectionException("Security violation while getting constructor for class: " + c.Name, e);
			}
			// TODO:catch (NoSuchMethodException e) {
			catch (Exception e)
			{
				throw new ReflectionException("Constructor not found for class: " + c.Name, e);
			}
		}

		//	/** Returns the elements of this enum class or null if this Class object does not represent an enum type. */
		//	static public Object[] getEnumConstants (Class c) {
		//		return c.getEnumConstants();
		//	}

		//	/** Returns an array of {@link Method} containing the public member methods of the class represented by the supplied Class. */
		static public Method[] getMethods(Type c)
		{
			var methods = c.GetMethods();
            var result = new Method[methods.Length];
			for (int i = 0, j = methods.Length; i < j; i++)
			{
				result[i] = new Method(methods[i]);
			}
			return result;
		}

		//	/** Returns a {@link Method} that represents the public member method for the supplied class which takes the supplied parameter
		//	 * types. */
		//	static public Method getMethod (Class c, String name, Class... parameterTypes) throws ReflectionException {
		//		try {
		//			return new Method(c.getMethod(name, parameterTypes));
		//		} catch (SecurityException e) {
		//			throw new ReflectionException("Security violation while getting method: " + name + ", for class: " + c.getName(), e);
		//		} catch (NoSuchMethodException e) {
		//			throw new ReflectionException("Method not found: " + name + ", for class: " + c.getName(), e);
		//		}
		//	}

		//	/** Returns an array of {@link Method} containing the methods declared by the class represented by the supplied Class. */
		//	static public Method[] getDeclaredMethods (Class c) {
		//		java.lang.reflect.Method[] methods = c.getDeclaredMethods();
		//		Method[] result = new Method[methods.length];
		//		for (int i = 0, j = methods.length; i < j; i++) {
		//			result[i] = new Method(methods[i]);
		//		}
		//		return result;
		//	}

		//	/** Returns a {@link Method} that represents the method declared by the supplied class which takes the supplied parameter
		//	 * types. */
		//	static public Method getDeclaredMethod (Class c, String name, Class... parameterTypes) throws ReflectionException {
		//		try {
		//			return new Method(c.getDeclaredMethod(name, parameterTypes));
		//		} catch (SecurityException e) {
		//			throw new ReflectionException("Security violation while getting method: " + name + ", for class: " + c.getName(), e);
		//		} catch (NoSuchMethodException e) {
		//			throw new ReflectionException("Method not found: " + name + ", for class: " + c.getName(), e);
		//		}
		//	}

		//	/** Returns an array of {@link Field} containing the public fields of the class represented by the supplied Class. */
		//	static public Field[] getFields (Class c) {
		//		java.lang.reflect.Field[] fields = c.getFields();
		//		Field[] result = new Field[fields.length];
		//		for (int i = 0, j = fields.length; i < j; i++) {
		//			result[i] = new Field(fields[i]);
		//		}
		//		return result;
		//	}

		//	/** Returns a {@link Field} that represents the specified public member field for the supplied class. */
		//	static public Field getField (Class c, String name) throws ReflectionException {
		//		try {
		//			return new Field(c.getField(name));
		//		} catch (SecurityException e) {
		//			throw new ReflectionException("Security violation while getting field: " + name + ", for class: " + c.getName(), e);
		//		} catch (NoSuchFieldException e) {
		//			throw new ReflectionException("Field not found: " + name + ", for class: " + c.getName(), e);
		//		}
		//	}

		//	/** Returns an array of {@link Field} objects reflecting all the fields declared by the supplied class. */
		static public Field[] getDeclaredFields(Type c)
		{
			// TODO: Not sure if this is equal to Java's c.getDeclaredFields(). -RP
			var fields = c.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
			Field[] result = new Field[fields.Length];
			for (int i = 0, j = fields.Length; i < j; i++)
			{
				result[i] = new Field(fields[i]);
			}
			return result;
		}

        //	/** Returns a {@link Field} that represents the specified declared field for the supplied class. */
        //	static public Field getDeclaredField (Class c, String name) throws ReflectionException {
        //		try {
        //			return new Field(c.getDeclaredField(name));
        //		} catch (SecurityException e) {
        //			throw new ReflectionException("Security violation while getting field: " + name + ", for class: " + c.getName(), e);
        //		} catch (NoSuchFieldException e) {
        //			throw new ReflectionException("Field not found: " + name + ", for class: " + c.getName(), e);
        //		}
        //	}

        //	/** Returns true if the supplied class includes an annotation of the given type. */
        //	static public boolean isAnnotationPresent (Class c, Class<? extends java.lang.annotation.Annotation> annotationType) {
        //		return c.isAnnotationPresent(annotationType);
        //	}

        //	/** Returns an array of {@link Annotation} objects reflecting all annotations declared by the supplied class, and inherited
        //	 * from its superclass. Returns an empty array if there are none. */
        //	static public Annotation[] getAnnotations (Class c) {
        //		java.lang.annotation.Annotation[] annotations = c.getAnnotations();
        //		Annotation[] result = new Annotation[annotations.length];
        //		for (int i = 0; i < annotations.length; i++) {
        //			result[i] = new Annotation(annotations[i]);
        //		}
        //		return result;
        //	}

        /** Returns an {@link Annotation} object reflecting the annotation provided, or null if this class doesn't have such an
     * annotation. This is a convenience function if the caller knows already which annotation type he's looking for. */
        static public Annotation getAnnotation(Type c, Type annotationType)
        {
            var annotation = c.GetCustomAttribute(annotationType);
            if (annotation != null) return new Annotation(annotation);
            return null;
        }

        //	/** Returns an array of {@link Annotation} objects reflecting all annotations declared by the supplied class, or an empty array
        //	 * if there are none. Does not include inherited annotations. */
        //	static public Annotation[] getDeclaredAnnotations (Class c) {
        //		java.lang.annotation.Annotation[] annotations = c.getDeclaredAnnotations();
        //		Annotation[] result = new Annotation[annotations.length];
        //		for (int i = 0; i < annotations.length; i++) {
        //			result[i] = new Annotation(annotations[i]);
        //		}
        //		return result;
        //	}

        //	/** Returns an {@link Annotation} object reflecting the annotation provided, or null if this class doesn't have such an
        //	 * annotation. This is a convenience function if the caller knows already which annotation type he's looking for. */
        //	static public Annotation getDeclaredAnnotation (Class c, Class<? extends java.lang.annotation.Annotation> annotationType) {
        //		java.lang.annotation.Annotation[] annotations = c.getDeclaredAnnotations();
        //		for (java.lang.annotation.Annotation annotation : annotations) {
        //			if (annotation.annotationType().equals(annotationType)) return new Annotation(annotation);
        //		}
        //		return null;
        //	}

        //	static public Class[] getInterfaces (Class c) {
        //		return c.getInterfaces();
        //	}

    }
}