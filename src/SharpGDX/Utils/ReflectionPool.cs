using SharpGDX.Utils.Reflect;
using SharpGDX.Shims;

namespace SharpGDX.Utils
{
	/** Pool that creates new instances of a type using reflection. The type must have a zero argument constructor.
 * {@link Constructor#setAccessible(boolean)} will be used if the class and/or constructor is not visible.
 * @author Nathan Sweet */
public class ReflectionPool<T> : Pool<T> {
	private readonly Constructor constructor;

	public ReflectionPool (Type type) 
	: this(type, 16, int.MaxValue)
		{
		
	}

	public ReflectionPool (Type type, int initialCapacity) 
	: this(type, initialCapacity, int.MaxValue)
	{
		
	}

	public ReflectionPool (Type type, int initialCapacity, int max) 
	: base(initialCapacity, max)
	{
		
		constructor = findConstructor(type);
		if (constructor == null)
			throw new RuntimeException("Class cannot be created (missing no-arg constructor): " + type.Name);
	}

	private Constructor? findConstructor (Type type) {
		try {
			return ClassReflection.getConstructor(type, (Type[]?)null);
		} catch (Exception ex1) {
			try {
				Constructor constructor = ClassReflection.getDeclaredConstructor(type, (Type[]?)null);
				constructor.setAccessible(true);
				return constructor;
			} catch (ReflectionException ex2) {
				return null;
			}
		}
	}

	protected override T newObject () {
		try {
			return (T)constructor.newInstance((Object[])null);
		} catch (Exception ex) {
			throw new GdxRuntimeException("Unable to create new instance: " + constructor.getDeclaringClass().Name, ex);
		}
	}
}
}
