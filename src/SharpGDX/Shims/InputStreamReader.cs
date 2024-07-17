using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class InputStreamReader : Reader
	{
	private readonly InputStream @in;

	private readonly Decoder utf8Decoder;

	public InputStreamReader(InputStream @in)
	{
		this.@in = @in;
		this.utf8Decoder = System.Text.Encoding.UTF8.GetDecoder();
	}

	public InputStreamReader(InputStream @in, String encoding) // TODO: throws UnsupportedEncodingException
		:this(@in)
	{

	// FIXME this is bad, but some APIs seem to use "ISO-8859-1", fuckers...
	// if (! encoding.equals("UTF-8")) {
	// throw new UnsupportedEncodingException(encoding);
	// }
	}

	public override int read(char[] b, int offset, int length) // TODO: throws IOException
	{
		byte[]
			buffer = new byte[length];
		int c = @in.read(buffer);
		// TODO: Not sure if this is correct or not
		return c <= 0 ? c : utf8Decoder.GetChars(buffer, 0, c, b, offset);
	}

		public override void close() // TODO: throws IOException
	{
		@in.close();
	}
}
}
