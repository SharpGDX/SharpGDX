using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class FilterInputStream : InputStream, ICloseable
	{
		public override int read()
		{
			return @in._stream.ReadByte();
		}

		protected InputStream @in;

		protected FilterInputStream(InputStream @in)
		
		{
			this.@in = @in;
		}

		public override int  read(byte[] b, int off, int len)
		{
			var bytesRead = @in._stream.Read(b, off, len);
			return bytesRead > 0 ? bytesRead : -1;
		}

		public override void close() { }
	}
}
