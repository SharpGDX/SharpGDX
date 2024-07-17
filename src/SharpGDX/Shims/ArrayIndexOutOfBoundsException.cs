namespace SharpGDX.Shims;

public class ArrayIndexOutOfBoundsException : Exception
{
	public ArrayIndexOutOfBoundsException(int index) : base(index.ToString())
	{
	}
}