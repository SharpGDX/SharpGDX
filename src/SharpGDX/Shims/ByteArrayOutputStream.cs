using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotImplementedException = System.NotImplementedException;

namespace SharpGDX.Shims
{
	public class ByteArrayOutputStream : OutputStream
	{
		public ByteArrayOutputStream(int length)
		{
		}

		public override void write(byte[] buffer, int offset, int count)
		{
			// TODO: Hmm... -RP
			stream.Write(buffer, offset, count);
		}

		public virtual byte[] toByteArray()
		{
			// TODO: Should this stay? -RP
			return stream.ToArray();
		}
	}
}