using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class StringWriter : Writer
	{
		private readonly StringBuilder _out;

		public StringWriter()
		{
			_out = new StringBuilder();
		}

		public StringWriter(int initialCapacity)
		{
			_out = new StringBuilder(initialCapacity);
		}

		public override void write(char[] b, int offset, int length) // TODO: throws IOException
		{
			_out.Append(b, offset, length);
		}

		public override String ToString()
		{
			return _out.ToString();
		}

		public StringBuilder getBuffer()
		{
			return _out;
		}

		public override void flush() // TODO:  throws IOException
		{
		}

		public override void close() // TODO: throws IOException
		{
		}
	}
}