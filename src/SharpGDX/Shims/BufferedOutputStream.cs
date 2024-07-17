using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	public class BufferedOutputStream : OutputStream
	{
		public BufferedOutputStream(OutputStream stream, int bufferSize)
		{

		}

		public override void close()
		{
			throw new NotImplementedException();
		}
	}
}
