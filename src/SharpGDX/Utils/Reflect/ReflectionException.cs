namespace SharpGDX.Utils.Reflect;

/**
 * Thrown when an exception occurs during reflection.
 * @author nexsoftware
 */
public class ReflectionException : Exception
{
	public ReflectionException()
	{
	}

	public ReflectionException(string message)
		: base(message)
	{
	}

	public ReflectionException(Exception cause)
		: base(cause.Message, cause)
	{
	}

	public ReflectionException(string message, Exception cause)
		: base(message, cause)
	{
	}
}