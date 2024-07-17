namespace SharpGDX.Utils;

/**
 * Typed runtime exception used throughout libGDX
 * 
 * @author mzechner
 */
public class GdxRuntimeException : Exception
{
	private static readonly long serialVersionUID = 6735854402467673117L;

	public GdxRuntimeException(string message)
		: base(message)
	{
	}

	public GdxRuntimeException(Exception t)
		: base(t.Message)
	{
	}

	public GdxRuntimeException(string message, Exception t)
		: base(message, t)
	{
	}
}