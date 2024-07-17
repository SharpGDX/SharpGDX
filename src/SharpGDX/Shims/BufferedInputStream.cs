namespace SharpGDX.Shims;

public class BufferedInputStream : FilterInputStream
{
	private readonly int _bufferSize;

	public BufferedInputStream(InputStream stream, int bufferSize)
	:base(stream)
	{
		_bufferSize = bufferSize;
	}
}