using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class OutputStreamWriter : Writer
	{
	private readonly OutputStream @out;



	public OutputStreamWriter(OutputStream @out, String encoding)
	: this(@out)
		{
		
	}

	public OutputStreamWriter(OutputStream @out)
	{
		this.@out = @out;
	}

	public override void write(char[] b, int offset, int length)// TODO: throws IOException
	{
		@out.write(System.Text.Encoding.UTF8.GetBytes(b, offset, length));
	}

		public override void flush()// TODO: throws IOException
		{
		@out.flush();
	}

		public override void close() // TODO: throws IOException
	{
		@out.close();
	}
}
}
