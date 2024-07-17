using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class OutputStream : ICloseable
	{
		// TODO: Should this be a memory stream?
		protected MemoryStream stream;
		

		protected OutputStream()
		{
			stream = new MemoryStream();
		}

		public virtual void write(byte[] buffer, int offset, int length)
		{
			stream.Write(buffer, offset, length);

		}

		public void flush()
		{
			stream.Flush();
		}

		public void write(byte[] bytes)
		{
			var s = 1;
		}

		public virtual void close()
		{
			// TODO: Should probably be disposing honestly. -RP
			stream.Close();
		}
	}
}
