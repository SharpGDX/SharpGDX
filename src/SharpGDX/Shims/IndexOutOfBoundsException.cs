namespace SharpGDX.Shims;

public class IndexOutOfBoundsException : Exception
{
	public IndexOutOfBoundsException(){}
	public IndexOutOfBoundsException(string message) : base(message)
	{
	}
}