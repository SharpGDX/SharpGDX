using SharpGDX.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Shims;
using System.Reflection;

namespace SharpGDX.Utils.Reflect
{
	/** Provides information about, and access to, a single method on a class or interface.
 * @author nexsoftware */
public sealed class Method {

	private readonly MethodInfo method;

	Method (MethodInfo method) {
		this.method = method;
	}

	/** Returns the name of the method. */
	public String getName () {
		return method.Name;
	}

	/** Returns a Class object that represents the formal return type of the method. */
	public Type getReturnType () {
		return method.ReturnType;
	}

	/** Returns an array of Class objects that represent the formal parameter types, in declaration order, of the method. */
	public Type[] getParameterTypes () {
			// TODO: Maybe yield return instead of select or cache the results? -RP
			return method.GetParameters().Select(f => f.ParameterType).ToArray();
	}

	/** Returns the Class object representing the class or interface that declares the method. */
	public Type getDeclaringClass () {
		// TODO: Will this be null in any case? -RP
		return method.DeclaringType;
	}

	public bool isAccessible ()
	{
		throw new NotImplementedException();
		// TODO: return method.isAccessible();
	}

	public void setAccessible (bool accessible) {
		throw new NotImplementedException();
		// TODO: method.setAccessible(accessible);
		}

		/** Return true if the method includes the {@code abstract} modifier. */
		public bool isAbstract () {
			throw new NotImplementedException();
			// TODO: return Modifier.isAbstract(method.getModifiers());
		}

		/** Return true if the method does not include any of the {@code private}, {@code protected}, or {@code public} modifiers. */
		public bool isDefaultAccess () {
		return !isPrivate() && !isProtected() && !isPublic();
	}

	/** Return true if the method includes the {@code final} modifier. */
	public bool isFinal () {
		throw new NotImplementedException();
		// TODO: return Modifier.isFinal(method.getModifiers());
		}

		/** Return true if the method includes the {@code private} modifier. */
		public bool isPrivate () {
			throw new NotImplementedException();
			// TODO: return Modifier.isPrivate(method.getModifiers());
		}

		/** Return true if the method includes the {@code protected} modifier. */
		public bool isProtected () {
			throw new NotImplementedException();
			// TODO: return Modifier.isProtected(method.getModifiers());
		}

		/** Return true if the method includes the {@code public} modifier. */
		public bool isPublic () {
			throw new NotImplementedException();
			// TODO: return Modifier.isPublic(method.getModifiers());
		}

		/** Return true if the method includes the {@code native} modifier. */
		public bool isNative () {
			throw new NotImplementedException();
			// TODO: return Modifier.isNative(method.getModifiers());
		}

		/** Return true if the method includes the {@code static} modifier. */
		public bool isStatic () {
			throw new NotImplementedException();
			// TODO: return Modifier.isStatic(method.getModifiers());
		}

		/** Return true if the method takes a variable number of arguments. */
		public bool isVarArgs () {
			throw new NotImplementedException();
			// TODO: return method.isVarArgs();
		}

		/** Invokes the underlying method on the supplied object with the supplied parameters. */
		public Object invoke (Object obj, Object[] args) // TODO: throws ReflectionException 
	{
		throw new NotImplementedException();
		// TODO: try
		//	{
		//	return method.invoke(obj, args);
		//} catch (IllegalArgumentException e) {
		//	throw new ReflectionException("Illegal argument(s) supplied to method: " + getName(), e);
		//} catch (IllegalAccessException e) {
		//	throw new ReflectionException("Illegal access to method: " + getName(), e);
		//} catch (InvocationTargetException e) {
		//	throw new ReflectionException("Exception occurred in method: " + getName(), e);
		//}
	}

	/** Returns true if the method includes an annotation of the provided class type. */
	public bool isAnnotationPresent (Type annotationType) {
			// TODO: Throw exception if annotationType is not 'Attribute'.
			throw new NotImplementedException();
			// TODO: return method.isAnnotationPresent(annotationType);
		}

		/** Returns an array of {@link Annotation} objects reflecting all annotations declared by this method, or an empty array if
		 * there are none. Does not include inherited annotations. Does not include parameter annotations. */
		public Annotation[] getDeclaredAnnotations () {
			throw new NotImplementedException();
			// TODO: Attribute[] annotations = method.getDeclaredAnnotations();
		//	Annotation[] result = new Annotation[annotations.Length];
		//for (int i = 0; i < annotations.Length; i++) {
		//	result[i] = new Annotation(annotations[i]);
		//}
		//return result;
	}

	/** Returns an {@link Annotation} object reflecting the annotation provided, or null of this method doesn't have such an
	 * annotation. This is a convenience function if the caller knows already which annotation type he's looking for. */
	public Annotation getDeclaredAnnotation (Type annotationType) {
			// TODO: Throw exception if annotationType is not 'Attribute'.
			throw new NotImplementedException();
			// TODO: Attribute[] annotations = method.getDeclaredAnnotations();
		//	if (annotations == null) {
		//	return null;
		//}
		//foreach (Attribute annotation in annotations) {
		//	if (annotation.annotationType().equals(annotationType)) {
		//		return new Annotation(annotation);
		//	}
		//}
		//return null;
	}

}
}
