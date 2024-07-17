namespace SharpGDX.Shims;

public class RuntimeException : Exception
{
	public RuntimeException()
	{
	}

	public RuntimeException(string message) 
		: base(message)
	{
	}

	public RuntimeException(string message, Exception exception) 
		: base(message, exception)
	{
	}
}